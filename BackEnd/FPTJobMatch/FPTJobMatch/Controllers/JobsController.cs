using FPT.DataAccess.Repository.IRepository;
using FPT.Models;
using FPT.Models.ViewModels;
using FPT.Utility;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Security.Claims;

namespace FPTJobMatch.Controllers
{
    public class JobsController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public JobsController(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment)
        {
            _unitOfWork = unitOfWork;
            _webHostEnvironment = webHostEnvironment;
        }
        public IActionResult Index(int? cityId, int? jobtypeId, string? keyword)
        {
            string? jobSeekerId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            IQueryable<Job> jobs = _unitOfWork.Job.GetAll(includeProperties: "Company.City,JobType,Category").AsQueryable();
            // Filter by city
            if (cityId.HasValue)
            {
                jobs = jobs.Where(j => j.Company.CityId == cityId);
            }

            // Filter by job type
            if (jobtypeId.HasValue)
            {
                jobs = jobs.Where(j => j.JobTypeId == jobtypeId);
            }

            // Filter by keyword
            if (!string.IsNullOrEmpty(keyword))
            {
                ViewBag.Keywords = keyword;
                jobs = jobs.Where(j => j.Title.Contains(keyword, StringComparison.OrdinalIgnoreCase));
            }

            JobPageVM jobPageVM = new JobPageVM
            {
                JobList = jobs.ToList(),
                CityList = _unitOfWork.City.GetAll().Select(u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.Id.ToString(),
                    Selected = (u.Id == cityId) // Select the current city if provided
                }),
                JobTypeList = _unitOfWork.JobType.GetAll().Select(u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.Id.ToString(),
                    Selected = (u.Id == jobtypeId) // Select the current job type if provided
                }),
                JobSeeker = _unitOfWork.ApplicationUser.Get(u => u.Id == jobSeekerId)
            };

            return View(jobPageVM);
        }

        [HttpGet]
        public IActionResult GetJob(int id)
        {
            Job job = _unitOfWork.Job.Get(j => j.Id == id, includeProperties: "Company.City,JobType,Category");
            return Json( new
            {
                success = true,
                data =job
            });
        }

        [HttpPost]
        public IActionResult SubmitCV(int jobId, IFormFile fileCV)
        {
            string? jobSeekerId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(jobSeekerId))
            {
                TempData["error"] = "User not authenticated";
                return Json(new
                {
                    success = false,
                    errorMessage = "User not authenticated"
                });
            }

            bool isSubmitted = _unitOfWork.ApplicantCV.IsSubmittedLast30Days(jobId, jobSeekerId);

            if (isSubmitted)
            {
                TempData["error"] = "You have already applied for this job within the last 30 days";
                return Json(new
                {
                    success = false,
                    errorMessage = "You have already applied for this job within the last 30 days"
                });
            }

            if (fileCV == null || fileCV.Length == 0)
            {
                TempData["error"] = "No CV file uploaded";
                return Json(new
                {
                    success = false,
                    errorMessage = "No CV file uploaded"
                });
            }

            string wwwRootPath = _webHostEnvironment.WebRootPath;
            try
            {
                // Generate a unique file name to prevent duplicating
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(fileCV.FileName);

                // Folder contained CVs
                string fileCVPath = @"filecv\";
                string finalPath = Path.Combine(wwwRootPath, fileCVPath);

                if (!Directory.Exists(finalPath))
                {
                    Directory.CreateDirectory(finalPath);
                }

                // Combine the final path with the file name
                var filePath = Path.Combine(finalPath, fileName);

                // Save the file to the specified path
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    fileCV.CopyTo(stream);
                }

                // Create data for applicant CV
                ApplicantCV applicantCV = new ApplicantCV
                {
                    DateSubmitted = DateTime.UtcNow,
                    FileCV = fileName,
                    JobSeekerId = jobSeekerId,
                    JobId = jobId,
                    CVStatus = SD.StatusPending
                };
                _unitOfWork.ApplicantCV.Add(applicantCV);
                _unitOfWork.Save();

                TempData["success"] = "Application submitted successfully";
                return Json(new
                {
                    success = true,
                    errorMessage = "Application submitted successfully"
                });
            }
            catch (Exception ex)
            {
                TempData["error"] = ex.Message;
                return Json(new { success = false, errorMessage = ex.Message });
            }
        }

        public IActionResult Detail()
        {
            return View();
        }
    }
}
