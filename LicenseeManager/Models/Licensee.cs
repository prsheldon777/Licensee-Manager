using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LicenseeManager.Models
{
    public class Licensee
    {
        [Display(Name = "Licensee ID")]
        public int LicenseeID { get; set; }

        [Required(ErrorMessage = "First name is required.")]
        [StringLength(50, ErrorMessage = "First name cannot exceed 50 characters.")]
        [Display(Name = "First Name")]
        public string? FirstName { get; set; }

        [Required(ErrorMessage = "Last name is required.")]
        [StringLength(50, ErrorMessage = "Last name cannot exceed 50 characters.")]
        [Display(Name = "Last Name")]
        public string? LastName { get; set; }

        [Required(ErrorMessage = "Email address is required.")]
        [EmailAddress(ErrorMessage = "Please enter a valid email address.")]
        [StringLength(100, ErrorMessage = "Email cannot exceed 100 characters.")]
        [Display(Name = "Email")]
        public string? Email { get; set; }

        [Required(ErrorMessage = "License number is required.")]
        [StringLength(50, ErrorMessage = "License number cannot exceed 50 characters.")]
        [Display(Name = "License Number")]
        public string? LicenseNumber { get; set; }

        [Required(ErrorMessage = "Please select a license type.")]
        [Display(Name = "License Type")]
        public int LicenseTypeID { get; set; }

        [Required(ErrorMessage = "Please select an office.")]
        [Display(Name = "Office")]
        public int OfficeID { get; set; }

        [Required(ErrorMessage = "Status must be selected.")]
        [Display(Name = "Status")]
        public LicenseeStatus Status { get; set; }

        [Required(ErrorMessage = "Issue date is required.")]
        [DataType(DataType.Date)]
        [Display(Name = "Issued Date")]
        public DateTime? IssueDate { get; set; }

        [Required(ErrorMessage = "Expiration date is required.")]
        [DataType(DataType.Date)]
        [FutureOrToday(ErrorMessage = "Expiration date cannot be before today.")]
        [Display(Name = "Expiration Date")]
        public DateTime? ExpirationDate { get; set; }

        [Display(Name = "Created On")]
        public DateTime? CreatedAt { get; set; }

        [Display(Name = "Updated On")]
        public DateTime? UpdatedAt { get; set; }

        [ForeignKey(nameof(LicenseTypeID))]
        [Display(Name = "License Type")]
        public LicenseType? LicenseType { get; set; }

        [ForeignKey(nameof(OfficeID))]
        [Display(Name = "Office")]
        public Office? Office { get; set; }

        [NotMapped]
        [Display(Name = "Full Name")]
        public string FullName => $"{FirstName} {LastName}".Trim();

        public class FutureOrTodayAttribute : ValidationAttribute
        {
            protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
            {
                if (value is DateTime date && date.Date < DateTime.Today)
                {
                    return new ValidationResult(ErrorMessage ?? "Date must be today or later.");
                }

                return ValidationResult.Success;
            }
        }
    }
}
