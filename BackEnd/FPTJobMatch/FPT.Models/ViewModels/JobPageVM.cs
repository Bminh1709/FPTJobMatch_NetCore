using FPT.Utility.Helpers;
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
    public class JobPageVM
    {
        public ApplicationUser JobSeeker { get; set; }
        [ValidateNever]
        public IEnumerable<SelectListItem> CityList { get; set; }
        [ValidateNever]
        public IEnumerable<SelectListItem> JobTypeList { get; set; }
        [ValidateNever]
        public PaginatedList<Job> JobList { get; set; }
    }
}
