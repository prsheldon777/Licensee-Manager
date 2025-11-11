using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LicenseeManager.Models
{
    /// <summary>
    /// Represents an audit record for changes to a licensee's status.
    /// </summary>
    /// <remarks>
    /// Mapped to the "LicenseeStatusAudit" table. Each record captures the licensee affected,
    /// the previous and new <see cref="LicenseeStatus"/> values, and the time the change occurred.
    /// The <see cref="Licensee"/> navigation property provides access to the related licensee entity.
    /// </remarks>
    [Table("LicenseeStatusAudit")]
    public class Audit
    {
        /// <summary>
        /// Primary key for the audit record.
        /// </summary>
        [Key]
        public int AuditId { get; set; }

        /// <summary>
        /// The identifier of the licensee whose status changed.
        /// This is a foreign key linking to the <see cref="Licensee.LicenseeID"/> property.
        /// </summary>
        [Display(Name = "Licensee ID")]
        public int LicenseeId { get; set; }

        /// <summary>
        /// The previous status value before the change.
        /// Stored as a <see cref="LicenseeStatus"/> enum.
        /// </summary>
        [Display(Name = "Previous Status")]
        public LicenseeStatus OldStatus { get; set; }

        /// <summary>
        /// The new status value after the change.
        /// Stored as a <see cref="LicenseeStatus"/> enum.
        /// </summary>
        [Display(Name = "New Status")]
        public LicenseeStatus NewStatus { get; set; }

        /// <summary>
        /// The date and time when the status change occurred.
        /// </summary>
        [Display(Name = "Changed On")]
        public DateTime ChangedAt { get; set; }

        /// <summary>
        /// Navigation property to the licensee associated with this audit record.
        /// </summary>
        [ForeignKey(nameof(LicenseeId))]
        [Display(Name = "Licensee")]
        public Licensee Licensee { get; set; }
    }
}
