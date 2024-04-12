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
    public class CategoryVM
    {
        public int ApprovedCount;
        public int NewCount;
        public int ApprovedThisMonthCount;
        public int NewThisMonthCount;
        [ValidateNever]
        public IEnumerable<Category> ApprovedCategortList { get; set; }
        [ValidateNever]
        public IEnumerable<Category> NewCategoryList { get; set; }
    }
}
