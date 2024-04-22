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
    public class Company
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public string? About { get; set; }
        public string? Address { get; set; }
        public int? Size { get; set; }
        public string? Logo { get; set; }
        public DateTime CreatedAt { get; set; }

        public int? CityId { get; set; }
        [ForeignKey("CityId")]
        public City City { get; set; }

        public string? EmployerId { get; set; }
        [ForeignKey("EmployerId")]
        public ApplicationUser? Employer { get; set; }
    }
}
