using Microsoft.EntityFrameworkCore;
using BusBuddy.Models;
using Microsoft.Extensions.Configuration;

namespace BusBuddy.Data
{
    public class BusBuddyContext : DbContext
    {
        public DbSet<Route> Routes { get; set; }
        public DbSet<Driver> Drivers { get; set; }
        public DbSet<Bus> Buses { get; set; }
        public DbSet<Activity> Activities { get; set; }
        public DbSet<Fuel> Fuel { get; set; }
        public DbSet<Maintenance> Maintenance { get; set; }
        public DbSet<SchoolCalendar> SchoolCalendar { get; set; }
        public DbSet<ActivitySchedule> ActivitySchedule { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                var config = new ConfigurationBuilder()
                    .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                    .Build();
                var connectionString = config.GetConnectionString("BusBuddyConnection");
                optionsBuilder.UseSqlServer(connectionString);
            }
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Route>().HasKey(r => r.ID);
            modelBuilder.Entity<Route>()
                .HasOne(r => r.AMBus)
                .WithMany()
                .HasForeignKey(r => r.AMBusID)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Route>()
                .HasOne(r => r.AMDriver)
                .WithMany()
                .HasForeignKey(r => r.AMDriverID)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Route>()
                .HasOne(r => r.PMBus)
                .WithMany()
                .HasForeignKey(r => r.PMBusID)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Route>()
                .HasOne(r => r.PMDriver)
                .WithMany()
                .HasForeignKey(r => r.PMDriverID)
                .OnDelete(DeleteBehavior.Restrict);
            // ...configure other entities as needed...
        }
    }
}
