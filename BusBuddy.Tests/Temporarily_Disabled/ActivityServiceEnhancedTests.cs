using NUnit.Framework;
using FluentAssertions;
using Bus_Buddy.Services;
using Bus_Buddy.Models;
using BusBuddy.Tests.Infrastructure;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BusBuddy.Tests.UnitTests.Services
{
    /// <summary>
    /// Tests for ActivityService - High coverage value service testing
    /// Targets uncovered async methods and business logic paths
    /// </summary>
    [TestFixture]
    public class ActivityServiceEnhancedTests : TestBase
    {
        private ActivityService _activityService = null!;

        [SetUp]
        public async Task SetUp()
        {
            await InitializeCleanDatabase();
            _activityService = GetService<ActivityService>();
        }

        [Test]
        public async Task GetAllActivitiesAsync_ShouldReturnAllActivities()
        {
            // Arrange
            await SeedTestDataAsync();

            // Act
            var activities = await _activityService.GetAllActivitiesAsync();

            // Assert
            activities.Should().NotBeNull();
            activities.Should().HaveCountGreaterThan(0);
        }

        [Test]
        public async Task GetActivityByIdAsync_WithValidId_ShouldReturnActivity()
        {
            // Arrange
            await SeedTestDataAsync();
            var allActivities = await _activityService.GetAllActivitiesAsync();
            var firstActivity = allActivities.First();

            // Act
            var activity = await _activityService.GetActivityByIdAsync(firstActivity.ActivityId);

            // Assert
            activity.Should().NotBeNull();
            activity!.ActivityId.Should().Be(firstActivity.ActivityId);
        }

        [Test]
        public async Task GetActivityByIdAsync_WithInvalidId_ShouldReturnNull()
        {
            // Act
            var activity = await _activityService.GetActivityByIdAsync(99999);

            // Assert
            activity.Should().BeNull();
        }

        [Test]
        public async Task CreateActivityAsync_WithValidActivity_ShouldCreateActivity()
        {
            // Arrange
            var newActivity = new Activity
            {
                ActivityName = "Test Activity Creation",
                ActivityType = "Test Type",
                Description = "Test Description for Coverage",
                Location = "Test Location",
                StartDate = DateTime.Today.AddDays(30),
                EndDate = DateTime.Today.AddDays(31),
                MaxParticipants = 50,
                IsActive = true
            };

            // Act
            var result = await _activityService.CreateActivityAsync(newActivity);

            // Assert
            result.Should().NotBeNull();
            result.ActivityName.Should().Be("Test Activity Creation");
            result.ActivityId.Should().BeGreaterThan(0);
        }

        [Test]
        public async Task UpdateActivityAsync_WithValidActivity_ShouldUpdateActivity()
        {
            // Arrange
            await SeedTestDataAsync();
            var activities = await _activityService.GetAllActivitiesAsync();
            var activityToUpdate = activities.First();

            activityToUpdate.ActivityName = "Updated Activity Name";
            activityToUpdate.Description = "Updated for test coverage";

            // Act
            var result = await _activityService.UpdateActivityAsync(activityToUpdate);

            // Assert
            result.Should().NotBeNull();
            result.ActivityName.Should().Be("Updated Activity Name");
            result.Description.Should().Be("Updated for test coverage");
        }

        [Test]
        public async Task DeleteActivityAsync_WithValidId_ShouldDeleteActivity()
        {
            // Arrange
            await SeedTestDataAsync();
            var activities = await _activityService.GetAllActivitiesAsync();
            var activityToDelete = activities.First();
            var initialCount = activities.Count();

            // Act
            var result = await _activityService.DeleteActivityAsync(activityToDelete.ActivityId);

            // Assert
            result.Should().BeTrue();

            var remainingActivities = await _activityService.GetAllActivitiesAsync();
            remainingActivities.Should().HaveCount(initialCount - 1);
        }

        [Test]
        public async Task DeleteActivityAsync_WithInvalidId_ShouldReturnFalse()
        {
            // Act
            var result = await _activityService.DeleteActivityAsync(99999);

            // Assert
            result.Should().BeFalse();
        }

        [Test]
        public async Task GetActivitiesByTypeAsync_WithValidType_ShouldReturnFilteredActivities()
        {
            // Arrange
            await SeedTestDataAsync();

            // Act
            var sportActivities = await _activityService.GetActivitiesByTypeAsync("Sports");

            // Assert
            sportActivities.Should().NotBeNull();
            sportActivities.Should().OnlyContain(a => a.ActivityType == "Sports");
        }

        [Test]
        public async Task GetActivitiesByDateRangeAsync_WithValidRange_ShouldReturnFilteredActivities()
        {
            // Arrange
            await SeedTestDataAsync();
            var startDate = DateTime.Today;
            var endDate = DateTime.Today.AddDays(365);

            // Act
            var activitiesInRange = await _activityService.GetActivitiesByDateRangeAsync(startDate, endDate);

            // Assert
            activitiesInRange.Should().NotBeNull();
            activitiesInRange.Should().OnlyContain(a =>
                a.StartDate >= startDate && a.StartDate <= endDate);
        }

        [Test]
        public async Task GetActiveActivitiesAsync_ShouldReturnOnlyActiveActivities()
        {
            // Arrange
            await SeedTestDataAsync();

            // Act
            var activeActivities = await _activityService.GetActiveActivitiesAsync();

            // Assert
            activeActivities.Should().NotBeNull();
            activeActivities.Should().OnlyContain(a => a.IsActive);
        }

        [Test]
        public async Task ActivateActivityAsync_WithValidId_ShouldActivateActivity()
        {
            // Arrange
            await SeedTestDataAsync();
            var activities = await _activityService.GetAllActivitiesAsync();
            var activity = activities.First();

            // Deactivate first
            activity.IsActive = false;
            await _activityService.UpdateActivityAsync(activity);

            // Act
            var result = await _activityService.ActivateActivityAsync(activity.ActivityId);

            // Assert
            result.Should().BeTrue();

            var updatedActivity = await _activityService.GetActivityByIdAsync(activity.ActivityId);
            updatedActivity!.IsActive.Should().BeTrue();
        }

        [Test]
        public async Task DeactivateActivityAsync_WithValidId_ShouldDeactivateActivity()
        {
            // Arrange
            await SeedTestDataAsync();
            var activities = await _activityService.GetAllActivitiesAsync();
            var activity = activities.First();

            // Act
            var result = await _activityService.DeactivateActivityAsync(activity.ActivityId);

            // Assert
            result.Should().BeTrue();

            var updatedActivity = await _activityService.GetActivityByIdAsync(activity.ActivityId);
            updatedActivity!.IsActive.Should().BeFalse();
        }
    }
}
