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
    public class JobVM
    {
        // List of Jobs for displaying
        [ValidateNever]
        public PaginatedList<Job> JobList { get; set; }
        // Job for creating
        public Job JobUploadModel { get; set; }

        [ValidateNever]
        public IEnumerable<SelectListItem> JobTypeList { get; set; }
        [ValidateNever]
        public IEnumerable<SelectListItem> CategoryList { get; set; }
    }
}
