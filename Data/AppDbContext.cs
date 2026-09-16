using HealthcareClaimsApi.Models;
using Microsoft.EntityFrameworkCore;

namespace HealthcareClaimsApi.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Patient> Patients => Set<Patient>();
    public DbSet<Appointment> Appointments => Set<Appointment>();
    public DbSet<Claim> Claims => Set<Claim>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Patient>()
            .HasMany(p => p.Appointments)
            .WithOne(a => a.Patient)
            .HasForeignKey(a => a.PatientId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Appointment>()
            .HasOne(a => a.Claim)
            .WithOne(c => c.Appointment)
            .HasForeignKey<Claim>(c => c.AppointmentId)
            .OnDelete(DeleteBehavior.Cascade);
        modelBuilder.Entity<Claim>()
            .Property(c => c.BilledAmount)
            .HasPrecision(10, 2);

        modelBuilder.Entity<Claim>()
            .Property(c => c.ApprovedAmount)
            .HasPrecision(10, 2);
    }
}
