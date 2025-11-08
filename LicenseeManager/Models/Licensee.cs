using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LicenseeManager.Models
{
    public class Licensee
    {
        [Display(Name = "Licensee ID")]
        public int LicenseeID { get; set; }
        [Required, StringLength(50)]
        [Display(Name = "First Name")]
        public string? FirstName { get; set; }
        [Required, StringLength(50)]
        [Display(Name = "Last Name")]
        public string? LastName { get; set; }
        [Required, EmailAddress]
        [Display(Name = "Email")]
        public string? Email { get; set; }
        [Required, StringLength(50)]
        [Display(Name = "License Number")]
        public string? LicenseNumber { get; set; }
        [Required]
        [Display(Name = "License Type ID")]
        public int LicenseTypeID { get; set; }
        [Required]
        [Display(Name = "Office ID")]
        public int OfficeID { get; set; }
        [Required]
        [Display(Name = "Status")]
        public LicenseeStatus Status { get; set; }
        [Display(Name = "Issued Date")]
        public DateTime? IssueDate { get; set; }
        [Display(Name = "Expiration Date")]
        public DateTime? ExpirationDate { get; set; }
        [Display(Name = "Created On")]
        public DateTime? CreatedAt { get; set; }
        [Display(Name = "Updated On")]
        public DateTime? UpdatedAt { get; set; }

        [Display(Name = "License Type")]
        public LicenseType? LicenseType { get; set; }
        [Display(Name = "Office")]
        public Office? Office { get; set; }

        [NotMapped]
        [Display(Name = "Full Name")]
        public string FullName => $"{FirstName} {LastName}".Trim();
    }
}
