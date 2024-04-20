using FPT.DataAccess.Repository.IRepository;
using FPT.Models;
using FPT.Utility;
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

        public async Task<IActionResult> Index(string? userType, string? sortType, string? keyword)
        {
            try
            {
                var users = await _unitOfWork.ApplicationUser.GetAllAsync();

                // Filter users based on userType
                if (!string.IsNullOrEmpty(userType))
                {
                    ViewBag.UserType = userType;
                    var role = userType switch
                    {
                        SD.Role_JobSeeker => SD.Role_JobSeeker,
                        SD.Role_Employer => SD.Role_Employer,
                        _ => null
                    };

                    if (role != null)
                    {
                        users = users.Where(u => _userManager.IsInRoleAsync(u, role).Result);
                    }
                }

                // Exclude Admin
                users = users.Where(u => !_userManager.IsInRoleAsync(u, SD.Role_Admin).Result);

                // Sort users based on sortType
                if (!string.IsNullOrEmpty(sortType))
                {
                    ViewBag.SortType = sortType;
                    if (sortType == "NewestFirst")
                    {
                        users = users.OrderByDescending(u => u.CreatedAt);
                    }
                    else if (sortType == "OldestFirst")
                    {
                        users = users.OrderBy(u => u.CreatedAt);
                    }
                }

                // Filter users based on keyword
                if (!string.IsNullOrEmpty(keyword))
                {
                    ViewBag.Keyword = keyword;
                    users = users.Where(u => u.Name.ToLower().Contains(keyword.ToLower()) || u.Email.ToLower().Contains(keyword.ToLower()));
                }

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
                    CreatedAt = DateTime.UtcNow
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
                        CreatedAt = DateTime.UtcNow
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
                        // Delete the user
                        _unitOfWork.ApplicationUser.Remove(user);
                        if (user.CompanyId != null)
                        {
                            var company = await _unitOfWork.Company.GetAsync(c => c.Id == user.CompanyId);
                            _unitOfWork.Company.Remove(company);
                        }
                        _unitOfWork.Save();

                        successMessage = "Delete User Successfully";
                        return RedirectToAction("Index");

                    default:
                        return RedirectToAction("Index");
                }

                // Update user status
                _unitOfWork.ApplicationUser.Update(user);
                _unitOfWork.Save();

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

    }
}
