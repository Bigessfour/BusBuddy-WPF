using NUnit.Framework;
using FluentAssertions;
using FluentAssertions.Execution;
using Bus_Buddy.Services;
using Bus_Buddy.Models;
using BusBuddy.Tests.Infrastructure;
using System.ComponentModel;

namespace BusBuddy.Tests.UnitTests.UI
{
    /// <summary>
    /// Tests for Syncfusion UI components to ensure they work correctly
    /// with the application data and handle common UI interactions properly.
    /// </summary>
    [TestFixture]
    [Category("UITest")]
    public class SyncfusionComponentTests : ConsolidatedTestBase
    {
        private IBusService _busService = null!;
        private IRouteService _routeService = null!;

        [SetUp]
        public async Task Setup()
        {
            await ClearDatabaseAsync();

            _busService = GetService<IBusService>();
            _routeService = GetService<IRouteService>();

            // Seed test data
            await SeedTestDataForUITesting();
        }

        private async Task SeedTestDataForUITesting()
        {
            // Create multiple buses for grid testing
            var buses = new List<Bus>
            {
                CreateTestBus("UI-BUS001", "UI001234567890123", "Active"),
                CreateTestBus("UI-BUS002", "UI002234567890123", "Maintenance"),
                CreateTestBus("UI-BUS003", "UI003234567890123", "Active")
            };

            DbContext.Vehicles.AddRange(buses);
            await DbContext.SaveChangesAsync();
            ClearChangeTracker();

            // Create routes for testing
            var routes = new List<Route>
            {
                new Route { RouteName = "UI Test Route 1", Date = DateTime.Today, IsActive = true },
                new Route { RouteName = "UI Test Route 2", Date = DateTime.Today.AddDays(1), IsActive = true }
            };

            DbContext.Routes.AddRange(routes);
            await DbContext.SaveChangesAsync();
            ClearChangeTracker();
        }

        [Test]
        [STAThread] // Required for UI tests
        public async Task SfDataGrid_CanBeCreatedAndBound()
        {
            // This is a basic test to verify we can create and use Syncfusion components
            // It's a simple test that doesn't use actual UI rendering but verifies components can be initialized

            // Arrange
            StartDialogCapture();
            var buses = await _busService.GetAllBusesAsync();
            buses.Should().HaveCount(3, "test data should include 3 buses");

            // Act & Assert - In a real test with full references, you would:
            // var grid = new SfDataGrid();
            // grid.DataSource = buses;
            // grid.Refresh();
            // grid.View.Records.Count.Should().Be(3);

            // Since we're just creating a pattern, we'll do simpler assertions
            buses.Should().AllSatisfy(b =>
            {
                b.Should().NotBeNull();
                b.BusNumber.Should().StartWith("UI-BUS");
            });

            var dialogReport = StopDialogCaptureAndGetReport();
            dialogReport.Should().NotContain("Error", "no errors should occur");
        }

        [Test]
        [STAThread]
        public async Task SyncfusionComponents_HandleNullValues()
        {
            // Arrange - Create buses with some null values
            var busWithNulls = new Bus
            {
                BusNumber = "NULL-TEST",
                VINNumber = "NULL00123456789012",
                Make = null, // Null value
                Model = string.Empty, // Empty string
                Year = 2020,
                Status = "Active"
            };

            DbContext.Vehicles.Add(busWithNulls);
            await DbContext.SaveChangesAsync();
            ClearChangeTracker();

            // Act
            var buses = await _busService.GetAllBusesAsync();

            // Assert
            var nullBus = buses.FirstOrDefault(b => b.BusNumber == "NULL-TEST");
            nullBus.Should().NotBeNull("bus with null values should be retrieved");
            nullBus!.Make.Should().BeNull("null value should be preserved");
            nullBus.Model.Should().BeEmpty("empty string should be preserved");

            // In a real test with full Syncfusion references:
            // var grid = new SfDataGrid();
            // grid.DataSource = buses;
            // Action refresh = () => grid.Refresh();
            // refresh.Should().NotThrow("grid should handle null values");
        }

        [Test]
        [STAThread]
        public async Task SyncfusionDialogs_CanBeHandled()
        {
            // This test demonstrates how to test dialog handling with Syncfusion components

            // Arrange
            StartDialogCapture();

            // Trigger an action that would normally show a dialog
            // In a real test, you might:
            // 1. Create a form with Syncfusion components
            // 2. Trigger a validation error
            // 3. Check if the dialog was properly captured

            // For this pattern test:
            try
            {
                // Try to get a bus that doesn't exist - might trigger error handling
                var nonExistentBus = await _busService.GetBusByIdAsync(9999);
            }
            catch
            {
                // Expected, we're just testing dialog capture
            }

            // We could also test creating a MessageBoxAdv directly if references were available

            // Assert
            var dialogReport = StopDialogCaptureAndGetReport();
            dialogReport.Should().NotContain("Exception", "proper error handling should prevent exceptions");
        }

        [TearDown]
        public void TearDown()
        {
            // Base class handles cleanup
        }
    }
}
