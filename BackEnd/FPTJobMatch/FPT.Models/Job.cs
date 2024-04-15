using FPT.Utility.Helpers;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FPT.Models
{
    public class Job
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Title is required")]
        public string Title { get; set; }
        [Required(ErrorMessage = "Address is required")]
        public string Address { get; set; }
        public string? Description { get; set; }
        public string? Responsibility { get; set; }
        public string? Experience { get; set; }
        public string? AdditionalDetail { get; set; }
        public DateTime CreatedAt { get; set; }

        [Required(ErrorMessage = "Deadline is required")]
        [GreaterThanToday(ErrorMessage = "The deadline must be greater than today's date")]
        public DateOnly Deadline { get; set; }

        public int? CategoryId { get; set; }
        [ForeignKey("CategoryId")]
        public Category? Category { get; set; }

        public string? EmployerId { get; set; }
        [ForeignKey("EmployerId")]
        public ApplicationUser? Employer { get; set; }

        public int? CompanyId { get; set; }
        [ForeignKey("CompanyId")]
        public Company? Company { get; set; }

        [Required(ErrorMessage = "Job Type is required")]
        public int JobTypeId { get; set; }
        [ForeignKey("JobTypeId")]
        public JobType? JobType { get; set; }
    }
}
