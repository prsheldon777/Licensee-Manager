using System;
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
            try
            {
                return View(await _context.Offices.ToListAsync());
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Failed to load office list: {ex.Message}");
                return RedirectToAction("Error", "Home");
            }
        }

        // GET: Offices/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            try
            {
                var office = await _context.Offices
                    .FirstOrDefaultAsync(m => m.OfficeID == id);

                if (office == null)
                    return NotFound();

                return View(office);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Failed to load details for Office ID {id}: {ex.Message}");
                return RedirectToAction("Error", "Home");
            }
        }

        // GET: Offices/Create
        public IActionResult Create()
        {
            // No DB work here, safe without try/catch
            return View();
        }

        // POST: Offices/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("OfficeID,Name,City,State")] Office office)
        {
            if (!ModelState.IsValid)
                return View(office);

            try
            {
                _context.Add(office);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Failed to create office '{office?.Name}': {ex.Message}");
                return RedirectToAction("Error", "Home");
            }
        }

        // GET: Offices/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            try
            {
                var office = await _context.Offices.FindAsync(id);
                if (office == null)
                    return NotFound();

                return View(office);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Failed to load Edit page for Office ID {id}: {ex.Message}");
                return RedirectToAction("Error", "Home");
            }
        }

        // POST: Offices/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("OfficeID,Name,City,State")] Office office)
        {
            if (id != office.OfficeID)
                return NotFound();

            if (!ModelState.IsValid)
                return View(office);

            try
            {
                _context.Update(office);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!OfficeExists(office.OfficeID))
                    return NotFound();
                else
                    throw;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Failed to edit Office ID {id}: {ex.Message}");
                return RedirectToAction("Error", "Home");
            }
        }

        // GET: Offices/Deactivate/5
        public async Task<IActionResult> Deactivate(int? id)
        {
            if (id == null)
                return NotFound();

            try
            {
                var office = await _context.Offices
                    .Include(o => o.Licensees)
                    .FirstOrDefaultAsync(o => o.OfficeID == id);

                if (office == null)
                    return NotFound();

                ViewBag.AvailableOffices = new SelectList(
                    _context.Offices.Where(o => o.Active && o.OfficeID != id),
                    "OfficeID", "Name"
                );

                return View(office);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Failed to load Deactivate page for Office ID {id}: {ex.Message}");
                return RedirectToAction("Error", "Home");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeactivateConfirmed(int id, int? newOfficeId)
        {
            try
            {
                var office = await _context.Offices
                    .Include(o => o.Licensees)
                    .FirstOrDefaultAsync(o => o.OfficeID == id);

                if (office == null)
                    return NotFound();

                // Reassign licensees if a replacement office is provided
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
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Failed to deactivate Office ID {id}: {ex.Message}");
                return RedirectToAction("Error", "Home");
            }
        }

        [HttpGet]
        public IActionResult Search(string term, string sortBy, string sortOrder = "asc")
        {
            try
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
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Office search failed: {ex.Message}");
                return RedirectToAction("Error", "Home");
            }
        }

        [HttpGet]
        public async Task<IActionResult> Activate(int id)
        {
            try
            {
                var office = await _context.Offices.FindAsync(id);
                if (office == null)
                    return NotFound();

                office.Active = true;
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = $"{office.Name} has been activated.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Failed to activate Office ID {id}: {ex.Message}");
                return RedirectToAction("Error", "Home");
            }
        }

        private bool OfficeExists(int id)
        {
            try
            {
                return _context.Offices.Any(e => e.OfficeID == id);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Failed to check Office existence for ID {id}: {ex.Message}");
                return false;
            }
        }
    }
}
