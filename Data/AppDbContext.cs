using Microsoft.EntityFrameworkCore;
using PatientTestManager.Models;

namespace PatientTestManager.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<Patient> Patients => Set<Patient>();
        public DbSet<Test> Tests => Set<Test>();

        protected override void OnConfiguring(DbContextOptionsBuilder options)
            => options.UseSqlServer("Server=(localdb)\\MSSQLLocalDB;Database=PatientTestDb;Trusted_Connection=True;Encrypt=False;");

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Patient>()
                .HasMany(p => p.Tests)
                .WithOne(t => t.Patient)
                .HasForeignKey(t => t.PatientId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
