using FPT.DataAccess.Repository.IRepository;
using FPT.Models;
using FPT.Models.ViewModels;
using FPT.Utility;
using FPT.Utility.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FPTJobMatch.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class HomeController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<ApplicationUser> _userManager;

        public HomeController(IUnitOfWork unitOfWork, UserManager<ApplicationUser> userManager)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index(string? userType, string? sortType, string? keyword, int? pageIndex)
        {
            try
            {
                PaginatedList<ApplicationUser> users = await _unitOfWork.ApplicationUser.GetFilteredUsersAsync(userType, sortType, keyword, pageIndex ?? 1);
                return View(users);
            }
            catch (Exception ex)
            {
                return RedirectToAction("GenericError", "Error", new { area = "", code = 500, errorMessage = ex.Message });
            }
        }

        public async Task<IActionResult> CreateUser(string fullname, string? phone, string email, string? company)
        {
            try
            {
                // Check if required information is provided
                if (String.IsNullOrEmpty(fullname) || String.IsNullOrEmpty(email))
                {
                    TempData["error"] = "Please fill in all required information";
                    return RedirectToAction("Index");
                }

                // Check if the email already exists
                var existingUser = await _userManager.FindByEmailAsync(email);
                if (existingUser != null)
                {
                    TempData["error"] = "Email already exists";
                    return RedirectToAction("Index");
                }

                // Check if the company already exists
                if (!string.IsNullOrEmpty(company) && await _unitOfWork.Company.IsExistAsync(company))
                {
                    TempData["error"] = "The company already exists";
                    return RedirectToAction("Index");
                }

                // Create a new user
                var newUser = new ApplicationUser
                {
                    UserName = email,
                    Email = email,
                    Name = fullname,
                    PhoneNumber = phone,
                    AccountStatus = SD.StatusActive,
                    CreatedAt = DateTime.UtcNow,
                    EmailConfirmed = true
                };

                // Set default password
                const string defaultPassword = "Aa12345.";
                var result = await _userManager.CreateAsync(newUser, defaultPassword);

                if (!result.Succeeded)
                {
                    TempData["error"] = "Failed to create user";
                    return RedirectToAction("Index");
                }

                // Check if the user role should be "Employer" and the company is not null
                if (!string.IsNullOrEmpty(company))
                {
                    // Create a new company if it doesn't exist
                    var newCompany = new Company
                    {
                        Name = company,
                        CreatedAt = DateTime.UtcNow,
                        Employer = newUser
                    };
                    _unitOfWork.Company.Add(newCompany);

                    // Associate the new company with the user
                    newUser.Company = newCompany;

                    await _userManager.AddToRoleAsync(newUser, SD.Role_Employer);
                }
                else
                {
                    await _userManager.AddToRoleAsync(newUser, SD.Role_JobSeeker);
                }

                _unitOfWork.Save();

                TempData["success"] = "Create user successfully";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                return RedirectToAction("GenericError", "Error", new { area = "", code = 500, errorMessage = ex.Message });
            }
        }



        [HttpPost]
        public async Task<IActionResult> StatusChange(string userId, string status)
        {
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                return RedirectToAction("Index");
            }

            if (user.AccountStatus == status)
            {
                return RedirectToAction("Index");
            }

            try
            {
                string successMessage = string.Empty;

                switch (status)
                {
                    case SD.StatusActive:
                        user.AccountStatus = SD.StatusActive;
                        successMessage = "Change Status to Active Successfully";
                        break;

                    case SD.StatusSuspending:
                        user.AccountStatus = SD.StatusSuspending;
                        successMessage = "Change Status to Suspend Successfully";
                        break;

                    case "Delete":
                        await HandleUserDeletionAsync(user);
                        _unitOfWork.Save();

                        TempData["success"] = "User deleted successfully";
                        return RedirectToAction("Index");

                    default:
                        return RedirectToAction("Index");
                }

                // Create and save notification
                var notification = new Notification
                {
                    Content = $"Your account status has been changed to {status}.",
                    CreatedAt = DateTime.UtcNow,
                    SenderId = User.FindFirstValue(ClaimTypes.NameIdentifier),
                    ReceiverId = user.Id
                };

                _unitOfWork.Notification.Add(notification);

                _unitOfWork.Save();

                TempData["success"] = successMessage;
            }
            catch (Exception ex)
            {
                return RedirectToAction("GenericError", "Home", new { area = "", code = 500, errorMessage = ex.Message });
            }

            return RedirectToAction("Index");
        }

        private async Task HandleUserDeletionAsync(ApplicationUser user)
        {
            if (await _userManager.IsInRoleAsync(user, SD.Role_Employer))
            {
                await _unitOfWork.Category.NullifyCreatedByUserIdAsync(user.Id);
                //await _unitOfWork.Company.RemoveByEmployerIdAsync(user.Id);
                await _unitOfWork.Job.RemoveRangeByEmployerIdAsync(user.Id);
            }
            else
            {
                await _unitOfWork.JobSeekerDetail.RemoveByUserIdAsync(user.Id);
            }

            await _unitOfWork.Notification.RemoveBySenderIdAsync(user.Id);
            await _unitOfWork.Notification.RemoveByReceiverIdAsync(user.Id);

            _unitOfWork.ApplicationUser.Remove(user);
        }

        [HttpPost]
        public async Task<IActionResult> ResetPassword(string userId, string newPassword)
        {
            // Check for null or empty userId
            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Index");
            }

            // Check for null or empty newPassword
            if (String.IsNullOrEmpty(newPassword))
            {
                TempData["error"] = "Password can not be empty";
                return RedirectToAction("Index");
            }

            try
            {
                // Find the user
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    return RedirectToAction("Index");
                }

                // Reset the user's password
                var resetToken = await _userManager.GeneratePasswordResetTokenAsync(user);
                var result = await _userManager.ResetPasswordAsync(user, resetToken, newPassword);

                if (result.Succeeded)
                {
                    TempData["success"] = "Password reset successfully";
                    return RedirectToAction("Index");
                }
                else
                {
                    TempData["error"] = "Failed to reset password";
                    return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                return RedirectToAction("GenericError", "Home", new { area = "", code = 500, errorMessage = ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> UserDetail(string userId)
        {
            try
            {
                ApplicationUser user = await _unitOfWork.ApplicationUser.GetAsync(u => u.Id == userId);

                if (user == null)
                {
                    TempData["error"] = "User not found";
                    return RedirectToAction(nameof(Index));
                }

                if (await _userManager.IsInRoleAsync(user, SD.Role_Employer))
                {
                    Company company = await _unitOfWork.Company.GetAsync(c => c.EmployerId == user.Id, includeProperties: "City");
                    ViewBag.Company = company;
                    ViewBag.Role = "Employer";
                } else
                {
                    JobSeekerDetail jobSeekerDetail = await _unitOfWork.JobSeekerDetail.GetAsync(u => u.JobSeekerId == user.Id);
                    ViewBag.JobSeekerDetail = jobSeekerDetail;
                    ViewBag.Role = "JobSeeker";
                } 

                return View(user);
            }
            catch (Exception ex)
            {
                return RedirectToAction("GenericError", "Error", new { area = "", code = 500, errorMessage = ex.Message });
            }
        }

    }
}
