using FPT.DataAccess.Repository.IRepository;
using FPT.Models;
using FPT.Models.ViewModels;
using FPT.Utility;
using FPT.Utility.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
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
        private readonly IWebHostEnvironment _webHostEnvironment;

        public HomeController(IUnitOfWork unitOfWork, UserManager<ApplicationUser> userManager, IWebHostEnvironment webHostEnvironment)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _webHostEnvironment = webHostEnvironment;
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

        [HttpPost]
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
                    await _unitOfWork.Save();

                    // Associate the new company with the user
                    newUser.CompanyId = newCompany.Id;
                    await _unitOfWork.Save();

                    await _userManager.AddToRoleAsync(newUser, SD.Role_Employer);
                }
                else
                {
                    JobSeekerDetail jobSeekerDetail = new()
                    {
                        JobSeeker = newUser,
                    };

                    _unitOfWork.JobSeekerDetail.Add(jobSeekerDetail);
                    await _unitOfWork.Save();
                    await _userManager.AddToRoleAsync(newUser, SD.Role_JobSeeker);
                }


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
            // Get that User
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                return RedirectToAction("Index");
            }

            // If account status is not changed, return right away
            if (user.AccountStatus == status)
            {
                return RedirectToAction("Index");
            }

            try
            {
                string successMessage = string.Empty;

                switch (status)
                {
                    // Activate Account
                    case SD.StatusActive:
                        user.AccountStatus = SD.StatusActive;
                        successMessage = "Change Status to Active Successfully";
                        break;
                    // Suspend Account
                    case SD.StatusSuspending:
                        user.AccountStatus = SD.StatusSuspending;
                        successMessage = "Change Status to Suspend Successfully";
                        break;
                    // Delete Account
                    case "Delete":
                        // CTRL + Click to this function to view the code
                        await HandleUserDeletionAsync(user);
                        await _unitOfWork.Save();

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

                await _unitOfWork.Save();

                TempData["success"] = successMessage;
            }
            catch (Exception ex)
            {
                return RedirectToAction("GenericError", "Error", new { area = "", code = 500, errorMessage = ex.Message });
            }

            return RedirectToAction("Index");
        }

        private async Task HandleUserDeletionAsync(ApplicationUser user)
        {
            // If Deleting an Employer
            if (await _userManager.IsInRoleAsync(user, SD.Role_Employer))
            {
                // Mark the Category which is created by the Employer null
                await _unitOfWork.Category.NullifyCreatedByUserIdAsync(user.Id);
                //await _unitOfWork.Company.RemoveByEmployerIdAsync(user.Id);

                // Remove all Jobs which are created by the Employer
                await _unitOfWork.Job.RemoveRangeByEmployerIdAsync(user.Id);

                // Get the Employer's Company for deleting logo in the system
                Company company = await _unitOfWork.Company.GetAsync(c => c.EmployerId  == user.Id);

                // Delete the Company's logo
                if (!string.IsNullOrEmpty(company.Logo))
                {
                    var logoPath = Path.Combine(_webHostEnvironment.WebRootPath, "images", "companyLogo", company.Logo);
                    if (System.IO.File.Exists(logoPath))
                    {
                        System.IO.File.Delete(logoPath);
                    }
                }
            }
            else
            {
                // If Deleting a JobSeeker
                await _unitOfWork.JobSeekerDetail.RemoveByUserIdAsync(user.Id);
            }

            // Delete all notifications
            await _unitOfWork.Notification.RemoveBySenderIdAsync(user.Id);
            await _unitOfWork.Notification.RemoveByReceiverIdAsync(user.Id);

            // Delete the user's avatar
            if (!string.IsNullOrEmpty(user.Avatar))
            {
                var avatarPath = Path.Combine(_webHostEnvironment.WebRootPath, "images", "avatar", user.Avatar);
                if (System.IO.File.Exists(avatarPath))
                {
                    System.IO.File.Delete(avatarPath);
                }
            }


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
