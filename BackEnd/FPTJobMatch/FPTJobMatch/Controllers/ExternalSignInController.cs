using FPT.DataAccess.Repository.IRepository;
using FPT.Models;
using FPT.Utility;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Win32;
using System.Security.Claims;

namespace FPTJobMatch.Controllers
{
    public class ExternalSignInController : Controller
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUnitOfWork _unitOfWork;
        public ExternalSignInController(SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager, IUnitOfWork unitOfWork)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _unitOfWork = unitOfWork;

        }

        // Initiate the process of external authentication with Google
        // Redirect users to Google for authentication
        public IActionResult ExternalLogin(string provider)
        {
            var redirectUrl = Url.Action("ExternalCallback", "ExternalSignIn");
            var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
            return Challenge(properties, provider);
        }

        public async Task<IActionResult> ExternalCallback()
        {
            var info = await _signInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                // Handle error
                return RedirectToAction("Index", "Access");
            }

            var result = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: false, bypassTwoFactor: true);
            if (result.Succeeded)
            {
                return RedirectToAction("Index", "Home");
            }
            if (result.IsLockedOut)
            {
                return RedirectToPage("./ Lockout");
            }
            else
            {
                // If the user doesn't exist, create a new user based on the email provided by the external login provider
                if (info.Principal.HasClaim(c => c.Type == ClaimTypes.Email))
                {
                    var name = info.Principal.FindFirstValue(ClaimTypes.Name);
                    var email = info.Principal.FindFirstValue(ClaimTypes.Email);

                    // Check if the user already exists
                    var existingUser = await _userManager.FindByEmailAsync(email);
                    if (existingUser != null)
                    {
                        TempData["success"] = "Sign in successfully";
                        await _signInManager.SignInAsync(existingUser, isPersistent: false);
                        return RedirectToAction("Index", "Home");
                    }

                    await CreateUserAsync(name, email);
                }

                // Redirect to the appropriate action (e.g., a page for completing registration)
                return RedirectToAction("Index", "Home");
            }
        }

        private async Task CreateUserAsync(string name, string email)
        {
            // Create a new user with the provided email
            var user = new ApplicationUser { UserName = email, Email = email, Name = name, AccountStatus = SD.StatusActive, CreatedAt = DateTime.UtcNow };

            // Attempt to create the user
            var result = await _userManager.CreateAsync(user);
            if (result.Succeeded)
            {
                // User creation succeeded, assign the "JobSeeker" role
                await _userManager.AddToRoleAsync(user, SD.Role_JobSeeker);

                JobSeekerDetail jobSeekerDetail = new()
                {
                    JobSeeker = user,
                };

                _unitOfWork.JobSeekerDetail.Add(jobSeekerDetail);
                _unitOfWork.Save();

                // Set success message
                TempData["success"] = "Sign up successfully";

                await _signInManager.SignInAsync(user, isPersistent: false);
            }
            else
            {
                // User creation failed, set error message
                TempData["error"] = "Failed to create user";
            }
        }
    }
}
