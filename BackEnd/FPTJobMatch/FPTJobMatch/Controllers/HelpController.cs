using FPT.DataAccess.Repository.IRepository;
using FPT.Models;
using FPT.Models.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using static Microsoft.Extensions.Logging.EventSource.LoggingEventSource;

namespace FPTJobMatch.Controllers
{
    public class HelpController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public HelpController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // localhost:7283/Help
        // Purpose: Viewing all types and some top articles
        public async Task<IActionResult> Index(string? keyword) // keyword is the title that user wanna search
        {
            try
            {
                // Get all Articles with the keyword
                var articles = await _unitOfWork.HelpArticle.GetAllArticlesFilteredAsync(null, keyword);

                // Instead of passing all articles to View
                // I also need to pass all types of articles
                HelpPageVM helpPageVM = new()
                {
                    // Get all types of articles
                    HelpTypes = await _unitOfWork.HelpType.GetAllAsync(),
                    // Just Get First 10 articles
                    HelpArticles = articles.Take(10),
                };

                // Pass the keyword to View so that on the searching input, we can see the text we typed
                ViewBag.Keyword = keyword;
                return View(helpPageVM);
            }
            catch (Exception ex)
            {
                // Catching all errors
                return RedirectToAction("GenericError", "Error", new { area = "", code = 500, errorMessage = ex.Message });
            }
        }

        // localhost:7283/Help/Collections/typedId=?? At here we need a type's ID
        // Purpose: Viewing all articles that related to the Type
        public async Task<IActionResult> Collections(int typeId)
        {
            try
            {
                // Get that Type of articles
                HelpType helpType = await _unitOfWork.HelpType.GetAsync(t => t.Id == typeId);

                // Also get all articles that related to the Type
                IEnumerable<HelpArticle> helpArticles = await _unitOfWork.HelpArticle.GetAllAsync(a => a.HelpTypeId == typeId);

                // Pass the helpType to View
                ViewBag.HelpType = helpType;

                return View(helpArticles);
            }
            catch (Exception ex)
            {
                return RedirectToAction("GenericError", "Error", new { area = "", code = 500, errorMessage = ex.Message });
            }
        }

        // localhost:7283/Help/Article/articleId=?? At here we need a article's ID
        // Purpose: Viewing specific Article
        public async Task<IActionResult> Article(int articleId)
        {
            try
            {
                // Get that article by articleId
                HelpArticle helpArticle = await _unitOfWork.HelpArticle.GetAsync(a => a.Id == articleId, includeProperties: "HelpType");

                return View(helpArticle);
            }
            catch (Exception ex)
            {
                return RedirectToAction("GenericError", "Error", new { area = "", code = 500, errorMessage = ex.Message });
            }
        }
    }
}
