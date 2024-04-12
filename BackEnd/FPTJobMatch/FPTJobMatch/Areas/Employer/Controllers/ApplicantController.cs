using FPT.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FPTJobMatch.Areas.Employer.Controllers
{
    [Area("Employer")]
    [Authorize(Roles = SD.Role_Employer)]
    public class ApplicantController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Detail()
        {
            return View();
        }
    }
}
