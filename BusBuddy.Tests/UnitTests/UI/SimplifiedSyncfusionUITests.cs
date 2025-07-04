using NUnit.Framework;
using FluentAssertions;
using System;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using Syncfusion.Licensing;
using Syncfusion.Windows.Forms;
using Syncfusion.WinForms.Controls;
using Syncfusion.Windows.Forms.Tools;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using Bus_Buddy;
using Bus_Buddy.Services;
using Bus_Buddy.Models;
using static Bus_Buddy.Services.IBusService;
using BusBuddy.Tests.Infrastructure;

namespace BusBuddy.Tests.UnitTests.UI
{
    /// <summary>
    /// Comprehensive Dashboard UI Test with Dialog Capture - Rigorous validation of all Dashboard components
    /// Tests Syncfusion MetroForm, panels, buttons, labels, and data integration per exact specifications
    /// NOW WITH DIALOG CAPTURE: Captures and analyzes all dialog boxes that appear during testing
    /// </summary>
    [TestFixture]
    [Apartment(ApartmentState.STA)]
    public class SimplifiedSyncfusionUITests : TestBase
    {
        private const string LicenseKey = "Ngo9BigBOggjHTQxAR8/V1JEaF5cXmRCf1FpRmJGdld5fUVHYVZUTXxaS00DNHVRdkdmWXhec3RSRGRYU0R2WUBWYEk=";

        private Dashboard _dashboard;
        private Mock<ILogger<Dashboard>> _loggerMock;
        private Mock<IBusService> _busServiceMock;
        private Mock<IConfigurationService> _configServiceMock;

        [SetUp]
        public void Setup()
        {
            // Start dialog capture FIRST
            StartDialogCapture();
            TestContext.WriteLine("=== DASHBOARD TEST SETUP - DIALOG CAPTURE STARTED ===");

            // Initialize Syncfusion license
            SyncfusionLicenseProvider.RegisterLicense(LicenseKey);

            // Setup mocks using existing service provider from TestBase
            _loggerMock = new Mock<ILogger<Dashboard>>();
            _busServiceMock = new Mock<IBusService>();
            _configServiceMock = new Mock<IConfigurationService>();

            // Mock bus service data to prevent null reference exceptions
            _busServiceMock.Setup(s => s.GetAllBusesAsync()).ReturnsAsync(new System.Collections.Generic.List<BusInfo>
            {
                new BusInfo { Status = "Active", Capacity = 50 },
                new BusInfo { Status = "In Service", Capacity = 40 },
                new BusInfo { Status = "Maintenance", Capacity = 30 }
            });
            _busServiceMock.Setup(s => s.GetAllRoutesAsync()).ReturnsAsync(new System.Collections.Generic.List<RouteInfo>
            {
                new RouteInfo(),
                new RouteInfo()
            });

            // Mock bus service data to prevent null reference exceptions
            _busServiceMock.Setup(s => s.GetAllBusesAsync()).ReturnsAsync(new System.Collections.Generic.List<Bus_Buddy.Services.BusInfo>
            {
                new Bus_Buddy.Services.BusInfo { Status = "Active", Capacity = 50 },
                new Bus_Buddy.Services.BusInfo { Status = "In Service", Capacity = 40 },
                new Bus_Buddy.Services.BusInfo { Status = "Maintenance", Capacity = 30 }
            });
            _busServiceMock.Setup(s => s.GetAllRoutesAsync()).ReturnsAsync(new System.Collections.Generic.List<Bus_Buddy.Services.RouteInfo>
            {
                new Bus_Buddy.Services.RouteInfo(),
                new Bus_Buddy.Services.RouteInfo()
            });

            try
            {
                // Initialize dashboard
                TestContext.WriteLine("Creating Dashboard instance...");
                _dashboard = new Dashboard(_loggerMock.Object, _busServiceMock.Object, _configServiceMock.Object);
                TestContext.WriteLine("Showing Dashboard...");
                _dashboard.Show();
                Application.DoEvents();
                TestContext.WriteLine("Dashboard setup complete");
            }
            catch (Exception ex)
            {
                TestContext.WriteLine($"Exception during dashboard setup: {ex.Message}");
                TestContext.WriteLine($"Stack trace: {ex.StackTrace}");

                // Check for dialogs even if setup failed
                var setupDialogs = GetCapturedDialogs();
                TestContext.WriteLine($"Dialogs captured during setup: {setupDialogs.Count}");
                foreach (var dialog in setupDialogs)
                {
                    TestContext.WriteLine($"Setup Dialog: {dialog.DialogType} - {dialog.Title} - {dialog.ErrorContext}");
                }
            }
        }

        [TearDown]
        public void TearDown()
        {
            TestContext.WriteLine("=== DASHBOARD TEST TEARDOWN - CAPTURING DIALOGS ===");

            // Get complete dialog capture report
            var dialogReport = StopDialogCaptureAndGetReport();

            TestContext.WriteLine("=== COMPLETE DIALOG CAPTURE REPORT ===");
            TestContext.WriteLine(dialogReport);

            // Log detailed dialog information
            LogCapturedDialogs();

            // Analyze captured dialogs for the "13 dialog boxes" issue
            var capturedDialogs = GetCapturedDialogs();
            if (capturedDialogs.Any())
            {
                TestContext.WriteLine($"\n=== DIALOG ANALYSIS ===");
                TestContext.WriteLine($"Total dialogs captured: {capturedDialogs.Count}");

                var errorDialogs = capturedDialogs.Where(d => d.DialogType.Contains("Error")).ToList();
                var warningDialogs = capturedDialogs.Where(d => d.DialogType.Contains("Warning")).ToList();
                var infoDialogs = capturedDialogs.Where(d => d.DialogType.Contains("Information")).ToList();

                TestContext.WriteLine($"Error dialogs: {errorDialogs.Count}");
                TestContext.WriteLine($"Warning dialogs: {warningDialogs.Count}");
                TestContext.WriteLine($"Information dialogs: {infoDialogs.Count}");

                // List all error contexts to identify issues
                foreach (var errorDialog in errorDialogs)
                {
                    TestContext.WriteLine($"ERROR DIALOG: {errorDialog.Title} - {errorDialog.ErrorContext}");
                }

                // Check if we found the "13 dialog boxes"
                if (capturedDialogs.Count >= 10)
                {
                    TestContext.WriteLine($"*** FOUND MULTIPLE DIALOGS ({capturedDialogs.Count}) - ANALYZING ***");
                    for (int i = 0; i < capturedDialogs.Count; i++)
                    {
                        var dialog = capturedDialogs[i];
                        TestContext.WriteLine($"Dialog #{i + 1}: {dialog.DialogType} - {dialog.Title} at {dialog.Timestamp:HH:mm:ss.fff}");
                    }
                }
            }
            else
            {
                TestContext.WriteLine("No dialogs captured during this test");
            }

            // Cleanup dashboard
            try
            {
                _dashboard?.Close();
                _dashboard?.Dispose();
            }
            catch (Exception ex)
            {
                TestContext.WriteLine($"Error during dashboard disposal: {ex.Message}");
            }

            Application.DoEvents();
        }

        [Test]
        public void Dashboard_Startup_InitializesSuccessfully()
        {
            _dashboard.Should().NotBeNull();
            _dashboard.Text.Should().Be("BusBuddy - Dashboard");
            _dashboard.AutoScaleMode.Should().Be(AutoScaleMode.Dpi);
            _dashboard.MetroColor.Should().Be(Color.FromArgb(46, 125, 185));
            _dashboard.CaptionBarColor.Should().Be(Color.FromArgb(46, 125, 185));
            _dashboard.CaptionForeColor.Should().Be(Color.White);
            _dashboard.Font.Name.Should().Be("Segoe UI");
        }

        [Test]
        public void MainPanel_ConfiguredCorrectly()
        {
            var mainPanel = FindControl<GradientPanel>(_dashboard, "mainPanel");
            mainPanel.Should().NotBeNull();
            mainPanel.Dock.Should().Be(DockStyle.Fill);
            mainPanel.Size.Should().Be(new Size(1200, 800));
            mainPanel.BorderStyle.Should().Be(BorderStyle.None);
        }

        [Test]
        public void HeaderPanel_ConfiguredCorrectly()
        {
            var headerPanel = FindControl<GradientPanel>(_dashboard, "headerPanel");
            headerPanel.Should().NotBeNull();
            headerPanel.Dock.Should().Be(DockStyle.Top);
            headerPanel.Size.Should().Be(new Size(1200, 80));
            headerPanel.BackgroundColor.Should().NotBeNull("Header panel should have background color configured");
        }

        [Test]
        public void ContentPanel_ConfiguredCorrectly()
        {
            var contentPanel = FindControl<GradientPanel>(_dashboard, "contentPanel");
            contentPanel.Should().NotBeNull();
            contentPanel.Dock.Should().Be(DockStyle.Fill);
            contentPanel.Size.Should().Be(new Size(1200, 720));
            contentPanel.Padding.Should().Be(new Padding(20));
        }

        [Test]
        public void Labels_ConfiguredCorrectly()
        {
            var titleLabel = FindControl<AutoLabel>(_dashboard, "titleLabel");
            titleLabel.Should().NotBeNull();
            titleLabel.Text.Should().Be("Bus Buddy Dashboard");
            titleLabel.Font.Size.Should().Be(18f);
            titleLabel.ForeColor.Should().Be(Color.White);

            var subtitleLabel = FindControl<AutoLabel>(_dashboard, "subtitleLabel");
            subtitleLabel.Should().NotBeNull();
            subtitleLabel.Text.Should().Contain("Fleet:");
            subtitleLabel.Font.Size.Should().Be(10f);
            subtitleLabel.ForeColor.Should().Be(Color.LightGray);
        }

        [Test]
        public void ManagementButtons_ConfiguredAndFunctional()
        {
            var buttons = new[]
            {
                ("busmanagementButton", "Bus Management"),
                ("drivermanagementButton", "Driver Management"),
                ("routemanagementButton", "Route Management"),
                ("schedulemanagementButton", "Schedule Management"),
                ("passengermanagementButton", "Passenger Management"),
                ("studentmanagementButton", "Student Management"),
                ("maintenanceButton", "Maintenance"),
                ("fueltrackingButton", "Fuel Tracking"),
                ("activitylogButton", "Activity Log"),
                ("ticketmanagementButton", "Ticket Management")
            };

            foreach (var (name, text) in buttons)
            {
                var button = FindControl<SfButton>(_dashboard, name);
                button.Should().NotBeNull($"{name} should exist");
                button.Text.Should().Be(text);
                button.BackColor.Should().Be(Color.FromArgb(63, 81, 181));
                button.ForeColor.Should().Be(Color.White);
                button.Font.Size.Should().Be(11f);
                button.Font.Bold.Should().BeTrue();
                button.Size.Should().Be(new Size(220, 120));
                button.Enabled.Should().BeTrue();
                button.Visible.Should().BeTrue();
                button.UseVisualStyleBackColor.Should().BeFalse();

                bool clicked = false;
                button.Click += (s, e) => clicked = true;
                button.PerformClick();
                clicked.Should().BeTrue($"{name} click event should trigger");
            }
        }

        [Test]
        public void ManagementButtons_ClickSequence_CaptureDialogs()
        {
            var buttonNames = new[]
            {
                "busmanagementButton",
                "drivermanagementButton",
                "routemanagementButton",
                "schedulemanagementButton",
                "passengermanagementButton",
                "studentmanagementButton",
                "maintenanceButton",
                "fueltrackingButton",
                "activitylogButton",
                "ticketmanagementButton"
            };

            foreach (var buttonName in buttonNames)
            {
                try
                {
                    var button = FindControl<SfButton>(_dashboard, buttonName);
                    if (button != null)
                    {
                        Console.WriteLine($"Clicking {buttonName}...");
                        button.PerformClick();

                        // Give time for any dialogs to appear
                        System.Threading.Thread.Sleep(500);

                        // Force application to process events
                        Application.DoEvents();
                        System.Threading.Thread.Sleep(200);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error clicking {buttonName}: {ex.Message}");
                }
            }

            // Additional wait to ensure all dialogs are captured
            System.Threading.Thread.Sleep(1000);
            Application.DoEvents();
        }

        [Test]
        public void QuickActionButtons_ConfiguredAndFunctional()
        {
            var buttons = new[]
            {
                ("refreshdataButton", "Refresh Data", Color.FromArgb(76, 175, 80)),
                ("reportsButton", "Reports", Color.FromArgb(156, 39, 176)),
                ("settingsButton", "Settings", Color.FromArgb(158, 158, 158))
            };

            foreach (var (name, text, color) in buttons)
            {
                var button = FindControl<SfButton>(_dashboard, name);
                button.Should().NotBeNull($"{name} should exist");
                button.Text.Should().Be(text);
                button.BackColor.Should().Be(color);
                button.ForeColor.Should().Be(Color.White);
                button.Font.Size.Should().Be(10f);
                button.Font.Bold.Should().BeTrue();
                button.Size.Should().Be(new Size(100, 40));
                button.Enabled.Should().BeTrue();
                button.Visible.Should().BeTrue();
                button.UseVisualStyleBackColor.Should().BeFalse();

                bool clicked = false;
                button.Click += (s, e) => clicked = true;
                button.PerformClick();
                clicked.Should().BeTrue($"{name} click event should trigger");
            }
        }

        [Test]
        public void HubTiles_ConfiguredCorrectly()
        {
            var summaryPanel = FindControl<GradientPanel>(_dashboard, "summaryPanel");
            var hubTiles = summaryPanel?.Controls.OfType<HubTile>().ToList();
            hubTiles.Should().NotBeNull();
            hubTiles.Should().HaveCount(5);

            var expectedTiles = new[]
            {
                ("Total Fleet", Color.FromArgb(63, 81, 181)),
                ("Active Routes", Color.FromArgb(76, 175, 80)),
                ("Active Buses", Color.FromArgb(255, 152, 0)),
                ("Maintenance", Color.FromArgb(244, 67, 54)),
                ("Total Capacity", Color.FromArgb(156, 39, 176))
            };

            foreach (var (title, color) in expectedTiles)
            {
                var tile = hubTiles.FirstOrDefault(t => t.Banner.Text == title);
                tile.Should().NotBeNull($"{title} tile should exist");
                tile.Size.Should().Be(new Size(180, 80));
                tile.BackColor.Should().Be(color);
                tile.Title.TextColor.Should().Be(Color.White);
                tile.Title.Font.Size.Should().Be(16f);
                tile.Body.TextColor.Should().Be(Color.WhiteSmoke);
                tile.Body.Font.Size.Should().Be(9f);
                tile.TileType.Should().Be(HubTileType.DefaultTile);
                tile.ImageTransitionSpeed.Should().Be(3000);
                tile.RotationTransitionSpeed.Should().Be(2000);
            }
        }

        [Test]
        public void SummaryPanel_ConfiguredCorrectly()
        {
            var summaryPanel = FindControl<GradientPanel>(_dashboard, "summaryPanel");
            summaryPanel.Should().NotBeNull();
            summaryPanel.Size.Should().Be(new Size(420, 420));
            summaryPanel.BackgroundColor.Should().NotBeNull("Summary panel should have background color configured");
            summaryPanel.BorderStyle.Should().Be(BorderStyle.FixedSingle);

            var summaryTitle = summaryPanel.Controls.OfType<AutoLabel>().FirstOrDefault(l => l.Text == "Fleet Summary");
            summaryTitle.Should().NotBeNull();
            summaryTitle.Font.Size.Should().Be(14f);
            summaryTitle.ForeColor.Should().Be(Color.FromArgb(46, 125, 185));

            var statsLabel = summaryPanel.Controls.OfType<AutoLabel>().FirstOrDefault(l => l.Text.Contains("Fleet Utilization"));
            statsLabel.Should().NotBeNull();
            statsLabel.Font.Size.Should().Be(9f);
            statsLabel.ForeColor.Should().Be(Color.FromArgb(95, 99, 104));
        }

        [Test]
        public void Dashboard_DataLoading_UpdatesUI()
        {
            _busServiceMock.Verify(s => s.GetAllBusesAsync(), Times.Once());
            _busServiceMock.Verify(s => s.GetAllRoutesAsync(), Times.Once());

            var subtitleLabel = FindControl<AutoLabel>(_dashboard, "subtitleLabel");
            subtitleLabel.Text.Should().Contain("Fleet: 3 buses | Routes: 2 | Active: 1 | Total Capacity: 120");
        }

        [Test]
        public void HighDpi_Rendering_Correct()
        {
            _dashboard.AutoScaleMode.Should().Be(AutoScaleMode.Dpi);
            var controls = GetAllControlsRecursively(_dashboard);
            foreach (var control in controls.Where(c => c.Visible))
            {
                control.Size.Width.Should().BeGreaterThan(0, $"{control.Name} width invalid");
                control.Size.Height.Should().BeGreaterThan(0, $"{control.Name} height invalid");
                control.Font.Size.Should().BeGreaterThan(0, $"{control.Name} font size invalid");
            }
        }

        private T FindControl<T>(Control parent, string name) where T : Control
        {
            return parent.Controls.Find(name, true).OfType<T>().FirstOrDefault() ?? parent.Controls.OfType<T>().FirstOrDefault();
        }

        private System.Collections.Generic.List<Control> GetAllControlsRecursively(Control parent)
        {
            var result = new System.Collections.Generic.List<Control>();
            foreach (Control control in parent.Controls)
            {
                result.Add(control);
                result.AddRange(GetAllControlsRecursively(control));
            }
            return result;
        }

        /*[Test]
        public void SyncfusionLicensing_ShouldBeProperlyRegistered()
        {
            // Arrange & Act
            SyncfusionLicenseProvider.RegisterLicense(syncfusionLicenseKey);

            // Assert - No exception should be thrown
            Assert.Pass("Syncfusion licensing registered successfully");
        }

        [Test]
        public void MetroForm_ShouldCreateAndConfigureProperly()
        {
            // Arrange & Act
            using var metroForm = new MetroForm();

            // Assert
            metroForm.Should().NotBeNull();
            metroForm.GetType().Name.Should().Be("MetroForm");
            metroForm.Size.Should().NotBe(System.Drawing.Size.Empty);
        }

        [Test]
        public void SfDataGrid_ShouldCreateWithBasicProperties()
        {
            // Arrange & Act
            using var dataGrid = new SfDataGrid();

            // Assert
            dataGrid.Should().NotBeNull();
            dataGrid.GetType().Name.Should().Be("SfDataGrid");
            dataGrid.AllowEditing.Should().BeFalse(); // Default value
            dataGrid.AllowSorting.Should().BeTrue(); // Default value
        }

        [Test]
        public void GradientPanel_ShouldCreateWithBasicConfiguration()
        {
            // Arrange & Act
            using var gradientPanel = new GradientPanel();

            // Assert
            gradientPanel.Should().NotBeNull();
            gradientPanel.GetType().Name.Should().Be("GradientPanel");
            gradientPanel.BackgroundColor.Should().NotBe(System.Drawing.Color.Empty);
        }

        [Test]
        public void ButtonAdv_ShouldCreateWithBasicFunctionality()
        {
            // Arrange & Act
            using var buttonAdv = new ButtonAdv();

            // Assert
            buttonAdv.Should().NotBeNull();
            buttonAdv.GetType().Name.Should().Be("ButtonAdv");
            buttonAdv.Text.Should().Be("");
            buttonAdv.UseVisualStyle.Should().BeTrue(); // Default for ButtonAdv
        }

        [Test]
        public void MetroForm_WithDataGrid_ShouldIntegrateProperly()
        {
            // Arrange
            using var metroForm = new MetroForm();
            using var dataGrid = new SfDataGrid();

            // Act
            metroForm.Controls.Add(dataGrid);
            dataGrid.Dock = DockStyle.Fill;

            // Assert
            metroForm.Controls.Count.Should().Be(1);
            metroForm.Controls[0].Should().Be(dataGrid);
            dataGrid.Parent.Should().Be(metroForm);
            dataGrid.Dock.Should().Be(DockStyle.Fill);
        }

        [Test]
        public void SyncfusionControls_ShouldSupportBasicEventHandling()
        {
            // Arrange
            using var buttonAdv = new ButtonAdv();
            var clickEventFired = false;
            buttonAdv.Click += (s, e) => clickEventFired = true;

            // Act
            buttonAdv.PerformClick();

            // Assert
            clickEventFired.Should().BeTrue();
        }

        [Test]
        public void MetroForm_ShouldSupportBasicHighDpiSettings()
        {
            // Arrange & Act
            using var metroForm = new MetroForm();
            metroForm.AutoScaleMode = AutoScaleMode.Dpi;

            // Assert
            metroForm.AutoScaleMode.Should().Be(AutoScaleMode.Dpi);
        }

        [Test]
        public void SfDataGrid_ShouldSupportBasicDataBinding()
        {
            // Arrange
            using var dataGrid = new SfDataGrid();
            var testData = new[]
            {
                new { Id = 1, Name = "Test Bus 1", Route = "Route A" },
                new { Id = 2, Name = "Test Bus 2", Route = "Route B" }
            };

            // Act
            dataGrid.DataSource = testData;

            // Assert
            dataGrid.DataSource.Should().NotBeNull();
            dataGrid.DataSource.Should().Be(testData);
        }

        [Test]
        public void GradientPanel_ShouldSupportNestedControls()
        {
            // Arrange
            using var gradientPanel = new GradientPanel();
            using var buttonAdv = new ButtonAdv();

            // Act
            gradientPanel.Controls.Add(buttonAdv);

            // Assert
            gradientPanel.Controls.Count.Should().Be(1);
            gradientPanel.Controls[0].Should().Be(buttonAdv);
            buttonAdv.Parent.Should().Be(gradientPanel);
        }

        [Test]
        public void SyncfusionControls_ShouldWorkInComplexLayout()
        {
            // Arrange
            using var metroForm = new MetroForm();
            using var gradientPanel = new GradientPanel();
            using var dataGrid = new SfDataGrid();
            using var buttonAdv = new ButtonAdv();

            // Act - Create a typical form layout
            gradientPanel.Dock = DockStyle.Top;
            gradientPanel.Height = 50;
            buttonAdv.Text = "Test Button";
            gradientPanel.Controls.Add(buttonAdv);

            dataGrid.Dock = DockStyle.Fill;

            metroForm.Controls.Add(gradientPanel);
            metroForm.Controls.Add(dataGrid);

            // Assert
            metroForm.Controls.Count.Should().Be(2);
            metroForm.Controls[0].Should().Be(gradientPanel);
            metroForm.Controls[1].Should().Be(dataGrid);
            gradientPanel.Controls.Count.Should().Be(1);
            gradientPanel.Controls[0].Should().Be(buttonAdv);
            buttonAdv.Text.Should().Be("Test Button");
        }

        [Test]
        public void SyncfusionBackgroundFix_ShouldBeAvailable()
        {
            // Arrange & Act - Test that our background fix utility exists
            var backgroundFixType = Type.GetType("Bus_Buddy.Utilities.SyncfusionBackgroundFix, Bus Buddy");

            // Assert
            backgroundFixType.Should().NotBeNull("SyncfusionBackgroundFix utility should be available");
            backgroundFixType?.IsClass.Should().BeTrue();
        }*/
    }
}
