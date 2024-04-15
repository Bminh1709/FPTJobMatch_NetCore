using FPT.DataAccess.Repository.IRepository;
using FPT.Models;
using FPT.Models.ViewModels;
using FPT.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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
        public IActionResult Index(int jobId, string? status, string? sortType)
        {
            Job job = _unitOfWork.Job.Get(j => j.Id == jobId);

            if (job == null)
            {
                // Return a view with an error message if the job is not found
                TempData["error"] = "Job not found";
                return RedirectToAction("Index", "Jobs");
            }

            IQueryable<ApplicantCV> applicantCVs = _unitOfWork.ApplicantCV.GetAll(a => a.JobId == jobId, includeProperties: "JobSeeker").AsQueryable();

            if (status != "All" && !string.IsNullOrEmpty(status))
            {
                applicantCVs = applicantCVs.Where(a => a.CVStatus == status);
            }

            if (!string.IsNullOrEmpty(sortType))
            {
                if (sortType == "Newest First")
                {
                    applicantCVs = applicantCVs.OrderByDescending(a => a.DateSubmitted);
                }
                else if (sortType == "Oldest First")
                {
                    applicantCVs = applicantCVs.OrderBy(a => a.DateSubmitted);
                }
            }

            ApplicantPageVM applicantPageVM = new ApplicantPageVM
            {
                ApplicantList = applicantCVs.ToList(),
                JobId = jobId,
                JobTitle = job.Title,
            };

            return View(applicantPageVM);
        }

        [HttpGet]
        public IActionResult GetCV(string applicantId, int jobId)
        {
            ApplicationUser jobSeeker = _unitOfWork.ApplicationUser.Get(u => u.Id == applicantId);
            ApplicantCV applicantCV = _unitOfWork.ApplicantCV.Get(a => a.JobId == jobId && a.JobSeekerId == applicantId);
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
        public IActionResult ResponseCV(int cvId, string applicantId, string responseMessage, int curJobId)
        {
            ApplicantCV applicantCV = _unitOfWork.ApplicantCV.Get(a => a.Id == cvId && a.JobSeekerId == applicantId);

            if (applicantCV != null)
            {
                if (applicantCV.CVStatus == SD.StatusResponded && applicantCV.ResponseMessage == responseMessage)
                {
                    TempData["error"] = "You already responsed this CV";
                    return RedirectToAction("Index", "Applicant", new { jobId = curJobId });
                }

                applicantCV.DateResponded = DateTime.UtcNow;
                applicantCV.ResponseMessage = responseMessage;
                applicantCV.CVStatus = SD.StatusResponded;

                _unitOfWork.ApplicantCV.Update(applicantCV);
                _unitOfWork.Save();

                TempData["success"] = "Response the CV Successfully";
                return RedirectToAction("Index", "Applicant", new { jobId = curJobId });
            }
            else
            {
                TempData["error"] = "Error, Try again!";
                return RedirectToAction("Index", "Applicant", new { jobId = curJobId });
            }
        }

        public IActionResult ApplicantDetail(string applicantId)
        {
            ApplicationUser applicant = _unitOfWork.ApplicationUser.Get(u => u.Id == applicantId);
            JobSeekerDetail jobSeekerDetail = _unitOfWork.JobSeekerDetail.Get(u => u.JobSeekerId == applicant.Id);

            JobSeekerProfileVM jobSeekerProfileVM = new JobSeekerProfileVM
            {
                JobSeeker = applicant,
                JobSeekerDetail = jobSeekerDetail,
            };
            return View(jobSeekerProfileVM);
        }
    }
}
