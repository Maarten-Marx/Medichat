using Data.Models;
using Microsoft.EntityFrameworkCore;

namespace API.Data;

public class MedichatContext : DbContext
{
    public MedichatContext()
    {
    }

    public MedichatContext(DbContextOptions<MedichatContext> options) : base(options)
    {
    }

    public DbSet<Appointment> Appointments { get; init; }
    public DbSet<Doctor> Doctors { get; init; }
    public DbSet<Medicine> Medicine { get; init; }
    public DbSet<Prescription> Prescriptions { get; init; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Appointment>().ToTable("Appointment");
        modelBuilder.Entity<Doctor>().ToTable("Doctor");
        modelBuilder.Entity<Medicine>().ToTable("Medicine");
        modelBuilder.Entity<Prescription>().ToTable("Prescription");
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseSqlServer("Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=Medichat;Integrated Security=True;Connect Timeout=30;Encrypt=False;Trust Server Certificate=False;Application Intent=ReadWrite;Multi Subnet Failover=False");
        }
    }
}