using HalloDoc.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Text.Encodings.Web;

namespace HalloDoc.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

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

        public IActionResult ResetPasscode()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }

    public class patientFormsController : Controller
    {
        public IActionResult PatientRequestForm()
        {
            return View();
        }
        public IActionResult Friend_FamilyRequestForm()
        {
            return View();
        }
        public IActionResult BusinessRequestForm()
        {
            return View();
        }
        public IActionResult ConciergeRequestForm()
        {
            return View();
        }
    }
}