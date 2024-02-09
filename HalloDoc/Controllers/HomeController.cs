
using HalloDoc.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using HalloDoc.NewFolder1;
using Microsoft.EntityFrameworkCore;
using DAL.DataContext;
using DAL.DataModels;

namespace HalloDoc.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;
      

        public HomeController(ApplicationDbContext db)
        {
            _context = db;
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

        //Post
            [HttpPost]
            public async Task<IActionResult> PatientLoginn(AspNetUser a)
                {  

                if(a.Id == null || a.PasswordHash == null)
                {
                    ModelState.AddModelError("EmptyField", "The Field Is Empty");
                }
                if(_context.AspNetUsers == null)
                {
                    return NotFound();
                }

                var user = await _context.AspNetUsers.FirstOrDefaultAsync(m => m.Id == a.Id);
                if(user !=null && user.PasswordHash == a.PasswordHash)
                {
                    return RedirectToAction(nameof(PatientSite));
                }
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

        //POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult PatientCreateAcc(AspNetUser Obj) 
        {
            if (ModelState.IsValid)
            {
                _context.Add(Obj);
                _context.SaveChanges();
                return RedirectToAction("PatientLoginn");
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