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
            IEnumerable<SelectListItem> cities = _unitOfWork.City.GetAll().Select(u => new SelectListItem
            {
                Text = u.Name,
                Value = u.Id.ToString()
            });
            return View(cities);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SignUp(EmployerRegisterVM model)
        {
            if (ModelState.IsValid)
            {
                // Check if the company name already exists
                bool isCompanyNameExists = _unitOfWork.Company.IsExist(model.CompanyName);
                if (isCompanyNameExists)
                {
                    ModelState.AddModelError(string.Empty, "Company name already exists. Please choose a different company name.");
                    return View(model);
                }
                var user = new ApplicationUser
                {
                    Name = model.Name,
                    UserName = model.Email,
                    Email = model.Email,
                    CreatedAt = DateTime.Now,
                    Status = new Status
                    {
                        Name = SD.StatusPending
                    }
                };
                // Store user data in AspNetUsers database table
                var result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, SD.Role_Employer);
                    await _signInManager.SignInAsync(user, isPersistent: false);

                    var company = new Company
                    {
                        Name = model.Name,
                        ApplicationUser = user,
                        CityId = model.CityId,
                        IsApproved = false
                    };
                    _unitOfWork.Company.Add(company);
                    _unitOfWork.Save();
                    return RedirectToAction("Index", "Home");
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            return View(model);
        }
    }
}
