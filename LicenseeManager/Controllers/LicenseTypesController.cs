using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Licensee_Manager.Models;
using LicenseeManager.Models;

namespace LicenseeManager.Controllers
{
    public class LicenseTypesController : Controller
    {
        private readonly AppDbContext _context;

        public LicenseTypesController(AppDbContext context)
        {
            _context = context;
        }

        // GET: LicenseTypes
        public async Task<IActionResult> Index()
        {
            return View(await _context.LicenseType.ToListAsync());
        }

        [HttpGet]
        public IActionResult Search(string term, string sortBy, string sortOrder)
        {
            var query = _context.LicenseType.AsQueryable();

            // 🔍 Filter by search term
            if (!string.IsNullOrWhiteSpace(term))
            {
                term = term.Trim();
                query = query.Where(t => t.Name.Contains(term));
            }

            // 🔃 Sorting support
            switch (sortBy)
            {
                case "Name":
                    query = sortOrder == "desc"
                        ? query.OrderByDescending(t => t.Name)
                        : query.OrderBy(t => t.Name);
                    break;
                default:
                    query = query.OrderBy(t => t.Name);
                    break;
            }

            var results = query.ToList();

            // Return the partial table for AJAX updates
            return PartialView("_LicenseTypeTable", results);
        }


        // GET: LicenseTypes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var licenseType = await _context.LicenseType
                .FirstOrDefaultAsync(m => m.LicenseTypeID == id);
            if (licenseType == null)
            {
                return NotFound();
            }

            return View(licenseType);
        }

        // GET: LicenseTypes/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: LicenseTypes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("LicenseTypeID,Name")] LicenseType licenseType)
        {
            if (ModelState.IsValid)
            {
                _context.Add(licenseType);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(licenseType);
        }

        // GET: LicenseTypes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var licenseType = await _context.LicenseType.FindAsync(id);
            if (licenseType == null)
            {
                return NotFound();
            }
            return View(licenseType);
        }

        // POST: LicenseTypes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("LicenseTypeID,Name")] LicenseType licenseType)
        {
            if (id != licenseType.LicenseTypeID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(licenseType);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!LicenseTypeExists(licenseType.LicenseTypeID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(licenseType);
        }

        // GET: LicenseTypes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var licenseType = await _context.LicenseType
                .FirstOrDefaultAsync(m => m.LicenseTypeID == id);
            if (licenseType == null)
            {
                return NotFound();
            }

            return View(licenseType);
        }

        // POST: LicenseTypes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var licenseType = await _context.LicenseType.FindAsync(id);
            if (licenseType != null)
            {
                _context.LicenseType.Remove(licenseType);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool LicenseTypeExists(int id)
        {
            return _context.LicenseType.Any(e => e.LicenseTypeID == id);
        }
    }
}
