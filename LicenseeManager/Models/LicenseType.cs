using System.ComponentModel.DataAnnotations;

namespace LicenseeManager.Models
{
    public class LicenseType
    {
        [Key]
        public int LicenseTypeID { get; set; }
        [Required(ErrorMessage = "License type name is required.")]
        [StringLength(50, ErrorMessage = "License type name cannot exceed 50 characters.")]
        [Display(Name = "License Type Name")]
        public string? Name { get; set; }
    }
}
