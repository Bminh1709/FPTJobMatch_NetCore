using FPT.DataAccess.Repository.IRepository;
using FPT.Models;
using FPT.Models.ViewModels;
using FPT.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FPTJobMatch.Areas.Employer.Controllers
{
    [Area("Employer")]
    [Authorize(Roles = SD.Role_Employer)]
    public class ApplicantController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public ApplicantController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IActionResult> Index(int jobId, string? status, string? sortType)
        {
            try { 
                Job job = await _unitOfWork.Job.GetAsync(j => j.Id == jobId);

                if (job == null)
                {
                    TempData["error"] = "Job not found";
                    return RedirectToAction("Index", "Jobs");
                }

                IEnumerable<ApplicantCV> applicantCVs = await _unitOfWork.ApplicantCV.GetAllByJobIdAsync(jobId, status, sortType);

                ApplicantPageVM applicantPageVM = new ApplicantPageVM
                {
                    ApplicantList = applicantCVs.ToList(),
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
                    TempData["error"] = "You already responded to this CV";
                    return RedirectToAction("Index", "Applicant", new { jobId = curJobId });
                }

                applicantCV.DateResponded = DateTime.UtcNow;
                applicantCV.ResponseMessage = responseMessage;
                applicantCV.CVStatus = SD.StatusResponded;

                _unitOfWork.ApplicantCV.Update(applicantCV);
                _unitOfWork.Save();

                TempData["success"] = "Responded to the CV Successfully";
                return RedirectToAction("Index", "Applicant", new { jobId = curJobId });
            }
            catch (Exception ex)
            {
                return RedirectToAction("GenericError", "Home", new { area = "", code = 500, errorMessage = ex.Message });
            }
        }

        public async Task<IActionResult> ApplicantDetail(string applicantId)
        {
            try { 
                ApplicationUser applicant = await _unitOfWork.ApplicationUser.GetAsync(u => u.Id == applicantId);
                JobSeekerDetail jobSeekerDetail = await _unitOfWork.JobSeekerDetail.GetAsync(u => u.JobSeekerId == applicant.Id);

                if (applicant == null || jobSeekerDetail == null)
                {
                    TempData["error"] = "Applicant or Job Seeker Detail not found";
                    return RedirectToAction("Index", "Applicant");
                }

                JobSeekerProfileVM jobSeekerProfileVM = new JobSeekerProfileVM
                {
                    JobSeeker = applicant,
                    JobSeekerDetail = jobSeekerDetail,
                };
                return View(jobSeekerProfileVM);
            }
            catch (Exception ex)
            {
                return RedirectToAction("GenericError", "Home", new { area = "", code = 500, errorMessage = ex.Message });
            }
        }
    }

}
