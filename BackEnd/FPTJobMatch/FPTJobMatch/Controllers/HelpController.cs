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
        public async Task<IActionResult> Index(string? keyword)
        {
            try
            {
                var articles = await _unitOfWork.HelpArticle.GetAllArticlesFilteredAsync(null, keyword);

                HelpPageVM helpPageVM = new()
                {
                    HelpTypes = await _unitOfWork.HelpType.GetAllAsync(),
                    HelpArticles = articles.Take(10),
                };

                ViewBag.Keyword = keyword;
                return View(helpPageVM);
            }
            catch (Exception ex)
            {
                return RedirectToAction("GenericError", "Error", new { area = "", code = 500, errorMessage = ex.Message });
            }
        }

        public async Task<IActionResult> Collections(int typeId)
        {
            try
            {
                HelpType helpType = await _unitOfWork.HelpType.GetAsync(t => t.Id == typeId);
                IEnumerable<HelpArticle> helpArticles = await _unitOfWork.HelpArticle.GetAllAsync(a => a.HelpTypeId == typeId);

                ViewBag.HelpType = helpType;
                return View(helpArticles);
            }
            catch (Exception ex)
            {
                return RedirectToAction("GenericError", "Error", new { area = "", code = 500, errorMessage = ex.Message });
            }
        }

        public async Task<IActionResult> Article(int articleId)
        {
            try
            {
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
