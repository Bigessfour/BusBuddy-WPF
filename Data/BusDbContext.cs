using Microsoft.EntityFrameworkCore;
using BusBuddy.Models;

namespace BusBuddy.Data
{
    public class BusDbContext : DbContext
    {
        public DbSet<Bus> Buses { get; set; } = null!;
        public DbSet<Route> Routes { get; set; } = null!;
        public DbSet<Driver> Drivers { get; set; } = null!;
        public DbSet<Activity> Activities { get; set; } = null!;
        public DbSet<Fuel> Fuel { get; set; } = null!;
        public DbSet<Maintenance> Maintenance { get; set; } = null!;
        public DbSet<SchoolCalendar> SchoolCalendar { get; set; } = null!;
        public DbSet<ActivitySchedule> ActivitySchedule { get; set; } = null!;
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Server=(localdb)\\MSSQLLocalDB;Database=BusBuddy;Trusted_Connection=True;");
        }
    }
}
