using Microsoft.AspNetCore.Mvc;

namespace FPTJobMatch.Areas.JobSeeker.Controllers
{
    [Area("JobSeeker")]
    public class ProfileController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
