using FPT.DataAccess.Repository;
using FPT.DataAccess.Repository.IRepository;
using FPT.Models;
using FPT.Models.ViewModels;
using FPT.Utility;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace FPTJobMatch.Areas.Employer.Controllers
{
    [Area("Employer")]
    public class AccountController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IEmailSender _emailSender;

        public AccountController(IUnitOfWork unitOfWork, UserManager<ApplicationUser> userManager, IEmailSender emailSender)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _emailSender = emailSender;
        }

        public async Task<IActionResult> SignUp()
        {
            var employerRegisterVM = new EmployerRegisterVM();
            await PrepareVMCityList(employerRegisterVM);
            return View(employerRegisterVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SignUp(EmployerRegisterVM model)
        {
            try
            {
                // Is Data Valid
                if (!ModelState.IsValid)
                {
                    await PrepareVMCityList(model);
                    return View(model);
                }

                // Is Email Exist
                var existingUser = await _userManager.FindByEmailAsync(model.Email);
                if (existingUser != null)
                {
                    ModelState.AddModelError("Email", "Email already exists");
                    await PrepareVMCityList(model);
                    return View(model);
                }

                // Is Company Exist
                if (await _unitOfWork.Company.IsExistAsync(model.CompanyName))
                {
                    ModelState.AddModelError("CompanyName", "Company name already exists");
                    await PrepareVMCityList(model);
                    return View(model);
                }

                // Create A New User in System
                var user = new ApplicationUser
                {
                    UserName = model.Email,
                    Email = model.Email,
                    Name = model.Name,
                    CreatedAt = DateTime.UtcNow,
                    AccountStatus = SD.StatusPending,
                };

                // Create User By Built-in Method "CreateAsync"
                var result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, SD.Role_Employer); // Assign Role for User
                    var company = new Company // Create a New Company
                    {
                        Name = model.CompanyName,
                        CityId = model.CityId,
                        CreatedAt = DateTime.UtcNow,
                        Employer = user
                    };

                    // Associate the company with the user
                    user.Company = company;

                    // Add Company to DB
                    _unitOfWork.Company.Add(company);
                    _unitOfWork.Save();


                    // Send Email for verifying
                    var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    var confirmationLink = Url.Action("ConfirmEmail", "Access", new { area = "", userId = user.Id, token }, Request.Scheme);
                    var emailBody = $"Please confirm your email by clicking <a href=\"{confirmationLink}\">here</a>.";
                    await _emailSender.SendEmailAsync(user.Email, "Confirm your email", emailBody);

                    TempData["success"] = "Sign up successfully";
                    return RedirectToAction("VerifyEmail", "Access", new { area = "", email = model.Email });
                }

                // Display Errors
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

                await PrepareVMCityList(model);
                return View(model);
            }
            catch (Exception ex)
            {
                return RedirectToAction("GenericError", "Error", new { area = "", code = 500, errorMessage = ex.Message });
            }
        }

        private async Task PrepareVMCityList(EmployerRegisterVM model)
        {
            var cities = await _unitOfWork.City.GetAllAsync();
            model.CityList = cities.Select(u => new SelectListItem
            {
                Text = u.Name,
                Value = u.Id.ToString()
            });
        }
    }

}
