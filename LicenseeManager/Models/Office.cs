using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LicenseeManager.Models
{
    public class Office
    {
        [Key]
        public int OfficeID { get; set; }
        [Required(ErrorMessage = "Office name is required.")]
        [StringLength(50, ErrorMessage = "Office cannot exceed 50 characters.")]
        [Display(Name = "Office Name")]
        public string? Name { get; set; }
        [Required(ErrorMessage = "Office city is required.")]
        [StringLength(50, ErrorMessage = "Office city exceed 50 characters.")]
        [Display(Name = "City")]
        public string? City { get; set; }
        [Required(ErrorMessage = "State is required.")]
        [StringLength(50, ErrorMessage = "State exceed 50 characters.")]
        [Display(Name = "State")]
        public string? State { get; set; }
        [Required]
        [Display(Name = "Active")]
        public bool Active { get; set; } = true;

        [NotMapped]
        [Display(Name = "Status")]
        public string ActiveStatus => Active ? "Active" : "Inactive";

        public ICollection<Licensee>? Licensees { get; set; }
    }
}
