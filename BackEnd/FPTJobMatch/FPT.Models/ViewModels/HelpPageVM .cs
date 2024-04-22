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
    public class HelpPageVM
    {
        public HelpType? HelpType { get; set; }
        [ValidateNever]
        public IEnumerable<HelpType> HelpTypes { get; set; }
        [ValidateNever]
        public IEnumerable<HelpArticle> HelpArticles { get; set; }
    }
}
