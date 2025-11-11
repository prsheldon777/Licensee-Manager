namespace LicenseeManager.Models
{
    /// <summary>
    /// Represents the status of a licensee within the system.
    /// </summary>
    /// <remarks>
    /// Enum values are persisted as integers in the database (see <c>AppDbContext.OnModelCreating</c> for conversion).
    /// Use these values to determine whether a licensee is active, inactive, or has expired.
    /// </remarks>
    public enum LicenseeStatus
    {
        /// <summary>
        /// The licensee is inactive and not currently authorized to perform license-holder actions.
        /// Typically used for users who have been deactivated but not removed.
        /// </summary>
        Inactive = 1,

        /// <summary>
        /// The licensee is active and in good standing.
        /// This indicates a current, valid license.
        /// </summary>
        Active = 2,

        /// <summary>
        /// The licensee's license has expired and is no longer valid.
        /// Expired licensees may need renewal to return to the <see cref="Active"/> state.
        /// </summary>
        Expired = 3
    }
}