using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
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
            try
            {
                return View(await _context.LicenseType.ToListAsync());
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Failed to load license type list: {ex.Message}");
                return RedirectToAction("Error", "Home");
            }
        }

        [HttpGet]
        public IActionResult Search(string term, string sortBy, string sortOrder)
        {
            try
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
                return PartialView("_LicenseTypeTable", results);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] License type search failed: {ex.Message}");
                return RedirectToAction("Error", "Home");
            }
        }

        // GET: LicenseTypes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            try
            {
                var licenseType = await _context.LicenseType
                    .FirstOrDefaultAsync(m => m.LicenseTypeID == id);

                if (licenseType == null)
                {
                    return NotFound();
                }

                return View(licenseType);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Failed to load details for LicenseType ID {id}: {ex.Message}");
                return RedirectToAction("Error", "Home");
            }
        }

        // GET: LicenseTypes/Create
        public IActionResult Create()
        {
            try
            {
                return View();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Failed to load Create LicenseType page: {ex.Message}");
                return RedirectToAction("Error", "Home");
            }
        }

        // POST: LicenseTypes/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("LicenseTypeID,Name")] LicenseType licenseType)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    _context.Add(licenseType);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                return View(licenseType);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Failed to create LicenseType '{licenseType?.Name}': {ex.Message}");
                return RedirectToAction("Error", "Home");
            }
        }

        // GET: LicenseTypes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            try
            {
                var licenseType = await _context.LicenseType.FindAsync(id);
                if (licenseType == null)
                {
                    return NotFound();
                }

                return View(licenseType);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Failed to load Edit page for LicenseType ID {id}: {ex.Message}");
                return RedirectToAction("Error", "Home");
            }
        }

        // POST: LicenseTypes/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("LicenseTypeID,Name")] LicenseType licenseType)
        {
            if (id != licenseType.LicenseTypeID)
            {
                return NotFound();
            }

            try
            {
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
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Failed to edit LicenseType ID {id}: {ex.Message}");
                return RedirectToAction("Error", "Home");
            }
        }

        // GET: LicenseTypes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            try
            {
                var licenseType = await _context.LicenseType
                    .FirstOrDefaultAsync(m => m.LicenseTypeID == id);

                if (licenseType == null)
                {
                    return NotFound();
                }

                return View(licenseType);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Failed to load Delete confirmation for LicenseType ID {id}: {ex.Message}");
                return RedirectToAction("Error", "Home");
            }
        }

        // POST: LicenseTypes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var licenseType = await _context.LicenseType.FindAsync(id);
                if (licenseType != null)
                {
                    _context.LicenseType.Remove(licenseType);
                    await _context.SaveChangesAsync();
                }

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Failed to delete LicenseType ID {id}: {ex.Message}");
                return RedirectToAction("Error", "Home");
            }
        }

        private bool LicenseTypeExists(int id)
        {
            try
            {
                return _context.LicenseType.Any(e => e.LicenseTypeID == id);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Failed to check LicenseType existence for ID {id}: {ex.Message}");
                return false;
            }
        }
    }
}
