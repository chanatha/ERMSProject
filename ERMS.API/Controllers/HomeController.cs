using Microsoft.AspNetCore.Mvc;

namespace ERMS.API.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
