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
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user != null)
                {
                    if (user.AccountStatus == SD.StatusSuspending)
                    {
                        ModelState.AddModelError(string.Empty, "This account has been suspended.");
                        return View(model);
                    }
                    else if (user.AccountStatus == SD.StatusPending)
                    {
                        ModelState.AddModelError(string.Empty, "This account is still processing.");
                        return View(model);
                    }

                    var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, lockoutOnFailure: false);

                    if (result.Succeeded)
                    {
                        // Get the roles associated with the user
                        var roles = await _userManager.GetRolesAsync(user);
                        if (roles.Contains("JobSeeker"))
                        {
                            return RedirectToAction("Index", "Home");
                        }
                        else if (roles.Contains("Employer"))
                        {
                            return RedirectToAction("Index", "Employer");
                        }
                        else if (roles.Contains("Admin"))
                        {
                            return RedirectToAction("Index", "Admin");
                        }
                    }

                }
            }

            ModelState.AddModelError(string.Empty, "Incorrect Email or Password");
            // Model state is invalid, return back
            return View(model);
        }

        public async Task<IActionResult> LogOut()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        public IActionResult AccessDenied()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ResetPassword(string oldPassword, string newPassword, string confirmNewPassword)
        {
            // Check for null values
            if (string.IsNullOrEmpty(oldPassword) || string.IsNullOrEmpty(newPassword) || string.IsNullOrEmpty(confirmNewPassword))
            {
                return Json(new { success = false, error = "All fields are required" });
            }

            string? currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            ApplicationUser user = await _userManager.FindByIdAsync(currentUserId);

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

            // Hash the new password using Identity's password hasher
            var passwordHasher = new PasswordHasher<ApplicationUser>();
            var hashedNewPassword = passwordHasher.HashPassword(user, newPassword);

            // Update the user's password hash in the database
            user.PasswordHash = hashedNewPassword;
            await _userManager.UpdateAsync(user);

            TempData["success"] = "Password updated successfully";
            return Json(new
            {
                success = false
            });
        }
    }
}
