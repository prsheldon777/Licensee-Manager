namespace LicenseeManager.Models
{
    /// <summary>
    /// View model used by the Error view to surface request information for troubleshooting.
    /// </summary>
    /// <remarks>
    /// The <see cref="RequestId"/> is commonly set from <c>Activity.Current?.Id</c> or the current HTTP context trace identifier.
    /// The <see cref="ShowRequestId"/> property indicates whether a non-empty RequestId is available to show on the page.
    /// </remarks>
    public class ErrorViewModel
    {
        /// <summary>
        /// Gets or sets the unique identifier for the current request.
        /// May be null when no request id is available.
        /// </summary>
        public string? RequestId { get; set; }

        /// <summary>
        /// Gets a value indicating whether the <see cref="RequestId"/> should be displayed.
        /// Returns <c>true</c> when <see cref="RequestId"/> is not null or empty.
        /// </summary>
        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
    }
}
