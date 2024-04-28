using FPT.DataAccess.Repository.IRepository;
using FPT.Models;
using FPT.Models.ViewModels;
using FPT.Utility;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;

namespace FPTJobMatch.Areas.JobSeeker.Controllers
{
    [Area("JobSeeker")]
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IEmailSender _emailSender;
        public AccountController(UserManager<ApplicationUser> userManager, IUnitOfWork unitOfWork, IEmailSender emailSender)
        {
            _userManager = userManager;
            _unitOfWork = unitOfWork;
            _emailSender = emailSender;
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

                        JobSeekerDetail jobSeekerDetail = new()
                        {
                            JobSeeker = user,
                        };

                        _unitOfWork.JobSeekerDetail.Add(jobSeekerDetail);
                        _unitOfWork.Save();

                        // Send Email for verifying
                        var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                        var confirmationLink = Url.Action("ConfirmEmail", "Access", new { area = "", userId = user.Id, token }, Request.Scheme);
                        var emailBody = $"Please confirm your email by clicking <a href=\"{confirmationLink}\">here</a>.";
                        await _emailSender.SendEmailAsync(user.Email, "Confirm your email", emailBody);

                        TempData["success"] = "Sign up successfully";
                        return RedirectToAction("VerifyEmail", "Access", new { area = "", email = model.Email });
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
