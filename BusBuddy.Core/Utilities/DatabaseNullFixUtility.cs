using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using BusBuddy.Core.Data;

namespace BusBuddy.Core.Utilities
{
    /// <summary>
    /// Console utility to immediately fix NULL values in the database
    /// Run this to resolve SqlNullValueException errors
    /// </summary>
    public class DatabaseNullFixUtility
    {
        public static async Task Main(string[] args)
        {
            Console.WriteLine("BusBuddy Database NULL Fix Utility");
            Console.WriteLine("===================================");

            try
            {
                // Setup services
                var services = new ServiceCollection();
                services.AddLogging(builder => builder.AddConsole().SetMinimumLevel(LogLevel.Information));

                // Add DbContext - you may need to adjust connection string
                services.AddDbContext<BusBuddyDbContext>(options =>
                {
                    // Use the same connection string as your main application
                    options.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=BusBuddyDb;Trusted_Connection=true;MultipleActiveResultSets=true");
                });

                var serviceProvider = services.BuildServiceProvider();
                var logger = serviceProvider.GetRequiredService<ILogger<DatabaseNullFixUtility>>();

                using var context = serviceProvider.GetRequiredService<BusBuddyDbContext>();

                logger.LogInformation("Starting database NULL value fix...");

                // Check if database exists
                if (!await context.Database.CanConnectAsync())
                {
                    logger.LogError("Cannot connect to database. Please check connection string.");
                    return;
                }

                // Execute the fix using raw SQL for immediate results
                await FixDriverNullValues(context, logger);
                await FixVehicleNullValues(context, logger);
                await FixRouteNullValues(context, logger);
                await FixActivityNullValues(context, logger);

                logger.LogInformation("Database NULL value fix completed successfully!");
                Console.WriteLine("\nPress any key to exit...");
                Console.ReadKey();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                Console.WriteLine($"Stack Trace: {ex.StackTrace}");
                Console.WriteLine("\nPress any key to exit...");
                Console.ReadKey();
            }
        }

        private static async Task FixDriverNullValues(BusBuddyDbContext context, ILogger logger)
        {
            logger.LogInformation("Fixing Driver NULL values...");

            var rowsAffected = await context.Database.ExecuteSqlRawAsync(@"
                UPDATE Drivers 
                SET DriverName = COALESCE(NULLIF(LTRIM(RTRIM(DriverName)), ''), 'Driver-' + CAST(DriverId AS VARCHAR(10)))
                WHERE DriverName IS NULL OR LTRIM(RTRIM(DriverName)) = '';

                UPDATE Drivers 
                SET DriversLicenceType = COALESCE(NULLIF(LTRIM(RTRIM(DriversLicenceType)), ''), 'Standard')
                WHERE DriversLicenceType IS NULL OR LTRIM(RTRIM(DriversLicenceType)) = '';

                UPDATE Drivers 
                SET Status = COALESCE(NULLIF(LTRIM(RTRIM(Status)), ''), 'Active')
                WHERE Status IS NULL OR LTRIM(RTRIM(Status)) = '';
            ");

            logger.LogInformation($"Fixed Driver NULL values - rows affected: {rowsAffected}");
        }

        private static async Task FixVehicleNullValues(BusBuddyDbContext context, ILogger logger)
        {
            logger.LogInformation("Fixing Vehicle NULL values...");

            var rowsAffected = await context.Database.ExecuteSqlRawAsync(@"
                UPDATE Vehicles 
                SET BusNumber = COALESCE(NULLIF(LTRIM(RTRIM(BusNumber)), ''), 'BUS-' + CAST(VehicleId AS VARCHAR(10)))
                WHERE BusNumber IS NULL OR LTRIM(RTRIM(BusNumber)) = '';

                UPDATE Vehicles 
                SET Make = COALESCE(NULLIF(LTRIM(RTRIM(Make)), ''), 'Unknown')
                WHERE Make IS NULL OR LTRIM(RTRIM(Make)) = '';

                UPDATE Vehicles 
                SET Model = COALESCE(NULLIF(LTRIM(RTRIM(Model)), ''), 'Unknown')
                WHERE Model IS NULL OR LTRIM(RTRIM(Model)) = '';

                UPDATE Vehicles 
                SET Status = COALESCE(NULLIF(LTRIM(RTRIM(Status)), ''), 'Active')
                WHERE Status IS NULL OR LTRIM(RTRIM(Status)) = '';

                UPDATE Vehicles 
                SET VINNumber = COALESCE(NULLIF(LTRIM(RTRIM(VINNumber)), ''), 'VIN' + CAST(VehicleId AS VARCHAR(10)) + '0000000000')
                WHERE VINNumber IS NULL OR LTRIM(RTRIM(VINNumber)) = '';

                UPDATE Vehicles 
                SET LicenseNumber = COALESCE(NULLIF(LTRIM(RTRIM(LicenseNumber)), ''), 'LIC-' + CAST(VehicleId AS VARCHAR(10)))
                WHERE LicenseNumber IS NULL OR LTRIM(RTRIM(LicenseNumber)) = '';
            ");

            logger.LogInformation($"Fixed Vehicle NULL values - rows affected: {rowsAffected}");
        }

        private static async Task FixRouteNullValues(BusBuddyDbContext context, ILogger logger)
        {
            logger.LogInformation("Fixing Route NULL values...");

            var rowsAffected = await context.Database.ExecuteSqlRawAsync(@"
                UPDATE Routes 
                SET RouteName = COALESCE(NULLIF(LTRIM(RTRIM(RouteName)), ''), 'Route-' + CAST(RouteId AS VARCHAR(10)))
                WHERE RouteName IS NULL OR LTRIM(RTRIM(RouteName)) = '';
            ");

            logger.LogInformation($"Fixed Route NULL values - rows affected: {rowsAffected}");
        }

        private static async Task FixActivityNullValues(BusBuddyDbContext context, ILogger logger)
        {
            logger.LogInformation("Fixing Activity NULL values...");

            try
            {
                var rowsAffected = await context.Database.ExecuteSqlRawAsync(@"
                    UPDATE Activities 
                    SET ActivityType = COALESCE(NULLIF(LTRIM(RTRIM(ActivityType)), ''), 'General')
                    WHERE ActivityType IS NULL OR LTRIM(RTRIM(ActivityType)) = '';

                    UPDATE Activities 
                    SET Destination = COALESCE(NULLIF(LTRIM(RTRIM(Destination)), ''), 'Unspecified')
                    WHERE Destination IS NULL OR LTRIM(RTRIM(Destination)) = '';

                    UPDATE Activities 
                    SET RequestedBy = COALESCE(NULLIF(LTRIM(RTRIM(RequestedBy)), ''), 'System')
                    WHERE RequestedBy IS NULL OR LTRIM(RTRIM(RequestedBy)) = '';

                    UPDATE Activities 
                    SET Status = COALESCE(NULLIF(LTRIM(RTRIM(Status)), ''), 'Scheduled')
                    WHERE Status IS NULL OR LTRIM(RTRIM(Status)) = '';

                    UPDATE Activities 
                    SET Description = COALESCE(NULLIF(LTRIM(RTRIM(Description)), ''), 'Activity')
                    WHERE Description IS NULL OR LTRIM(RTRIM(Description)) = '';
                ");

                logger.LogInformation($"Fixed Activity NULL values - rows affected: {rowsAffected}");
            }
            catch (Exception ex)
            {
                logger.LogWarning($"Activities table may not exist or has different schema: {ex.Message}");
            }
        }
    }
}
