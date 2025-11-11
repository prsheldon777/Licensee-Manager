using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LicenseeManager.Models;
using Licensee_Manager.Models;

namespace LicenseeManager.Controllers
{
    /// <summary>
    /// Controller responsible for presenting audit records related to licensee status changes.
    /// Provides actions to list audits and view details for a specific audit entry.
    /// </summary>
    public class AuditsController : Controller
    {
        private readonly AppDbContext _context;

        /// <summary>
        /// Initializes a new instance of the <see cref="AuditsController"/> class.
        /// </summary>
        /// <param name="context">
        /// The application's database context used to query audit and related licensee data.
        /// This dependency is provided via dependency injection.
        /// </param>
        public AuditsController(AppDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// GET: Audits
        /// Retrieves the list of audit records including related licensee information and returns the Index view.
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous operation. The task result is an <see cref="IActionResult"/>
        /// that renders the view populated with the list of audits.
        /// </returns>
        /// <exception cref="Exception">
        /// Any exception thrown while querying the database is caught; errors are logged to the console
        /// and the user is redirected to the Home/Error page.
        /// </exception>
        public async Task<IActionResult> Index()
        {
            try
            {
                var appDbContext = _context.Audits.Include(a => a.Licensee);
                return View(await appDbContext.ToListAsync());
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Failed to load audit list: {ex.Message}");
                // In a production environment, this would be written to a persistent error log.
                return RedirectToAction("Error", "Home");
            }
        }

        /// <summary>
        /// GET: Audits/Details/{id}
        /// Retrieves a single audit record by its identifier, including related licensee information, and returns the Details view.
        /// </summary>
        /// <param name="id">The identifier of the audit record to display. This parameter is nullable.</param>
        /// <returns>
        /// A task that represents the asynchronous operation. The task result is an <see cref="IActionResult"/>.
        /// - Returns <see cref="NotFoundResult"/> when <paramref name="id"/> is null or the audit record does not exist.
        /// - Returns a ViewResult with the audit model when found.
        /// - Redirects to the Home/Error page if an exception occurs while querying the database.
        /// </returns>
        /// <exception cref="Exception">
        /// Any exception thrown while querying the database is caught; errors are logged to the console
        /// and the user is redirected to the Home/Error page.
        /// </exception>
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            try
            {
                var audit = await _context.Audits
                    .Include(a => a.Licensee)
                    .FirstOrDefaultAsync(m => m.AuditId == id);

                if (audit == null)
                {
                    return NotFound();
                }

                return View(audit);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Failed to load audit details for ID {id}: {ex.Message}");
                // In production, errors like this should be logged using a structured logger or database log.
                return RedirectToAction("Error", "Home");
            }
        }
    }
}
