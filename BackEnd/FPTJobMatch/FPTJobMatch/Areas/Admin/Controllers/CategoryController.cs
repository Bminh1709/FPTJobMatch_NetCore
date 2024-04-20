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
            try
            {
                DateTime today = DateTime.Today;
                IEnumerable<Category> approvedCategories = await _unitOfWork.Category.GetCategoriesByStatus(true);
                IEnumerable<Category> newCategories = await _unitOfWork.Category.GetCategoriesByStatus(false);

                var categoryVM = new CategoryVM
                {
                    ApprovedCategortList = approvedCategories,
                    NewCategoryList = newCategories,
                    ApprovedCount = _unitOfWork.Category.CountCategories(approvedCategories, false),
                    NewCount = _unitOfWork.Category.CountCategories(newCategories, false),
                    ApprovedThisMonthCount = _unitOfWork.Category.CountCategories(approvedCategories, true),
                    NewThisMonthCount = _unitOfWork.Category.CountCategories(newCategories, true)
                };

                return View(categoryVM);
            }
            catch (Exception ex)
            {
                return RedirectToAction("GenericError", "Error", new { area = "", code = 500, errorMessage = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> HandleCategory(string submitBtn, int categoryId)
        {
            try
            {
                Category category = await _unitOfWork.Category.GetAsync(c => c.Id == categoryId);

                var user = await _userManager.GetUserAsync(User);

                string notificationContent = GetNotificationContent(submitBtn, category);

                if (submitBtn == "approve")
                {
                    category.IsApproved = true;
                    TempData["success"] = $"The category {category.Name} has been approved";
                }
                else if (submitBtn == "delete")
                {
                    // Retrieve all jobs associated with the category
                    var jobsToDelete = await _unitOfWork.Job.GetAllAsync(j => j.CategoryId == category.Id);

                    // Remove all jobs associated with the category
                    _unitOfWork.Job.RemoveRange(jobsToDelete);

                    _unitOfWork.Category.Remove(category);
                    TempData["success"] = $"The category {category.Name} has been deleted";
                }

                var newNotification = new Notification
                {
                    ReceiverId = category.CreatedByUserId,
                    Sender = user,
                    CreatedAt = DateTime.UtcNow,
                    Content = notificationContent
                };

                _unitOfWork.Notification.Add(newNotification);
                _unitOfWork.Save();
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                return RedirectToAction("GenericError", "Error", new { area = "", code = 500, errorMessage = ex.Message });
            }
        }

        private string GetNotificationContent(string submitBtn, Category category)
        {
            if (submitBtn == "approve")
            {
                return $"The category {category.Name} has been approved by Admin";
            }
            else if (submitBtn == "delete")
            {
                return $"The category {category.Name} has been deleted by Admin";
            }
            return string.Empty;
        }
    }


}
