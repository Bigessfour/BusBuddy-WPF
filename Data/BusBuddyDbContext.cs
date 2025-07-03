using Microsoft.EntityFrameworkCore;
using Bus_Buddy.Models;

namespace Bus_Buddy.Data;

/// <summary>
/// Entity Framework DbContext for BusBuddy application
/// Manages database connection and entity configurations
/// </summary>
public class BusBuddyDbContext : DbContext
{
    public BusBuddyDbContext(DbContextOptions<BusBuddyDbContext> options) : base(options)
    {
    }

    // DbSets for all entities
    public DbSet<Bus> Vehicles { get; set; }
    public DbSet<Driver> Drivers { get; set; }
    public DbSet<Route> Routes { get; set; }
    public DbSet<Activity> Activities { get; set; }
    public DbSet<Fuel> FuelRecords { get; set; }
    public DbSet<Maintenance> MaintenanceRecords { get; set; }
    public DbSet<Student> Students { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure Bus (Vehicle) entity
        modelBuilder.Entity<Bus>(entity =>
        {
            entity.ToTable("Vehicles");
            entity.HasKey(e => e.VehicleId);
            entity.Property(e => e.BusNumber).IsRequired().HasMaxLength(20);
            entity.Property(e => e.VINNumber).IsRequired().HasMaxLength(17);
            entity.Property(e => e.LicenseNumber).IsRequired().HasMaxLength(20);
            entity.Property(e => e.PurchasePrice).HasColumnType("decimal(10,2)");
            entity.HasIndex(e => e.BusNumber).IsUnique();
            entity.HasIndex(e => e.VINNumber).IsUnique();
        });

        // Configure Driver entity
        modelBuilder.Entity<Driver>(entity =>
        {
            entity.ToTable("Drivers");
            entity.HasKey(e => e.DriverId);
            entity.Property(e => e.DriverName).IsRequired().HasMaxLength(100);
        });

        // Configure Route entity with multiple relationships
        modelBuilder.Entity<Route>(entity =>
        {
            entity.ToTable("Routes");
            entity.HasKey(e => e.RouteId);

            // Configure AM relationships
            entity.HasOne(r => r.AMVehicle)
                  .WithMany(v => v.Routes)
                  .HasForeignKey(r => r.AMVehicleId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(r => r.AMDriver)
                  .WithMany(d => d.AMRoutes)
                  .HasForeignKey(r => r.AMDriverId)
                  .OnDelete(DeleteBehavior.Restrict);

            // Configure PM relationships
            entity.HasOne(r => r.PMVehicle)
                  .WithMany()
                  .HasForeignKey(r => r.PMVehicleId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(r => r.PMDriver)
                  .WithMany(d => d.PMRoutes)
                  .HasForeignKey(r => r.PMDriverId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        // Configure Activity entity
        modelBuilder.Entity<Activity>(entity =>
        {
            entity.ToTable("Activity");
            entity.HasKey(e => e.ActivityId);

            entity.HasOne(a => a.Vehicle)
                  .WithMany(v => v.Activities)
                  .HasForeignKey(a => a.VehicleId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(a => a.Route)
                  .WithMany()
                  .HasForeignKey(a => a.RouteId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(a => a.Driver)
                  .WithMany(d => d.Activities)
                  .HasForeignKey(a => a.DriverId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        // Configure Fuel entity
        modelBuilder.Entity<Fuel>(entity =>
        {
            entity.ToTable("Fuel");
            entity.HasKey(e => e.FuelId);
            entity.Property(e => e.Gallons).HasColumnType("decimal(8,3)");
            entity.Property(e => e.PricePerGallon).HasColumnType("decimal(8,3)");
            entity.Property(e => e.TotalCost).HasColumnType("decimal(10,2)");
            entity.Property(e => e.MilesPerGallon).HasColumnType("decimal(5,2)");

            entity.HasOne(f => f.Vehicle)
                  .WithMany(v => v.FuelRecords)
                  .HasForeignKey(f => f.VehicleId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        // Configure Maintenance entity
        modelBuilder.Entity<Maintenance>(entity =>
        {
            entity.ToTable("Maintenance");
            entity.HasKey(e => e.MaintenanceId);
            entity.Property(e => e.Cost).HasColumnType("decimal(10,2)");

            entity.HasOne(m => m.Vehicle)
                  .WithMany(v => v.MaintenanceRecords)
                  .HasForeignKey(m => m.VehicleId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        // Configure Student entity
        modelBuilder.Entity<Student>(entity =>
        {
            entity.ToTable("Students");
            entity.HasKey(e => e.StudentId);
            entity.Property(e => e.StudentName).IsRequired().HasMaxLength(100);
        });

        // Seed some initial data
        SeedData(modelBuilder);
    }

    private void SeedData(ModelBuilder modelBuilder)
    {
        // Seed sample buses
        modelBuilder.Entity<Bus>().HasData(
            new Bus
            {
                VehicleId = 1,
                BusNumber = "001",
                Year = 2020,
                Make = "Blue Bird",
                Model = "Vision",
                SeatingCapacity = 72,
                VINNumber = "1BAANKCL7LF123456",
                LicenseNumber = "TX123456",
                Status = "Active",
                PurchaseDate = new DateTime(2020, 8, 15),
                PurchasePrice = 85000.00m
            },
            new Bus
            {
                VehicleId = 2,
                BusNumber = "002",
                Year = 2019,
                Make = "Thomas Built",
                Model = "Saf-T-Liner C2",
                SeatingCapacity = 66,
                VINNumber = "4DRBTAAN7KB654321",
                LicenseNumber = "TX654321",
                Status = "Active",
                PurchaseDate = new DateTime(2019, 7, 10),
                PurchasePrice = 82000.00m
            }
        );

        // Seed sample drivers
        modelBuilder.Entity<Driver>().HasData(
            new Driver
            {
                DriverId = 1,
                DriverName = "John Smith",
                DriverPhone = "555-0123",
                DriverEmail = "john.smith@school.edu",
                DriversLicenceType = "CDL",
                TrainingComplete = true
            },
            new Driver
            {
                DriverId = 2,
                DriverName = "Mary Johnson",
                DriverPhone = "555-0456",
                DriverEmail = "mary.johnson@school.edu",
                DriversLicenceType = "CDL",
                TrainingComplete = true
            }
        );
    }
}
