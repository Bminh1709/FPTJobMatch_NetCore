using FPT.DataAccess.Repository.IRepository;
using FPT.Models;
using FPT.Models.ViewModels;
using FPT.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace FPTJobMatch.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class CategoryController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<ApplicationUser> _userManager;

        public CategoryController(IUnitOfWork unitOfWork, UserManager<ApplicationUser> userManager)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var today = DateTime.Today;

            var approvedCategories = await _unitOfWork.Category.GetAllAsync(c => c.IsApproved);
            var newCategories = await _unitOfWork.Category.GetAllAsync(c => !c.IsApproved);

            var categoryVM = new CategoryVM
            {
                ApprovedCategortList = approvedCategories,
                NewCategoryList = newCategories,
                ApprovedCount = approvedCategories.Count(),
                NewCount = newCategories.Count(),
                ApprovedThisMonthCount = approvedCategories.Count(c => c.CreatedAt.Month == today.Month && c.CreatedAt.Year == today.Year),
                NewThisMonthCount = newCategories.Count(c => c.CreatedAt.Month == today.Month && c.CreatedAt.Year == today.Year)
            };

            return View(categoryVM);
        }

        [HttpPost]
        public async Task<IActionResult> HandleCategory(string submitBtn, int categoryId)
        {
            try { 
                Category category = await _unitOfWork.Category.GetAsync(c => c.Id == categoryId);

                var user = await _userManager.GetUserAsync(User);

                Notification newNotification = new Notification();
                newNotification.Receiver = category.CreatedByUser;
                newNotification.Sender = user;
                newNotification.CreatedAt = DateTime.UtcNow;

                if (submitBtn == "approve")
                {
                    category.IsApproved = true;
                    newNotification.Content = $"The category {category.Name} has been approved by Admin";
                }
                else if (submitBtn == "delete")
                {
                    newNotification.Content = $"The category {category.Name} has been deleted by Admin";
                    _unitOfWork.Category.Remove(category);
                }

                _unitOfWork.Notification.Add(newNotification);
                _unitOfWork.Save();
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                return RedirectToAction("GenericError", "Home", new { area = "", code = 500, errorMessage = ex.Message });
            }
        }
    }


}
