using DAL.DataContext;
using DAL.DataModels;
using DAL.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Org.BouncyCastle.Asn1.Ocsp;
using System.Net;

namespace HalloDoc.Controllers
{
    public class AdminDashboardController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdminDashboardController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult MainPage()
        {
            AdminMainPageVM model = new AdminMainPageVM();
            AdminDashboardVM dbmodel = new AdminDashboardVM();
            model.DashboardVM = AdminDashboardPage(dbmodel);
            return View(model);
        }


        public List<AdminDashboardVM> AdminDashboardPage(AdminDashboardVM model)
        {
            var result = (from req in _context.Requests
                          join reqclient in _context.RequestClients
                          on req.RequestId equals reqclient.RequestId
                          select new AdminDashboardVM() {

                              PatientName = reqclient.FirstName + " " + reqclient.LastName,
                              BirthDate = new DateOnly((int)reqclient.IntYear, int.Parse(reqclient.StrMonth), (int)reqclient.IntDate).ToString("MMMM dd, yyyy"),
                              RequestorName = req.FirstName + " " + req.LastName,
                              RequestDate = req.CreatedDate.ToString("MMMM dd , yyyy hh mm"),
                              phone = reqclient.PhoneNumber,
                              address = reqclient.Street + " ," + reqclient.City + " ," + reqclient.State + ", " + reqclient.ZipCode,
                              requestId = req.RequestTypeId,
                              YourPhone = req.PhoneNumber

                          }).ToList();

            return result;
        }

        public IActionResult SearchByName(string SearchString, string selectButton)
        {

            var result = (from req in _context.Requests
                         join reqclient in _context.RequestClients
                         on req.RequestId equals reqclient.RequestId
                         select new AdminDashboardVM
                         {
                             PatientName = reqclient.FirstName + " " + reqclient.LastName,
                             BirthDate = new DateOnly((int)reqclient.IntYear, int.Parse(reqclient.StrMonth), (int)reqclient.IntDate).ToString("MMMM dd, yyyy"),
                             RequestorName = req.FirstName + " " + req.LastName,
                             RequestDate = req.CreatedDate.ToString("MMMM dd , yyyy hh mm"),
                             phone = reqclient.PhoneNumber,
                             address = reqclient.Street + " ," + reqclient.City + " ," + reqclient.State + ", " + reqclient.ZipCode,
                             requestId = req.RequestTypeId,
                             YourPhone = req.PhoneNumber
                         });

                result = result.Where(s => (String.IsNullOrEmpty(SearchString) || s.PatientName.Contains(SearchString)) && (String.IsNullOrEmpty(selectButton) || s.requestId == int.Parse(selectButton)) );


            return PartialView("AdminDashboardNew", result.ToList());
        }
    }
}
