using HalloDoc.DataModels;
using HalloDoc.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Text.Encodings.Web;
using HalloDoc.DataContext;
using HalloDoc.NewFolder1;
using Microsoft.EntityFrameworkCore;

namespace HalloDoc.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;
        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
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
        [HttpPost]
        public async Task<IActionResult> PatientLoginn(RegisterDTO a)
        {
            if(a.UserId == null )
            {
                return NotFound();
            }

            var user = await _context.RegisterDTOs.FirstOrDefaultAsync(m => m.UserId == a.UserId);
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

       
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }

    
}