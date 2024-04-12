using FPT.DataAccess.Repository;
using FPT.DataAccess.Repository.IRepository;
using FPT.Models;
using FPT.Models.ViewModels;
using FPT.Utility;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace FPTJobMatch.Areas.Employer.Controllers
{
    [Area("Employer")]
    public class AccountController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        public AccountController(IUnitOfWork unitOfWork, UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _signInManager = signInManager;
        }
        public IActionResult SignUp()
        {
            EmployerRegisterVM employerRegisterVM = new EmployerRegisterVM
            {
                CityList = _unitOfWork.City.GetAll().Select(u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.Id.ToString()
                }),
            };
            return View(employerRegisterVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SignUp(EmployerRegisterVM model)
        {
            if (!ModelState.IsValid)
            {
                PrepareViewModelCityList(model);
                return View(model);
            }

            if (_unitOfWork.Company.IsExist(model.CompanyName))
            {
                ModelState.AddModelError("CompanyName", "Company name already exists");
                PrepareViewModelCityList(model);
                return View(model);
            }

            var user = new ApplicationUser
            {
                UserName = model.Email,
                Email = model.Email,
                Name = model.Name,
                CreatedAt = DateTime.UtcNow,
                AccountStatus = SD.StatusPending,
            };

            var result = await _userManager.CreateAsync(user, model.Password);
            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, SD.Role_Employer);
                // await _signInManager.SignInAsync(user, isPersistent: false);

                var company = new Company
                {
                    Name = model.CompanyName,
                    CityId = model.CityId,
                    CreatedAt = DateTime.UtcNow
                };

                // Associate the company with the user
                user.Company = company;

                // Add company and user to Unit of Work and save changes
                _unitOfWork.Company.Add(company);
                _unitOfWork.Save();

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

            PrepareViewModelCityList(model);
            return View(model);
        }


        private void PrepareViewModelCityList(EmployerRegisterVM model)
        {
            model.CityList = _unitOfWork.City.GetAll().Select(u => new SelectListItem
            {
                Text = u.Name,
                Value = u.Id.ToString()
            });
        }

    }
}
