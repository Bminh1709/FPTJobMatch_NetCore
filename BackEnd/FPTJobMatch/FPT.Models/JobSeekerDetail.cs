using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FPT.Models
{
    public class JobSeekerDetail
    {
        [Key]
        public int Id { get; set; }
        public string JobSeekerId { get; set; }
        [ForeignKey("JobSeekerId")]
        public ApplicationUser JobSeeker { get; set; }
        public string? Gender { get; set; }
        public DateOnly? DateOfBirth { get; set; }
        public string? Address { get; set; }
        public string? AboutMe { get; set; }
    }
}
