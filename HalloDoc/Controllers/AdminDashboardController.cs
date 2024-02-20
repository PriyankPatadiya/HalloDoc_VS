using Microsoft.AspNetCore.Mvc;

namespace HalloDoc.Controllers
{
    public class AdminDashboardController : Controller
    {
        public IActionResult MainPage()
        {

            return View();
        }
    }
}
