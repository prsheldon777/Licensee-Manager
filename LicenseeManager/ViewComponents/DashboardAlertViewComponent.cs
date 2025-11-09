using Licensee_Manager.Models;
using LicenseeManager.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace LicenseeManager.ViewComponents
{
    public class DashboardAlertViewComponent : ViewComponent
    {
        private readonly AppDbContext _context;

        public DashboardAlertViewComponent(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            // Use the same stored procedure the dashboard uses
            int range = 30; // You can make this configurable if needed

            var expired = await _context.Licensees
                .FromSqlRaw("EXEC GetExpiredLicensees @Mode = {0}, @DaysAhead = {1}", 0, range)
                .ToListAsync();

            var expiringSoon = await _context.Licensees
                .FromSqlRaw("EXEC GetExpiredLicensees @Mode = {0}, @DaysAhead = {1}", 1, range)
                .ToListAsync();

            // Calculate totals
            int expiredCount = expired?.Count ?? 0;
            int expiringSoonCount = expiringSoon?.Count ?? 0;

            ViewBag.TotalAlerts = expiredCount + expiringSoonCount;
            ViewBag.Expired = expiredCount;
            ViewBag.ExpiringSoon = expiringSoonCount;

            return View();
        }
    }
}
