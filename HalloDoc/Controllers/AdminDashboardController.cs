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
        private readonly IViewCaseAdmin _viewCaseAdmin;
        public AdminDashboardController(ApplicationDbContext context, IAdminDashboard admin, IViewCaseAdmin Viewadmin)
        {
            _context = context;
            _admin = admin;
            _viewCaseAdmin = Viewadmin;
        }

        public IActionResult MainPage(string pageName)
        {
            AdminMainPageVM MainModel = new AdminMainPageVM()
            {
                PageName = PageName.Dashboard
            };

            AdminDashboardVM vm = new AdminDashboardVM();
            
            MainModel.DashboardVM = AdminDashCallFromMain(vm , "1");
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
            ViewCaseVM result = _viewCaseAdmin.getViewCaseData(int.Parse(requestclientId)).FirstOrDefault();
            
 
            MainModel.Casemodel = result;
            return View("MainPage", MainModel);
        }

        public IActionResult SearchByName(string SearchString, string selectButton , string StatusButton)
        {

            var result = _admin.GetRequestsQuery(StatusButton);
            result = result.Where(s => (String.IsNullOrEmpty(SearchString) || s.PatientName.Contains(SearchString)) && (String.IsNullOrEmpty(selectButton) || s.requestId == int.Parse(selectButton)));
            if (!String.IsNullOrEmpty(StatusButton))
            {
                ViewBag.Status = int.Parse(StatusButton);
            }

            return PartialView("AdminDashboardNew", result.ToList());
        }

        
    }
}
