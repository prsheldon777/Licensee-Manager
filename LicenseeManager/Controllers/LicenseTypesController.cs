using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Licensee_Manager.Models;
using LicenseeManager.Models;

namespace LicenseeManager.Controllers
{
    /// <summary>
    /// Controller responsible for managing <see cref="LicenseType"/> entities.
    /// Provides actions to list, search, view details, create and edit license types.
    /// </summary>
    public class LicenseTypesController : Controller
    {
        private readonly AppDbContext _context;

        /// <summary>
        /// Initializes a new instance of the <see cref="LicenseTypesController"/> class.
        /// </summary>
        /// <param name="context">The application database context used to access license type data. Provided by dependency injection.</param>
        public LicenseTypesController(AppDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// GET: LicenseTypes
        /// Retrieves all license types and returns the Index view.
        /// </summary>
        /// <returns>
        /// An asynchronous task that resolves to an <see cref="IActionResult"/> which renders the Index view
        /// populated with all <see cref="LicenseType"/> records.
        /// On error, logs the exception and redirects to Home/Error.
        /// </returns>
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

        /// <summary>
        /// GET: LicenseTypes/Search
        /// Performs a search and optional sorting over license types and returns a partial view with results.
        /// </summary>
        /// <param name="term">Optional search term to filter license types by name. Ignored if null or whitespace.</param>
        /// <param name="sortBy">Optional column to sort by. Currently supports "Name".</param>
        /// <param name="sortOrder">Sort direction; expected values "asc" or "desc". If not "desc", ascending order is used.</param>
        /// <returns>
        /// A <see cref="PartialViewResult"/> rendering "_LicenseTypeTable" with filtered and sorted results.
        /// On error, logs the exception and redirects to Home/Error.
        /// </returns>
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

        /// <summary>
        /// GET: LicenseTypes/Details/{id}
        /// Retrieves a single license type by its identifier and returns the Details view.
        /// </summary>
        /// <param name="id">The identifier of the <see cref="LicenseType"/> to display. If null, returns NotFound.</param>
        /// <returns>
        /// An asynchronous task that resolves to an <see cref="IActionResult"/>.
        /// Returns <see cref="NotFoundResult"/> when <paramref name="id"/> is null or when the entity does not exist.
        /// Otherwise returns a View with the <see cref="LicenseType"/> model.
        /// On error, logs and redirects to Home/Error.
        /// </returns>
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

        /// <summary>
        /// GET: LicenseTypes/Create
        /// Returns the Create view for creating a new license type.
        /// </summary>
        /// <returns>
        /// An <see cref="IActionResult"/> that renders the Create view.
        /// On error, logs the exception and redirects to Home/Error.
        /// </returns>
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

        /// <summary>
        /// POST: LicenseTypes/Create
        /// Persists a new <see cref="LicenseType"/> when the posted model is valid.
        /// </summary>
        /// <param name="licenseType">The license type model bound from the request.</param>
        /// <returns>
        /// Redirects to Index on success. If model state is invalid, re-displays the Create view with validation messages.
        /// On exception, logs the error and redirects to Home/Error.
        /// </returns>
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

        /// <summary>
        /// GET: LicenseTypes/Edit/{id}
        /// Loads the Edit view for the specified license type.
        /// </summary>
        /// <param name="id">The identifier of the <see cref="LicenseType"/> to edit. If null, returns NotFound.</param>
        /// <returns>
        /// An asynchronous task that resolves to an <see cref="IActionResult"/>.
        /// Returns NotFound when <paramref name="id"/> is null or the entity is not found.
        /// Otherwise returns a View populated with the <see cref="LicenseType"/> model.
        /// On error, logs the exception and redirects to Home/Error.
        /// </returns>
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

        /// <summary>
        /// POST: LicenseTypes/Edit/{id}
        /// Persists changes to an existing <see cref="LicenseType"/>. Handles concurrency conflicts by verifying existence.
        /// </summary>
        /// <param name="id">The identifier of the license type being edited. Must match <see cref="LicenseType.LicenseTypeID"/>.</param>
        /// <param name="licenseType">The bound license type model containing updated values.</param>
        /// <returns>
        /// Redirects to Index on success. If model state is invalid, re-displays the Edit view with validation messages.
        /// Returns NotFound when the provided id does not match the model or the entity no longer exists.
        /// On concurrency conflict, checks for existence and either returns NotFound or rethrows the concurrency exception.
        /// On other exceptions, logs and redirects to Home/Error.
        /// </returns>
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

        /// <summary>
        /// Determines whether a <see cref="LicenseType"/> with the specified id exists.
        /// </summary>
        /// <param name="id">The identifier to check for existence.</param>
        /// <returns>
        /// <c>true</c> if a license type with the given id exists; otherwise <c>false</c>.
        /// If an exception occurs while checking, logs the error and returns <c>false</c>.
        /// </returns>
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
