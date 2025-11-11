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
    /// <summary>
    /// Controller responsible for managing Office entities.
    /// Provides actions for listing, creating, editing, activating, deactivating and searching offices.
    /// </summary>
    public class OfficesController : Controller
    {
        private readonly AppDbContext _context;

        /// <summary>
        /// Initializes a new instance of the <see cref="OfficesController"/> class.
        /// </summary>
        /// <param name="context">The application database context used to query and persist office data.</param>
        public OfficesController(AppDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// GET: Offices
        /// Retrieves all offices and returns the Index view.
        /// </summary>
        /// <returns>An <see cref="IActionResult"/> that renders the Index view with the list of offices.
        /// On error, logs the exception and redirects to Home/Error.</returns>
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

        /// <summary>
        /// GET: Offices/Details/{id}
        /// Shows details for a specific office.
        /// </summary>
        /// <param name="id">The identifier of the office to display. If null, returns NotFound.</param>
        /// <returns>
        /// A task that resolves to an <see cref="IActionResult"/>.
        /// - Returns <see cref="NotFoundResult"/> when <paramref name="id"/> is null or the office is not found.
        /// - Returns a view with the office model when found.
        /// - On exception, logs and redirects to Home/Error.
        /// </returns>
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

        /// <summary>
        /// GET: Offices/Create
        /// Returns the Create view to add a new office.
        /// </summary>
        /// <returns>An <see cref="IActionResult"/> that renders the Create view.</returns>
        public IActionResult Create()
        {
            // No DB work here, safe without try/catch
            return View();
        }

        /// <summary>
        /// POST: Offices/Create
        /// Persists a new office when the posted model is valid.
        /// </summary>
        /// <param name="office">The <see cref="Office"/> model bound from the request form.</param>
        /// <returns>
        /// - Redirects to Index on success.
        /// - Returns the Create view with validation errors when model state is invalid.
        /// - On exception, logs and redirects to Home/Error.
        /// </returns>
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

        /// <summary>
        /// GET: Offices/Edit/{id}
        /// Loads the Edit view for an office.
        /// </summary>
        /// <param name="id">The identifier of the office to edit. If null, returns NotFound.</param>
        /// <returns>
        /// - A view populated with the office model when found.
        /// - NotFound when the id is null or the office does not exist.
        /// - On exception, logs and redirects to Home/Error.
        /// </returns>
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

        /// <summary>
        /// POST: Offices/Edit/{id}
        /// Persists changes to an existing office when the posted model is valid.
        /// </summary>
        /// <param name="id">The identifier of the office being edited.</param>
        /// <param name="office">The <see cref="Office"/> model containing updated values.</param>
        /// <returns>
        /// - Redirects to Index on successful update.
        /// - Returns the Edit view with validation messages when model state is invalid.
        /// - Returns NotFound when the provided id does not match the model id.
        /// - On concurrency conflict, checks existence and rethrows if necessary.
        /// - On other exceptions, logs and redirects to Home/Error.
        /// </returns>
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

        /// <summary>
        /// GET: Offices/Deactivate/{id}
        /// Shows a confirmation page for deactivating an office and optionally reassigning its licensees.
        /// </summary>
        /// <param name="id">The identifier of the office to deactivate. If null, returns NotFound.</param>
        /// <returns>
        /// - A view populated with the office and a SelectList of other available active offices for reassignment.
        /// - NotFound when the id is null or the office is not found.
        /// - On exception, logs and redirects to Home/Error.
        /// </returns>
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

        /// <summary>
        /// POST: Offices/DeactivateConfirmed
        /// Deactivates the specified office and optionally reassigns its licensees to another office.
        /// </summary>
        /// <param name="id">The identifier of the office to deactivate.</param>
        /// <param name="newOfficeId">Optional replacement office id to reassign licensees to. If null, licensees are not reassigned.</param>
        /// <returns>
        /// - Redirects to Index on success with a TempData message.
        /// - Returns NotFound when the office cannot be found.
        /// - On exception, logs and redirects to Home/Error.
        /// </returns>
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

        /// <summary>
        /// GET: Offices/Search
        /// Performs a search and optional sorting over offices and returns a partial view with results.
        /// </summary>
        /// <param name="term">Optional search term used to match Name, City or State. Trimmed and ignored if null or whitespace.</param>
        /// <param name="sortBy">Optional column name to sort by ("Name", "City", "State", "ActiveStatus").</param>
        /// <param name="sortOrder">Sort order, either "asc" (ascending) or "desc" (descending). Defaults to "asc".</param>
        /// <returns>
        /// A <see cref="PartialViewResult"/> rendering "_OfficeTable" populated with the filtered and sorted list of offices.
        /// On exception, logs and redirects to Home/Error.
        /// </returns>
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

        /// <summary>
        /// GET: Offices/Activate/{id}
        /// Activates the specified office.
        /// </summary>
        /// <param name="id">The identifier of the office to activate.</param>
        /// <returns>
        /// - Redirects to Index on success with a TempData success message.
        /// - Returns NotFound when the office does not exist.
        /// - On exception, logs and redirects to Home/Error.
        /// </returns>
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

        /// <summary>
        /// Determines whether an office with the specified id exists.
        /// </summary>
        /// <param name="id">The office identifier to check.</param>
        /// <returns><c>true</c> if the office exists; otherwise <c>false</c>. If an exception occurs while checking,
        /// logs the error and returns <c>false</c>.</returns>
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
