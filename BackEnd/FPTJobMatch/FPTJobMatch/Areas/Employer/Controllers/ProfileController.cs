using Microsoft.AspNetCore.Mvc;

namespace FPTJobMatch.Areas.Employer.Controllers
{
    [Area("Employer")]
    public class ProfileController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
