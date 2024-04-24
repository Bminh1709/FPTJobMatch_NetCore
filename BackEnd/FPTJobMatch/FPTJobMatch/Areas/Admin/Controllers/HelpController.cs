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

        // localhost:7283/Admin/Help
        // Purpose: Viewing all types and articles uploaded
        public async Task<IActionResult> Index(string? sortType, string? keyword)
        {
            try
            {
                // The question is why we need a ViewModel
                // Because in "return View()" we cannot pass 2 lists, typeList and articleList like this ""return View(typeList, articleList)"" for example 
                // We just pass 1 of them, but I wanna pass 2 so that I need a ViewModel below
                HelpPageVM helpPageVM = new()
                {
                    // All Types
                    HelpTypes = await _unitOfWork.HelpType.GetAllAsync(),
                    // All Articles
                    HelpArticles = await _unitOfWork.HelpArticle.GetAllArticlesFilteredAsync(sortType, keyword),
                };

                return View(helpPageVM);
            }
            catch (Exception ex)
            {
                return RedirectToAction("GenericError", "Error", new { area = "", code = 500, errorMessage = ex.Message });
            }
        }

        // localhost:7283/Admin/UpsertType
        // Purpose: Creating and Updating a Type
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpsertType(HelpType helpType)
        {
            try
            {
                // Check if Name and Description of The Type are not null (empty)
                if (String.IsNullOrEmpty(helpType.Name) || String.IsNullOrEmpty(helpType.Description))
                {
                    return RedirectToAction(nameof(Index));
                }

                // If the Id is null => Creating a new Type
                if (helpType.Id == 0)
                {
                    // Check if Type's Name is existed or not
                    bool isExisted = await _unitOfWork.HelpType.IsExists(helpType.Name);
                    if (isExisted)
                    {
                        TempData["error"] = "This type already exists";
                        return RedirectToAction(nameof(Index));
                    }

                    // Get current User who is in the system (signed in and using the website)
                    var user = await _userManager.GetUserAsync(User);
                    if (user == null)
                    {
                        TempData["error"] = "Please log in first";
                        return RedirectToAction("Index", "Access", new { area = "" });
                    }

                    helpType.Admin = user;
                    TempData["success"] = "Help Type created successfully";

                    // Add the Type to Database
                    _unitOfWork.HelpType.Add(helpType);
                }
                else // If not, updating the existing Type
                {
                    // Get that Type in DATABASE
                    HelpType existingHelpType = await _unitOfWork.HelpType.GetAsync(h => h.Id == helpType.Id);
                    if (existingHelpType == null)
                    {
                        TempData["error"] = "Help type not found";
                        return RedirectToAction(nameof(Index));
                    }
                    existingHelpType.Name = helpType.Name;
                    existingHelpType.Description = helpType.Description;

                    // Update the Type to DATABASE
                    _unitOfWork.HelpType.Update(existingHelpType);

                    TempData["success"] = "Help Type updated successfully";
                }

                // If not include this line Save(), all changes will not make
                _unitOfWork.Save();

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                return RedirectToAction("GenericError", "Error", new { area = "", code = 500, errorMessage = ex.Message });
            }
        }

        // localhost:7283/Admin/GetHelpType/id=??
        // Purpose: Getting Type's information
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

        // localhost:7283/Admin/DeleteType/typeId=??
        // Purpose: Deleting Type
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

        // localhost:7283/Admin/Article/articleId=??
        // Purpose: Viewing Article's information
        public async Task<IActionResult> Article(int? articleId)
        {
            try
            {
                // Get All Types
                HelpArticleVM helpArticleVM = new()
                {
                    HelpTypeItems = (await _unitOfWork.HelpType.GetAllAsync()).Select(u => new SelectListItem
                    {
                        Text = u.Name,
                        Value = u.Id.ToString()
                    })
                };

                // If articleId is null, means that admin wanna create a new Article
                if (articleId == null)
                {
                    return View(helpArticleVM);
                }
                // If not null, means that admin wanna update an existing Article
                helpArticleVM.HelpArticle = await _unitOfWork.HelpArticle.GetAsync(h => h.Id == articleId);

                return View(helpArticleVM);
            }
            catch (Exception ex)
            {
                return RedirectToAction("GenericError", "Error", new { area = "", code = 500, errorMessage = ex.Message });
            }

        }

        // localhost:7283/Admin/UpsertArticle/model=??
        // Purpose: Creating and Updating an Article
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpsertArticle(HelpArticleVM model)
        {
            try
            {
                // Check if Title and Content of the article is not empty
                if (String.IsNullOrEmpty(model.HelpArticle.Title) || String.IsNullOrEmpty(model.HelpArticle.Content))
                {
                    TempData["error"] = "Fill in the information fully";
                    return RedirectToAction(nameof(Article));
                }

                // if Article's Id = 0, means that Creating a new Article
                if (model.HelpArticle.Id == 0)
                {
                    var user = await _userManager.GetUserAsync(User);
                    if (user == null)
                    {
                        TempData["error"] = "Please log in first";
                        return RedirectToAction("Index", "Access", new { area = "" });
                    }

                    // Set some properties for the article
                    model.HelpArticle.Admin = user;
                    model.HelpArticle.CreatedAt = DateTime.UtcNow;

                    // Add Article to DATABASE
                    _unitOfWork.HelpArticle.Add(model.HelpArticle);
                    TempData["success"] = "Article created successfully";
                }
                else // means that Updating a new Article
                {
                    // Get the existing Article in DATABASE
                    HelpArticle existingHelpArticle = await _unitOfWork.HelpArticle.GetAsync(h => h.Id == model.HelpArticle.Id);
                    if (existingHelpArticle == null)
                    {
                        TempData["error"] = "Article not found";
                        return RedirectToAction(nameof(Index));
                    }

                    // Set some new data for the article's properties
                    existingHelpArticle.Title = model.HelpArticle.Title;
                    existingHelpArticle.Content = model.HelpArticle.Content;
                    existingHelpArticle.HelpTypeId = model.HelpArticle.HelpTypeId;
                    existingHelpArticle.UpdatedAt = DateTime.UtcNow;

                    // Update article to DATABASE
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
