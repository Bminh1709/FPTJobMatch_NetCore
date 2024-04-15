using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FPT.Models
{
    public class ApplicantCV
    {
        [Key]
        public int Id { get; set; }
        public DateTime DateSubmitted { get; set; }
        public DateTime? DateResponded { get; set; }
        public string? ResponseMessage { get; set; }
        [Required(ErrorMessage = "File is required")]
        public string FileCV { get; set; }

        public string JobSeekerId { get; set; }
        [ForeignKey("JobSeekerId")]
        public ApplicationUser JobSeeker { get; set; }

        public string? CVStatus { get; set; }

        public int JobId { get; set; }
        [ForeignKey("JobId")]
        public Job Job { get; set; }
    }
}
