using FPT.DataAccess.Repository.IRepository;
using FPT.Models;
using FPT.Models.ViewModels;
using FPT.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FPTJobMatch.Areas.Employer.Controllers
{
    [Area("Employer")]
    [Authorize(Roles = SD.Role_Employer)]
    public class ApplicantController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<ApplicationUser> _userManager;
        public ApplicantController(IUnitOfWork unitOfWork, UserManager<ApplicationUser> userManager)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index(int jobId, string? status, string? sortType)
        {
            try {
                // Get the job
                Job job = await _unitOfWork.Job.GetAsync(j => j.Id == jobId);

                if (job == null)
                {
                    TempData["error"] = "Job not found";
                    return RedirectToAction("Index", "Jobs");
                }

                IEnumerable<ApplicantCV> applicantCVs = await _unitOfWork.ApplicantCV.GetAllJobFilteredAsync(jobId, status, sortType);

                ApplicantPageVM applicantPageVM = new()
                {
                    ApplicantList = applicantCVs,
                    JobId = jobId,
                    JobTitle = job.Title,
                };

                return View(applicantPageVM);
            }
            catch (Exception ex)
            {
                return RedirectToAction("GenericError", "Home", new { area = "", code = 500, errorMessage = ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetCV(string applicantId, int jobId)
        {
            ApplicationUser jobSeeker = await _unitOfWork.ApplicationUser.GetAsync(u => u.Id == applicantId);
            ApplicantCV applicantCV = await _unitOfWork.ApplicantCV.GetAsync(a => a.JobId == jobId && a.JobSeekerId == applicantId);

            if (jobSeeker == null || applicantCV == null)
            {
                return Json(new { success = false, errorMessage = "Applicant or CV not found" });
            }

            return Json(new
            {
                success = true,
                applicant = new
                {
                    Id = jobSeeker.Id,
                    Name = jobSeeker.Name,
                    Email = jobSeeker.Email,
                    PhoneNumber = jobSeeker.PhoneNumber,
                    FileCV = applicantCV.FileCV,
                    CVId = applicantCV.Id,
                    ResponseMessage = applicantCV.ResponseMessage
                }
            });
        }

        [HttpPost]
        public async Task<IActionResult> ResponseCV(int cvId, string applicantId, string responseMessage, int curJobId)
        {
            try { 
                ApplicantCV applicantCV = await _unitOfWork.ApplicantCV.GetAsync(a => a.Id == cvId && a.JobSeekerId == applicantId);

                if (applicantCV == null)
                {
                    TempData["error"] = "Error, Try again!";
                    return RedirectToAction("Index", "Applicant", new { jobId = curJobId });
                }

                if (applicantCV.CVStatus == SD.StatusResponded && applicantCV.ResponseMessage == responseMessage)
                {
                    TempData["error"] = "You have already responded to this CV";
                    return RedirectToAction("Index", "Applicant", new { jobId = curJobId });
                }

                applicantCV.DateResponded = DateTime.UtcNow;
                applicantCV.ResponseMessage = responseMessage;
                applicantCV.CVStatus = SD.StatusResponded;

                _unitOfWork.ApplicantCV.Update(applicantCV);
                _unitOfWork.Save();

                var user = await _userManager.GetUserAsync(User);

                if (user == null)
                {
                    return RedirectToAction("Index", "Access", new { area = "" });
                }

                Notification newNotification = new()
                {
                    ReceiverId = applicantCV.JobSeekerId,
                    Sender = user,
                    CreatedAt = DateTime.UtcNow,
                    Content = $"Your application has been responded to by {user.Name}",
                };

                _unitOfWork.Notification.Add(newNotification);
                _unitOfWork.Save();

                TempData["success"] = "Successfully responded to the CV.";
                return RedirectToAction("Index", "Applicant", new { jobId = curJobId });
            }
            catch (Exception ex)
            {
                return RedirectToAction("GenericError", "Error", new { area = "", code = 500, errorMessage = ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> ApplicantDetail(string applicantId)
        {
            try { 
                ApplicationUser applicant = await _unitOfWork.ApplicationUser.GetAsync(u => u.Id == applicantId);
                JobSeekerDetail jobSeekerDetail = await _unitOfWork.JobSeekerDetail.GetAsync(u => u.JobSeekerId == applicant.Id);

                if (applicant == null || jobSeekerDetail == null)
                {
                    TempData["error"] = "Applicant not found";
                    return NotFound();
                }

                JobSeekerProfileVM jobSeekerProfileVM = new()
                {
                    JobSeeker = applicant,
                    JobSeekerDetail = jobSeekerDetail,
                };
                return View(jobSeekerProfileVM);
            }
            catch (Exception ex)
            {
                return RedirectToAction("GenericError", "Error", new { area = "", code = 500, errorMessage = ex.Message });
            }
        }
    }

}
