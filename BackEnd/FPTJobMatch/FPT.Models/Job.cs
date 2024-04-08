using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FPT.Models
{
    public class Job
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string? Address { get; set; }
        public string? Description { get; set; }
        public string? Responsibility { get; set; }
        public string? Experience { get; set; }
        public string? AdditionalDetail { get; set; }
        public DateTime CreatedAt { get; set; }


        public int CategoryId { get; set; }
        [ForeignKey("CategoryId")]
        public Category Category { get; set; }


        public int ApplicationUserId { get; set; }
        [ForeignKey("ApplicationUserId")]
        public ApplicationUser ApplicationUser { get; set; }

        public int CompanyId { get; set; }
        [ForeignKey("CompanyId")]
        public Company Company { get; set; }

        public int JobTypeId { get; set; }
        [ForeignKey("JobTypeId")]
        public JobType JobType { get; set; }
    }
}
