using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Bus_Buddy.Data;
using Bus_Buddy.Services;
using System;

namespace BusBuddy.Tests.Infrastructure
{
    /// <summary>
    /// Base class for all unit and integration tests
    /// Provides common setup for testing infrastructure
    /// </summary>
    public abstract class TestBase : IDisposable
    {
        protected ServiceProvider ServiceProvider { get; private set; } = null!;
        protected BusBuddyDbContext DbContext { get; private set; } = null!;
        protected IConfiguration Configuration { get; private set; } = null!;

        protected TestBase()
        {
            SetupServices();
            DbContext = ServiceProvider.GetRequiredService<BusBuddyDbContext>();
        }

        private void SetupServices()
        {
            var services = new ServiceCollection();

            // Configuration
            Configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.test.json", optional: false)
                .Build();

            services.AddSingleton(Configuration);

            // Logging
            services.AddLogging(builder =>
            {
                builder.AddConsole();
                builder.SetMinimumLevel(LogLevel.Warning);
            });

            // Database - Use In-Memory for tests
            services.AddDbContext<BusBuddyDbContext>(options =>
            {
                options.UseInMemoryDatabase($"TestDb_{Guid.NewGuid()}");
                options.EnableSensitiveDataLogging();
            });

            // Services
            services.AddScoped<IBusService, BusService>();
            services.AddScoped<IRouteService, RouteService>();
            services.AddScoped<IActivityService, ActivityService>();
            services.AddScoped<IScheduleService, ScheduleService>();
            services.AddScoped<IStudentService, StudentService>();
            services.AddScoped<IFuelService, FuelService>();
            services.AddScoped<IMaintenanceService, MaintenanceService>();
            services.AddScoped<ITicketService, TicketService>();

            ServiceProvider = services.BuildServiceProvider();
        }

        public virtual void Dispose()
        {
            DbContext?.Dispose();
            ServiceProvider?.Dispose();
        }

        /// <summary>
        /// Clears all data from the in-memory database for test isolation
        /// </summary>
        protected async Task ClearDatabaseAsync()
        {
            if (DbContext != null)
            {
                // Remove dependent entities first to avoid foreign key constraint violations
                // Order matters: remove child entities before parent entities
                DbContext.MaintenanceRecords.RemoveRange(DbContext.MaintenanceRecords);
                DbContext.FuelRecords.RemoveRange(DbContext.FuelRecords);
                DbContext.Tickets.RemoveRange(DbContext.Tickets);
                DbContext.Schedules.RemoveRange(DbContext.Schedules);
                DbContext.Activities.RemoveRange(DbContext.Activities);

                // Now remove parent entities
                DbContext.Vehicles.RemoveRange(DbContext.Vehicles);
                DbContext.Drivers.RemoveRange(DbContext.Drivers);
                DbContext.Routes.RemoveRange(DbContext.Routes);
                DbContext.Students.RemoveRange(DbContext.Students);

                await DbContext.SaveChangesAsync();
            }
        }
    }
}
