using FPT.Models;
using FPT.Models.ViewModels;
using FPT.Utility;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FPTJobMatch.Controllers
{
    public class AccessController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public AccessController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Index(SignInVM model)
        {
            try
            {
                // Is Info Valid?
                if (!ModelState.IsValid)
                {
                    return View(model);
                }

                // Find User by Email
                var user = await _userManager.FindByEmailAsync(model.Email);

                // Is Email Existed?
                if (user == null)
                {
                    ModelState.AddModelError(string.Empty, "Incorrect Email");
                    return View(model);
                }

                // Check if the email is confirmed
                if (!user.EmailConfirmed)
                {
                    // Redirect to the VerifyEmail action
                    return RedirectToAction("VerifyEmail", "Access", new { email = model.Email });
                }

                // Check Account Status
                if (user.AccountStatus == SD.StatusSuspending)
                {
                    ModelState.AddModelError(string.Empty, "This account has been suspended");
                    return View(model);
                }
                if (user.AccountStatus == SD.StatusPending)
                {
                    ModelState.AddModelError(string.Empty, "This account is still processing");
                    return View(model);
                }

                // Sign In into System
                var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, lockoutOnFailure: false);

                // Is SignIn Success?
                if (!result.Succeeded)
                {
                    ModelState.AddModelError(string.Empty, "Incorrect Email or Password");
                    return View(model);
                }

                // Get role and Navigate to Route
                var role = await _userManager.GetRolesAsync(user);
                var roleAreas = new Dictionary<string, string>
                    {
                        { SD.Role_JobSeeker, "" },
                        { SD.Role_Employer, "Employer" },
                        { SD.Role_Admin, "Admin" }
                    };

                if (roleAreas.ContainsKey(role.Single()))
                {
                    return RedirectToAction("Index", "Home", new { area = roleAreas[role.Single()] });
                }

                // Default redirect if role is not found
                return RedirectToAction("Index", "Home");

            }
            catch (Exception ex)
            {
                return RedirectToAction("GenericError", "Error", new { area = "", code = 500, errorMessage = ex.Message });
            }
        }

        public async Task<IActionResult> LogOut()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public async Task<IActionResult> ResetPassword(string oldPassword, string newPassword, string confirmNewPassword)
        {
            try
            {
                // Check for null values
                if (string.IsNullOrEmpty(oldPassword) || string.IsNullOrEmpty(newPassword) || string.IsNullOrEmpty(confirmNewPassword))
                {
                    return Json(new { success = false, error = "All fields are required" });
                }

                // -- First way to get user --
                //string? currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                //ApplicationUser user = await _userManager.FindByIdAsync(currentUserId);
                // -- Second way to get user --
                var user = await _userManager.GetUserAsync(User);

                // Check if the user exists
                if (user == null)
                {
                    return Json(new { success = false, error = "User not found" });
                }

                // Check if the old password matches the user's current password
                var passwordCheck = await _userManager.CheckPasswordAsync(user, oldPassword);
                if (!passwordCheck)
                {
                    return Json(new { success = false, error = "Incorrect old password" });
                }

                // Check if the new password matches the confirm new password
                if (newPassword != confirmNewPassword)
                {
                    return Json(new { success = false, error = "New password and confirm new password do not match" });
                }

                //// Hash the new password using Identity's password hasher
                //var passwordHasher = new PasswordHasher<ApplicationUser>();
                //var hashedNewPassword = passwordHasher.HashPassword(user, newPassword);

                //// Update the user's password hash in the database
                //user.PasswordHash = hashedNewPassword;
                //await _userManager.UpdateAsync(user);

                //TempData["success"] = "Password updated successfully";
                //return Json(new { success = true });

                // Reset the user's password
                var result = await _userManager.ResetPasswordAsync(user, await _userManager.GeneratePasswordResetTokenAsync(user), newPassword);

                if (!result.Succeeded)
                {
                    return Json(new { success = false, error = "Failed to update password" });
                }

                TempData["success"] = "Password updated successfully";
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return RedirectToAction("GenericError", "Home", new { area = "", code = 500, errorMessage = ex.Message });
            }
        }


        public IActionResult VerifyEmail(string email)
        {
            ViewBag.EmailVerify = email;
            return View();
        }

        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            try
            {
                if (userId == null || token == null)
                {
                    return RedirectToAction("GenericError", "Home", new { area = "", code = 400, errorMessage = "Invalid parameters: userId or token is null" });
                }

                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    return RedirectToAction("GenericError", "Home", new { area = "", code = 404, errorMessage = "User not found" });
                }

                var result = await _userManager.ConfirmEmailAsync(user, token);
                if (result.Succeeded)
                {
                    return RedirectToAction("Index", "Access", new { area = "" });
                }
                else
                {
                    return RedirectToAction("GenericError", "Home", new { area = "", code = 500, errorMessage = "Failed to confirm email" });
                }
            }
            catch (Exception ex)
            {
                return RedirectToAction("GenericError", "Home", new { area = "", code = 500, errorMessage = ex.Message });
            }
        }
    }
}
