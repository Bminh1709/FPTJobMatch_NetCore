using FPT.Utility.Helpers;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FPT.Models.ViewModels
{
    public class EmployerRegisterVM
    {
        [Required(ErrorMessage = "Name is required")]
        [StringLength(1000, MinimumLength = 5, ErrorMessage = "You have to enter full name")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress]
        public string Email { get; set; }
        [Required(ErrorMessage = "Password is required")]
        [MinLength(8, ErrorMessage = "At least 8 characters long")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required(ErrorMessage = "Confirm Password is required")]
        [Display(Name = "Confirm password")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Passwords do not match")]
        public string ConfirmPassword { get; set; }
        [Required(ErrorMessage = "Company Name is required")]
        public string CompanyName { get; set; }
        [Required(ErrorMessage = "City is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Please select a city")]
        public int CityId { get; set; }
        [CheckBoxRequired(ErrorMessage = "Please read and accept the Terms of Use and Policies")]
        public bool isPolicyAgreed { get; set; }
        [ValidateNever]
        public IEnumerable<SelectListItem> CityList { get; set; } 
    }
}
