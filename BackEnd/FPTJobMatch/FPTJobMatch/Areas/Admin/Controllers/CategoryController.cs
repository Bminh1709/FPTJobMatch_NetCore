using FPT.DataAccess.Repository.IRepository;
using FPT.Models;
using FPT.Models.ViewModels;
using FPT.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FPTJobMatch.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class CategoryController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public CategoryController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            var today = DateTime.Today;

            var approvedCategories = _unitOfWork.Category.GetAll(c => c.IsApproved);
            var newCategories = _unitOfWork.Category.GetAll(c => !c.IsApproved);

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
        public IActionResult HandleCategory(string submitBtn, int categoryId)
        {
            Category category = _unitOfWork.Category.Get(c => c.Id  == categoryId);

            if (submitBtn == "approve") 
            {
                category.IsApproved = true;
            }
            else if (submitBtn == "delete") 
            {
                _unitOfWork.Category.Remove(category);
            }

            _unitOfWork.Save();
            return RedirectToAction("Index");
        }
    }

}
