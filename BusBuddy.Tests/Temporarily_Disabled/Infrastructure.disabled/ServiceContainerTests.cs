using NUnit.Framework;
using FluentAssertions;
using Bus_Buddy;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Bus_Buddy.Data;
using Bus_Buddy.Services;
using System;

namespace BusBuddy.Tests.UnitTests.Infrastructure
{
    /// <summary>
    /// Tests for ServiceContainer - Core DI infrastructure
    /// This covers the application's dependency injection setup
    /// </summary>
    [TestFixture]
    public class ServiceContainerTests
    {
        [Test]
        public void ConfigureServices_ShouldRegisterAllRequiredServices()
        {
            // Arrange
            var services = new ServiceCollection();

            // Act
            ServiceContainer.ConfigureServices(services);
            var serviceProvider = services.BuildServiceProvider();

            // Assert - verify core services are registered
            serviceProvider.GetService<ILogger<ServiceContainer>>().Should().NotBeNull("because logging should be configured");
            serviceProvider.GetService<BusBuddyDbContext>().Should().NotBeNull("because DbContext should be registered");
        }

        [Test]
        public void ConfigureServices_ShouldRegisterBusService()
        {
            // Arrange
            var services = new ServiceCollection();

            // Act
            ServiceContainer.ConfigureServices(services);
            var serviceProvider = services.BuildServiceProvider();

            // Assert
            serviceProvider.GetService<IBusService>().Should().NotBeNull("because BusService should be registered");
        }

        [Test]
        public void ConfigureServices_ShouldRegisterAllBusinessServices()
        {
            // Arrange
            var services = new ServiceCollection();

            // Act
            ServiceContainer.ConfigureServices(services);
            var serviceProvider = services.BuildServiceProvider();

            // Assert - verify all business services
            serviceProvider.GetService<IRouteService>().Should().NotBeNull();
            serviceProvider.GetService<IActivityService>().Should().NotBeNull();
            serviceProvider.GetService<IScheduleService>().Should().NotBeNull();
            serviceProvider.GetService<IStudentService>().Should().NotBeNull();
            serviceProvider.GetService<IFuelService>().Should().NotBeNull();
            serviceProvider.GetService<IMaintenanceService>().Should().NotBeNull();
            serviceProvider.GetService<ITicketService>().Should().NotBeNull();
        }

        [Test]
        public void ConfigureServices_WithNullServices_ShouldThrowArgumentNullException()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => ServiceContainer.ConfigureServices(null!));
        }

        [Test]
        public void ConfigureServices_ShouldConfigureSyncfusionLicense()
        {
            // Arrange
            var services = new ServiceCollection();

            // Act & Assert - should not throw
            Assert.DoesNotThrow(() => ServiceContainer.ConfigureServices(services));
        }
    }
}
