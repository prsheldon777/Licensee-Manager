using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LicenseeManager.Models
{
    [Table("LicenseeStatusAudit")]
    public class Audit
    {
        [Key]
        public int AuditId { get; set; }

        [Display(Name = "Licensee ID")]
        public int LicenseeId { get; set; }

        [Display(Name = "Previous Status")]
        public LicenseeStatus OldStatus { get; set; }

        [Display(Name = "New Status")]
        public LicenseeStatus NewStatus { get; set; }

        [Display(Name = "Changed On")]
        public DateTime ChangedAt { get; set; }

        [ForeignKey(nameof(LicenseeId))]
        [Display(Name = "Licensee")]
        public Licensee Licensee { get; set; }
    }
}
