using FPT.DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Mvc;

namespace FPTJobMatch.Controllers
{
    public class JobsController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public JobsController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Detail()
        {
            return View();
        }
    }
}
