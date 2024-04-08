using FPT.Models;
using FPT.Models.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

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
                var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, lockoutOnFailure: false);
                if (result.Succeeded)
                {
                    // Get the user
                    var user = await _userManager.FindByEmailAsync(model.Email);
                    if (user != null)
                    {
                        // Get the roles associated with the user
                        var roles = await _userManager.GetRolesAsync(user);
                        if (roles.Contains("JobSeeker"))
                        {
                            return RedirectToAction("Index", "JobSeeker");
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

                    // If user does not have any roles, redirect to default page
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                    return View(model);
                }
            }

            // Model state is invalid, return back
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> LogOut()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
    }
}
