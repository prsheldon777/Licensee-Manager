using Licensee_Manager.Models;
using LicenseeManager.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using Licensee_Manager.Models;
using Microsoft.EntityFrameworkCore;

namespace LicenseeManager.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly AppDbContext _context;

        public HomeController(ILogger<HomeController> logger, AppDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index(int? daysAhead)
        {
            // Default to 30 if not provided or invalid
            var range = (daysAhead.HasValue && daysAhead.Value > 0) ? daysAhead.Value : 30;

            // In a real production system, this would likely be run by a scheduled
            // SQL Server Agent job (e.g., nightly) instead of on page load.
            // For this demo, we call it here so reviewers can immediately see
            // licenses automatically transitioning to 'Expired'.
            _context.Database.ExecuteSqlRaw("EXEC SetExpired");

            var expired = _context.Licensees
                .FromSqlRaw("EXEC GetExpiredLicensees @Mode = {0}, @DaysAhead = {1}",
                            0, range)
                .AsEnumerable()
                .ToList();

            var expiringSoon = _context.Licensees
                .FromSqlRaw("EXEC GetExpiredLicensees @Mode = {0}, @DaysAhead = {1}",
                            1, range)
                .AsEnumerable()
                .ToList();

            ViewBag.ExpiredLicensees = expired;
            ViewBag.ExpiringSoonLicensees = expiringSoon;
            ViewBag.DaysAhead = range;  // so the view knows what was selected

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
