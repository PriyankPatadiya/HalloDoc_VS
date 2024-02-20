
using HalloDoc.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using DAL.DataContext;
using DAL.DataModels;
using BAL.Interface;
using DAL.ViewModels;
using System.Text;
using MimeKit.Cryptography;

namespace HalloDoc.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;
        private readonly ILogin _login;
        private readonly ICreateAcc _createacc;
        private readonly IEmailService _emailService;
        //private readonly IJWTTokenservice _jwtTokenService;


        public HomeController(ApplicationDbContext db , ILogin login , ICreateAcc createacc, IEmailService emailService)
        {
            _context = db;
            _login = login;
            _createacc = createacc;
            _emailService = emailService;
            //_jwtTokenService = jwtservice;

        }
        public IActionResult Privacy(string name, int numTimes = 1)
        {
            ViewData["Messege"] = "Hello" + name;
            ViewData["numTimes"] = numTimes;
            return View();
        }
        public IActionResult SubmitRequest()
        {
            return View();
        }

        public IActionResult PatientSite()
        {
            return View();
        }

        public IActionResult PatientLoginn()
        {
            return View();
        }

        public IActionResult PatientDashboard()
        {
            return View();
        }

        public IActionResult ResetPassword()
        {
            
            return View();
        }


        public IActionResult PatientCreateAcc()
        {
            return View();
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Remove("Email");
            return RedirectToAction("PatientLoginn");
        }


        //Post  Login Action


        [HttpPost]
            public async Task<IActionResult> PatientLoginn(LoginVM a)
            {
            
            if (ModelState.IsValid)
            {
                var user = _login.LoginVarify(a);
                
                if(user == true)
                {
                    HttpContext.Session.SetString("Email", a.Email);
                    return RedirectToAction("PatientDashboard", "PatientDashBoard");
                }
                else
                {
                    return View();
                }
            }
            return View(a);
            }


        //POST Create Action

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult PatientCreateAcc(CreateAccVM Obj) 
        {
            if (ModelState.IsValid)
            {
                if(Obj.Password != Obj.ConfirmPassword)
                {
                    ModelState.AddModelError("ConfirmPassword", "The Password and Confirm Password do not match");
                    return View(Obj);
                }
                var newUser = new AspNetUser
                {
                    Id = Guid.NewGuid().ToString(),
                    UserName = Obj.UserName,
                    PasswordHash = Obj.Password,
                };

                _createacc.AddUser(newUser);
                return RedirectToAction(nameof(Privacy));
            }
            return View(Obj);
        }



       [HttpPost]
        [ValidateAntiForgeryToken]

        public IActionResult ResetPassword(ForgetPasswordVM model)
        {
            string to = model.email;
            string subject = "Forget Your Password... Do not worry You can change your password.";

            var body = new StringBuilder();
            body.AppendFormat("Hello");
            body.AppendLine(@"Your KAUH Account about to activate click 
                             the link below to complete the actination process");
            body.AppendLine("<a href=\"http://localhost:49496/Activated.aspx\">login</a>");

            string Body = body.ToString();


            if (ModelState.IsValid)
            {
                var user = _context.Users.Where(x => x.Email == to);
                _emailService.SendEmail(to, subject, Body);
                model.EmailSent = true;
            }
            return View();
        }
            
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }

    
}