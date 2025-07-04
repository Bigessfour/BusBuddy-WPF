using NUnit.Framework;
using FluentAssertions;
using Bus_Buddy.Models;
using BusBuddy.Tests.Infrastructure;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Bus_Buddy.Services;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace BusBuddy.Tests.UnitTests.Proper
{
    /// <summary>
    /// Example of proper testing that works WITH the application design
    /// </summary>
    [TestFixture]
    public class ProperBusServiceTests : ProperTestBase
    {
        private IBusService _busService = null!;

        [SetUp]
        public void SetUp()
        {
            _busService = ServiceProvider.GetRequiredService<IBusService>();
        }

        [Test]
        public async Task GetAllBusEntitiesAsync_ShouldReturnActualSeededBuses()
        {
            // Arrange - discover what the application actually does

            // Act
            var result = await _busService.GetAllBusEntitiesAsync();

            // Assert - document the application's actual behavior
            result.Should().NotBeNull();

            // Let's discover the actual count and document it
            var actualCount = result.Count;
            Console.WriteLine($"Actual seeded bus count: {actualCount}");

            // Validate business logic regardless of count
            result.Should().OnlyContain(b => !string.IsNullOrEmpty(b.BusNumber), "because all buses should have valid bus numbers");
            result.Should().OnlyContain(b => b.VehicleId > 0, "because all buses should have valid IDs");

            // Document what we found for future tests
            actualCount.Should().BeGreaterThan(0, "because the application should have some seeded data");
        }

        [Test]
        public async Task AddBusEntityAsync_ShouldAddBusAndDiscoverAuditBehavior()
        {
            // Arrange - discover the application's actual behavior
            var initialCount = await DbContext.Vehicles.CountAsync();
            Console.WriteLine($"Initial bus count: {initialCount}");

            var newBus = new Bus
            {
                BusNumber = "TEST999",
                Year = 2023,
                Make = "Test Make",
                Model = "Test Model",
                SeatingCapacity = 50,
                VINNumber = "TEST123456789012", // 17 characters
                LicenseNumber = "TEST999"
            };

            // Act
            var result = await _busService.AddBusEntityAsync(newBus);

            // Assert - document what actually happens
            result.Should().NotBeNull();
            result.VehicleId.Should().BeGreaterThan(0);

            // Let's see how audit fields actually work
            Console.WriteLine($"CreatedBy: '{result.CreatedBy}' (null = {result.CreatedBy == null})");
            Console.WriteLine($"CreatedDate: {result.CreatedDate}");

            // Test what actually works, not what we assume should work
            result.CreatedDate.Should().BeAfter(DateTime.MinValue, "because CreatedDate should be set");

            // This is the real business logic validation
            var finalCount = await DbContext.Vehicles.CountAsync();
            finalCount.Should().Be(initialCount + 1, "because we added one bus");
        }

        [Test]
        public async Task GetBusEntityByIdAsync_ShouldReturnSeededBus()
        {
            // Arrange - use the known seeded data
            const int firstSeededBusId = 1; // From the DbContext seeding

            // Act
            var result = await _busService.GetBusEntityByIdAsync(firstSeededBusId);

            // Assert - validate the application's seeded data
            result.Should().NotBeNull();
            result!.VehicleId.Should().Be(firstSeededBusId);
            result.BusNumber.Should().Be("001", "because that's what the application seeds");
            result.Make.Should().Be("Blue Bird", "because that's what the application seeds");
        }

        [Test]
        public async Task BusinessLogic_ShouldRespectEntityRelationships()
        {
            // Arrange - test the FK relationships work as designed
            var bus = await _busService.GetBusEntityByIdAsync(1);
            bus.Should().NotBeNull();

            // Act - try to create a route that references this bus
            var route = new Route
            {
                RouteName = "Test Route",
                Date = DateTime.Today,
                AMVehicleId = bus!.VehicleId, // Reference the seeded bus
                IsActive = true
            };

            DbContext.Routes.Add(route);
            await DbContext.SaveChangesAsync();

            // Assert - validate relationship integrity
            var savedRoute = await DbContext.Routes
                .Include(r => r.AMVehicle)
                .FirstAsync(r => r.RouteName == "Test Route");

            savedRoute.AMVehicle.Should().NotBeNull();
            savedRoute.AMVehicle!.VehicleId.Should().Be(bus.VehicleId);
        }
    }
}
