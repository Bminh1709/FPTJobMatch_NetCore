using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FPT.Models
{
    public class HelpType
    {
        [Key]
        public int Id { get; set; }
        [Required(ErrorMessage = "Name is required")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Short description is required")]
        public string Description { get; set; }
        public string? AdminId { get; set; }
        [ForeignKey("AdminId")]
        public ApplicationUser? Admin { get; set; }
    }
}
