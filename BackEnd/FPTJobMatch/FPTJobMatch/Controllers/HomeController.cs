using FPT.DataAccess.Repository.IRepository;
using FPT.Models;
using FPT.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

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
            IEnumerable<Job> jobs = _unitOfWork.Job.GetAll(c => c.Category.IsApproved == true ,includeProperties: "Category,Company.City,JobType").Take(12).ToList();
            IEnumerable<Category> categories = _unitOfWork.Category.GetAll(c => c.IsApproved == true).Take(4).ToList();
            ViewBag.Categories = categories;
            return View(jobs);
        }

        public IActionResult SignIn()
        {
            return View();
        }

        public IActionResult Help()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
