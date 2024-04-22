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

        public async Task<IActionResult> Index()
        {
            try 
            {
                var topCategoryList = await _unitOfWork.Category
                    .GetAllAsync(c => c.IsApproved == true);

                var topJobList = await _unitOfWork.Job
                    .GetAllAsync(c => c.Category.IsApproved == true, includeProperties: "Category,Company.City,JobType");

                var cityList = (await _unitOfWork.City.GetAllAsync())
                    .Select(u => new SelectListItem
                    {
                        Text = u.Name,
                        Value = u.Id.ToString()
                    })
                    .ToList();

                var homeVM = new HomePageVM
                {
                    TopCategoryList = topCategoryList.Take(4),
                    TopJobList = topJobList.Take(12),
                    CityList = cityList
                };

                return View(homeVM);
            }
            catch (Exception ex)
            {
                return RedirectToAction("GenericError", "Error", new { area = "", code = 500, errorMessage = ex.Message });
            }
        }

        [Authorize]
        public async Task<IActionResult> Notification()
        {
            try { 
                string? currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                if (string.IsNullOrWhiteSpace(currentUserId))
                {
                    TempData["error"] = "Sign In First";
                    return RedirectToAction("Index", "Access", new { area = "" });
                }

                IEnumerable<Notification> notifications = await _unitOfWork.Notification.GetAllAsync(n => n.ReceiverId == currentUserId, includeProperties: "Sender");
                return View(notifications.OrderByDescending(n => n.CreatedAt));
            }
            catch (Exception ex)
            {
                return RedirectToAction("GenericError", "Home", new { area = "", code = 500, errorMessage = ex.Message });
            }
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
