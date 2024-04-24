using FPT.DataAccess.Repository.IRepository;
using FPT.Models;
using FPT.Models.ViewModels;
using FPT.Utility;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
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

        public async Task<IActionResult> Index(int? cityId, int? jobtypeId, string? keyword, int? pageIndex)
        {
            try { 
                string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                // Call GetAllFilteredAsync method to get filtered jobs
                var jobList = await _unitOfWork.Job.GetAllFilteredAsync(
                    c => c.Category.IsApproved == true,
                    includeProperties: "Company.City,JobType,Category",
                    cityId: cityId,
                    jobtypeId: jobtypeId,
                    keyword: keyword,
                    pageIndex: pageIndex ?? 1
                );

                ViewBag.Keywords = keyword;
                ViewBag.CityId = cityId;
                ViewBag.JobtypeId = jobtypeId;

                var cityList = await _unitOfWork.City.GetAllAsync();
                var jobTypeList = await _unitOfWork.JobType.GetAllAsync();

                var jobPageVM = new JobPageVM
                {
                    JobList = jobList,
                    CityList = cityList.Select(u => new SelectListItem
                    {
                        Text = u.Name,
                        Value = u.Id.ToString(),
                        Selected = (u.Id == cityId) // Select the current city if provided
                    }),
                    JobTypeList = jobTypeList.Select(u => new SelectListItem
                    {
                        Text = u.Name,
                        Value = u.Id.ToString(),
                        Selected = (u.Id == jobtypeId) // Select the current job type if provided
                    }),
                    JobSeeker = await _unitOfWork.ApplicationUser.GetAsync(u => u.Id == userId)
                };

                return View(jobPageVM);
            }
            catch (Exception ex)
            {
                return RedirectToAction("GenericError", "Error", new { area = "", code = 500, errorMessage = ex.Message });
            }
        }


        [HttpGet]
        public async Task<IActionResult> GetJob(int id)
        {
            var job = await _unitOfWork.Job.GetAsync(j => j.Id == id, includeProperties: "Company.City,JobType,Category");
            return Json(new
            {
                success = true,
                data = job
            });
        }

        [HttpPost]
        public async Task<IActionResult> SubmitCV(int jobId, IFormFile fileCV)
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

            bool isSubmitted = await _unitOfWork.ApplicantCV.IsSubmittedLast30DaysAsync(jobId, jobSeekerId);

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

            try
            {
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(fileCV.FileName);
                var fileCVPath = @"filecv\";
                var finalPath = Path.Combine(_webHostEnvironment.WebRootPath, fileCVPath);

                if (!Directory.Exists(finalPath))
                {
                    Directory.CreateDirectory(finalPath);
                }

                var filePath = Path.Combine(finalPath, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await fileCV.CopyToAsync(stream);
                }

                var applicantCV = new ApplicantCV
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

        public async Task<IActionResult> Detail(int jobId)
        {
            try
            {
                // Retrieve job data by jobId
                Job job = await _unitOfWork.Job.GetAsync(j => j.Id == jobId, includeProperties: "Company.City,Category,JobType");

                if (job == null)
                {
                    return NotFound();
                }

                return View(job);
            }
            catch (Exception ex)
            {
                return RedirectToAction("GenericError", "Error", new { area = "", code = 500, errorMessage = ex.Message });
            }
        }
    }


}
