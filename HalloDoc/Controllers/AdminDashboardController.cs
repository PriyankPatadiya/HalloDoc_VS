using BAL.Interface;
using DAL.DataContext;
using DAL.ViewModels;
using Microsoft.AspNetCore.Mvc;


namespace HalloDoc.Controllers
{
    public class AdminDashboardController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IAdminDashboard _admin;
        private readonly IAdminActions _adminActions;
        public AdminDashboardController(ApplicationDbContext context, IAdminDashboard admin, IAdminActions action)
        {
            _context = context;
            _admin = admin;
            _adminActions = action;
        }

        public IActionResult MainPage()
        {
            AdminMainPageVM MainModel = new AdminMainPageVM()
            {
                PageName = PageName.Dashboard
            };

            AdminDashboardVM vm = new AdminDashboardVM();

            MainModel.DashboardVM = AdminDashCallFromMain(vm, "1");
            if (!String.IsNullOrEmpty("1"))
            {
                ViewBag.Status = int.Parse("1");
            }

            return View(MainModel);
        }


        public AdminDashboardVM AdminDashCallFromMain(AdminDashboardVM model, string StatusButton)
        {
            var result = _admin.GetRequestsQuery(StatusButton);
            model.DashboardTableVM = result.ToList();
            model.NewCount = _admin.CountRequests("1");
            model.PendingCount = _admin.CountRequests("2");
            model.ActiveCount = _admin.CountRequests("3");
            model.ConcludeCount = _admin.CountRequests("4");
            model.ToCloseCount = _admin.CountRequests("5");
            model.UnpaidCount = _admin.CountRequests("6");
            model.Region = _context.Regions.ToList();
            return model;
        }

        [HttpGet]
        public IActionResult ViewCaseAdmin()
        {
            var requestclientId = HttpContext.Request.Query["reqcliId"];
            AdminMainPageVM MainModel = new AdminMainPageVM()
            {
                PageName = PageName.ViewCaseForm
            };
            ViewCaseVM result = _adminActions.getViewCaseData(int.Parse(requestclientId)).FirstOrDefault();
            int requestid = _admin.getRequestIdbyRequestClientId(int.Parse(requestclientId));
            int status = _admin.getStatusByRequetId(requestid);
            result.Status = status;
            result.requestid = requestid;
            result.regiontable = _context.Regions.ToList();
            MainModel.Casemodel = result;
            return View("MainPage", MainModel);
        }

        public IActionResult ViewNotesAdmin()
        {
            var requestId = HttpContext.Request.Query["reqid"];
            AdminMainPageVM MainModel = new AdminMainPageVM()
            {
                PageName = PageName.ViewNotes
            };
            var result = _adminActions.viewnotes(int.Parse(requestId));
            MainModel.NotesVM = result;
            return View("MainPage", MainModel);
        }

        public IActionResult SearchByName(string SearchString, string selectButton, string StatusButton, string SelectedStateId, string partialviewpath)
        {

            var result = _admin.GetRequestsQuery(StatusButton);


            result = result.Where(s => (String.IsNullOrEmpty(SearchString) || s.PatientName.Contains(SearchString)) && (String.IsNullOrEmpty(selectButton) || s.requestId == int.Parse(selectButton)) && (SelectedStateId == "0" || s.regionId == int.Parse(SelectedStateId)));


            if (!String.IsNullOrEmpty(StatusButton))
            {
                ViewBag.Status = int.Parse(StatusButton);
            }

            return PartialView(partialviewpath, result.ToList());
        }

        [HttpPost]
        public IActionResult CancelCase(int reeqid, string Reason)
        {
            string Notes = Request.Form["Notes"];
            _adminActions.changeStatusOnCancleCase(reeqid, Reason, Notes);
            return Ok();
        }

        public IActionResult filterPhyByRegion(string RegionId)
        {
            var physician = _adminActions.GetPhysicianByRegion(RegionId);
            return Json(physician);
        }

        [HttpPost]
        public IActionResult AssignCase(int reeqid, string physicianId, string Notes)
        {
            _adminActions.ChangeOnAssign(reeqid, int.Parse(physicianId), Notes);
            return View("MainPage");
        }

        public IActionResult BlockCase(int reeqid, string reason)
        {
            _adminActions.BlockCase(reeqid, reason);
            return View("MainPage");
        }

        public IActionResult ViewDocuments(int reeqid)
        {
            AdminMainPageVM mainmodel = new AdminMainPageVM
            {
                PageName = PageName.viewdocument
            };
            
            return View();
        }
    }
}
