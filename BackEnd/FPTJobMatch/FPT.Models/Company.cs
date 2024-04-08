using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FPT.Models
{
    public class Company
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string? About { get; set; }
        public string? Address { get; set; }
        public int? Size { get; set; }
        public string? Logo { get; set; }
        public bool? IsApproved { get; set; }
        public DateTime CreatedAt { get; set; }


        public int ApplicationUserId { get; set; }
        [ForeignKey("ApplicationUserId")]
        public ApplicationUser ApplicationUser { get; set; }

        public int CityId { get; set; }
        [ForeignKey("CityId")]
        public City City { get; set; }
    }
}
