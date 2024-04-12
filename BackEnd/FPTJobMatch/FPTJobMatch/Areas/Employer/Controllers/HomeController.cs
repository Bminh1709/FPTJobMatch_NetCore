using FPT.DataAccess.Repository.IRepository;
using FPT.Models;
using FPT.Models.ViewModels;
using FPT.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Security.Claims;

namespace FPTJobMatch.Areas.Employer.Controllers
{
    [Area("Employer")]
    [Authorize(Roles = SD.Role_Employer)]
    public class HomeController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<ApplicationUser> _userManager;
        public HomeController(IUnitOfWork unitOfWork, UserManager<ApplicationUser> userManager)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
        }
        public IActionResult Index()
        {
            // Get the ID of the current user
            string? currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Filter jobs by the current user's ID
            IEnumerable<Job> jobs = _unitOfWork.Job.GetAll(includeProperties: "Company")
                                                .Where(j => j.EmployerId == currentUserId);

            var viewModel = new JobVM
            {
                Jobs = jobs,
                JobUploadModel = new Job(),
                JobTypeList = _unitOfWork.JobType.GetAll().Select(u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.Id.ToString()
                }),
                CategoryList = _unitOfWork.Category.GetAll().Select(u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.Id.ToString()
                }),
            };

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> CreateJob(JobVM viewModel)
        {
            if (!ModelState.IsValid || viewModel.JobUploadModel == null)
            {
                return RedirectToAction("Index");
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Index");
            }

            Job newJob = viewModel.JobUploadModel;
            newJob.CreatedAt = DateTime.UtcNow;
            newJob.EmployerId = user.Id;
            newJob.CompanyId = user.CompanyId;

            // Create or update the category
            if (newJob.Category != null)
            {
                var category = new Category
                {
                    Name = newJob.Category.Name,
                    IsApproved = false,
                    CreatedAt = DateTime.UtcNow
                };

                _unitOfWork.Category.Add(category);
                newJob.Category = category;
            }

            _unitOfWork.Job.Add(newJob);
            _unitOfWork.Save();

            TempData["success"] = "Create Job Successfully";
            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult GetJob(int id)
        {
            // Get Job's info
            Job job = _unitOfWork.Job.Get(j => j.Id == id, includeProperties: "Category,JobType,Company.City");
            //Company company = _unitOfWork.Company.Get(c => c.Id == job.CompanyId, includeProperties: "City");
            //string cityName = company.City.Name;
            // Get Total Number of CVs
            int numOfCVs = _unitOfWork.ApplicantCV.CountCVs(cv => cv.Job == job);
            int numOfNewCVs = _unitOfWork.ApplicantCV.CountCVs(cv => cv.Job == job && cv.CVStatus == SD.StatusPending);
            return Json(new { 
                success = true, 
                data = new { job, numOfNewCVs, numOfCVs } 
            });
        }
       
    }
}
