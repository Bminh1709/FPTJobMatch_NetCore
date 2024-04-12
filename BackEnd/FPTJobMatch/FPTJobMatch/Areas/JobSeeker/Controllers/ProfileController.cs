using FPT.DataAccess.Repository.IRepository;
using FPT.Models;
using FPT.Models.ViewModels;
using FPT.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FPTJobMatch.Areas.JobSeeker.Controllers
{
    [Area("JobSeeker")]
    [Authorize(Roles = SD.Role_JobSeeker)]
    public class ProfileController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public ProfileController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            // Get the ID of the current user
            string? currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Retrieve the user data and related information
            ApplicationUser currentUser = _unitOfWork.ApplicationUser.Get(u => u.Id == currentUserId);
            JobSeekerDetail jobSeekerDetail = _unitOfWork.JobSeekerDetail.Get(d => d.JobSeekerId == currentUserId);
            IEnumerable<ApplicantCV> applicantCVsList = _unitOfWork.ApplicantCV.GetAll(cv => cv.JobSeekerId == currentUserId, includeProperties: "Job");

            // Create the view model
            JobSeekerProfileVM jobSeekerProfileVM = new JobSeekerProfileVM
            {
                JobSeeker = currentUser,
                JobSeekerDetail = jobSeekerDetail,
                applicantCVsList = applicantCVsList
            };

            // Pass the view model to the view
            return View(jobSeekerProfileVM);
        }

    }
}
