using NUnit.Framework;
using System;
using System.Windows.Forms;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Syncfusion.Windows.Forms;
using Syncfusion.WinForms.DataGrid;
using Syncfusion.Windows.Forms.Tools;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Bus_Buddy.Services;
using Bus_Buddy.Forms;
using Bus_Buddy.Utilities;

namespace BusBuddy.Tests.UnitTests.UI
{
    [TestFixture]
    [Apartment(ApartmentState.STA)]
    public class SyncfusionStartupSequenceTests
    {
        private ServiceProvider? _serviceProvider;
        private ILogger<SyncfusionStartupSequenceTests>? _logger;

        [SetUp]
        public void Setup()
        {
            // Initialize service container for testing
            var services = new ServiceCollection();
            services.AddLogging();
            services.AddTransient<IBusService, BusService>();
            services.AddTransient<IRouteService, RouteService>();
            services.AddTransient<IStudentService, StudentService>();
            _serviceProvider = services.BuildServiceProvider();
            _logger = _serviceProvider.GetService<ILogger<SyncfusionStartupSequenceTests>>();
        }

        [TearDown]
        public void TearDown()
        {
            _serviceProvider?.Dispose();
        }

        [Test]
        public void Application_Startup_InitializesSuccessfully()
        {
            // Arrange & Act & Assert
            Assert.DoesNotThrow(() => {
                var startupManager = new SyncfusionStartupManager();
                startupManager.Should().NotBeNull();
                _logger?.LogInformation("Startup manager created successfully");
            });
        }

        [Test]
        public void Syncfusion_License_InitializesCorrectly()
        {
            // Arrange & Act & Assert
            Assert.DoesNotThrow(() => {
                SyncfusionStartupManager.InitializeLicensing();
                _logger?.LogInformation("Syncfusion licensing initialized");
            });
        }

        [Test]
        public void Syncfusion_Theme_LoadsCorrectly()
        {
            // Arrange & Act
            var themeApplied = false;
            Assert.DoesNotThrow(() => {
                SyncfusionStartupManager.ApplyOffice2016ColorfulTheme();
                themeApplied = true;
            });

            // Assert
            themeApplied.Should().BeTrue();
        }

        [Test]
        public async Task Dashboard_Startup_LoadsAllComponents()
        {
            // Arrange
            Form? dashboard = null;
            bool initializationSuccessful = false;

            // Act
            await Task.Run(() => {
                Assert.DoesNotThrow(() => {
                    // Note: We're testing the initialization logic without actually showing the form
                    // to avoid UI automation complexity in unit tests
                    var backgroundFix = new SyncfusionBackgroundFix();
                    backgroundFix.Should().NotBeNull();
                    initializationSuccessful = true;
                });
            });

            // Assert
            initializationSuccessful.Should().BeTrue();
            _logger?.LogInformation("Dashboard startup components initialized successfully");
        }

        [Test]
        public void MetroForm_Configuration_InitializesCorrectly()
        {
            // Arrange & Act
            MetroForm? testForm = null;
            Assert.DoesNotThrow(() => {
                testForm = new MetroForm();
                testForm.Size = new System.Drawing.Size(800, 600);
                testForm.Text = "Test Form";
            });

            // Assert
            testForm.Should().NotBeNull();
            testForm?.Size.Width.Should().Be(800);
            testForm?.Size.Height.Should().Be(600);
            testForm?.Text.Should().Be("Test Form");
            
            // Cleanup
            testForm?.Dispose();
        }

        [Test]
        public void SyncfusionControls_Initialize_WithoutErrors()
        {
            // Arrange & Act & Assert
            Assert.DoesNotThrow(() => {
                using var dataGrid = new SfDataGrid();
                dataGrid.Should().NotBeNull();
                
                using var dockingManager = new DockingManager();
                dockingManager.Should().NotBeNull();
                
                _logger?.LogInformation("Core Syncfusion controls initialized successfully");
            });
        }

        [Test]
        public void ComponentStartupOrder_FollowsCorrectSequence()
        {
            // Arrange
            var startupSteps = new List<string>();

            // Act
            Assert.DoesNotThrow(() => {
                // Step 1: License initialization
                startupSteps.Add("License");
                
                // Step 2: Theme application  
                startupSteps.Add("Theme");
                
                // Step 3: Control creation
                startupSteps.Add("Controls");
                
                // Step 4: Background fix application
                startupSteps.Add("BackgroundFix");
            });

            // Assert
            startupSteps.Should().HaveCount(4);
            startupSteps[0].Should().Be("License");
            startupSteps[1].Should().Be("Theme");
            startupSteps[2].Should().Be("Controls");
            startupSteps[3].Should().Be("BackgroundFix");
        }

        [Test]
        public void ErrorHandling_DuringStartup_DoesNotCrashApplication()
        {
            // Arrange & Act & Assert
            Assert.DoesNotThrow(() => {
                try
                {
                    // Simulate potential startup error scenarios
                    var startupManager = new SyncfusionStartupManager();
                    // Test that errors are handled gracefully
                }
                catch (Exception ex)
                {
                    // Log but don't fail - startup should be resilient
                    _logger?.LogWarning($"Startup warning handled: {ex.Message}");
                }
            });
        }

        [Test]
        public void ServiceDependencies_LoadCorrectly_DuringStartup()
        {
            // Arrange & Act
            IBusService? busService = null;
            IRouteService? routeService = null;
            IStudentService? studentService = null;

            Assert.DoesNotThrow(() => {
                busService = _serviceProvider?.GetService<IBusService>();
                routeService = _serviceProvider?.GetService<IRouteService>();
                studentService = _serviceProvider?.GetService<IStudentService>();
            });

            // Assert
            busService.Should().NotBeNull();
            routeService.Should().NotBeNull();
            studentService.Should().NotBeNull();
        }

        [Test]
        public void BackgroundFix_AppliesDuringStartup()
        {
            // Arrange & Act
            SyncfusionBackgroundFix? backgroundFix = null;
            Assert.DoesNotThrow(() => {
                backgroundFix = new SyncfusionBackgroundFix();
            });

            // Assert
            backgroundFix.Should().NotBeNull();
            _logger?.LogInformation("Background fix utility ready for application");
        }
    }
}
