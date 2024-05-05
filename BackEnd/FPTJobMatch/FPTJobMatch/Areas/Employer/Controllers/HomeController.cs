﻿using FPT.DataAccess.Repository.IRepository;
using FPT.Models;
using FPT.Models.ViewModels;
using FPT.Utility;
using FPT.Utility.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Security.Claims;

namespace FPTJobMatch.Areas.Employer.Controllers
{
    [Area("Employer")]
    [Authorize(Roles = SD.Role_Employer)]
    public class HomeController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<ApplicationUser> _userManager;

        public HomeController(IUnitOfWork unitOfWork, UserManager<ApplicationUser> userManager)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
        }

        // Display jobs
        public async Task<IActionResult> Index(int? jobTypeId, int? pageIndex)
        {
            try { 
                // Get User's ID who signing in the system
                string? currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                // Is User Signed In?
                if (string.IsNullOrEmpty(currentUserId))
                {
                    // If not sign in, navigating to login page
                    return RedirectToAction("Index", "Access", new { area = "" });
                }

                PaginatedList<Job> jobsTask;

                // If user filters by Job Type
                if (jobTypeId.HasValue)
                {
                    ViewBag.JobTypeId = jobTypeId;
                    // j.EmployerId == currentUserId => Jobs of current employer
                    // j.JobTypeId == jobTypeId      => Filter by Job Type
                    // pageIndex: pageIndex ?? 1     => Pagination
                    jobsTask = await _unitOfWork.Job.GetAllFilteredAsync(j => j.EmployerId == currentUserId && j.JobTypeId == jobTypeId, pageIndex: pageIndex ?? 1);
                }
                else
                {
                    jobsTask = await _unitOfWork.Job.GetAllFilteredAsync(j => j.EmployerId == currentUserId, pageIndex: pageIndex ?? 1);
                }

                // Retrieve job types and categories
                var jobTypesList = await _unitOfWork.JobType.GetAllAsync(); // Get All Job Type
                var categoriesList = await _unitOfWork.Category.GetCategoriesByStatus(true); // Get All Categories which are approved by admin

                // Wait for the completion of tasks (Async) 
                // await Task.WhenAll(jobsTask, jobTypesTask, categoriesTask);

                // Using custom View Model
                var viewModel = new JobVM
                {
                    JobList = jobsTask,
                    JobTypeList = jobTypesList.Select(u => new SelectListItem
                    {
                        Text = u.Name,
                        Value = u.Id.ToString()
                    }),
                    CategoryList = categoriesList.Select(u => new SelectListItem
                    {
                        Text = u.Name,
                        Value = u.Id.ToString()
                    }),
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                // Catch Exception and navigate to Error Page
                return RedirectToAction("GenericError", "Error", new { area = "", code = 500, errorMessage = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateJob(JobVM viewModel)
        {
            try 
            { 
                // Is Data valid Or The uploaded Job is empty?
                if (!ModelState.IsValid || viewModel.JobUploadModel == null)
                {
                    TempData["error"] = "Please fill out the form completely";
                    return RedirectToAction("Index");
                }

                // Get Current User
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    return RedirectToAction("Index", "Access", new { area = "" });
                }

                // Create a new Job
                Job newJob = viewModel.JobUploadModel;
                newJob.CreatedAt = DateTime.UtcNow;
                newJob.Employer = user;
                newJob.CompanyId = user.CompanyId;

                // If User wanna create a new category
                if (newJob.Category != null)
                {
                    var existingCategory = await _unitOfWork.Category.GetAsync(c => c.Name.ToLower() == newJob.Category.Name.ToLower());

                    // If user wants to create a new category which is not existed before
                    if (existingCategory == null)
                    {
                        var newCategory = new Category
                        {
                            Name = newJob.Category.Name,
                            IsApproved = false,
                            CreatedAt = DateTime.UtcNow,
                            CreatedByUser = user,
                        };
                        // Add Category to DB
                        _unitOfWork.Category.Add(newCategory);

                        // Get Admin's Data
                        ApplicationUser? admin = await _userManager.FindByEmailAsync("minhbee203@gmail.com");
                        // Create and save notification
                        var notification = new Notification
                        {
                            Content = $"New Job Category named {newJob.Category.Name} just added by {user.Name}.",
                            CreatedAt = DateTime.UtcNow,
                            SenderId = user.Id,
                            ReceiverId = admin?.Id
                        };

                        // Add Notification to DB
                        _unitOfWork.Notification.Add(notification);

                        await _unitOfWork.Save();

                        newJob.Category = newCategory;
                    }
                    else
                    {
                        // Use the existing category
                        newJob.Category = existingCategory;
                    }
                }

                // Add Job to DB
                _unitOfWork.Job.Add(newJob);
                await _unitOfWork.Save();

                TempData["success"] = "Job created successfully";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                return RedirectToAction("GenericError", "Error", new { area = "", code = 500, errorMessage = ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetJob(int id)
        {
            Job job = await _unitOfWork.Job.GetAsync(j => j.Id == id, includeProperties: "Category,JobType");
            int numOfCVs = await _unitOfWork.ApplicantCV.CountCVsAsync(cv => cv.Job == job);
            int numOfNewCVs = await _unitOfWork.ApplicantCV.CountCVsAsync(cv => cv.Job == job && cv.CVStatus == SD.StatusPending);

            return Json(new
            {
                success = true,
                data = new { job, numOfNewCVs, numOfCVs }
            });
        }

        [HttpPost]
        public async Task<IActionResult> DeleteJob(int jobId)
        {
            try
            {
                Job job = await _unitOfWork.Job.GetAsync(j => j.Id == jobId);

                if (job == null)
                {
                    TempData["error"] = "Job not found or already deleted";
                    return RedirectToAction("Index");
                }

                _unitOfWork.Job.Remove(job);
                await _unitOfWork.Save();

                TempData["success"] = "Job deleted successfully";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["error"] = "An error occurred while deleting the job";
                return RedirectToAction("GenericError", "Error", new { area = "", code = 500, errorMessage = ex.Message });
            }
        }
    }

}
