using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LicenseeManager.Models
{
    /// <summary>
    /// Represents a license holder in the system.
    /// </summary>
    /// <remarks>
    /// The <see cref="Licensee"/> model contains contact information, license metadata, status,
    /// issue/expiration dates, audit timestamps, and navigation properties to related <see cref="LicenseType"/>
    /// and <see cref="Office"/> entities.
    /// </remarks>
    public class Licensee
    {
        /// <summary>
        /// Primary key for the licensee record.
        /// </summary>
        [Display(Name = "Licensee ID")]
        public int LicenseeID { get; set; }

        /// <summary>
        /// Given name of the licensee.
        /// </summary>
        [Required(ErrorMessage = "First name is required.")]
        [StringLength(50, ErrorMessage = "First name cannot exceed 50 characters.")]
        [Display(Name = "First Name")]
        public string? FirstName { get; set; }

        /// <summary>
        /// Family name of the licensee.
        /// </summary>
        [Required(ErrorMessage = "Last name is required.")]
        [StringLength(50, ErrorMessage = "Last name cannot exceed 50 characters.")]
        [Display(Name = "Last Name")]
        public string? LastName { get; set; }

        /// <summary>
        /// Contact email address for the licensee.
        /// </summary>
        [Required(ErrorMessage = "Email address is required.")]
        [EmailAddress(ErrorMessage = "Please enter a valid email address.")]
        [StringLength(100, ErrorMessage = "Email cannot exceed 100 characters.")]
        [Display(Name = "Email")]
        public string? Email { get; set; }

        /// <summary>
        /// Issued license number for the licensee.
        /// </summary>
        [Required(ErrorMessage = "License number is required.")]
        [StringLength(50, ErrorMessage = "License number cannot exceed 50 characters.")]
        [Display(Name = "License Number")]
        public string? LicenseNumber { get; set; }

        /// <summary>
        /// Foreign key to the license type assigned to this licensee.
        /// </summary>
        [Required(ErrorMessage = "Please select a license type.")]
        [Display(Name = "License Type")]
        public int LicenseTypeID { get; set; }

        /// <summary>
        /// Foreign key to the office responsible for this licensee.
        /// </summary>
        [Required(ErrorMessage = "Please select an office.")]
        [Display(Name = "Office")]
        public int OfficeID { get; set; }

        /// <summary>
        /// Current status of the licensee (e.g., Active, Inactive, Expired).
        /// </summary>
        [Required(ErrorMessage = "Status must be selected.")]
        [Display(Name = "Status")]
        public LicenseeStatus Status { get; set; }

        /// <summary>
        /// Date the license was issued. Displayed/edited as a date-only value.
        /// </summary>
        [Required(ErrorMessage = "Issue date is required.")]
        [DataType(DataType.Date)]
        [Display(Name = "Issued Date")]
        public DateTime? IssueDate { get; set; }

        /// <summary>
        /// Date the license expires. Must be today or a future date.
        /// </summary>
        [Required(ErrorMessage = "Expiration date is required.")]
        [DataType(DataType.Date)]
        [FutureOrToday(ErrorMessage = "Expiration date cannot be before today.")]
        [Display(Name = "Expiration Date")]
        public DateTime? ExpirationDate { get; set; }

        /// <summary>
        /// Timestamp when the licensee record was created.
        /// </summary>
        [Display(Name = "Created On")]
        public DateTime? CreatedAt { get; set; } = DateTime.Now;

        /// <summary>
        /// Timestamp when the licensee record was last updated.
        /// </summary>
        [Display(Name = "Updated On")]
        public DateTime? UpdatedAt { get; set; }

        /// <summary>
        /// Navigation property for the related license type.
        /// </summary>
        [ForeignKey(nameof(LicenseTypeID))]
        [Display(Name = "License Type")]
        public LicenseType? LicenseType { get; set; }

        /// <summary>
        /// Navigation property for the related office.
        /// </summary>
        [ForeignKey(nameof(OfficeID))]
        [Display(Name = "Office")]
        public Office? Office { get; set; }

        /// <summary>
        /// Computed convenience property combining first and last name.
        /// </summary>
        [NotMapped]
        [Display(Name = "Full Name")]
        public string FullName => $"{FirstName} {LastName}".Trim();

        /// <summary>
        /// Validation attribute that ensures a date is either today or in the future.
        /// </summary>
        /// <remarks>
        /// Applied to <see cref="ExpirationDate"/> to prevent entering a past expiration date.
        /// </remarks>
        public class FutureOrTodayAttribute : ValidationAttribute
        {
            /// <summary>
            /// Validates that the provided value is a <see cref="DateTime"/> that is today or later.
            /// </summary>
            /// <param name="value">The value to validate (expected to be a DateTime).</param>
            /// <param name="validationContext">Contextual information about the validation operation.</param>
            /// <returns>
            /// A <see cref="ValidationResult"/> indicating success, or a failure result containing the configured error message.
            /// </returns>
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
