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
    public class LicenseesController : Controller
    {
        private readonly AppDbContext _context;

        public LicenseesController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Licensees
        public async Task<IActionResult> Index()
        {
            var appDbContext = _context.Licensees.Include(l => l.LicenseType).Include(l => l.Office).OrderByDescending(l => l.Status);

            return View(await appDbContext.ToListAsync());
        }

        // GET: Licensees/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var licensee = await _context.Licensees
                .Include(l => l.LicenseType)
                .Include(l => l.Office)
                .FirstOrDefaultAsync(m => m.LicenseeID == id);
            if (licensee == null)
            {
                return NotFound();
            }

            return View(licensee);
        }

        // GET: Licensees/Create
        public IActionResult Create()
        {

            ViewData["LicenseTypeID"] = new SelectList(_context.LicenseType, "LicenseTypeID", "Name");
            ViewData["OfficeID"] = new SelectList(_context.Offices, "OfficeID", "Name");
            return View();
        }

        // POST: Licensees/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("LicenseeID,FirstName,LastName,Email,LicenseNumber,LicenseTypeID,OfficeID,status,IssueDate,ExpirationDate,CreatedAt,UpdatedAt")] Licensee licensee)
        {
            if (ModelState.IsValid)
            {
                licensee.CreatedAt = DateTime.Now;

                _context.Add(licensee);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["LicenseTypeID"] = new SelectList(_context.LicenseType, "LicenseTypeID", "Name", licensee.LicenseTypeID);
            ViewData["OfficeID"] = new SelectList(_context.Offices, "OfficeID", "OfficeID", licensee.OfficeID);
            return View(licensee);
        }

        // GET: Licensees/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var licensee = await _context.Licensees.FindAsync(id);
            if (licensee == null)
            {
                return NotFound();
            }
            ViewData["LicenseTypeID"] = new SelectList(_context.LicenseType, "LicenseTypeID", "Name", licensee.LicenseTypeID);
            ViewData["OfficeID"] = new SelectList(_context.Offices, "OfficeID", "OfficeID", licensee.OfficeID);
            return View(licensee);
        }

        // POST: Licensees/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("LicenseeID,FirstName,LastName,Email,LicenseNumber,LicenseTypeID,OfficeID,Status,IssueDate,ExpirationDate,CreatedAt,UpdatedAt")] Licensee licensee)
        {
            if (id != licensee.LicenseeID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    licensee.UpdatedAt = DateTime.Now;

                    _context.Update(licensee);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!LicenseeExists(licensee.LicenseeID))
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
            ViewData["LicenseTypeID"] = new SelectList(_context.LicenseType, "LicenseTypeID", "LicenseTypeID", licensee.LicenseTypeID);
            ViewData["OfficeID"] = new SelectList(_context.Offices, "OfficeID", "OfficeID", licensee.OfficeID);
            return View(licensee);
        }

        // GET: Licensees/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var licensee = await _context.Licensees
                .Include(l => l.LicenseType)
                .Include(l => l.Office)
                .FirstOrDefaultAsync(m => m.LicenseeID == id);
            if (licensee == null)
            {
                return NotFound();
            }

            return View(licensee);
        }

        // POST: Licensees/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var licensee = await _context.Licensees.FindAsync(id);
            if (licensee != null)
            {
                _context.Licensees.Remove(licensee);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool LicenseeExists(int id)
        {
            return _context.Licensees.Any(e => e.LicenseeID == id);
        }

        [HttpGet]
        public IActionResult Search(string term, string sortBy, string sortOrder = "asc")
        {
            var query = _context.Licensees
                .Include(l => l.LicenseType)
                .Include(l => l.Office)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(term))
            {
                term = term.Trim();
                query = query.Where(l =>
                    l.FirstName.Contains(term) ||
                    l.LastName.Contains(term));
            }

            // Sorting
            switch (sortBy)
            {
                case "FirstName":
                    query = sortOrder == "asc" ? query.OrderBy(l => l.FirstName) : query.OrderByDescending(l => l.FirstName);
                    break;
                case "LastName":
                    query = sortOrder == "asc" ? query.OrderBy(l => l.LastName) : query.OrderByDescending(l => l.LastName);
                    break;
                case "Email":
                    query = sortOrder == "asc" ? query.OrderBy(l => l.Email) : query.OrderByDescending(l => l.Email);
                    break;
                case "LicenseNumber":
                    query = sortOrder == "asc" ? query.OrderBy(l => l.LicenseNumber) : query.OrderByDescending(l => l.LicenseNumber);
                    break;
                case "status":
                    query = sortOrder == "asc" ? query.OrderBy(l => l.Status) : query.OrderByDescending(l => l.Status);
                    break;
                case "IssueDate":
                    query = sortOrder == "asc" ? query.OrderBy(l => l.IssueDate) : query.OrderByDescending(l => l.IssueDate);
                    break;
                case "ExpirationDate":
                    query = sortOrder == "asc" ? query.OrderBy(l => l.ExpirationDate) : query.OrderByDescending(l => l.ExpirationDate);
                    break;
                case "CreatedAt":
                    query = sortOrder == "asc" ? query.OrderBy(l => l.CreatedAt) : query.OrderByDescending(l => l.CreatedAt);
                    break;
                case "UpdatedAt":
                    query = sortOrder == "asc" ? query.OrderBy(l => l.UpdatedAt) : query.OrderByDescending(l => l.UpdatedAt);
                    break;
                case "LicenseType":
                    query = sortOrder == "asc" ? query.OrderBy(l => l.LicenseType.Name) : query.OrderByDescending(l => l.LicenseType.Name);
                    break;
                case "Office":
                    query = sortOrder == "asc" ? query.OrderBy(l => l.Office.Name) : query.OrderByDescending(l => l.Office.Name);
                    break;
                default:
                    query = query.OrderBy(l => l.LastName);
                    break;
            }

            var results = query.ToList();
            return PartialView("_LicenseeTable", results);
        }
    }
}
