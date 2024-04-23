using BAL.Interface;
using BAL.Repository;
using DAL.DataContext;
using DAL.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace HalloDoc.Controllers
{
    public class ProviderDashboardController : Controller
    {
        private readonly IAdminDashboard _admin;
        private readonly IProviderSite _provider;
        private readonly IProviderDashboard _pDashboard;
        private readonly ApplicationDbContext _context;
        private readonly IScheduling _schedule;
        public ProviderDashboardController(IAdminDashboard admin, IProviderSite provider, IProviderDashboard pDashboard, IScheduling schedule, ApplicationDbContext context)
        {
            _admin = admin;
            _provider = provider;
            _pDashboard = pDashboard;
            _schedule = schedule;
            _context = context;
        }

        #region Dashboard
        [CustomAuthorize(new string[] { "Provider" }, "18")]

        public IActionResult Dashboard()
        {
            var physicianId = HttpContext.Session.GetInt32("PhysicianId");
            AdminDashboardVM model = new AdminDashboardVM();
            model.NewCount = _provider.CountRequests("1", physicianId);
            model.PendingCount = _provider.CountRequests("2", physicianId);
            model.ActiveCount = _provider.CountRequests("3", physicianId);
            model.ConcludeCount = _provider.CountRequests("4", physicianId);
            model.ToCloseCount = _provider.CountRequests("5", physicianId);
            model.UnpaidCount = _provider.CountRequests("6", physicianId);
            model.Region = _admin.regions();
            TempData["username"] = _pDashboard.getPhysicianbyEmail((int)physicianId).FirstName;
            return View("Dashboard/DashboardPage", model);
        }
        public IActionResult filterDashboardTable(string SearchString, string selectButton, string StatusButton, string partialviewpath, int pagesize, int currentpage)
        {
            var physicianId = HttpContext.Session.GetInt32("PhysicianId");
            var result = _admin.GetRequestsQuery(StatusButton);
            result = result.Where(s => (s.physicianId == physicianId) && (String.IsNullOrEmpty(SearchString) || s.PatientName.ToLower().Contains(SearchString.ToLower())) && (System.String.IsNullOrEmpty(selectButton) || s.requestId == int.Parse(selectButton)));

            int totalItems = result.Count();
            int totalPages = (int)Math.Ceiling((double)totalItems / pagesize);
            if (SearchString != null || selectButton != null)
            {
                if (totalPages <= 1)
                {
                    currentpage = 1;
                }
            }
            var paginatedData = result.ToList().Skip((currentpage - 1) * pagesize).Take(pagesize).ToList();
            if (paginatedData.Count != 0)
            {
                if (!String.IsNullOrEmpty(StatusButton))
                {
                    ViewBag.Status = int.Parse(StatusButton);
                    ViewBag.CurrentPage = currentpage;
                    ViewBag.TotalPages = totalPages;
                }
                return PartialView(partialviewpath, paginatedData);
            }
            else
            {
                ViewBag.EmptyMessage = "There is No data to show you....";
                return PartialView(partialviewpath);
            }
        }

        #endregion

        #region Actions
        [CustomAuthorize(new string[] { "Provider" }, "18")]
        public IActionResult Accept(int reqid)
        {
            _pDashboard.AcceptRequest(reqid);
            return RedirectToAction("Dashboard");
        }

        [HttpPost]
        public IActionResult TransferThisRequest(int reeqid, string Notes)
        {
            _pDashboard.transferRequest(reeqid, Notes);

            return RedirectToAction("Dashboard");
        }

        public IActionResult SendAgreement(int requestid)
        {
            TempData["isFromPhysician"] = true;
            return RedirectToAction("SendAgreement", "AdminDashboard", new { requestid = requestid });
        }
        public IActionResult HouseCall(int requestId)
        {
            var request = _context.Requests.FirstOrDefault(u => u.RequestId == requestId);
            request.Status = 5;
            request.ModifiedDate = DateTime.Now;
            _context.Requests.Update(request); _context.SaveChanges();
            return RedirectToAction("Dashboard");
        }
        public IActionResult Consult(int requestId)
        {
            var request = _context.Requests.FirstOrDefault(u => u.RequestId == requestId);
            request.Status = 6;
            request.ModifiedDate = DateTime.Now;
            _context.Requests.Update(request); _context.SaveChanges();
            return RedirectToAction("Dashboard");
        }

        public IActionResult ConcludeCare(int requestId)
        {
            var physicianid = HttpContext.Session.GetInt32("PhysicianId");
            ViewBag.username = _context.Physicians.FirstOrDefault(i => i.PhysicianId == physicianid).FirstName;
            ViewdocumentVM model = new ViewdocumentVM();
            var requestfiles = _admin.filesbyrequestid(requestId);
            model.RequestWiseFile = requestfiles;
            model.requestid = requestId;
            model.confirmationNumber = _context.Requests.FirstOrDefault(u => u.RequestId == requestId).ConfirmationNumber.ToUpper();
            model.patientName = _context.RequestClients.FirstOrDefault(u => u.RequestId == requestId).FirstName;
            return View("Actions/ConcludeCare", model);
        }

        [HttpPost]
        public IActionResult SubmitConclude(int reqId)
        {
            var Notes = Request.Form["ProviderNote"];
            var physicianId = HttpContext.Session.GetInt32("PhysicianId");

            var encounterForm = _context.EncounterForms.FirstOrDefault(u => u.RequestId == reqId);
            if (encounterForm != null && encounterForm.IsFinalize)
            {
                _pDashboard.concludeRequest(reqId, (int)physicianId, Notes);

                TempData["Message"] = "Request Closed";
                TempData["MessageType"] = "success";
                return RedirectToAction("Dashboard");
            }
            TempData["Message"] = "Request is Not Finalized";
            TempData["MessageType"] = "error";
            return RedirectToAction("ConcludeCare", new { requestId = reqId });
        }

        #endregion

        #region Scheduling

        [CustomAuthorize(new string[] { "Provider" }, "19")]
        public IActionResult Scheduling()
        {
            var physicianId = HttpContext.Session.GetInt32("PhysicianId");
            ViewBag.region = from region in _context.Regions
                             join phyregion in _context.PhysicianRegions
                             on region.RegionId equals phyregion.RegionId
                             where phyregion.PhysicianId == physicianId
                             select region;
            return View("Scheduling/Scheduling");
        }

        public IActionResult GetPhysicianShift()
        {
            var physicianId = HttpContext.Session.GetInt32("PhysicianId");
            var physicians = (from physician in _context.Physicians
            where physician.PhysicianId == physicianId
                              select physician)
                             .FirstOrDefault();
            return Json(physicians);
        }

        public IActionResult getEvents()
        {
            var physicianId = HttpContext.Session.GetInt32("PhysicianId");

            var events = _schedule.getProviderEvents((int)physicianId);
            return Json(events);
        }

        #endregion
    }
}
