using Licensee_Manager.Models;
using LicenseeManager.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

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
