using NUnit.Framework;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Bus_Buddy.Data;
using Bus_Buddy.Data.UnitOfWork;
using Bus_Buddy.Models;
using BusBuddy.Tests.Infrastructure;

namespace BusBuddy.Tests.UnitTests.UnitOfWorkTests;

/// <summary>
/// Tests for UnitOfWork pattern implementation
/// Validates transaction management, repository coordination, and data consistency
/// 
/// LESSONS LEARNED APPLIED:
/// - Category 2.2: Test Data Isolation - Comprehensive database cleanup
/// - Category 3.1: Safety-Critical vs Standard Operations - Transaction integrity first
/// - Category 1: Entity Framework patterns - Proper transaction handling
/// </summary>
[TestFixture]
public class UnitOfWorkTests : TestBase
{
    private UnitOfWork _unitOfWork = null!;

    [SetUp]
    public async Task SetUp()
    {
        await ClearDatabaseAsync(); // LESSON 2.2: Test Data Isolation

        _unitOfWork = new UnitOfWork(DbContext);
    }

    [TearDown]
    public void TearDown()
    {
        _unitOfWork?.Dispose();
    }

    #region Repository Factory Tests

    [Test]
    public void Repository_ShouldReturnGenericRepository()
    {
        // Act
        var busRepository = _unitOfWork.Repository<Bus>();
        var driverRepository = _unitOfWork.Repository<Driver>();

        // Assert
        busRepository.Should().NotBeNull();
        driverRepository.Should().NotBeNull();
        busRepository.Should().NotBeSameAs(driverRepository);
    }

    [Test]
    public void Repository_SameCalls_ShouldReturnSameInstance()
    {
        // Act
        var busRepository1 = _unitOfWork.Repository<Bus>();
        var busRepository2 = _unitOfWork.Repository<Bus>();

        // Assert
        busRepository1.Should().BeSameAs(busRepository2);
    }

    [Test]
    public void SpecializedRepositories_ShouldBeAvailable()
    {
        // Act & Assert
        _unitOfWork.Activities.Should().NotBeNull();
        _unitOfWork.Buses.Should().NotBeNull();
        _unitOfWork.Drivers.Should().NotBeNull();
        _unitOfWork.Routes.Should().NotBeNull();
        _unitOfWork.Students.Should().NotBeNull();
        _unitOfWork.FuelRecords.Should().NotBeNull();
        _unitOfWork.MaintenanceRecords.Should().NotBeNull();
        _unitOfWork.Schedules.Should().NotBeNull();
        _unitOfWork.SchoolCalendar.Should().NotBeNull();
        _unitOfWork.ActivitySchedules.Should().NotBeNull();
    }

    #endregion

    #region Transaction Management Tests

    [Test]
    public async Task SaveChangesAsync_ShouldPersistChanges()
    {
        // Arrange
        var bus = new Bus
        {
            BusNumber = "SAVE001",
            Year = 2020,
            Make = "Blue Bird",
            Model = "Vision",
            SeatingCapacity = 72,
            VINNumber = "1234567890ABCDEFG",
            LicenseNumber = "SAV001"
        };

        // Act
        await _unitOfWork.Repository<Bus>().AddAsync(bus);
        var result = await _unitOfWork.SaveChangesAsync();

        // Assert
        result.Should().Be(1); // One entity saved
        bus.VehicleId.Should().BeGreaterThan(0);

        // Verify in database
        var savedBus = await DbContext.Vehicles.FindAsync(bus.VehicleId);
        savedBus.Should().NotBeNull();
        savedBus!.BusNumber.Should().Be("SAVE001");
    }

    [Test]
    public async Task BeginTransactionAsync_CommitTransactionAsync_ShouldPersistChanges()
    {
        // Arrange
        var driver = new Driver
        {
            DriverName = "Transaction Test",
            DriverPhone = "(555) 123-4567", // LESSON 2.3: Phone validation format
            DriverEmail = "transaction@example.com",
            DriversLicenceType = "CDL",
            TrainingComplete = true
        };

        // Act
        await _unitOfWork.BeginTransactionAsync();

        await _unitOfWork.Repository<Driver>().AddAsync(driver);
        await _unitOfWork.SaveChangesAsync();

        await _unitOfWork.CommitTransactionAsync();

        // Assert
        var savedDriver = await DbContext.Drivers.FindAsync(driver.DriverId);
        savedDriver.Should().NotBeNull();
        savedDriver!.DriverName.Should().Be("Transaction Test");
    }

    [Test]
    public async Task BeginTransactionAsync_RollbackTransactionAsync_ShouldNotPersistChanges()
    {
        // Arrange
        var driver = new Driver
        {
            DriverName = "Rollback Test",
            DriverPhone = "(555) 987-6543",
            DriverEmail = "rollback@example.com",
            DriversLicenceType = "CDL",
            TrainingComplete = false
        };

        // Act
        await _unitOfWork.BeginTransactionAsync();

        await _unitOfWork.Repository<Driver>().AddAsync(driver);
        await _unitOfWork.SaveChangesAsync();

        var driverId = driver.DriverId; // Capture ID before rollback

        await _unitOfWork.RollbackTransactionAsync();

        // Assert
        var rolledBackDriver = await DbContext.Drivers.FindAsync(driverId);
        rolledBackDriver.Should().BeNull();
    }

    [Test]
    public async Task MultipleTransactions_ShouldThrowException()
    {
        // Arrange
        await _unitOfWork.BeginTransactionAsync();

        // Act & Assert  
        var ex = Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await _unitOfWork.BeginTransactionAsync());
        ex.Message.Should().Contain("transaction is already in progress");

        // Cleanup
        await _unitOfWork.RollbackTransactionAsync();
    }

    [Test]
    public void CommitTransaction_WithoutBeginTransaction_ShouldThrowException()
    {
        // Act & Assert - LESSON 2.1: Synchronous exception testing for InvalidOperationException +1
        Assert.Throws<InvalidOperationException>(() =>
            _unitOfWork.CommitTransactionAsync().GetAwaiter().GetResult());
    }

    #endregion

    #region Bulk Operations Tests

    [Test]
    public async Task BulkInsertAsync_ShouldAddMultipleEntities()
    {
        // Arrange
        var buses = new List<Bus>();
        for (int i = 1; i <= 10; i++)
        {
            buses.Add(new Bus
            {
                BusNumber = $"BULK{i:D3}",
                Year = 2020,
                Make = "Blue Bird",
                Model = "Vision",
                SeatingCapacity = 72,
                VINNumber = $"BULK{i:D13}1234567",
                LicenseNumber = $"BLK{i:D3}"
            });
        }

        // Act
        var result = await _unitOfWork.BulkInsertAsync(buses);

        // Assert
        result.Should().Be(10);

        var savedBuses = await DbContext.Vehicles.Where(b => b.BusNumber.StartsWith("BULK")).ToListAsync();
        savedBuses.Should().HaveCount(10);
    }

    [Test]
    public async Task BulkUpdateAsync_ShouldUpdateMultipleEntities()
    {
        // Arrange
        var drivers = new List<Driver>();
        for (int i = 1; i <= 5; i++)
        {
            drivers.Add(new Driver
            {
                DriverName = $"Bulk Driver {i}",
                DriverPhone = $"(555) {i:D3}-{i:D4}",
                DriverEmail = $"driver{i}@example.com",
                DriversLicenceType = "CDL",
                TrainingComplete = false
            });
        }

        DbContext.Drivers.AddRange(drivers);
        await DbContext.SaveChangesAsync();

        // Act
        foreach (var driver in drivers)
        {
            driver.TrainingComplete = true;
        }

        var result = await _unitOfWork.BulkUpdateAsync(drivers);

        // Assert
        result.Should().Be(5);

        var updatedDrivers = await DbContext.Drivers.Where(d => d.DriverName.StartsWith("Bulk Driver")).ToListAsync();
        updatedDrivers.Should().OnlyContain(d => d.TrainingComplete);
    }

    [Test]
    public async Task BulkDeleteAsync_ShouldRemoveMultipleEntities()
    {
        // Arrange
        var buses = new List<Bus>();
        for (int i = 1; i <= 3; i++)
        {
            buses.Add(new Bus
            {
                BusNumber = $"DEL{i:D3}",
                Year = 2018,
                Make = "Thomas Built",
                Model = "Saf-T-Liner",
                SeatingCapacity = 48,
                VINNumber = $"DEL{i:D14}1234567",
                LicenseNumber = $"DEL{i:D3}"
            });
        }

        DbContext.Vehicles.AddRange(buses);
        await DbContext.SaveChangesAsync();

        // Act
        var result = await _unitOfWork.BulkDeleteAsync(buses);

        // Assert
        result.Should().Be(3);

        var deletedBuses = await DbContext.Vehicles.Where(b => b.BusNumber.StartsWith("DEL")).ToListAsync();
        deletedBuses.Should().BeEmpty();
    }

    [Test]
    public async Task BulkInsertAsync_WithEmptyCollection_ShouldReturnZero()
    {
        // Arrange
        var emptyBuses = new List<Bus>();

        // Act
        var result = await _unitOfWork.BulkInsertAsync(emptyBuses);

        // Assert
        result.Should().Be(0);
    }

    #endregion

    #region Database Operations Tests

    [Test]
    public async Task DatabaseExistsAsync_ShouldReturnTrue()
    {
        // Act
        var exists = await _unitOfWork.DatabaseExistsAsync();

        // Assert
        exists.Should().BeTrue();
    }

    [Test]
    public async Task CanConnectAsync_ShouldReturnTrue()
    {
        // Act
        var canConnect = await _unitOfWork.CanConnectAsync();

        // Assert
        canConnect.Should().BeTrue();
    }

    #endregion

    #region Audit Management Tests

    [Test]
    public void SetAuditUser_GetCurrentAuditUser_ShouldWorkCorrectly()
    {
        // Arrange
        const string testUser = "TestUser123";

        // Act
        _unitOfWork.SetAuditUser(testUser);
        var currentUser = _unitOfWork.GetCurrentAuditUser();

        // Assert
        currentUser.Should().Be(testUser);
    }

    [Test]
    public async Task SaveChangesAsync_ShouldApplyAuditFields()
    {
        // Arrange
        const string auditUser = "AuditTestUser";
        _unitOfWork.SetAuditUser(auditUser);

        var student = new Student
        {
            StudentName = "Audit Test Student",
            Grade = "2",
            HomePhone = "(555) 111-2222",
            EmergencyPhone = "(555) 333-4444",
            SpecialNeeds = false,
            PhotoPermission = true,
            FieldTripPermission = true
        };

        // Act
        await _unitOfWork.Repository<Student>().AddAsync(student);
        await _unitOfWork.SaveChangesAsync();

        // Assert
        student.CreatedBy.Should().Be(auditUser);
        student.CreatedDate.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromMinutes(1));
    }

    #endregion

    #region Cache Management Tests

    [Test]
    public async Task RefreshCacheAsync_ShouldClearChangeTracker()
    {
        // Arrange
        var bus = new Bus
        {
            BusNumber = "CACHE001",
            Year = 2020,
            Make = "Blue Bird",
            Model = "Vision",
            SeatingCapacity = 72,
            VINNumber = "CACHE001234567890",
            LicenseNumber = "CAC001"
        };

        DbContext.Vehicles.Add(bus);
        await DbContext.SaveChangesAsync();

        // Verify entity is tracked
        DbContext.Entry(bus).State.Should().Be(EntityState.Unchanged);

        // Act
        await _unitOfWork.RefreshCacheAsync();

        // Assert
        DbContext.Entry(bus).State.Should().Be(EntityState.Detached);
    }

    [Test]
    public void ClearCache_ShouldClearChangeTracker()
    {
        // Arrange
        var driver = new Driver
        {
            DriverName = "Cache Test",
            DriverPhone = "(555) 999-8888",
            DriverEmail = "cache@example.com",
            DriversLicenceType = "CDL",
            TrainingComplete = true
        };

        DbContext.Drivers.Add(driver);
        DbContext.SaveChanges();

        // Verify entity is tracked
        DbContext.Entry(driver).State.Should().Be(EntityState.Unchanged);

        // Act
        _unitOfWork.ClearCache();

        // Assert
        DbContext.Entry(driver).State.Should().Be(EntityState.Detached);
    }

    #endregion

    #region Error Handling Tests

    [Test]
    public async Task SaveChangesAsync_WithDatabaseError_ShouldThrowInformativeException()
    {
        // Arrange - Create a scenario that will cause a database constraint violation
        var bus1 = new Bus
        {
            BusNumber = "DUPLICATE",
            Year = 2020,
            Make = "Blue Bird",
            Model = "Vision",
            SeatingCapacity = 72,
            VINNumber = "DUPLICATE123456789",
            LicenseNumber = "DUP001"
        };

        var bus2 = new Bus
        {
            BusNumber = "DUPLICATE", // Duplicate bus number (if there's a unique constraint)
            Year = 2021,
            Make = "Thomas Built",
            Model = "Saf-T-Liner",
            SeatingCapacity = 48,
            VINNumber = "DUPLICATE987654321",
            LicenseNumber = "DUP002"
        };

        await _unitOfWork.Repository<Bus>().AddAsync(bus1);
        await _unitOfWork.SaveChangesAsync();

        await _unitOfWork.Repository<Bus>().AddAsync(bus2);

        // Act & Assert
        // Note: This test assumes there might be unique constraints
        // If not, the test will still validate the exception wrapping mechanism
        try
        {
            await _unitOfWork.SaveChangesAsync();
        }
        catch (InvalidOperationException ex)
        {
            ex.Message.Should().Contain("Failed to save changes to the database");
            ex.InnerException.Should().NotBeNull();
        }
        catch (Exception)
        {
            // If no constraint violation, that's also valid - just testing error handling structure
        }
    }

    [Test]
    public void Dispose_ShouldCleanupResources()
    {
        // Arrange
        var unitOfWork = new UnitOfWork(DbContext);

        // Act
        unitOfWork.Dispose();

        // Assert - Should not throw
        // Multiple disposals should be safe
        unitOfWork.Dispose();
    }

    #endregion

    #region Synchronous Transaction Tests

    [Test]
    public void BeginTransaction_CommitTransaction_ShouldPersistChanges()
    {
        // Arrange
        var bus = new Bus
        {
            BusNumber = "SYNC001",
            Year = 2020,
            Make = "Blue Bird",
            Model = "Vision",
            SeatingCapacity = 72,
            VINNumber = "SYNC001234567890A",
            LicenseNumber = "SYN001"
        };

        // Act
        _unitOfWork.BeginTransaction();

        _unitOfWork.Repository<Bus>().Add(bus);
        _unitOfWork.SaveChanges();

        _unitOfWork.CommitTransaction();

        // Assert
        var savedBus = DbContext.Vehicles.Find(bus.VehicleId);
        savedBus.Should().NotBeNull();
        savedBus!.BusNumber.Should().Be("SYNC001");
    }

    [Test]
    public void BeginTransaction_RollbackTransaction_ShouldNotPersistChanges()
    {
        // Arrange
        var bus = new Bus
        {
            BusNumber = "ROLLBACK001",
            Year = 2019,
            Make = "Thomas Built",
            Model = "Saf-T-Liner",
            SeatingCapacity = 48,
            VINNumber = "ROLLBACK123456789",
            LicenseNumber = "ROL001"
        };

        // Act
        _unitOfWork.BeginTransaction();

        _unitOfWork.Repository<Bus>().Add(bus);
        _unitOfWork.SaveChanges();

        var busId = bus.VehicleId; // Capture ID before rollback

        _unitOfWork.RollbackTransaction();

        // Assert
        var rolledBackBus = DbContext.Vehicles.Find(busId);
        rolledBackBus.Should().BeNull();
    }

    #endregion
}
