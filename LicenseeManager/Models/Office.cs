using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LicenseeManager.Models
{
    /// <summary>
    /// Represents a physical office/location that can be assigned to licensees.
    /// </summary>
    /// <remarks>
    /// This entity stores basic location information (name, city, state) and an <see cref="Active"/> flag.
    /// The <see cref="Licensees"/> navigation property exposes related licensee records.
    /// </remarks>
    public class Office
    {
        /// <summary>
        /// Primary key for the office.
        /// </summary>
        [Key]
        public int OfficeID { get; set; }

        /// <summary>
        /// Human-friendly office name.
        /// </summary>
        /// <remarks>
        /// This field is required and has a maximum length of 50 characters as enforced by data annotations.
        /// </remarks>
        [Required(ErrorMessage = "Office name is required.")]
        [StringLength(50, ErrorMessage = "Office cannot exceed 50 characters.")]
        [Display(Name = "Office Name")]
        public string? Name { get; set; }
        /// <summary>
        /// The city where the office is located.
        /// </summary>
        /// <remarks>
        /// This field is required and limited to 50 characters by validation attributes.
        /// </remarks>
        [Required(ErrorMessage = "Office city is required.")]
        [StringLength(50, ErrorMessage = "Office city exceed 50 characters.")]
        [Display(Name = "City")]
        public string? City { get; set; }

        /// <summary>
        /// The state where the office is located.
        /// </summary>
        /// <remarks>
        /// This field is required and limited to 50 characters by validation attributes.
        /// </remarks>
        [Required(ErrorMessage = "State is required.")]
        [StringLength(50, ErrorMessage = "State exceed 50 characters.")]
        [Display(Name = "State")]
        public string? State { get; set; }

        /// <summary>
        /// Indicates whether the office is active.
        /// </summary>
        /// <remarks>
        /// Defaults to <c>true</c>. Use this flag to exclude inactive offices from selection lists or business logic.
        /// </remarks>
        [Required]
        [Display(Name = "Active")]
        public bool Active { get; set; } = true;

        /// <summary>
        /// Read-only string representation of the office active status for display purposes.
        /// </summary>
        /// <remarks>
        /// This property is not mapped to the database. It returns "Active" when <see cref="Active"/> is true,
        /// otherwise "Inactive".
        /// </remarks>
        [NotMapped]
        [Display(Name = "Status")]
        public string ActiveStatus => Active ? "Active" : "Inactive";

        /// <summary>
        /// Navigation collection of licensees associated with this office.
        /// </summary>
        /// <remarks>
        /// This is the inverse navigation for the <see cref="Licensee.OfficeID"/> foreign key.
        /// It may be null when no related licensees are loaded.
        /// </remarks>
        public ICollection<Licensee>? Licensees { get; set; }
    }
}
