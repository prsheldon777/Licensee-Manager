using System.ComponentModel.DataAnnotations;

namespace LicenseeManager.Models
{
    /// <summary>
    /// Represents a type of license that can be assigned to a <see cref="Licensee"/>.
    /// </summary>
    /// <remarks>
    /// Instances of this class are stored in the LicenseType table and are referenced by
    /// licensee records via the LicenseTypeID foreign key.
    /// </remarks>
    public class LicenseType
    {
        /// <summary>
        /// Primary key for the license type.
        /// </summary>
        [Key]
        public int LicenseTypeID { get; set; }

        /// <summary>
        /// Human-friendly name of the license type.
        /// </summary>
        /// <remarks>
        /// This value is required and cannot exceed 50 characters. It is displayed in UI as "License Type Name".
        /// </remarks>
        [Required(ErrorMessage = "License type name is required.")]
        [StringLength(50, ErrorMessage = "License type name cannot exceed 50 characters.")]
        [Display(Name = "License Type Name")]
        public string? Name { get; set; }
    }
}
