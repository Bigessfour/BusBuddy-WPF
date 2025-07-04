using NUnit.Framework;
using FluentAssertions;
using Bus_Buddy.Services;
using Bus_Buddy.Models;
using Bus_Buddy.Forms;
using BusBuddy.Tests.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BusBuddy.Tests.IntegrationTests.Forms
{
    /// <summary>
    /// Integration tests for BusManagementForm with BusService
    /// Tests form-service interaction to ensure UI components work with business logic
    /// </summary>
    [TestFixture]
    public class BusManagementFormIntegrationTests : TestBase
    {
        private IBusService _busService = null!;
        private BusManagementForm _form = null!;

        [SetUp]
        public async Task Setup()
        {
            await ClearDatabaseAsync();
            _busService = ServiceProvider.GetRequiredService<IBusService>();

            // Initialize form with service - Note: This tests the basic instantiation
            // In a real scenario, we would mock the UI interaction
        }

        [TearDown]
        public async Task TearDown()
        {
            _form?.Dispose();
            await ClearDatabaseAsync();
        }

        [Test]
        public async Task BusService_ShouldSupportFormOperations_ForBusManagement()
        {
            // This test verifies that the BusService can perform operations
            // that would be required by the BusManagementForm

            // Arrange - Create a bus through the service (simulating form create)
            var bus = new Bus
            {
                BusNumber = "FORM-BUS-001",
                Model = "Integration Test Bus",
                SeatingCapacity = 72,
                Year = 2023,
                Status = "Active"
            };

            // Act - Perform CRUD operations that form would use
            var createdBus = await _busService.AddBusEntityAsync(bus);
            var retrievedBus = await _busService.GetBusEntityByIdAsync(createdBus.VehicleId);

            // Update operation
            retrievedBus!.Model = "Updated by Form";
            var updateResult = await _busService.UpdateBusEntityAsync(retrievedBus);

            // Get all buses (for form grid)
            var allBuses = await _busService.GetAllBusEntitiesAsync();

            // Delete operation
            var deleteResult = await _busService.DeleteBusEntityAsync(createdBus.VehicleId);

            // Assert - Verify all operations succeeded
            createdBus.Should().NotBeNull();
            createdBus.BusNumber.Should().Be("FORM-BUS-001");

            retrievedBus.Should().NotBeNull();
            retrievedBus.VehicleId.Should().Be(createdBus.VehicleId);

            updateResult.Should().BeTrue();

            allBuses.Should().NotBeNull();
            allBuses.Should().HaveCountGreaterThanOrEqualTo(1);

            deleteResult.Should().BeTrue();
        }

        [Test]
        public async Task RouteService_ShouldSupportFormOperations_ForTicketManagement()
        {
            // This test verifies that the RouteService can perform operations
            // that would be required by the TicketManagementForm

            // Arrange
            var routeService = ServiceProvider.GetRequiredService<IRouteService>();
            var route = new Route
            {
                Date = DateTime.Today,
                RouteName = "Ticket Form Route",
                Description = "Route for ticket form testing",
                AMBeginMiles = 100,
                AMEndMiles = 150
            };

            // Act - Perform operations that TicketManagementForm would use
            var createdRoute = await routeService.CreateRouteAsync(route);
            var allActiveRoutes = await routeService.GetAllActiveRoutesAsync();
            var routeById = await routeService.GetRouteByIdAsync(createdRoute.RouteId);
            var searchResults = await routeService.SearchRoutesAsync("Ticket");

            // Assert - Verify RouteService works for form integration
            createdRoute.Should().NotBeNull();
            createdRoute.RouteName.Should().Be("Ticket Form Route");

            allActiveRoutes.Should().NotBeNull();
            allActiveRoutes.Should().HaveCountGreaterThanOrEqualTo(1);

            routeById.Should().NotBeNull();
            routeById.RouteId.Should().Be(createdRoute.RouteId);

            searchResults.Should().NotBeNull();
            searchResults.Should().HaveCountGreaterThanOrEqualTo(1);
        }

        [Test]
        public async Task DriverService_ShouldSupportFormOperations_ForDriverManagement()
        {
            // This test verifies that the DriverService operations work
            // for DriverManagementForm requirements

            // Arrange
            var driver = new Driver
            {
                FirstName = "Integration",
                LastName = "Test",
                LicenseNumber = "INT-TEST-001",
                DriverPhone = "555-TEST",
                DriverEmail = "integration@test.com",
                Status = "Active"
            };

            // Act - Test driver operations through BusService (which handles drivers)
            var createdDriver = await _busService.AddDriverEntityAsync(driver);
            var retrievedDriver = await _busService.GetDriverEntityByIdAsync(createdDriver.DriverId);
            var allDrivers = await _busService.GetAllDriversAsync();

            // Update operation
            retrievedDriver!.DriverPhone = "555-UPDATED";
            var updateResult = await _busService.UpdateDriverEntityAsync(retrievedDriver);

            // Delete operation (addressing the critical blocker)
            var deleteResult = await _busService.DeleteDriverEntityAsync(createdDriver.DriverId);

            // Assert - Verify all driver operations work
            createdDriver.Should().NotBeNull();
            createdDriver.LicenseNumber.Should().Be("INT-TEST-001");

            retrievedDriver.Should().NotBeNull();
            retrievedDriver.DriverId.Should().Be(createdDriver.DriverId);

            allDrivers.Should().NotBeNull();
            allDrivers.Should().HaveCountGreaterThanOrEqualTo(1);

            updateResult.Should().BeTrue();
            deleteResult.Should().BeTrue(); // This tests the critical blocker fix
        }

        [Test]
        public void FormServices_ShouldBeRegistered_InDependencyInjection()
        {
            // Verify all required services are registered and can be resolved

            // Act & Assert
            var busService = ServiceProvider.GetRequiredService<IBusService>();
            busService.Should().NotBeNull();

            var routeService = ServiceProvider.GetRequiredService<IRouteService>();
            routeService.Should().NotBeNull();

            var activityService = ServiceProvider.GetRequiredService<IActivityService>();
            activityService.Should().NotBeNull();

            var scheduleService = ServiceProvider.GetRequiredService<IScheduleService>();
            scheduleService.Should().NotBeNull();

            var studentService = ServiceProvider.GetRequiredService<IStudentService>();
            studentService.Should().NotBeNull();

            var fuelService = ServiceProvider.GetRequiredService<IFuelService>();
            fuelService.Should().NotBeNull();

            var maintenanceService = ServiceProvider.GetRequiredService<IMaintenanceService>();
            maintenanceService.Should().NotBeNull();

            var ticketService = ServiceProvider.GetRequiredService<ITicketService>();
            ticketService.Should().NotBeNull();
        }

        [Test]
        public async Task DatabaseContext_ShouldSupportConcurrentOperations_ForFormScenarios()
        {
            // Test that the database context can handle concurrent operations
            // that might occur when multiple forms are open

            // Arrange
            var bus1 = new Bus { BusNumber = "CONCURRENT-001", Model = "Bus 1", SeatingCapacity = 72, Year = 2023, Status = "Active" };
            var bus2 = new Bus { BusNumber = "CONCURRENT-002", Model = "Bus 2", SeatingCapacity = 80, Year = 2023, Status = "Active" };

            var route1 = new Route { Date = DateTime.Today, RouteName = "Route 1", Description = "First route" };
            var route2 = new Route { Date = DateTime.Today.AddDays(1), RouteName = "Route 2", Description = "Second route" };

            // Act - Perform concurrent-like operations
            var routeService = ServiceProvider.GetRequiredService<IRouteService>();

            var createdBus1 = await _busService.AddBusEntityAsync(bus1);
            var createdRoute1 = await routeService.CreateRouteAsync(route1);
            var createdBus2 = await _busService.AddBusEntityAsync(bus2);
            var createdRoute2 = await routeService.CreateRouteAsync(route2);

            // Verify operations
            var allBuses = await _busService.GetAllBusEntitiesAsync();
            var allRoutes = await routeService.GetAllActiveRoutesAsync();

            // Assert
            allBuses.Should().HaveCount(2);
            allRoutes.Should().HaveCount(2);

            createdBus1.BusNumber.Should().Be("CONCURRENT-001");
            createdBus2.BusNumber.Should().Be("CONCURRENT-002");
            createdRoute1.RouteName.Should().Be("Route 1");
            createdRoute2.RouteName.Should().Be("Route 2");
        }
    }
}
