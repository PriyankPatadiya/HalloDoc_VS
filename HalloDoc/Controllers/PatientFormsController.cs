using Microsoft.AspNetCore.Mvc;

namespace HalloDoc.Controllers
{
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
