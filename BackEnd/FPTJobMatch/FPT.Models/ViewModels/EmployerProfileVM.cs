using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FPT.Models.ViewModels
{
    public class EmployerProfileVM
    {
        public ApplicationUser Employer { get; set; }
        public Company Company { get; set; }
        [ValidateNever]
        public IEnumerable<SelectListItem> CityList { get; set; }
    }
}
