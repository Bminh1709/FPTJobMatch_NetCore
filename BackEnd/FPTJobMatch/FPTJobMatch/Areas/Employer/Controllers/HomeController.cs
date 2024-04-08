using Microsoft.AspNetCore.Mvc;

namespace FPTJobMatch.Areas.Employer.Controllers
{
    [Area("Employer")]
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Applicants()
        {
            return View();
        }

        public IActionResult Profile()
        {
            return View();
        }

    }
}
