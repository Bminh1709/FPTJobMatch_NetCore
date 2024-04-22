using FPT.DataAccess.Repository.IRepository;
using FPT.Models;
using FPT.Models.ViewModels;
using FPT.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace FPTJobMatch.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class HelpController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<ApplicationUser> _userManager;

        public HelpController(IUnitOfWork unitOfWork, UserManager<ApplicationUser> userManager)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
        }
        public async Task<IActionResult> Index(string? sortType, string? keyword)
        {
            try
            {
                HelpPageVM helpPageVM = new()
                {
                    HelpTypes = await _unitOfWork.HelpType.GetAllAsync(),
                    HelpArticles = await _unitOfWork.HelpArticle.GetAllArticlesFilteredAsync(sortType, keyword),
                };

                return View(helpPageVM);
            }
            catch (Exception ex)
            {
                return RedirectToAction("GenericError", "Error", new { area = "", code = 500, errorMessage = ex.Message });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpsertType(HelpType helpType)
        {
            try
            {
                if (String.IsNullOrEmpty(helpType.Name) || String.IsNullOrEmpty(helpType.Description))
                {
                    return RedirectToAction(nameof(Index));
                }

                if (helpType.Id == 0)
                {
                    bool isExisted = await _unitOfWork.HelpType.IsExists(helpType.Name);
                    if (isExisted)
                    {
                        TempData["error"] = "This type already exists";
                        return RedirectToAction(nameof(Index));
                    }

                    var user = await _userManager.GetUserAsync(User);
                    if (user == null)
                    {
                        TempData["error"] = "Please log in first";
                        return RedirectToAction("Index", "Access", new { area = "" });
                    }

                    helpType.Admin = user;
                    TempData["success"] = "Help Type created successfully";
                    _unitOfWork.HelpType.Add(helpType);
                }
                else
                {
                    HelpType existingHelpType = await _unitOfWork.HelpType.GetAsync(h => h.Id == helpType.Id);
                    if (existingHelpType == null)
                    {
                        TempData["error"] = "Help type not found";
                        return RedirectToAction(nameof(Index));
                    }
                    existingHelpType.Name = helpType.Name;
                    existingHelpType.Description = helpType.Description;
                    _unitOfWork.HelpType.Update(existingHelpType);

                    TempData["success"] = "Help Type updated successfully";
                }

                _unitOfWork.Save();

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                return RedirectToAction("GenericError", "Error", new { area = "", code = 500, errorMessage = ex.Message });
            }
        }

        public async Task<IActionResult> GetHelpType(int id)
        {
            try
            {
                HelpType helpType = await _unitOfWork.HelpType.GetAsync(h => h.Id == id);
                return Json(new
                {
                    success = true,
                    data = new { helpType.Name, helpType.Description }
                });
            }
            catch (Exception ex)
            {
                return RedirectToAction("GenericError", "Error", new { area = "", code = 500, errorMessage = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> DeleteType(int typeId)
        {
            try
            {
                await _unitOfWork.HelpType.RemoveById(typeId);
                _unitOfWork.Save();
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                return RedirectToAction("GenericError", "Error", new { area = "", code = 500, errorMessage = ex.Message });
            }
        }

        public async Task<IActionResult> Article(int? articleId)
        {
            try
            {
                HelpArticleVM helpArticleVM = new()
                {
                    HelpTypeItems = (await _unitOfWork.HelpType.GetAllAsync()).Select(u => new SelectListItem
                    {
                        Text = u.Name,
                        Value = u.Id.ToString()
                    })
                };

                if (articleId == null)
                {
                    return View(helpArticleVM);
                }

                helpArticleVM.HelpArticle = await _unitOfWork.HelpArticle.GetAsync(h => h.Id == articleId);

                return View(helpArticleVM);
            }
            catch (Exception ex)
            {
                return RedirectToAction("GenericError", "Error", new { area = "", code = 500, errorMessage = ex.Message });
            }

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpsertArticle(HelpArticleVM model)
        {
            try
            {
                if (String.IsNullOrEmpty(model.HelpArticle.Title) || String.IsNullOrEmpty(model.HelpArticle.Content))
                {
                    TempData["error"] = "Fill in the information fully";
                    return RedirectToAction(nameof(Article));
                }

                if (model.HelpArticle.Id == 0)
                {
                    var user = await _userManager.GetUserAsync(User);
                    if (user == null)
                    {
                        TempData["error"] = "Please log in first";
                        return RedirectToAction("Index", "Access", new { area = "" });
                    }

                    model.HelpArticle.Admin = user;
                    model.HelpArticle.CreatedAt = DateTime.UtcNow;
                    _unitOfWork.HelpArticle.Add(model.HelpArticle);
                    TempData["success"] = "Article created successfully";
                }
                else
                {
                    HelpArticle existingHelpArticle = await _unitOfWork.HelpArticle.GetAsync(h => h.Id == model.HelpArticle.Id);
                    if (existingHelpArticle == null)
                    {
                        TempData["error"] = "Article not found";
                        return RedirectToAction(nameof(Index));
                    }
                    existingHelpArticle.Title = model.HelpArticle.Title;
                    existingHelpArticle.Content = model.HelpArticle.Content;
                    existingHelpArticle.HelpTypeId = model.HelpArticle.HelpTypeId;
                    existingHelpArticle.UpdatedAt = DateTime.UtcNow;

                    _unitOfWork.HelpArticle.Update(existingHelpArticle);

                    TempData["success"] = "Article updated successfully";
                }

                _unitOfWork.Save();

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                return RedirectToAction("GenericError", "Error", new { area = "", code = 500, errorMessage = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> DeleteArticle(int articleId)
        {
            try
            {
                await _unitOfWork.HelpArticle.RemoveById(articleId);
                _unitOfWork.Save();
                TempData["error"] = "Article deleted successfully";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                return RedirectToAction("GenericError", "Error", new { area = "", code = 500, errorMessage = ex.Message });
            }
        }

    }
}
