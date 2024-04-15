using FPT.DataAccess.Repository.IRepository;
using FPT.Models;
using FPT.Models.ViewModels;
using FPT.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FPTJobMatch.Areas.JobSeeker.Controllers
{
    [Area("JobSeeker")]
    [Authorize(Roles = SD.Role_JobSeeker)]
    public class ProfileController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUnitOfWork _unitOfWork;
        public ProfileController(UserManager<ApplicationUser> userManager, IUnitOfWork unitOfWork)
        {
            _userManager = userManager;
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            // Get the ID of the current user
            string? currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Retrieve the user data and related information
            ApplicationUser currentUser = _unitOfWork.ApplicationUser.Get(u => u.Id == currentUserId);
            JobSeekerDetail jobSeekerDetail = _unitOfWork.JobSeekerDetail.Get(d => d.JobSeekerId == currentUserId);
            IEnumerable<ApplicantCV> applicantCVsList = _unitOfWork.ApplicantCV.GetAll(cv => cv.JobSeekerId == currentUserId, includeProperties: "Job.Employer,Job.Company");

            // Create the view model
            JobSeekerProfileVM jobSeekerProfileVM = new JobSeekerProfileVM
            {
                JobSeeker = currentUser,
                JobSeekerDetail = jobSeekerDetail,
                applicantCVsList = applicantCVsList,
            };

            // Pass the view model to the view
            return View(jobSeekerProfileVM);
        }

        [HttpPost]
        public IActionResult UpdateInfo(string aboutMe, string fullname, string phone, DateOnly dateOfBirth, string gender, string address)
        {
            string? currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Check if the JobSeekerDetail exists
            JobSeekerDetail jobSeekerDetail = _unitOfWork.JobSeekerDetail.Get(u => u.JobSeekerId == currentUserId);
            if (jobSeekerDetail == null)
            {
                // Create a new JobSeekerDetail if it doesn't exist
                jobSeekerDetail = new JobSeekerDetail
                {
                    JobSeekerId = currentUserId
                };
            }

            // Update the JobSeekerDetail properties
            jobSeekerDetail.AboutMe = aboutMe;
            jobSeekerDetail.DateOfBirth = dateOfBirth;
            jobSeekerDetail.Gender = gender;
            jobSeekerDetail.Address = address;

            // Update or add the JobSeekerDetail in the database
            _unitOfWork.JobSeekerDetail.Update(jobSeekerDetail);

            // Update the ApplicationUser (jobSeeker) properties if fullname or phone is provided
            if (!string.IsNullOrEmpty(fullname) || !string.IsNullOrEmpty(phone))
            {
                ApplicationUser jobSeeker = _unitOfWork.ApplicationUser.Get(u => u.Id == currentUserId);
                if (jobSeeker != null)
                {
                    jobSeeker.Name = fullname ?? jobSeeker.Name; // Keep the existing name if fullname is null
                    jobSeeker.PhoneNumber = phone ?? jobSeeker.PhoneNumber; // Keep the existing phone number if phone is null

                    _unitOfWork.ApplicationUser.Update(jobSeeker);
                }
            }

            _unitOfWork.Save();
            TempData["success"] = "Update Info Successfully";
            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult GetCV(int cvId)
        {
            ApplicantCV applicantCV = _unitOfWork.ApplicantCV.Get(a => a.Id == cvId);
            return Json(new
            {
                success = true,
                data = new { applicantCV.Id, applicantCV.FileCV, applicantCV.ResponseMessage }
            });
        }


    }
}
