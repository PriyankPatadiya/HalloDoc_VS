
using HalloDoc.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using HalloDoc.NewFolder1;
using Microsoft.EntityFrameworkCore;
using DAL.DataContext;
using DAL.DataModels;
using BAL.Interface;
using DAL.ViewModels;

namespace HalloDoc.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;
        private readonly ILogin _login;
        private readonly ICreateAcc _createacc;
      

        public HomeController(ApplicationDbContext db , ILogin login , ICreateAcc createacc)
        {
            _context = db;
            _login = login;
            _createacc = createacc; 

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
        //Post
            [HttpPost]
            public async Task<IActionResult> PatientLoginn(LoginVM a)
            {
            
            if (ModelState.IsValid)
            {
                var user = _login.LoginVarify(a);
                
                if(user == true)
                {
                    HttpContext.Session.SetString("Email", a.Email);
                    return RedirectToAction("PatientDashboard", "PatientDashboard" , new {id = a.Email});
                }
                else
                {
                    return View();
                }
            }
            return View(a);
            }


        public IActionResult ResetPassword()
        {
            return View();
        }

       
        public IActionResult PatientCreateAcc()
        {
            return View();
        }

        //POST
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

       
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }

    
}