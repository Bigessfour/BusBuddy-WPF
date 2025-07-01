using Microsoft.EntityFrameworkCore;
using BusBuddy.Models;

namespace BusBuddy.Data
{
    public class BusBuddyContext : DbContext
    {
        public DbSet<Route> Routes { get; set; }
        public DbSet<Driver> Drivers { get; set; }
        public DbSet<Vehicle> Vehicles { get; set; }
        public DbSet<Activity> Activities { get; set; }
        public DbSet<Fuel> Fuel { get; set; }
        public DbSet<Maintenance> Maintenance { get; set; }
        public DbSet<SchoolCalendar> SchoolCalendar { get; set; }
        public DbSet<ActivitySchedule> ActivitySchedule { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Server=(localdb)\\MSSQLLocalDB;Database=BusBuddy;Trusted_Connection=True;");
        }
    }
}
