using Microsoft.AspNetCore.Mvc;

namespace AdminHLDC.Controllers
{
    public class AdminDashboardController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
