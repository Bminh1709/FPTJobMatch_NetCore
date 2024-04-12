using FPT.Utility.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FPT.Models.ViewModels
{
    public class JobSeekerRegisterVM
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
        [CheckBoxRequired(ErrorMessage = "Please read and accept the Terms of Use and Policies")]
        public bool isPolicyAgreed { get; set; }
    }
}
