using FPT.Models.ViewModels;
using FPT.Models;
using FPT.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using FPT.DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace FPTJobMatch.Areas.Employer.Controllers
{
    [Area("Employer")]
    [Authorize(Roles = SD.Role_Employer)]
    public class ProfileController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUnitOfWork _unitOfWork;

        public ProfileController(UserManager<ApplicationUser> userManager, IUnitOfWork unitOfWork)
        {
            _userManager = userManager;
            _unitOfWork = unitOfWork;
        }
        public async Task<IActionResult> Index()
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);

                if (user == null)
                {
                    TempData["error"] = "User not found";
                    return RedirectToAction("Index", "Access", new { area = "" });
                }

                Company company = await _unitOfWork.Company.GetAsync(c => c.Id == user.CompanyId);
                IEnumerable<SelectListItem> cities = (await _unitOfWork.City.GetAllAsync()).Select(u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.Id.ToString()
                });

                EmployerProfileVM employerProfileVM = new()
                {
                    Employer = user,
                    Company = company,
                    CityList = cities
                };

                return View(employerProfileVM);
            }
            catch (Exception ex)
            {
                return RedirectToAction("GenericError", "Error", new { area = "", code = 500, errorMessage = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> UpdateCompany(EmployerProfileVM model)
        {
            try
            {
                var existingCompany = await _unitOfWork.Company.GetAsync(c => c.Id == model.Company.Id);

                if (existingCompany == null)
                {
                    return NotFound(); 
                }

                // Check if the company name is being changed
                if (existingCompany.Name != model.Company.Name)
                {
                    TempData["error"] = "Contact the admin to change the company name";
                    return RedirectToAction("Index");
                }

                // Update the company information
                existingCompany.About = model.Company.About;
                existingCompany.Size = model.Company.Size;
                existingCompany.CityId = model.Company.CityId;
                existingCompany.Address = model.Company.Address;

                _unitOfWork.Company.Update(existingCompany);
                _unitOfWork.Save();

                TempData["success"] = "Company information updated successfully";
                return RedirectToAction("Index"); 
            }
            catch (Exception ex)
            {
                return RedirectToAction("GenericError", "Error", new { area = "", code = 500, errorMessage = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> UpdateInfo(EmployerProfileVM model)
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);

                // Check if user exists
                if (user == null)
                {
                    TempData["error"] = "User not found";
                    return RedirectToAction("Index", "Access", new { area = "" });
                }

                // Update user information if submitted values are not null
                if (!string.IsNullOrWhiteSpace(model.Employer.Name))
                {
                    user.Name = model.Employer.Name;
                }

                if (!string.IsNullOrWhiteSpace(model.Employer.PhoneNumber))
                {
                    user.PhoneNumber = model.Employer.PhoneNumber;
                }

                var result = await _userManager.UpdateAsync(user);

                // Check if user update was successful
                if (!result.Succeeded)
                {
                    // Return error message if user update failed
                    TempData["error"] = "Failed to update user information";
                    return RedirectToAction("Index");
                }

                // Return success message if user update was successful
                TempData["success"] = "User information updated successfully";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                return RedirectToAction("GenericError", "Error", new { area = "", code = 500, errorMessage = ex.Message });
            }
        }
    }
}
