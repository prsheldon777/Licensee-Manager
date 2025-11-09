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
    public class OfficesController : Controller
    {
        private readonly AppDbContext _context;

        public OfficesController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Offices
        public async Task<IActionResult> Index()
        {
            return View(await _context.Offices.ToListAsync());
        }

        // GET: Offices/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var office = await _context.Offices
                .FirstOrDefaultAsync(m => m.OfficeID == id);
            if (office == null)
            {
                return NotFound();
            }

            return View(office);
        }

        // GET: Offices/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Offices/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("OfficeID,Name,City,State")] Office office)
        {
            if (ModelState.IsValid)
            {
                _context.Add(office);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(office);
        }

        // GET: Offices/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var office = await _context.Offices.FindAsync(id);
            if (office == null)
            {
                return NotFound();
            }
            return View(office);
        }

        // POST: Offices/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("OfficeID,Name,City,State")] Office office)
        {
            if (id != office.OfficeID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(office);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OfficeExists(office.OfficeID))
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
            return View(office);
        }

        // GET: Offices/Delete/5
        public async Task<IActionResult> Deactivate(int? id)
        {
            if (id == null)
                return NotFound();

            var office = await _context.Offices
                .Include(o => o.Licensees)
                .FirstOrDefaultAsync(o => o.OfficeID == id);

            if (office == null)
                return NotFound();

            // Get all active offices except the one being deactivated
            ViewBag.AvailableOffices = new SelectList(
                _context.Offices.Where(o => o.Active && o.OfficeID != id),
                "OfficeID", "Name"
            );

            return View(office);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeactivateConfirmed(int id, int? newOfficeId)
        {
            var office = await _context.Offices
                .Include(o => o.Licensees)
                .FirstOrDefaultAsync(o => o.OfficeID == id);

            if (office == null)
                return NotFound();

            // If there's a replacement office selected, reassign
            if (newOfficeId.HasValue)
            {
                var licensees = _context.Licensees.Where(l => l.OfficeID == id).ToList();
                foreach (var l in licensees)
                    l.OfficeID = newOfficeId.Value;

                await _context.SaveChangesAsync();
            }

            // Mark office inactive
            office.Active = false;
            await _context.SaveChangesAsync();

            TempData["Message"] = "Office deactivated successfully.";
            return RedirectToAction(nameof(Index));
        }

        private bool OfficeExists(int id)
        {
            return _context.Offices.Any(e => e.OfficeID == id);
        }

        [HttpGet]
        public IActionResult Search(string term, string sortBy, string sortOrder = "asc")
        {
            var query = _context.Offices.AsQueryable();

            if (!string.IsNullOrWhiteSpace(term))
            {
                term = term.Trim();
                query = query.Where(o =>
                    o.Name.Contains(term) ||
                    o.City.Contains(term) ||
                    o.State.Contains(term));
            }

            // Sorting logic
            switch (sortBy)
            {
                case "Name":
                    query = sortOrder == "asc" ? query.OrderBy(o => o.Name) : query.OrderByDescending(o => o.Name);
                    break;
                case "City":
                    query = sortOrder == "asc" ? query.OrderBy(o => o.City) : query.OrderByDescending(o => o.City);
                    break;
                case "State":
                    query = sortOrder == "asc" ? query.OrderBy(o => o.State) : query.OrderByDescending(o => o.State);
                    break;
                case "ActiveStatus":
                    query = sortOrder == "asc" ? query.OrderBy(o => o.Active) : query.OrderByDescending(o => o.Active);
                    break;
                default:
                    query = query.OrderBy(o => o.OfficeID);
                    break;
            }

            var results = query.ToList();
            return PartialView("_OfficeTable", results);
        }

        [HttpGet]
        public async Task<IActionResult> Activate(int id)
        {
            var office = await _context.Offices.FindAsync(id);
            if (office == null)
            {
                return NotFound();
            }

            office.Active = true;
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = $"{office.Name} has been activated.";
            return RedirectToAction(nameof(Index));
        }
    }
}
