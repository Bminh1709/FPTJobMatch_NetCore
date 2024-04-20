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

        public async Task<IActionResult> Index()
        {
            try {
                var user = await _userManager.GetUserAsync(User);

                if (user == null)
                {
                    TempData["error"] = "User not found";
                    return RedirectToAction("Index", "Access", new { area = "" });
                }

                JobSeekerDetail jobSeekerDetail = await _unitOfWork.JobSeekerDetail.GetAsync(d => d.JobSeekerId == user.Id);
                IEnumerable<ApplicantCV> applicantCVsList = await _unitOfWork.ApplicantCV.GetAllAsync(cv => cv.JobSeekerId == user.Id, includeProperties: "Job.Employer,Job.Company");

                JobSeekerProfileVM jobSeekerProfileVM = new JobSeekerProfileVM
                {
                    JobSeeker = user,
                    JobSeekerDetail = jobSeekerDetail,
                    ApplicantCVsList = applicantCVsList,
                };

                return View(jobSeekerProfileVM);
            }
            catch (Exception ex)
            {
                return RedirectToAction("GenericError", "Error", new { area = "", code = 500, errorMessage = ex.Message });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateInfo(string aboutMe, string fullname, string phone, DateOnly dateOfBirth, string gender, string address)
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);

                if (user == null)
                {
                    TempData["error"] = "User not found";
                    return RedirectToAction("Index", "Access", new { area = "" });
                }

                // Retrieve or create the JobSeekerDetail for the user
                JobSeekerDetail jobSeekerDetail = await _unitOfWork.JobSeekerDetail.GetAsync(u => u.JobSeeker == user) ?? new JobSeekerDetail { JobSeeker = user };

                // Update JobSeekerDetail properties
                jobSeekerDetail.AboutMe = aboutMe;
                jobSeekerDetail.DateOfBirth = dateOfBirth;
                jobSeekerDetail.Gender = gender;
                jobSeekerDetail.Address = address;

                _unitOfWork.JobSeekerDetail.Update(jobSeekerDetail);

                // Update user's name and phone number if provided
                if (!string.IsNullOrEmpty(fullname))
                {
                    user.Name = fullname;
                }
                if (!string.IsNullOrEmpty(phone))
                {
                    user.PhoneNumber = phone;
                }

                _unitOfWork.ApplicationUser.Update(user);
                _unitOfWork.Save();

                TempData["success"] = "Update Info Successfully";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                return RedirectToAction("GenericError", "Error", new { area = "", code = 500, errorMessage = ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetCV(int cvId)
        {
            ApplicantCV applicantCV = await _unitOfWork.ApplicantCV.GetAsync(a => a.Id == cvId);
            return Json(new
            {
                success = true,
                data = new { applicantCV.Id, applicantCV.FileCV, applicantCV.ResponseMessage }
            });
        }
    }

}
