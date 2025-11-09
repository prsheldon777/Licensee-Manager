using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LicenseeManager.Models;
using Licensee_Manager.Models;

namespace LicenseeManager.Controllers
{
    public class AuditsController : Controller
    {
        private readonly AppDbContext _context;

        public AuditsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Audits
        public async Task<IActionResult> Index()
        {
            try
            {
                var appDbContext = _context.Audits.Include(a => a.Licensee);
                return View(await appDbContext.ToListAsync());
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Failed to load audit list: {ex.Message}");
                // In a production environment, this would be written to a persistent error log.
                return RedirectToAction("Error", "Home");
            }
        }

        // GET: Audits/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            try
            {
                var audit = await _context.Audits
                    .Include(a => a.Licensee)
                    .FirstOrDefaultAsync(m => m.AuditId == id);

                if (audit == null)
                {
                    return NotFound();
                }

                return View(audit);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Failed to load audit details for ID {id}: {ex.Message}");
                // In production, errors like this should be logged using a structured logger or database log.
                return RedirectToAction("Error", "Home");
            }
        }
    }
}
