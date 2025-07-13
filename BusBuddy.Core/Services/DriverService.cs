using BusBuddy.Core.Data;
using BusBuddy.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Text;
using System.Text.RegularExpressions;

namespace BusBuddy.Core.Services
{
    /// <summary>
    /// Service implementation for managing school bus drivers
    /// Provides CRUD operations and business logic for driver management, including route assignments
    /// </summary>
    public class DriverService : IDriverService
    {
        private readonly IBusBuddyDbContextFactory _contextFactory;
        private readonly ILogger<DriverService> _logger;
        private static readonly SemaphoreSlim _semaphore = new(1, 1);

        public DriverService(IBusBuddyDbContextFactory contextFactory, ILogger<DriverService> logger)
        {
            _contextFactory = contextFactory;
            _logger = logger;
        }

        #region Basic CRUD Operations

        public async Task<List<Driver>> GetAllDriversAsync()
        {
            await _semaphore.WaitAsync();
            try
            {
                _logger.LogInformation("Retrieving all drivers from database");
                using var context = _contextFactory.CreateDbContext();

                // Fix any NULL values before attempting to read drivers
                await FixNullDriverValuesIfNeeded(context);

                return await context.Drivers
                    .AsNoTracking()
                    .ToListAsync();
            }
            catch (System.Data.SqlTypes.SqlNullValueException ex)
            {
                _logger.LogWarning(ex, "SQL NULL value error when retrieving drivers. Attempting to fix NULL values and retry.");

                // Try to fix NULL values and retry once
                try
                {
                    using var fixContext = _contextFactory.CreateWriteDbContext();
                    await FixNullDriverValuesIfNeeded(fixContext);

                    using var retryContext = _contextFactory.CreateDbContext();
                    return await retryContext.Drivers
                        .AsNoTracking()
                        .ToListAsync();
                }
                catch (Exception retryEx)
                {
                    _logger.LogError(retryEx, "Failed to fix NULL values and retry. Returning empty list to avoid application failure.");
                    return new List<Driver>();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving all drivers");
                throw;
            }
            finally
            {
                _semaphore.Release();
            }
        }

        public async Task<Driver?> GetDriverByIdAsync(int driverId)
        {
            try
            {
                _logger.LogInformation("Retrieving driver with ID: {DriverId}", driverId);
                using var context = _contextFactory.CreateDbContext();
                return await context.Drivers
                    .AsNoTracking()
                    .Include(d => d.AMRoutes)
                    .Include(d => d.PMRoutes)
                    .FirstOrDefaultAsync(d => d.DriverId == driverId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving driver with ID: {DriverId}", driverId);
                throw;
            }
        }

        public async Task<Driver> AddDriverAsync(Driver driver)
        {
            try
            {
                _logger.LogInformation("Adding new driver: {DriverName}", driver.DriverName);

                // Validate driver data
                var validationErrors = await ValidateDriverAsync(driver);
                if (validationErrors.Count > 0)
                {
                    throw new ArgumentException($"Driver validation failed: {string.Join(", ", validationErrors)}");
                }

                // Set default values
                if (driver.CreatedDate == default)
                {
                    driver.CreatedDate = DateTime.UtcNow;
                }

                if (string.IsNullOrWhiteSpace(driver.Status))
                {
                    driver.Status = "Active";
                }

                using var context = _contextFactory.CreateWriteDbContext();
                context.Drivers.Add(driver);
                await context.SaveChangesAsync();

                _logger.LogInformation("Successfully added driver with ID: {DriverId}", driver.DriverId);
                return driver;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding driver: {DriverName}", driver.DriverName);
                throw;
            }
        }

        public async Task<bool> UpdateDriverAsync(Driver driver)
        {
            try
            {
                _logger.LogInformation("Updating driver with ID: {DriverId}", driver.DriverId);

                // Validate driver data
                var validationErrors = await ValidateDriverAsync(driver);
                if (validationErrors.Count > 0)
                {
                    throw new ArgumentException($"Driver validation failed: {string.Join(", ", validationErrors)}");
                }

                // Update modification timestamp
                driver.UpdatedDate = DateTime.UtcNow;

                using var context = _contextFactory.CreateWriteDbContext();
                context.Entry(driver).State = EntityState.Modified;

                // Don't modify relationships here - use specific methods for that
                context.Entry(driver).Collection(d => d.AMRoutes).IsModified = false;
                context.Entry(driver).Collection(d => d.PMRoutes).IsModified = false;
                context.Entry(driver).Collection(d => d.Schedules).IsModified = false;
                context.Entry(driver).Collection(d => d.Activities).IsModified = false;
                context.Entry(driver).Collection(d => d.ScheduledActivities).IsModified = false;

                try
                {
                    var result = await context.SaveChangesAsync();
                    var success = result > 0;

                    if (success)
                    {
                        _logger.LogInformation("Successfully updated driver: {DriverName}", driver.DriverName);
                    }
                    else
                    {
                        _logger.LogWarning("No changes were made when updating driver: {DriverId}", driver.DriverId);
                    }

                    return success;
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    if (!await context.Drivers.AnyAsync(e => e.DriverId == driver.DriverId))
                    {
                        _logger.LogWarning("Driver with ID {DriverId} not found for update", driver.DriverId);
                        return false;
                    }
                    else
                    {
                        _logger.LogError(ex, "Concurrency error updating driver: {DriverId}", driver.DriverId);
                        throw;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating driver with ID: {DriverId}", driver.DriverId);
                throw;
            }
        }

        public async Task<bool> DeleteDriverAsync(int driverId)
        {
            try
            {
                _logger.LogInformation("Deleting driver with ID: {DriverId}", driverId);

                // First check if the driver is assigned to any routes
                using var checkContext = _contextFactory.CreateDbContext();
                var hasActiveRoutes = await checkContext.Routes
                    .AnyAsync(r => (r.AMDriverId == driverId || r.PMDriverId == driverId) && r.Date >= DateTime.Today);

                if (hasActiveRoutes)
                {
                    _logger.LogWarning("Cannot delete driver {DriverId} as they are assigned to active routes", driverId);
                    throw new InvalidOperationException("Cannot delete driver as they are assigned to active routes. Remove from routes first or mark as inactive.");
                }

                using var context = _contextFactory.CreateWriteDbContext();
                var driver = await context.Drivers.FindAsync(driverId);
                if (driver == null)
                {
                    _logger.LogWarning("Driver with ID {DriverId} not found for deletion", driverId);
                    return false;
                }

                context.Drivers.Remove(driver);
                await context.SaveChangesAsync();

                _logger.LogInformation("Successfully deleted driver: {DriverName}", driver.DriverName);
                return true;
            }
            catch (InvalidOperationException)
            {
                // Rethrow business rule exceptions
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting driver with ID: {DriverId}", driverId);
                throw;
            }
        }

        #endregion

        #region Query Operations

        public async Task<List<Driver>> GetActiveDriversAsync()
        {
            try
            {
                _logger.LogInformation("Retrieving active drivers");
                using var context = _contextFactory.CreateDbContext();

                // Fix any NULL values before attempting to read drivers
                await FixNullDriverValuesIfNeeded(context);

                return await context.Drivers
                    .AsNoTracking()
                    .Where(d => d.Status == "Active")
                    .ToListAsync();
            }
            catch (System.Data.SqlTypes.SqlNullValueException ex)
            {
                _logger.LogWarning(ex, "SQL NULL value error when retrieving active drivers. Attempting to fix and retry.");

                try
                {
                    using var fixContext = _contextFactory.CreateWriteDbContext();
                    await FixNullDriverValuesIfNeeded(fixContext);

                    using var retryContext = _contextFactory.CreateDbContext();
                    return await retryContext.Drivers
                        .AsNoTracking()
                        .Where(d => d.Status == "Active")
                        .ToListAsync();
                }
                catch (Exception retryEx)
                {
                    _logger.LogError(retryEx, "Failed to fix NULL values and retry. Returning empty list.");
                    return new List<Driver>();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving active drivers");
                throw;
            }
        }

        public async Task<List<Driver>> GetDriversByQualificationStatusAsync(string status)
        {
            try
            {
                _logger.LogInformation("Retrieving drivers by qualification status: {Status}", status);
                using var context = _contextFactory.CreateDbContext();

                // Since QualificationStatus is a computed property, we need to calculate it in memory
                var drivers = await context.Drivers
                    .AsNoTracking()
                    .ToListAsync();

                return drivers.Where(d => d.QualificationStatus == status).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving drivers by qualification status: {Status}", status);
                throw;
            }
        }

        public async Task<List<Driver>> GetDriversByLicenseStatusAsync(string status)
        {
            try
            {
                _logger.LogInformation("Retrieving drivers by license status: {Status}", status);
                using var context = _contextFactory.CreateDbContext();

                // Handle license status filter
                var drivers = await context.Drivers
                    .AsNoTracking()
                    .ToListAsync();

                return drivers.Where(d => d.LicenseStatus == status).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving drivers by license status: {Status}", status);
                throw;
            }
        }

        public async Task<List<Driver>> SearchDriversAsync(string searchTerm)
        {
            try
            {
                _logger.LogInformation("Searching drivers with term: {SearchTerm}", searchTerm);

                if (string.IsNullOrWhiteSpace(searchTerm))
                {
                    return await GetAllDriversAsync();
                }

                var term = searchTerm.ToLower();
                using var context = _contextFactory.CreateDbContext();
                return await context.Drivers
                    .AsNoTracking()
                    .Where(d =>
                        d.DriverName.ToLower().Contains(term) ||
                        (d.FirstName != null && d.FirstName.ToLower().Contains(term)) ||
                        (d.LastName != null && d.LastName.ToLower().Contains(term)) ||
                        (d.DriverPhone != null && d.DriverPhone.Contains(term)) ||
                        (d.DriverEmail != null && d.DriverEmail.ToLower().Contains(term)) ||
                        (d.LicenseNumber != null && d.LicenseNumber.Contains(term)))
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching drivers with term: {SearchTerm}", searchTerm);
                throw;
            }
        }

        #endregion

        #region Route Assignment

        public async Task<List<Driver>> GetAvailableDriversForRouteAsync(DateTime routeDate, bool isAMRoute)
        {
            try
            {
                _logger.LogInformation("Finding available drivers for route date: {RouteDate}, AM: {IsAMRoute}",
                    routeDate.ToShortDateString(), isAMRoute);

                using var context = _contextFactory.CreateDbContext();

                // Get active drivers
                var allActiveDrivers = await context.Drivers
                    .AsNoTracking()
                    .Where(d => d.Status == "Active" && d.TrainingComplete)
                    .ToListAsync();

                // Filter out drivers with expired licenses
                var qualifiedDrivers = allActiveDrivers
                    .Where(d => d.LicenseStatus != "Expired")
                    .ToList();

                // Get all drivers already assigned to routes on that date
                var busyDriverIds = await context.Routes
                    .Where(r => r.Date.Date == routeDate.Date)
                    .Select(r => isAMRoute ? r.AMDriverId : r.PMDriverId)
                    .Where(id => id != null)
                    .ToListAsync();

                // Return drivers not already assigned
                return qualifiedDrivers
                    .Where(d => !busyDriverIds.Contains(d.DriverId))
                    .ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error finding available drivers for route date: {RouteDate}",
                    routeDate.ToShortDateString());
                throw;
            }
        }

        public async Task<bool> AssignDriverToRouteAsync(int driverId, int routeId, bool isAMRoute)
        {
            try
            {
                _logger.LogInformation("Assigning driver {DriverId} to route {RouteId}, AM: {IsAMRoute}",
                    driverId, routeId, isAMRoute);

                using var context = _contextFactory.CreateWriteDbContext();

                // Check driver exists and is qualified
                var driver = await context.Drivers.FindAsync(driverId);
                if (driver == null)
                {
                    _logger.LogWarning("Driver with ID {DriverId} not found", driverId);
                    return false;
                }

                if (driver.Status != "Active" || !driver.TrainingComplete || driver.LicenseStatus == "Expired")
                {
                    _logger.LogWarning("Driver {DriverId} is not qualified for assignment", driverId);
                    throw new InvalidOperationException("Driver is not qualified for assignment: " +
                        (driver.Status != "Active" ? "inactive status" :
                         !driver.TrainingComplete ? "training incomplete" :
                         "expired license"));
                }

                // Check route exists
                var route = await context.Routes.FindAsync(routeId);
                if (route == null)
                {
                    _logger.LogWarning("Route with ID {RouteId} not found", routeId);
                    return false;
                }

                // Check driver is available
                if (!await IsDriverAvailableForRouteAsync(driverId, route.Date, isAMRoute))
                {
                    _logger.LogWarning("Driver {DriverId} is already assigned to another route on {Date}",
                        driverId, route.Date.ToShortDateString());
                    throw new InvalidOperationException("Driver is already assigned to another route at this time");
                }

                // Update route with driver assignment
                if (isAMRoute)
                {
                    route.AMDriverId = driverId;
                    route.DriverName = driver.DriverName; // For convenience in some queries
                }
                else
                {
                    route.PMDriverId = driverId;
                    route.DriverName = driver.DriverName; // For convenience in some queries
                }

                await context.SaveChangesAsync();
                _logger.LogInformation("Successfully assigned driver {DriverId} to route {RouteId}", driverId, routeId);
                return true;
            }
            catch (InvalidOperationException)
            {
                // Rethrow business rule exceptions
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error assigning driver {DriverId} to route {RouteId}", driverId, routeId);
                throw;
            }
        }

        public async Task<bool> RemoveDriverFromRouteAsync(int routeId, bool isAMRoute)
        {
            try
            {
                _logger.LogInformation("Removing driver from route {RouteId}, AM: {IsAMRoute}", routeId, isAMRoute);

                using var context = _contextFactory.CreateWriteDbContext();

                var route = await context.Routes.FindAsync(routeId);
                if (route == null)
                {
                    _logger.LogWarning("Route with ID {RouteId} not found", routeId);
                    return false;
                }

                if (isAMRoute)
                {
                    route.AMDriverId = null;
                }
                else
                {
                    route.PMDriverId = null;
                }

                await context.SaveChangesAsync();
                _logger.LogInformation("Successfully removed driver from route {RouteId}", routeId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error removing driver from route {RouteId}", routeId);
                throw;
            }
        }

        public async Task<List<Route>> GetDriverRoutesAsync(int driverId, DateTime? startDate = null, DateTime? endDate = null)
        {
            try
            {
                _logger.LogInformation("Getting routes for driver {DriverId} from {StartDate} to {EndDate}",
                    driverId, startDate?.ToShortDateString() ?? "all past", endDate?.ToShortDateString() ?? "all future");

                using var context = _contextFactory.CreateDbContext();

                var query = context.Routes
                    .AsNoTracking()
                    .Where(r => r.AMDriverId == driverId || r.PMDriverId == driverId);

                if (startDate.HasValue)
                {
                    query = query.Where(r => r.Date >= startDate.Value.Date);
                }

                if (endDate.HasValue)
                {
                    query = query.Where(r => r.Date <= endDate.Value.Date);
                }

                // Include vehicle information
                return await query
                    .Include(r => r.AMVehicle)
                    .Include(r => r.PMVehicle)
                    .OrderBy(r => r.Date)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting routes for driver {DriverId}", driverId);
                throw;
            }
        }

        public async Task<bool> IsDriverAvailableForRouteAsync(int driverId, DateTime routeDate, bool isAMRoute)
        {
            try
            {
                _logger.LogInformation("Checking if driver {DriverId} is available on {RouteDate}, AM: {IsAMRoute}",
                    driverId, routeDate.ToShortDateString(), isAMRoute);

                using var context = _contextFactory.CreateDbContext();

                // Check if the driver exists and is qualified
                var driver = await context.Drivers.FindAsync(driverId);
                if (driver == null || driver.Status != "Active" || !driver.TrainingComplete || driver.LicenseStatus == "Expired")
                {
                    return false;
                }

                // Check if the driver is already assigned to another route at the same time
                var isAssigned = await context.Routes
                    .AnyAsync(r => r.Date.Date == routeDate.Date &&
                                  (isAMRoute ? r.AMDriverId == driverId : r.PMDriverId == driverId));

                return !isAssigned;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking driver {DriverId} availability", driverId);
                throw;
            }
        }

        #endregion

        #region License and Qualification Management

        public async Task<bool> UpdateDriverLicenseInfoAsync(int driverId, string licenseNumber, string licenseClass,
            DateTime expiryDate, string? endorsements = null)
        {
            try
            {
                _logger.LogInformation("Updating license info for driver {DriverId}", driverId);

                using var context = _contextFactory.CreateWriteDbContext();

                var driver = await context.Drivers.FindAsync(driverId);
                if (driver == null)
                {
                    _logger.LogWarning("Driver with ID {DriverId} not found", driverId);
                    return false;
                }

                // Validate license information
                if (string.IsNullOrWhiteSpace(licenseNumber))
                {
                    throw new ArgumentException("License number cannot be empty");
                }

                if (string.IsNullOrWhiteSpace(licenseClass))
                {
                    throw new ArgumentException("License class cannot be empty");
                }

                if (expiryDate < DateTime.Today)
                {
                    throw new ArgumentException("License expiry date cannot be in the past");
                }

                // Update license information
                driver.LicenseNumber = licenseNumber;
                driver.LicenseClass = licenseClass;
                driver.LicenseExpiryDate = expiryDate;
                driver.Endorsements = endorsements;
                driver.UpdatedDate = DateTime.UtcNow;

                await context.SaveChangesAsync();
                _logger.LogInformation("Successfully updated license info for driver {DriverId}", driverId);
                return true;
            }
            catch (ArgumentException)
            {
                // Rethrow validation exceptions
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating license info for driver {DriverId}", driverId);
                throw;
            }
        }

        public async Task<bool> UpdateDriverQualificationAsync(int driverId, bool trainingComplete,
            DateTime? backgroundCheckDate = null, DateTime? drugTestDate = null, DateTime? physicalExamDate = null)
        {
            try
            {
                _logger.LogInformation("Updating qualification info for driver {DriverId}", driverId);

                using var context = _contextFactory.CreateWriteDbContext();

                var driver = await context.Drivers.FindAsync(driverId);
                if (driver == null)
                {
                    _logger.LogWarning("Driver with ID {DriverId} not found", driverId);
                    return false;
                }

                // Update qualification information
                driver.TrainingComplete = trainingComplete;

                if (backgroundCheckDate.HasValue)
                {
                    driver.BackgroundCheckDate = backgroundCheckDate;
                    // Standard 2-year expiry for background checks
                    driver.BackgroundCheckExpiry = backgroundCheckDate.Value.AddYears(2);
                }

                if (drugTestDate.HasValue)
                {
                    driver.DrugTestDate = drugTestDate;
                    // Standard 1-year expiry for drug tests
                    driver.DrugTestExpiry = drugTestDate.Value.AddYears(1);
                }

                if (physicalExamDate.HasValue)
                {
                    driver.PhysicalExamDate = physicalExamDate;
                    // Standard 2-year expiry for physical exams
                    driver.PhysicalExamExpiry = physicalExamDate.Value.AddYears(2);
                }

                driver.UpdatedDate = DateTime.UtcNow;

                await context.SaveChangesAsync();
                _logger.LogInformation("Successfully updated qualification info for driver {DriverId}", driverId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating qualification info for driver {DriverId}", driverId);
                throw;
            }
        }

        public async Task<bool> UpdateDriverStatusAsync(int driverId, string status)
        {
            try
            {
                _logger.LogInformation("Updating status for driver {DriverId} to {Status}", driverId, status);

                // Validate status
                var validStatuses = new[] { "Active", "Inactive", "On Leave", "Suspended", "Terminated" };
                if (!validStatuses.Contains(status))
                {
                    throw new ArgumentException($"Invalid status. Valid values are: {string.Join(", ", validStatuses)}");
                }

                using var context = _contextFactory.CreateWriteDbContext();

                var driver = await context.Drivers.FindAsync(driverId);
                if (driver == null)
                {
                    _logger.LogWarning("Driver with ID {DriverId} not found", driverId);
                    return false;
                }

                // If setting to inactive status, check for active route assignments
                if (status != "Active" && driver.Status == "Active")
                {
                    var hasActiveRoutes = await context.Routes
                        .AnyAsync(r => (r.AMDriverId == driverId || r.PMDriverId == driverId) && r.Date >= DateTime.Today);

                    if (hasActiveRoutes)
                    {
                        _logger.LogWarning("Cannot mark driver {DriverId} as {Status} as they have active route assignments", driverId, status);
                        throw new InvalidOperationException("Cannot change driver status as they have active route assignments. Please reassign routes first.");
                    }
                }

                driver.Status = status;
                driver.UpdatedDate = DateTime.UtcNow;

                await context.SaveChangesAsync();
                _logger.LogInformation("Successfully updated status for driver {DriverId} to {Status}", driverId, status);
                return true;
            }
            catch (ArgumentException)
            {
                // Rethrow validation exceptions
                throw;
            }
            catch (InvalidOperationException)
            {
                // Rethrow business rule exceptions
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating status for driver {DriverId}", driverId);
                throw;
            }
        }

        #endregion

        #region Driver Validation

        public async Task<List<string>> ValidateDriverAsync(Driver driver)
        {
            var errors = new List<string>();

            try
            {
                // Required field validation
                if (string.IsNullOrWhiteSpace(driver.DriverName))
                {
                    errors.Add("Driver name is required");
                }

                if (string.IsNullOrWhiteSpace(driver.DriversLicenceType))
                {
                    errors.Add("Driver's license type is required");
                }

                // Phone number validation
                if (!string.IsNullOrWhiteSpace(driver.DriverPhone))
                {
                    var phonePattern = @"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$";
                    if (!Regex.IsMatch(driver.DriverPhone, phonePattern))
                    {
                        errors.Add("Invalid phone number format");
                    }
                }

                // Email validation
                if (!string.IsNullOrWhiteSpace(driver.DriverEmail))
                {
                    var emailPattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";
                    if (!Regex.IsMatch(driver.DriverEmail, emailPattern))
                    {
                        errors.Add("Invalid email format");
                    }
                }

                // State validation
                if (!string.IsNullOrWhiteSpace(driver.State))
                {
                    if (driver.State.Length != 2)
                    {
                        errors.Add("State must be a 2-letter abbreviation");
                    }
                }

                // ZIP code validation
                if (!string.IsNullOrWhiteSpace(driver.Zip))
                {
                    var zipPattern = @"^\d{5}(-\d{4})?$";
                    if (!Regex.IsMatch(driver.Zip, zipPattern))
                    {
                        errors.Add("Invalid ZIP code format");
                    }
                }

                // License expiry validation
                if (driver.LicenseExpiryDate.HasValue && driver.LicenseExpiryDate < DateTime.Today)
                {
                    errors.Add("License is expired");
                }

                // Status validation
                var validStatuses = new[] { "Active", "Inactive", "On Leave", "Suspended", "Terminated" };
                if (!string.IsNullOrWhiteSpace(driver.Status) && !validStatuses.Contains(driver.Status))
                {
                    errors.Add($"Invalid status. Valid values are: {string.Join(", ", validStatuses)}");
                }

                // Check for uniqueness of license number
                if (!string.IsNullOrWhiteSpace(driver.LicenseNumber))
                {
                    using var context = _contextFactory.CreateDbContext();
                    var existingDriver = await context.Drivers
                        .Where(d => d.LicenseNumber == driver.LicenseNumber && d.DriverId != driver.DriverId)
                        .FirstOrDefaultAsync();

                    if (existingDriver != null)
                    {
                        errors.Add($"License number '{driver.LicenseNumber}' is already assigned to another driver");
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during driver validation");
                errors.Add("Validation error occurred");
            }

            return errors;
        }

        #endregion

        #region Analytics and Reporting

        public async Task<Dictionary<string, int>> GetDriverStatisticsAsync()
        {
            try
            {
                _logger.LogInformation("Calculating driver statistics");

                using var context = _contextFactory.CreateDbContext();
                var stats = new Dictionary<string, int>
                {
                    ["TotalDrivers"] = await context.Drivers.CountAsync(),
                    ["ActiveDrivers"] = await context.Drivers.CountAsync(d => d.Status == "Active"),
                    ["InactiveDrivers"] = await context.Drivers.CountAsync(d => d.Status != "Active"),
                    ["QualifiedDrivers"] = await context.Drivers.CountAsync(d => d.Status == "Active" && d.TrainingComplete),
                    ["TrainingIncompleteDrivers"] = await context.Drivers.CountAsync(d => !d.TrainingComplete)
                };

                // We need to handle license status in memory since it's a computed property
                var driversWithLicenseInfo = await context.Drivers
                    .AsNoTracking()
                    .Where(d => d.LicenseExpiryDate.HasValue)
                    .ToListAsync();

                stats["ExpiredLicenses"] = driversWithLicenseInfo.Count(d => d.LicenseStatus == "Expired");
                stats["ExpiringLicenses"] = driversWithLicenseInfo.Count(d => d.LicenseStatus == "Expiring Soon");
                stats["CurrentLicenses"] = driversWithLicenseInfo.Count(d => d.LicenseStatus == "Current");

                // Routes and driver assignment stats
                stats["DriversWithRouteAssignments"] = await context.Routes
                    .Where(r => r.Date >= DateTime.Today)
                    .Select(r => r.AMDriverId)
                    .Union(context.Routes.Where(r => r.Date >= DateTime.Today).Select(r => r.PMDriverId))
                    .Where(id => id.HasValue)
                    .Select(id => id!.Value)
                    .Distinct()
                    .CountAsync();

                return stats;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calculating driver statistics");
                throw;
            }
        }

        public async Task<List<Driver>> GetDriversNeedingRenewalAsync()
        {
            try
            {
                _logger.LogInformation("Finding drivers needing renewal");

                using var context = _contextFactory.CreateDbContext();

                // Get all active drivers
                var activeDrivers = await context.Drivers
                    .AsNoTracking()
                    .Where(d => d.Status == "Active")
                    .ToListAsync();

                // Filter in memory based on computed properties
                var driversNeedingRenewal = activeDrivers
                    .Where(d => d.LicenseStatus == "Expired" || d.LicenseStatus == "Expiring Soon")
                    .ToList();

                // Also add drivers with expiring qualifications
                var thirtyDaysFromNow = DateTime.Today.AddDays(30);
                var driversWithExpiringQualifications = await context.Drivers
                    .AsNoTracking()
                    .Where(d => d.Status == "Active" &&
                               (d.BackgroundCheckExpiry.HasValue && d.BackgroundCheckExpiry <= thirtyDaysFromNow ||
                                d.DrugTestExpiry.HasValue && d.DrugTestExpiry <= thirtyDaysFromNow ||
                                d.PhysicalExamExpiry.HasValue && d.PhysicalExamExpiry <= thirtyDaysFromNow))
                    .ToListAsync();

                // Combine the lists avoiding duplicates
                var result = driversNeedingRenewal.Union(driversWithExpiringQualifications, new DriverIdComparer()).ToList();

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error finding drivers needing renewal");
                throw;
            }
        }

        public async Task<Dictionary<string, double>> GetDriverAssignmentMetricsAsync(DateTime startDate, DateTime endDate)
        {
            try
            {
                _logger.LogInformation("Calculating driver assignment metrics from {StartDate} to {EndDate}",
                    startDate.ToShortDateString(), endDate.ToShortDateString());

                using var context = _contextFactory.CreateDbContext();

                // Get all routes in the date range
                var routes = await context.Routes
                    .Where(r => r.Date >= startDate && r.Date <= endDate)
                    .ToListAsync();

                var metrics = new Dictionary<string, double>();

                // Calculate total routes
                var totalRoutes = routes.Count * 2; // AM and PM
                metrics["TotalRoutePeriods"] = totalRoutes;

                // Calculate assigned routes
                var amAssignments = routes.Count(r => r.AMDriverId.HasValue);
                var pmAssignments = routes.Count(r => r.PMDriverId.HasValue);
                var totalAssignments = amAssignments + pmAssignments;

                metrics["AssignedRoutePeriods"] = totalAssignments;
                metrics["UnassignedRoutePeriods"] = totalRoutes - totalAssignments;

                // Calculate assignment percentage
                metrics["AssignmentPercentage"] = totalRoutes > 0
                    ? Math.Round((double)totalAssignments / totalRoutes * 100, 2)
                    : 0;

                // Calculate metrics per driver
                var allDriverIds = routes
                    .Where(r => r.AMDriverId.HasValue)
                    .Select(r => r.AMDriverId!.Value)
                    .Union(routes
                        .Where(r => r.PMDriverId.HasValue)
                        .Select(r => r.PMDriverId!.Value))
                    .Distinct()
                    .ToList();

                metrics["DriversWithAssignments"] = allDriverIds.Count;

                // Calculate average routes per driver
                metrics["AverageAssignmentsPerDriver"] = allDriverIds.Count > 0
                    ? Math.Round((double)totalAssignments / allDriverIds.Count, 2)
                    : 0;

                // Calculate driver utilization
                var totalActiveDrivers = await context.Drivers.CountAsync(d => d.Status == "Active");
                metrics["DriverUtilizationPercentage"] = totalActiveDrivers > 0
                    ? Math.Round((double)allDriverIds.Count / totalActiveDrivers * 100, 2)
                    : 0;

                return metrics;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calculating driver assignment metrics");
                throw;
            }
        }

        public async Task<string> ExportDriversToCsvAsync()
        {
            try
            {
                _logger.LogInformation("Exporting drivers to CSV format");

                var drivers = await GetAllDriversAsync();
                var csv = new StringBuilder();

                // CSV Header
                csv.AppendLine("Driver ID,Driver Name,Phone,Email,Address,City,State,ZIP," +
                              "License Type,License Number,License Class,License Expiry," +
                              "Training Complete,Status,Hire Date,Notes");

                // CSV Data
                foreach (var driver in drivers)
                {
                    csv.AppendLine($"{driver.DriverId}," +
                                  $"\"{driver.DriverName}\"," +
                                  $"\"{driver.DriverPhone ?? ""}\"," +
                                  $"\"{driver.DriverEmail ?? ""}\"," +
                                  $"\"{driver.Address ?? ""}\"," +
                                  $"\"{driver.City ?? ""}\"," +
                                  $"\"{driver.State ?? ""}\"," +
                                  $"\"{driver.Zip ?? ""}\"," +
                                  $"\"{driver.DriversLicenceType}\"," +
                                  $"\"{driver.LicenseNumber ?? ""}\"," +
                                  $"\"{driver.LicenseClass ?? ""}\"," +
                                  $"{driver.LicenseExpiryDate?.ToString("yyyy-MM-dd") ?? ""}," +
                                  $"{driver.TrainingComplete}," +
                                  $"\"{driver.Status}\"," +
                                  $"{driver.HireDate?.ToString("yyyy-MM-dd") ?? ""}," +
                                  $"\"{driver.Notes?.Replace("\"", "\"\"") ?? ""}\"");
                }

                _logger.LogInformation("Successfully exported {Count} drivers to CSV", drivers.Count);
                return csv.ToString();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error exporting drivers to CSV");
                throw;
            }
        }

        #endregion

        #region NULL Value Handling

        /// <summary>
        /// Fix NULL values in the Drivers table to prevent SqlNullValueException
        /// </summary>
        private async Task FixNullDriverValuesIfNeeded(BusBuddyDbContext context)
        {
            try
            {
                // Check if there are any NULL values in required columns
                var hasNullValues = await context.Database.ExecuteSqlRawAsync(@"
                    SELECT CASE WHEN EXISTS (
                        SELECT 1 FROM Drivers 
                        WHERE DriverName IS NULL OR DriversLicenceType IS NULL OR Status IS NULL
                    ) THEN 1 ELSE 0 END
                ");

                if (hasNullValues > 0)
                {
                    _logger.LogWarning("Found NULL values in required Driver columns. Fixing automatically.");

                    // Fix NULL values
                    await context.Database.ExecuteSqlRawAsync(@"
                        UPDATE Drivers 
                        SET DriverName = COALESCE(NULLIF(LTRIM(RTRIM(DriverName)), ''), 'Driver-' + CAST(DriverId AS VARCHAR(10)))
                        WHERE DriverName IS NULL OR LTRIM(RTRIM(DriverName)) = '';

                        UPDATE Drivers 
                        SET DriversLicenceType = COALESCE(NULLIF(LTRIM(RTRIM(DriversLicenceType)), ''), 'Standard')
                        WHERE DriversLicenceType IS NULL OR LTRIM(RTRIM(DriversLicenceType)) = '';

                        UPDATE Drivers 
                        SET Status = COALESCE(NULLIF(LTRIM(RTRIM(Status)), ''), 'Active')
                        WHERE Status IS NULL OR LTRIM(RTRIM(Status)) = '';

                        UPDATE Drivers 
                        SET FirstName = NULL
                        WHERE FirstName = '';

                        UPDATE Drivers 
                        SET LastName = NULL
                        WHERE LastName = '';

                        UPDATE Drivers 
                        SET DriverPhone = NULL
                        WHERE DriverPhone = '';

                        UPDATE Drivers 
                        SET DriverEmail = NULL
                        WHERE DriverEmail = '';

                        UPDATE Drivers 
                        SET Address = NULL
                        WHERE Address = '';

                        UPDATE Drivers 
                        SET City = NULL
                        WHERE City = '';

                        UPDATE Drivers 
                        SET State = NULL
                        WHERE State = '';

                        UPDATE Drivers 
                        SET Zip = NULL
                        WHERE Zip = '';
                    ");

                    _logger.LogInformation("Successfully fixed NULL values in Drivers table.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while attempting to fix NULL values in Drivers table");
                // Don't throw - let the calling method handle the original exception
            }
        }

        #endregion

        #region DEBUG Instrumentation

#if DEBUG
        public async Task<Dictionary<string, object>> GetDriverDiagnosticsAsync(int driverId)
        {
            try
            {
                _logger.LogDebug("Retrieving diagnostic information for driver {DriverId}", driverId);

                using var context = _contextFactory.CreateDbContext();
                var driver = await context.Drivers
                    .AsNoTracking()
                    .Include(d => d.AMRoutes)
                    .Include(d => d.PMRoutes)
                    .FirstOrDefaultAsync(d => d.DriverId == driverId);

                if (driver == null)
                {
                    _logger.LogWarning("Driver with ID {DriverId} not found for diagnostics", driverId);
                    return new Dictionary<string, object> { { "Error", "Driver not found" } };
                }

                // Create a comprehensive diagnostic report
                var diagnostics = new Dictionary<string, object>
                {
                    { "DriverId", driver.DriverId },
                    { "DriverName", driver.DriverName },
                    { "FullName", driver.FullName },
                    { "RecordCreationTime", driver.CreatedDate },
                    { "LastUpdateTime", driver.UpdatedDate ?? DateTime.MinValue },
                    { "RecordAgeInDays", (DateTime.UtcNow - driver.CreatedDate).TotalDays },
                    { "QualificationStatus", driver.QualificationStatus },
                    { "LicenseStatus", driver.LicenseStatus },
                    { "IsAvailable", driver.IsAvailable },
                    { "Status", driver.Status },
                    { "TrainingComplete", driver.TrainingComplete },
                    { "AMRouteCount", driver.AMRoutes.Count },
                    { "PMRouteCount", driver.PMRoutes.Count },
                    { "LicenseExpiryDate", driver.LicenseExpiryDate as object ?? "Not Set" },
                    { "DaysUntilLicenseExpiry", driver.LicenseExpiryDate.HasValue
                        ? (object)(driver.LicenseExpiryDate.Value - DateTime.Today).TotalDays
                        : "Not Applicable" },
                    { "BackgroundCheckExpiryDate", driver.BackgroundCheckExpiry as object ?? "Not Set" },
                    { "DaysUntilBackgroundCheckExpiry", driver.BackgroundCheckExpiry.HasValue
                        ? (object)(driver.BackgroundCheckExpiry.Value - DateTime.Today).TotalDays
                        : "Not Applicable" },
                    { "DrugTestExpiryDate", driver.DrugTestExpiry as object ?? "Not Set" },
                    { "DaysUntilDrugTestExpiry", driver.DrugTestExpiry.HasValue
                        ? (object)(driver.DrugTestExpiry.Value - DateTime.Today).TotalDays
                        : "Not Applicable" },
                    { "PhysicalExamExpiryDate", driver.PhysicalExamExpiry as object ?? "Not Set" },
                    { "DaysUntilPhysicalExamExpiry", driver.PhysicalExamExpiry.HasValue
                        ? (object)(driver.PhysicalExamExpiry.Value - DateTime.Today).TotalDays
                        : "Not Applicable" },
                    { "NeedsAttention", driver.NeedsAttention },
                    { "ModelState", SerializeDriverForDiagnostics(driver) }
                };

                // Add upcoming route information
                var upcomingRoutes = await context.Routes
                    .AsNoTracking()
                    .Where(r => r.Date >= DateTime.Today && (r.AMDriverId == driverId || r.PMDriverId == driverId))
                    .OrderBy(r => r.Date)
                    .Take(5)
                    .ToListAsync();

                var routeInfoList = upcomingRoutes.Select(r => new
                {
                    r.RouteId,
                    r.Date,
                    r.RouteName,
                    IsAMRoute = r.AMDriverId == driverId,
                    IsPMRoute = r.PMDriverId == driverId,
                    BusNumber = GetBusNumber(r, driverId)
                }).ToList();

                diagnostics.Add("UpcomingRoutes", routeInfoList);

                return diagnostics;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating diagnostics for driver {DriverId}", driverId);
                return new Dictionary<string, object> { { "Error", ex.Message } };
            }
        }

        public async Task<Dictionary<string, object>> GetDriverOperationMetricsAsync()
        {
            try
            {
                _logger.LogDebug("Retrieving driver operation metrics");

                var metrics = new Dictionary<string, object>();
                using var context = _contextFactory.CreateDbContext();

                // Driver Record Metrics
                metrics["TotalDriverCount"] = await context.Drivers.CountAsync();
                metrics["ActiveDriverCount"] = await context.Drivers.CountAsync(d => d.Status == "Active");
                metrics["InactiveDriverCount"] = await context.Drivers.CountAsync(d => d.Status != "Active");
                metrics["QualifiedDriverCount"] = await context.Drivers.CountAsync(d => d.Status == "Active" && d.TrainingComplete);
                metrics["UnqualifiedDriverCount"] = await context.Drivers.CountAsync(d => !d.TrainingComplete);

                // License Status Metrics (processed in memory due to computed properties)
                var drivers = await context.Drivers.AsNoTracking().ToListAsync();
                metrics["ExpiredLicenseCount"] = drivers.Count(d => d.LicenseStatus == "Expired");
                metrics["ExpiringLicenseCount"] = drivers.Count(d => d.LicenseStatus == "Expiring Soon");
                metrics["CurrentLicenseCount"] = drivers.Count(d => d.LicenseStatus == "Current");

                // Route Assignment Metrics
                var today = DateTime.Today;
                metrics["DriversWithActiveAssignments"] = await context.Routes
                    .Where(r => r.Date >= today)
                    .Select(r => r.AMDriverId)
                    .Union(context.Routes.Where(r => r.Date >= today).Select(r => r.PMDriverId))
                    .Where(id => id.HasValue)
                    .Select(id => id!.Value)
                    .Distinct()
                    .CountAsync();

                // Database Performance Metrics
                var sw = new System.Diagnostics.Stopwatch();

                sw.Start();
                await context.Drivers.AsNoTracking().ToListAsync();
                sw.Stop();
                metrics["AllDriversQueryTimeMs"] = sw.ElapsedMilliseconds;

                sw.Restart();
                await context.Drivers.AsNoTracking().Where(d => d.Status == "Active").ToListAsync();
                sw.Stop();
                metrics["ActiveDriversQueryTimeMs"] = sw.ElapsedMilliseconds;

                sw.Restart();
                var activeDriverIds = await context.Drivers
                    .Where(d => d.Status == "Active")
                    .Select(d => d.DriverId)
                    .ToListAsync();

                await context.Routes
                    .Where(r => activeDriverIds.Contains(r.AMDriverId ?? -1) || activeDriverIds.Contains(r.PMDriverId ?? -1))
                    .ToListAsync();
                sw.Stop();
                metrics["DriverRoutesQueryTimeMs"] = sw.ElapsedMilliseconds;

                // Driver availability metrics
                metrics["AvailableAMDriversToday"] = (await GetAvailableDriversForRouteAsync(DateTime.Today, true)).Count;
                metrics["AvailablePMDriversToday"] = (await GetAvailableDriversForRouteAsync(DateTime.Today, false)).Count;

                return metrics;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating driver operation metrics");
                return new Dictionary<string, object> { { "Error", ex.Message } };
            }
        }

        /// <summary>
        /// Serializes a driver object for diagnostic viewing
        /// Only available in DEBUG builds
        /// </summary>
        private object SerializeDriverForDiagnostics(Driver driver)
        {
            return new
            {
                // Basic Info
                driver.DriverId,
                driver.DriverName,
                driver.FirstName,
                driver.LastName,
                driver.FullName,

                // Contact Info
                ContactInfo = new
                {
                    driver.DriverPhone,
                    driver.DriverEmail,
                    driver.Address,
                    driver.City,
                    driver.State,
                    driver.Zip,
                    driver.FullAddress,
                    driver.EmergencyContactName,
                    driver.EmergencyContactPhone
                },

                // License Info
                LicenseInfo = new
                {
                    driver.DriversLicenceType,
                    driver.LicenseNumber,
                    driver.LicenseClass,
                    driver.LicenseIssueDate,
                    driver.LicenseExpiryDate,
                    driver.LicenseStatus,
                    driver.Endorsements
                },

                // Qualification Info
                QualificationInfo = new
                {
                    driver.TrainingComplete,
                    driver.QualificationStatus,
                    driver.BackgroundCheckDate,
                    driver.BackgroundCheckExpiry,
                    driver.DrugTestDate,
                    driver.DrugTestExpiry,
                    driver.PhysicalExamDate,
                    driver.PhysicalExamExpiry
                },

                // Status Info
                StatusInfo = new
                {
                    driver.Status,
                    driver.IsAvailable,
                    driver.NeedsAttention,
                    driver.HireDate,
                    driver.CreatedDate,
                    driver.UpdatedDate,
                    driver.CreatedBy,
                    driver.UpdatedBy,
                    driver.Notes,
                    driver.MedicalRestrictions
                }
            };
        }
#endif

        #endregion

        #region Helper Classes

        private class DriverIdComparer : IEqualityComparer<Driver>
        {
            public bool Equals(Driver? x, Driver? y)
            {
                if (x == null || y == null)
                    return false;

                return x.DriverId == y.DriverId;
            }

            public int GetHashCode(Driver obj)
            {
                return obj.DriverId.GetHashCode();
            }
        }

        private string GetBusNumber(Route route, int driverId)
        {
            if (route.AMDriverId == driverId)
            {
                return route.AMVehicle != null ? route.AMVehicle.BusNumber ?? "Unknown" : "Unknown";
            }
            else
            {
                return route.PMVehicle != null ? route.PMVehicle.BusNumber ?? "Unknown" : "Unknown";
            }
        }

        #endregion
    }
}
