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
    public class ApplicantPageVM
    {
        public int JobId { get; set; }
        public string JobTitle { get; set; }
        [ValidateNever]
        public IEnumerable<ApplicantCV> ApplicantList { get; set; }
    }
}
