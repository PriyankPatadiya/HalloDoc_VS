using Microsoft.AspNetCore.Mvc;

namespace HalloDoc.Controllers
{
    public class PatientDashBoardController : Controller
    {
        public IActionResult PatientDashboard()
        {
            return View();
        }
    }
}
