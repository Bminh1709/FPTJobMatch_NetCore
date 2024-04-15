using FPT.DataAccess.Repository.IRepository;
using FPT.Models;
using FPT.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Diagnostics;
using System.Security.Claims;

namespace FPTJobMatch.Controllers
{
    public class HomeController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public HomeController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            HomePageVM homeVM = new HomePageVM
            {
                TopCategoryList = _unitOfWork.Category.GetAll(c => c.IsApproved == true).Take(4),
                TopJobList = _unitOfWork.Job.GetAll(c => c.Category.IsApproved == true, includeProperties: "Category,Company.City,JobType").Take(12),
                CityList = _unitOfWork.City.GetAll().Select(u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.Id.ToString()
                }),
            };
            return View(homeVM);
        }

        public IActionResult SignIn()
        {
            return View();
        }

        public IActionResult Help()
        {
            return View();
        }

        [Authorize]
        public IActionResult Notification()
        {
            string? currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            IEnumerable<Notification> notifications = _unitOfWork.Notification.GetAll(n => n.ReceiverId == currentUserId, includeProperties: "Sender");
            return View(notifications);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
