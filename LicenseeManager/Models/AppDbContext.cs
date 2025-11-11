using LicenseeManager.Models;
using Microsoft.EntityFrameworkCore;

namespace Licensee_Manager.Models
{
    /// <summary>
    /// Entity Framework Core database context for the Licensee Manager application.
    /// </summary>
    /// <remarks>
    /// This context exposes DbSet properties for application entities and configures
    /// model-level mappings such as table names, triggers, and value conversions.
    /// It is intended to be configured and registered with dependency injection.
    /// </remarks>
    public class AppDbContext : DbContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AppDbContext"/> class.
        /// </summary>
        /// <param name="options">
        /// The options to be used by a <see cref="DbContext"/>. These options are typically
        /// configured in <c>Program.cs</c> or <c>Startup.cs</c> and provided via dependency injection.
        /// </param>
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        /// <summary>
        /// Gets or sets the Offices DbSet.
        /// Represents the collection of <see cref="Office"/> entities in the database.
        /// </summary>
        public DbSet<Office> Offices { get; set; }

        /// <summary>
        /// Gets or sets the Licensees DbSet.
        /// Represents the collection of <see cref="Licensee"/> entities in the database.
        /// </summary>
        public DbSet<Licensee> Licensees { get; set; }

        /// <summary>
        /// Gets or sets the LicenseType DbSet.
        /// Represents the collection of <see cref="LicenseType"/> entities in the database.
        /// </summary>
        public DbSet<LicenseType> LicenseType { get; set; }

        /// <summary>
        /// Gets or sets the Audits DbSet.
        /// Represents the collection of <see cref="Audit"/> entities (licensee status audit records).
        /// </summary>
        public DbSet<Audit> Audits { get; set; }

        /// <summary>
        /// Configures the EF Core model mappings for the application's entities.
        /// </summary>
        /// <param name="modelBuilder">The builder used to configure entity mappings.</param>
        /// <remarks>
        /// - Maps the <see cref="Licensee"/> entity to the "Licensees" table and associates the
        ///   database trigger named "UpdateLicensee" with that table.
        /// - Configures enum-to-int conversions for <see cref="Licensee.Status"/>, <see cref="Audit.OldStatus"/>, 
        ///   and <see cref="Audit.NewStatus"/> so enum values are stored as integers in the database.
        /// </remarks>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Licensee>()
                .ToTable("Licensees", t => t.HasTrigger("UpdateLicensee"));

            modelBuilder.Entity<Licensee>()
                .Property(l => l.Status)
                .HasConversion<int>();

            modelBuilder.Entity<Audit>()
            .Property(a => a.OldStatus)
            .HasConversion<int>();

            modelBuilder.Entity<Audit>()
                .Property(a => a.NewStatus)
                .HasConversion<int>();
        }
    }
}
