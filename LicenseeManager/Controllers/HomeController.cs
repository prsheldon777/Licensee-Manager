using Licensee_Manager.Models;
using LicenseeManager.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace LicenseeManager.Controllers
{
    /// <summary>
    /// Controller that provides the application's primary pages: dashboard (Index), Privacy, and Error.
    /// </summary>
    /// <remarks>
    /// - The Index action runs maintenance SQL procedures (for demo purposes) and populates view data for
    ///   expired and soon-to-expire licensees.
    /// - The controller depends on an <see cref="ILogger{HomeController}"/> for logging and an <see cref="AppDbContext"/>
    ///   for database access; both are provided via dependency injection.
    /// </remarks>
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly AppDbContext _context;

        /// <summary>
        /// Initializes a new instance of the <see cref="HomeController"/> class.
        /// </summary>
        /// <param name="logger">Logger instance for diagnostic logging.</param>
        /// <param name="context">Application database context used to query and execute database commands.</param>
        public HomeController(ILogger<HomeController> logger, AppDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        /// <summary>
        /// Displays the dashboard view which shows expired and expiring licensees.
        /// </summary>
        /// <param name="daysAhead">
        /// Optional number of days in the future to consider when determining "expiring soon".
        /// If null or less than or equal to zero, a default of 30 days is used.
        /// </param>
        /// <returns>
        /// An <see cref="IActionResult"/> that renders the Index view populated with:
        /// - ViewBag.ExpiredLicensees: list of expired licensees
        /// - ViewBag.ExpiringSoonLicensees: list of licensees expiring within the given horizon
        /// - ViewBag.DaysAhead: the effective range used for the query
        /// </returns>
        /// <remarks>
        /// For demonstration the method executes the stored procedure "SetExpired" and then
        /// queries "GetExpiredLicensees" with two modes to retrieve expired and expiring licensees.
        /// In production this maintenance work would typically be performed by a scheduled job.
        /// </remarks>
        /// <exception cref="Exception">
        /// Exceptions while executing database commands or querying results are caught; the exception is logged
        /// to the console and the user is redirected to the Home/Error page.
        /// </exception>
        public IActionResult Index(int? daysAhead)
        {
            try
            {
                // Default to 30 if not provided or invalid
                var range = (daysAhead.HasValue && daysAhead.Value > 0) ? daysAhead.Value : 30;

                // In production, this would likely be a scheduled SQL Agent job.
                // For demo purposes, run the expiration procedure here.
                _context.Database.ExecuteSqlRaw("EXEC SetExpired");

                var expired = _context.Licensees
                    .FromSqlRaw("EXEC GetExpiredLicensees @Mode = {0}, @DaysAhead = {1}", 0, range)
                    .AsEnumerable()
                    .ToList();

                var expiringSoon = _context.Licensees
                    .FromSqlRaw("EXEC GetExpiredLicensees @Mode = {0}, @DaysAhead = {1}", 1, range)
                    .AsEnumerable()
                    .ToList();

                ViewBag.ExpiredLicensees = expired;
                ViewBag.ExpiringSoonLicensees = expiringSoon;
                ViewBag.DaysAhead = range;

                return View();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Failed to load dashboard: {ex.Message}");
                // In production, this would be logged to a persistent error log instead.
                return RedirectToAction("Error", "Home");
            }
        }

        /// <summary>
        /// Returns the Privacy view.
        /// </summary>
        /// <returns>An <see cref="IActionResult"/> that renders the Privacy page.</returns>
        /// <exception cref="Exception">
        /// Any exception during view rendering is caught; it is logged to the console and the user is redirected
        /// to the Home/Error page.
        /// </exception>
        public IActionResult Privacy()
        {
            try
            {
                return View();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Failed to load Privacy page: {ex.Message}");
                return RedirectToAction("Error", "Home");
            }
        }

        /// <summary>
        /// Displays the Error view. This action is decorated with response caching settings that prevent caching.
        /// </summary>
        /// <returns>An <see cref="IActionResult"/> that renders the Error page. If the Error view fails to render,
        /// a plain content message is returned.</returns>
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            try
            {
                return View();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Error view rendering failed: {ex.Message}");
                return Content("An unexpected error occurred. Please contact support.");
            }
        }
    }
}
