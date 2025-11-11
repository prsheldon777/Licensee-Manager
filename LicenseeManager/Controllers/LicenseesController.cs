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
    /// <summary>
    /// Controller responsible for managing licensee records.
    /// Provides actions to list, view details, create, edit, and search licensees.
    /// All data access is performed via an injected <see cref="AppDbContext"/>.
    /// </summary>
    public class LicenseesController : Controller
    {
        private readonly AppDbContext _context;

        /// <summary>
        /// Initializes a new instance of the <see cref="LicenseesController"/> class.
        /// </summary>
        /// <param name="context">The application's database context used to query and persist licensee-related data.</param>
        public LicenseesController(AppDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// GET: Licensees
        /// Retrieves the list of licensees including their license type and office information.
        /// The list is ordered with active statuses first by descending status.
        /// </summary>
        /// <returns>
        /// An asynchronous task that resolves to an <see cref="IActionResult"/> rendering the default Index view
        /// populated with the list of licensees.
        /// </returns>
        /// <exception cref="Exception">
        /// Exceptions thrown while querying the database are caught; errors are written to the console
        /// and the user is redirected to the Home/Error page.
        /// </exception>
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

        /// <summary>
        /// GET: Licensees/Details/5
        /// Retrieves a single licensee by identifier including related license type and office.
        /// </summary>
        /// <param name="id">The identifier of the licensee to display. Nullable.</param>
        /// <returns>
        /// - <see cref="NotFoundResult"/> when <paramref name="id"/> is null or the licensee does not exist.
        /// - A view rendering the licensee when found.
        /// </returns>
        /// <exception cref="Exception">
        /// Database query exceptions are caught and logged to the console; the user is redirected to Home/Error.
        /// </exception>
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

        /// <summary>
        /// GET: Licensees/Create
        /// Prepares data required for the Create view, including active offices, license types, and status options.
        /// Excludes the 'Expired' status from the initial creation status list.
        /// </summary>
        /// <returns>An <see cref="IActionResult"/> that renders the Create view with populated ViewData entries.</returns>
        /// <exception cref="Exception">
        /// Any exception while preparing view data is caught, logged, and redirects to Home/Error.
        /// </exception>
        public async Task<IActionResult> Create()
        {
            try
            {
                var activeOffices = await _context.Offices
                    .Where(o => o.Active)
                    .ToListAsync();

                var statusOptions = Enum.GetValues(typeof(LicenseeStatus))
                    .Cast<LicenseeStatus>()
                    .Where(s => s != LicenseeStatus.Expired) // exclude Expired for creation
                    .Select(s => new SelectListItem
                    {
                        Value = ((int)s).ToString(),
                        Text = s.ToString()
                    })
                    .ToList();

                ViewData["LicenseTypeID"] = new SelectList(_context.LicenseType, "LicenseTypeID", "Name");
                ViewData["OfficeID"] = new SelectList(activeOffices, "OfficeID", "Name");
                ViewData["StatusList"] = new SelectList(statusOptions, "Value", "Text");
                return View();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] LicenseesController.Create (GET) failed at {DateTime.Now}: {ex.Message}");
                return RedirectToAction("Error", "Home");
            }
        }

        /// <summary>
        /// POST: Licensees/Create
        /// Attempts to create a new licensee record. If model validation passes, sets created timestamp and persists the entity.
        /// If validation fails, repopulates select lists and returns the Create view with validation messages.
        /// </summary>
        /// <param name="licensee">The licensee model bound from the request form.</param>
        /// <returns>
        /// - Redirects to Index on successful creation.
        /// - Returns the Create view with the provided model and populated select lists when validation fails.
        /// </returns>
        /// <remarks>
        /// Uses anti-forgery validation via <see cref="ValidateAntiForgeryTokenAttribute"/>.
        /// </remarks>
        /// <exception cref="Exception">
        /// Exceptions during database operations are caught and redirect to Home/Error after logging.
        /// </exception>
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

                // Build the status dropdown logic
                var statuses = Enum.GetValues(typeof(LicenseeStatus))
                    .Cast<LicenseeStatus>()
                    .Select(s => new SelectListItem
                    {
                        Value = ((int)s).ToString(),
                        Text = s.ToString()
                    })
                    .ToList();

                // If NOT expired, remove "Expired" from the dropdown
                if (licensee.Status != LicenseeStatus.Expired)
                {
                    statuses = statuses
                        .Where(s => s.Text != LicenseeStatus.Expired.ToString())
                        .ToList();
                }

                ViewData["LicenseTypeID"] = new SelectList(_context.LicenseType, "LicenseTypeID", "Name", licensee.LicenseTypeID);
                ViewData["OfficeID"] = new SelectList(activeOffices, "OfficeID", "Name", licensee.OfficeID);
                ViewData["StatusList"] = new SelectList(statuses, "Value", "Text", ((int)licensee.Status).ToString());
                return View(licensee);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] LicenseesController.Create (POST fallback) failed at {DateTime.Now}: {ex.Message}");
                return RedirectToAction("Error", "Home");
            }
        }

        /// <summary>
        /// GET: Licensees/Edit/5
        /// Loads an existing licensee for editing and prepares select list data (license types, active offices, status list).
        /// Excludes the 'Expired' status unless the current entity is already expired.
        /// </summary>
        /// <param name="id">The identifier of the licensee to edit. Nullable.</param>
        /// <returns>
        /// - <see cref="NotFoundResult"/> if <paramref name="id"/> is null or the licensee does not exist.
        /// - A view rendering the Edit form populated with the licensee model and select lists.
        /// </returns>
        /// <exception cref="Exception">
        /// Errors encountered while loading the licensee or preparing view data are caught and cause a redirect to Home/Error.
        /// </exception>
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

                // Build the status dropdown logic
                var statuses = Enum.GetValues(typeof(LicenseeStatus))
                    .Cast<LicenseeStatus>()
                    .Select(s => new SelectListItem
                    {
                        Value = ((int)s).ToString(),
                        Text = s.ToString()
                    })
                    .ToList();

                // If NOT expired, remove "Expired" from the dropdown
                if (licensee.Status != LicenseeStatus.Expired)
                {
                    statuses = statuses
                        .Where(s => s.Text != LicenseeStatus.Expired.ToString())
                        .ToList();
                }

                ViewData["LicenseTypeID"] = new SelectList(_context.LicenseType, "LicenseTypeID", "Name", licensee.LicenseTypeID);
                ViewData["OfficeID"] = new SelectList(activeOffices, "OfficeID", "Name", licensee.OfficeID);
                ViewData["StatusList"] = new SelectList(statuses, "Value", "Text", ((int)licensee.Status).ToString());
                return View(licensee);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] LicenseesController.Edit (GET) failed for ID {id} at {DateTime.Now}: {ex.Message}");
                return RedirectToAction("Error", "Home");
            }
        }

        /// <summary>
        /// POST: Licensees/Edit/5
        /// Persists changes to an existing licensee. Validates that the id matches the model and prevents saving an Expired status.
        /// Handles concurrency exceptions and other database errors.
        /// </summary>
        /// <param name="id">The identifier of the licensee being edited.</param>
        /// <param name="licensee">The bound licensee model containing updated values.</param>
        /// <returns>
        /// - Redirects to Index after a successful update.
        /// - Returns the Edit view with validation messages when model state is invalid.
        /// - Returns NotFound when the id does not match or the entity no longer exists.
        /// </returns>
        /// <exception cref="DbUpdateConcurrencyException">
        /// Thrown when a concurrency conflict occurs while saving; method checks for existence and rethrows if not resolvable.
        /// </exception>
        /// <exception cref="Exception">
        /// Other database errors are caught, logged, and redirect to Home/Error.
        /// </exception>
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

                if (licensee.Status == LicenseeStatus.Expired)
                {
                    ModelState.AddModelError("Status", "A licensee cannot be saved with an expired status. Please renew their license or mark them as inactive instead.");
                    return View(licensee);
                }

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

                // Build the status dropdown logic
                var statuses = Enum.GetValues(typeof(LicenseeStatus))
                    .Cast<LicenseeStatus>()
                    .Select(s => new SelectListItem
                    {
                        Value = ((int)s).ToString(),
                        Text = s.ToString()
                    })
                    .ToList();

                // If NOT expired, remove "Expired" from the dropdown
                if (licensee.Status != LicenseeStatus.Expired)
                {
                    statuses = statuses
                        .Where(s => s.Text != LicenseeStatus.Expired.ToString())
                        .ToList();
                }

                ViewData["LicenseTypeID"] = new SelectList(_context.LicenseType, "LicenseTypeID", "Name", licensee.LicenseTypeID);
                ViewData["OfficeID"] = new SelectList(activeOffices, "OfficeID", "Name", licensee.OfficeID);
                ViewData["StatusList"] = new SelectList(statuses, "Value", "Text", ((int)licensee.Status).ToString());
                return View(licensee);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] LicenseesController.Edit (POST fallback) failed at {DateTime.Now}: {ex.Message}");
                return RedirectToAction("Error", "Home");
            }
        }

        /// <summary>
        /// Determines whether a licensee with the specified id exists in the database.
        /// </summary>
        /// <param name="id">The licensee identifier to check for existence.</param>
        /// <returns><c>true</c> if a licensee with the given id exists; otherwise, <c>false</c>.</returns>
        private bool LicenseeExists(int id)
        {
            return _context.Licensees.Any(e => e.LicenseeID == id);
        }

        /// <summary>
        /// GET: Licensees/Search
        /// Performs a search and optional sorting over licensee records and returns a partial view with results.
        /// Supports searching by first or last name and many sortable columns.
        /// </summary>
        /// <param name="term">Optional search term used to match first or last name.</param>
        /// <param name="sortBy">Optional column name to sort by (e.g., "FirstName", "LastName", "Email").</param>
        /// <param name="sortOrder">Sort order, either "asc" (ascending) or "desc" (descending). Defaults to "asc".</param>
        /// <returns>
        /// A partial view named "_LicenseeTable" populated with the filtered and sorted list of licensees.
        /// </returns>
        /// <exception cref="Exception">
        /// Exceptions during query execution are caught, logged, and redirect to Home/Error.
        /// </exception>
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
