using BAL.Interface;
using DAL.DataContext;
using DAL.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace HalloDoc.Controllers
{
    public class UserCreateRequestFormsController : Controller
    {

        private readonly ApplicationDbContext _context;
        private readonly IPatientRequest _request;

        public UserCreateRequestFormsController(ApplicationDbContext context, IPatientRequest req)
        {
            _request = req;
            _context = context;
        }
        public IActionResult RequestForMe()
        {
            var email = HttpContext.Session.GetString("Email");
            var user = _context.Users.FirstOrDefault(u => u.Email == email);

            var req = new PatientReqVM();
            req.FirstName = user.FirstName;
            req.LastName = user.LastName;
            req.Email = email;
            req.PhoneNumber = user.Mobile;
            
            return View(req);
        }


        //public IActionResult RequestForSomeoneElse()
        //{
        //    return View();
        //}

        [HttpPost]
        public IActionResult RequestForMe(PatientReqVM model)
        {
            var email = HttpContext.Session.GetString("Email");
            var user = _context.AspNetUsers.Any(u => u.Email == email);

            

            _request.AddPatientForm(model);

            return View();
            
        }
    }
}
