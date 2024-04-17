using BAL.Interface;
using BAL.Repository;
using DAL.DataContext;
using DAL.DataModels;
using DAL.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Org.BouncyCastle.Asn1.Ocsp;
using Org.BouncyCastle.Ocsp;
using Org.BouncyCastle.Utilities;
using System.Drawing;

namespace HalloDoc.Controllers
{
    [CustomAuthorize(new string[] { "Provider" })]
    public class ProviderDashboardController : Controller
    {
        private readonly IAdminDashboard _admin;
        private readonly IProviderSite _provider;
        private readonly ApplicationDbContext _context;
        public ProviderDashboardController(IAdminDashboard admin, IProviderSite provider, ApplicationDbContext context)
        {
            _admin = admin;
            _provider = provider;
            _context = context;
        }
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
            TempData["username"] = _context.Physicians.FirstOrDefault(u => u.PhysicianId == physicianId).FirstName;
            return View("Dashboard/DashboardPage", model);
        }
        public IActionResult filterDashboardTable(string SearchString, string selectButton, string StatusButton, string partialviewpath, int pagesize, int currentpage)
        {
            var physicianId = HttpContext.Session.GetInt32("PhysicianId");
            var result = _admin.GetRequestsQuery(StatusButton);
            result = result.Where(s => (s.physicianId == physicianId) && (String.IsNullOrEmpty(SearchString) || s.PatientName.Contains(SearchString)) && (System.String.IsNullOrEmpty(selectButton) || s.requestId == int.Parse(selectButton)));

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
        public IActionResult Accept(int reqid)
        {
            var result = _context.Requests.FirstOrDefault(u => u.RequestId == reqid);
            result.Status = 2;
            result.ModifiedDate = DateTime.Now;
            _context.Requests.Update(result);
            _context.SaveChanges();

            RequestStatusLog requestStatusLog = new RequestStatusLog();
            requestStatusLog.Status = result.Status;
            requestStatusLog.RequestId = reqid;
            requestStatusLog.CreatedDate = DateTime.Now;

            _context.RequestStatusLogs.Add(requestStatusLog);
            _context.SaveChanges();
            return RedirectToAction("Dashboard");
        }

        [HttpPost]
        public IActionResult TransferThisRequest(int reeqid, string Notes)
        {
            var request = _context.Requests.FirstOrDefault(u => u.RequestId == reeqid);
            request.Status = 1;
            request.PhysicianId = null;
            _context.Requests.Update(request);
            _context.SaveChanges();

            RequestStatusLog log = new RequestStatusLog();
            log.Status = request.Status;
            log.RequestId = reeqid;
            log.Notes = Notes;
            log.CreatedDate = DateTime.Now;
            _context.RequestStatusLogs.Add(log); _context.SaveChanges();

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

            var request = _context.Requests.FirstOrDefault(u => u.RequestId == reqId);
            var encounterForm = _context.EncounterForms.FirstOrDefault(u => u.RequestId == reqId);
            if (encounterForm != null && encounterForm.IsFinalize)
            {
                request.Status = 8;
                request.ModifiedDate = DateTime.Now;
                _context.Requests.Update(request);
                _context.SaveChanges();

                RequestStatusLog log = new RequestStatusLog()
                {
                    RequestId = reqId,
                    Status = request.Status,
                    PhysicianId = physicianId,
                    CreatedDate = DateTime.Now,
                    Notes = Notes,

                };
                _context.RequestStatusLogs.Add(log);
                _context.SaveChanges();

                RequestNote note = _context.RequestNotes.FirstOrDefault(u => u.RequestId == reqId);
                note.PhysicianNotes = Notes;
                _context.RequestNotes.Update(note); _context.SaveChanges();

                TempData["Message"] = "Request Closed";
                TempData["MessageType"] = "success";
                return RedirectToAction("Dashboard");
            }
            TempData["Message"] = "Request is Not Finalized";
            TempData["MessageType"] = "error";
            return RedirectToAction("ConcludeCare", new { requestId = reqId });
        }

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

            var events = (from s in _context.Shifts
                          join pd in _context.Physicians on s.PhysicianId equals pd.PhysicianId
                          join sd in _context.ShiftDetails on s.ShiftId equals sd.ShiftId into shiftGroup
                          from sd in shiftGroup.DefaultIfEmpty()
                      

                          select new SchedulingVM
                          {
                              title = string.Concat(sd.StartTime, " ", sd.EndTime, " ", pd.FirstName, " ", pd.LastName),
                              Shiftid = sd.ShiftDetailId,
                              Startdate = sd.ShiftDate.Date.Add(sd.StartTime.ToTimeSpan()),
                              Enddate = sd.ShiftDate.Date.Add(sd.EndTime.ToTimeSpan()),
                              Status = sd.Status,
                              Physicianid = pd.PhysicianId,
                              PhysicianName = pd.FirstName + ' ' + pd.LastName,
                              Shiftdate = sd.ShiftDate,
                              ShiftDetailId = sd.ShiftDetailId,
                              Regionid = sd.RegionId,
                              ShiftDeleted = sd.IsDeleted[0]

                          }).ToList();
            events = events.Where(item => item.Physicianid == physicianId && !item.ShiftDeleted).ToList();
            return Json(events);
        }
    }
}
