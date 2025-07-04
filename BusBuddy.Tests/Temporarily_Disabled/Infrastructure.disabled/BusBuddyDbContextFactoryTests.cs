using NUnit.Framework;
using FluentAssertions;
using Bus_Buddy;
using Bus_Buddy.Data;
using Microsoft.EntityFrameworkCore;
using System;

namespace BusBuddy.Tests.UnitTests.Infrastructure
{
    /// <summary>
    /// Tests for BusBuddyDbContextFactory - Database context creation
    /// This covers the factory pattern for database context creation
    /// </summary>
    [TestFixture]
    public class BusBuddyDbContextFactoryTests
    {
        private BusBuddyDbContextFactory _factory = null!;

        [SetUp]
        public void SetUp()
        {
            _factory = new BusBuddyDbContextFactory();
        }

        [Test]
        public void CreateDbContext_WithEmptyArgs_ShouldCreateContext()
        {
            // Arrange
            var args = Array.Empty<string>();

            // Act
            var context = _factory.CreateDbContext(args);

            // Assert
            context.Should().NotBeNull();
            context.Should().BeOfType<BusBuddyDbContext>();
        }

        [Test]
        public void CreateDbContext_WithNullArgs_ShouldCreateContext()
        {
            // Act
            var context = _factory.CreateDbContext(null!);

            // Assert
            context.Should().NotBeNull();
            context.Should().BeOfType<BusBuddyDbContext>();
        }

        [Test]
        public void CreateDbContext_ShouldConfigureDbContext()
        {
            // Arrange
            var args = Array.Empty<string>();

            // Act
            var context = _factory.CreateDbContext(args);

            // Assert - verify basic DbContext configuration
            context.Database.Should().NotBeNull();

            // Verify DbSets are configured
            context.Vehicles.Should().NotBeNull();
            context.Drivers.Should().NotBeNull();
            context.Students.Should().NotBeNull();
            context.Routes.Should().NotBeNull();
        }

        [Test]
        public void CreateDbContext_ShouldUseSqlServerProvider()
        {
            // Arrange
            var args = Array.Empty<string>();

            // Act
            var context = _factory.CreateDbContext(args);

            // Assert
            context.Database.ProviderName.Should().Contain("SqlServer",
                "because the factory should configure SQL Server by default");
        }

        [Test]
        public void CreateDbContext_MultipleCallsShouldCreateSeparateInstances()
        {
            // Arrange
            var args = Array.Empty<string>();

            // Act
            var context1 = _factory.CreateDbContext(args);
            var context2 = _factory.CreateDbContext(args);

            // Assert
            context1.Should().NotBeSameAs(context2,
                "because each call should create a new instance");
        }

        [Test]
        public void CreateDbContext_ShouldSetAuditUserToSystem()
        {
            // Arrange
            var args = Array.Empty<string>();

            // Act
            var context = _factory.CreateDbContext(args);

            // Assert
            context.GetCurrentAuditUser().Should().Be("System",
                "because factory should set default audit user");
        }

        [TearDown]
        public void TearDown()
        {
            // Cleanup any resources if needed
        }
    }
}
