using LicenseeManager.Models;
using Microsoft.EntityFrameworkCore;

namespace Licensee_Manager.Models
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<Office> Offices { get; set; }
        public DbSet<Licensee> Licensees { get; set; }
        public DbSet<LicenseType> LicenseType { get; set; }
        public DbSet<Audit> Audits { get; set; }

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
