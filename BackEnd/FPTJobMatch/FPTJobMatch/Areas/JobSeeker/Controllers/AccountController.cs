using FPT.Models;
using FPT.Models.ViewModels;
using FPT.Utility;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace FPTJobMatch.Areas.JobSeeker.Controllers
{
    [Area("JobSeeker")]
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }
        public IActionResult SignUp()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken] // Prevent Cross-Site Request Forgery (CSRF) attacks
        public async Task<IActionResult> SignUp(JobSeekerRegisterVM model)
        {
            try 
            {
                // Check if the email already exists
                var existingUser = await _userManager.FindByEmailAsync(model.Email);
                if (existingUser != null)
                {
                    ModelState.AddModelError("Email", "Email already exists");
                    return View(model);
                }

                if (ModelState.IsValid)
                {
                    var user = new ApplicationUser
                    {
                        Name = model.Name,
                        UserName = model.Email,
                        Email = model.Email,
                        CreatedAt = DateTime.UtcNow,
                        AccountStatus = SD.StatusActive
                    };
                    // Store user data in AspNetUsers database table
                    var result = await _userManager.CreateAsync(user, model.Password);
                    if (result.Succeeded)
                    {
                        await _userManager.AddToRoleAsync(user, SD.Role_JobSeeker);
                        TempData["success"] = "Sign up successfully";
                        return RedirectToAction("Index", "Access", new { area = "" });
                    }
                    // Handle password-related errors separately
                    foreach (var error in result.Errors)
                    {
                        if (error.Code.StartsWith("Password"))
                        {
                            ModelState.AddModelError("Password", error.Description);
                        }
                        else
                        {
                            ModelState.AddModelError(string.Empty, error.Description);
                        }
                    }
                }

                TempData["error"] = "Sign up again";
                return View(model);
            }
            catch (Exception ex)
            {
                return RedirectToAction("GenericError", "Home", new { area = "", code = 500, errorMessage = ex.Message });
            }
        }
    }
}
