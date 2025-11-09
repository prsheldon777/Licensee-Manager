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
            try
            {
                var appDbContext = _context.Licensees
                    .Include(l => l.LicenseType)
                    .Include(l => l.Office)
                    .OrderByDescending(l => l.Status);

                return View(await appDbContext.ToListAsync());
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] LicenseesController.Index failed at {DateTime.Now}: {ex.Message}");
                return RedirectToAction("Error", "Home");
            }
        }

        // GET: Licensees/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            try
            {
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
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] LicenseesController.Details failed for ID {id} at {DateTime.Now}: {ex.Message}");
                return RedirectToAction("Error", "Home");
            }
        }

        // GET: Licensees/Create
        public async Task<IActionResult> Create()
        {
            try
            {
                var activeOffices = await _context.Offices
                    .Where(o => o.Active)
                    .ToListAsync();

                ViewData["LicenseTypeID"] = new SelectList(_context.LicenseType, "LicenseTypeID", "Name");
                ViewData["OfficeID"] = new SelectList(activeOffices, "OfficeID", "Name");
                return View();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] LicenseesController.Create (GET) failed at {DateTime.Now}: {ex.Message}");
                return RedirectToAction("Error", "Home");
            }
        }

        // POST: Licensees/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("LicenseeID,FirstName,LastName,Email,LicenseNumber,LicenseTypeID,OfficeID,Status,IssueDate,ExpirationDate,CreatedAt,UpdatedAt")] Licensee licensee)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    licensee.CreatedAt = DateTime.Now;
                    _context.Add(licensee);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[ERROR] LicenseesController.Create (POST) failed at {DateTime.Now}: {ex.Message}");
                    return RedirectToAction("Error", "Home");
                }
            }

            try
            {
                var activeOffices = await _context.Offices.Where(o => o.Active).ToListAsync();
                ViewData["LicenseTypeID"] = new SelectList(_context.LicenseType, "LicenseTypeID", "Name", licensee.LicenseTypeID);
                ViewData["OfficeID"] = new SelectList(activeOffices, "OfficeID", "Name", licensee.OfficeID);
                return View(licensee);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] LicenseesController.Create (POST fallback) failed at {DateTime.Now}: {ex.Message}");
                return RedirectToAction("Error", "Home");
            }
        }

        // GET: Licensees/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            try
            {
                var licensee = await _context.Licensees.FindAsync(id);
                if (licensee == null)
                {
                    return NotFound();
                }

                var activeOffices = await _context.Offices.Where(o => o.Active).ToListAsync();

                ViewData["LicenseTypeID"] = new SelectList(_context.LicenseType, "LicenseTypeID", "Name", licensee.LicenseTypeID);
                ViewData["OfficeID"] = new SelectList(activeOffices, "OfficeID", "Name", licensee.OfficeID);
                return View(licensee);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] LicenseesController.Edit (GET) failed for ID {id} at {DateTime.Now}: {ex.Message}");
                return RedirectToAction("Error", "Home");
            }
        }

        // POST: Licensees/Edit/5
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
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!LicenseeExists(licensee.LicenseeID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        Console.WriteLine($"[ERROR] Concurrency conflict in LicenseesController.Edit for ID {id} at {DateTime.Now}");
                        throw;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[ERROR] LicenseesController.Edit (POST) failed for ID {id} at {DateTime.Now}: {ex.Message}");
                    return RedirectToAction("Error", "Home");
                }
            }

            try
            {
                var activeOffices = await _context.Offices.Where(o => o.Active).ToListAsync();
                ViewData["LicenseTypeID"] = new SelectList(_context.LicenseType, "LicenseTypeID", "Name", licensee.LicenseTypeID);
                ViewData["OfficeID"] = new SelectList(activeOffices, "OfficeID", "Name", licensee.OfficeID);
                return View(licensee);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] LicenseesController.Edit (POST fallback) failed at {DateTime.Now}: {ex.Message}");
                return RedirectToAction("Error", "Home");
            }
        }

        private bool LicenseeExists(int id)
        {
            return _context.Licensees.Any(e => e.LicenseeID == id);
        }

        [HttpGet]
        public IActionResult Search(string term, string sortBy, string sortOrder = "asc")
        {
            try
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
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] LicenseesController.Search failed at {DateTime.Now}: {ex.Message}");
                return RedirectToAction("Error", "Home");
            }
        }
    }
}
