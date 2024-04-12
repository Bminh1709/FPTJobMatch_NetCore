using FPT.Models;
using FPT.Models.ViewModels;
using FPT.Utility;
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
    }
}
