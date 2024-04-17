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

        public async Task<IActionResult> Index()
        {
            try { 
                string? currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                // Is User Signed In?
                if (string.IsNullOrEmpty(currentUserId))
                {
                    return RedirectToAction("Index", "Access", new { area = "" });
                }

                var jobsTask = _unitOfWork.Job.GetAllAsync(j => j.EmployerId == currentUserId, includeProperties: "Company");

                // Retrieve job types and categories
                var jobTypesTask = _unitOfWork.JobType.GetAllAsync();
                var categoriesTask = _unitOfWork.Category.GetAllAsync();

                // Wait for the completion of tasks (Async) 
                await Task.WhenAll(jobsTask, jobTypesTask, categoriesTask);

                var viewModel = new JobVM
                {
                    JobList = jobsTask.Result,
                    JobTypeList = jobTypesTask.Result.Select(u => new SelectListItem
                    {
                        Text = u.Name,
                        Value = u.Id.ToString()
                    }),
                    CategoryList = categoriesTask.Result.Select(u => new SelectListItem
                    {
                        Text = u.Name,
                        Value = u.Id.ToString()
                    }),
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                return RedirectToAction("GenericError", "Error", new { area = "", code = 500, errorMessage = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateJob(JobVM viewModel)
        {
            try 
            { 
                // Is Data valid Or The uploaded Job is empty?
                if (!ModelState.IsValid || viewModel.JobUploadModel == null)
                {
                    TempData["error"] = "Please fill out the form completely";
                    return RedirectToAction("Index");
                }

                // Get Current User
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    return RedirectToAction("Index", "Access", new { area = "" });
                }

                // Create a new Job
                Job newJob = viewModel.JobUploadModel;
                newJob.CreatedAt = DateTime.UtcNow;
                newJob.Employer = user;
                newJob.Company = user.Company;

                // If User wanna create a new category
                if (newJob.Category != null)
                {
                    var newCategory = new Category
                    {
                        Name = newJob.Category.Name,
                        IsApproved = false,
                        CreatedAt = DateTime.UtcNow,
                        CreatedByUser = user,
                    };

                    _unitOfWork.Category.Add(newCategory);
                    newJob.Category = newCategory;
                }

                _unitOfWork.Job.Add(newJob);
                _unitOfWork.Save();

                TempData["success"] = "Job created successfully";
                return RedirectToAction("Index");
            }
            catch (Exception)
            {
                TempData["error"] = "An error occurred while creating the job";
                return RedirectToAction("Index");
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetJob(int id)
        {
            Job job = await _unitOfWork.Job.GetAsync(j => j.Id == id, includeProperties: "Category,JobType,Company.City");
            int numOfCVs = await _unitOfWork.ApplicantCV.CountCVsAsync(cv => cv.Job == job);
            int numOfNewCVs = await _unitOfWork.ApplicantCV.CountCVsAsync(cv => cv.Job == job && cv.CVStatus == SD.StatusPending);

            return Json(new
            {
                success = true,
                data = new { job, numOfNewCVs, numOfCVs }
            });
        }
    }

}
