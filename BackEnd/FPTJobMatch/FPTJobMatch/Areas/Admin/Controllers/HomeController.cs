using FPT.DataAccess.Repository.IRepository;
using FPT.Models;
using FPT.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace FPTJobMatch.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class HomeController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<ApplicationUser> _userManager;
        public HomeController(IUnitOfWork unitOfWork, UserManager<ApplicationUser> userManager)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
        }
        public IActionResult Index(string? userType)
        {
            IEnumerable<ApplicationUser> users = _unitOfWork.ApplicationUser.GetAll();

            if (userType == SD.Role_JobSeeker)
            {
                users = users.Where(u => _userManager.IsInRoleAsync(u, SD.Role_JobSeeker).Result);
            }
            else if (userType == SD.Role_Employer)
            {
                users = users.Where(u => _userManager.IsInRoleAsync(u, SD.Role_Employer).Result);
            }

            // Exclude users with the role 'Admin'
            users = users.Where(u => !_userManager.IsInRoleAsync(u, SD.Role_Admin).Result);

            return View(users);
        }



        [HttpPost]
        public async Task<IActionResult> StatusChange(string id, string status)
        {
            ApplicationUser? user = await _userManager.FindByIdAsync(id);

            if (user == null)
            {
                return RedirectToAction("Index");
            }

            if (user.AccountStatus == status)
            {
                return RedirectToAction("Index");
            }

            try
            {
                if (status == SD.StatusActive) // Activate
                {
                    TempData["success"] = "Change Status to Active Successfully";
                    user.AccountStatus = SD.StatusActive;
                }
                else if (status == SD.StatusSuspending) // Suspend
                {
                    TempData["success"] = "Change Status to Suspend Successfully";
                    user.AccountStatus = SD.StatusSuspending;
                }
                else if (status == "Delete") // Delete
                {
                    // Delete the user
                    _unitOfWork.ApplicationUser.Remove(user);
                    if (user.CompanyId != null)
                    {
                        Company company = _unitOfWork.Company.Get(c => c.Id == user.CompanyId);
                        _unitOfWork.Company.Remove(company);
                    }
                    _unitOfWork.Save();

                    TempData["success"] = "Delete User Successfully";
                    return RedirectToAction("Index");
                }

                _unitOfWork.ApplicationUser.Update(user);
                _unitOfWork.Save();
            }
            catch (Exception ex)
            {
                // Handle exceptions (e.g., log the error, display an error message)
                ModelState.AddModelError(string.Empty, "An error occurred while processing the request.");
                // Redirect to an error page or display an appropriate error message
                TempData["error"] = "An error occurred";
                return RedirectToAction("Index");
            }

            return RedirectToAction("Index");
        }

    }
}
