using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
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
            var appDbContext = _context.Audits.Include(a => a.Licensee);
            return View(await appDbContext.ToListAsync());
        }

        // GET: Audits/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var audit = await _context.Audits
                .Include(a => a.Licensee)
                .FirstOrDefaultAsync(m => m.AuditId == id);
            if (audit == null)
            {
                return NotFound();
            }

            return View(audit);
        }

        private bool AuditExists(int id)
        {
            return _context.Audits.Any(e => e.AuditId == id);
        }
    }
}
