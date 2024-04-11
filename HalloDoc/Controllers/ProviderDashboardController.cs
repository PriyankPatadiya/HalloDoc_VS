using BAL.Interface;
using BAL.Repository;
using DAL.DataContext;
using DAL.DataModels;
using DAL.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Org.BouncyCastle.Ocsp;
using Org.BouncyCastle.Utilities;

namespace HalloDoc.Controllers
{
    [CustomAuthorize(new string[] {"Provider"})]
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
            return View("Dashboard/DashboardPage", model);
        }
        public IActionResult filterDashboardTable(string SearchString, string selectButton, string StatusButton, string partialviewpath, int pagesize, int currentpage)
        {
            var physicianId = HttpContext.Session.GetInt32("PhysicianId");
            var result = _admin.GetRequestsQuery(StatusButton);
            result = result.Where(s => (s.physicianId == physicianId) && (String.IsNullOrEmpty(SearchString) || s.PatientName.Contains(SearchString)) && (System.String.IsNullOrEmpty(selectButton) || s.requestId == int.Parse(selectButton)));

            int totalItems = result.Count();
            int totalPages = (int)Math.Ceiling((double)totalItems / pagesize);
            if (SearchString != null || selectButton != null )
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
        public IActionResult ViewCase(string reqcliId)
        {
            return RedirectToAction("ViewCase", "AdminDashBoard", new { reqcliId = reqcliId });
        }
        public IActionResult ViewNotes(int reqid)
        {
            return RedirectToAction("ViewNotesAdminn", "AdminDashboard", new { reqid = reqid });
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
        public IActionResult ViewDocuments(int reeqid)
        {
            return RedirectToAction("ViewDocuments","AdminDashboard", new { reeqid = reeqid });
        }
    }
}
