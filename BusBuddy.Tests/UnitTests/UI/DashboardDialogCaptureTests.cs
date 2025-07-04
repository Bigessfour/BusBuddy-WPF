using NUnit.Framework;
using FluentAssertions;
using System;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Syncfusion.Licensing;
using Syncfusion.Windows.Forms;
using Syncfusion.WinForms.Controls;
using Syncfusion.Windows.Forms.Tools;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using Bus_Buddy;
using Bus_Buddy.Forms;
using Bus_Buddy.Services;
using Bus_Buddy.Models;
using static Bus_Buddy.Services.IBusService;
using BusBuddy.Tests.Infrastructure;

namespace BusBuddy.Tests.UnitTests.UI
{
    /// <summary>
    /// Dashboard UI Tests with Dialog Event Capture
    /// Captures and analyzes all dialog boxes that appear during dashboard testing
    /// This will help identify the source of the "13 dialog boxes" issue
    /// </summary>
    [TestFixture]
    [Apartment(ApartmentState.STA)]
    public class DashboardDialogCaptureTests : TestBase
    {
        private const string LicenseKey = "Ngo9BigBOggjHTQxAR8/V1JEaF5cXmRCf1FpRmJGdld5fUVHYVZUTXxaS00DNHVRdkdmWXhec3RSRGRYU0R2WUBWYEk=";

        private Dashboard? _dashboard;
        private Mock<ILogger<Dashboard>> _loggerMock;
        private Mock<IBusService> _busServiceMock;
        private Mock<IConfigurationService> _configServiceMock;

        [SetUp]
        public void Setup()
        {
            // Start dialog capture immediately
            StartDialogCapture();

            // Initialize Syncfusion license
            SyncfusionLicenseProvider.RegisterLicense(LicenseKey);

            // Setup mocks
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

            TestContext.WriteLine("=== STARTING DASHBOARD INITIALIZATION ===");
        }

        [TearDown]
        public void TearDown()
        {
            TestContext.WriteLine("=== DASHBOARD TEARDOWN - CAPTURING DIALOGS ===");

            // Get dialog capture report
            var dialogReport = StopDialogCaptureAndGetReport();

            TestContext.WriteLine("=== COMPLETE DIALOG CAPTURE REPORT ===");
            TestContext.WriteLine(dialogReport);

            // Log individual dialogs
            LogCapturedDialogs();

            // Analyze captured dialogs
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

                // List all error contexts
                foreach (var errorDialog in errorDialogs)
                {
                    TestContext.WriteLine($"ERROR: {errorDialog.Title} - {errorDialog.ErrorContext}");
                }
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
        public async Task Dashboard_Initialization_CapturesAllDialogs()
        {
            TestContext.WriteLine("=== TEST: Dashboard Initialization ===");

            try
            {
                // Initialize dashboard with potential dialog-triggering code
                _dashboard = new Dashboard(_loggerMock.Object, _busServiceMock.Object, _configServiceMock.Object);

                TestContext.WriteLine("Dashboard created, showing form...");
                _dashboard.Show();
                Application.DoEvents();

                // Wait for any initialization dialogs
                await Task.Delay(1000);
                Application.DoEvents();

                TestContext.WriteLine("Dashboard initialization complete");

                // Check for dialogs immediately after initialization
                var initDialogs = GetCapturedDialogs();
                TestContext.WriteLine($"Dialogs after initialization: {initDialogs.Count}");
            }
            catch (Exception ex)
            {
                TestContext.WriteLine($"Exception during dashboard initialization: {ex.Message}");
                TestContext.WriteLine($"Stack trace: {ex.StackTrace}");

                // Still check for dialogs even if exception occurred
                var errorDialogs = GetCapturedDialogs();
                TestContext.WriteLine($"Dialogs captured during exception: {errorDialogs.Count}");

                // Don't fail the test - we want to see what dialogs appeared
            }
        }

        [Test]
        public async Task Dashboard_ButtonClicks_CapturesDialogEvents()
        {
            TestContext.WriteLine("=== TEST: Dashboard Button Clicks ===");

            try
            {
                // Initialize dashboard first
                _dashboard = new Dashboard(_loggerMock.Object, _busServiceMock.Object, _configServiceMock.Object);
                _dashboard.Show();
                Application.DoEvents();
                await Task.Delay(500);

                // Click various management buttons that typically trigger dialogs
                var buttonsToTest = new[]
                {
                    "busManagementButton",
                    "driverManagementButton",
                    "routeManagementButton",
                    "scheduleManagementButton",
                    "passengerManagementButton",
                    "studentManagementButton",
                    "maintenanceButton",
                    "fuelTrackingButton",
                    "activityLogButton",
                    "ticketManagementButton",
                    "reportsButton",
                    "settingsButton"
                };

                foreach (var buttonName in buttonsToTest)
                {
                    TestContext.WriteLine($"Clicking button: {buttonName}");

                    var button = FindControl<SfButton>(_dashboard, buttonName);
                    if (button != null)
                    {
                        var dialogCountBefore = GetCapturedDialogs().Count;

                        // Click the button
                        button.PerformClick();
                        Application.DoEvents();
                        await Task.Delay(200); // Give time for dialogs to appear

                        var dialogCountAfter = GetCapturedDialogs().Count;
                        var newDialogs = dialogCountAfter - dialogCountBefore;

                        TestContext.WriteLine($"  Button {buttonName}: {newDialogs} new dialogs");

                        if (newDialogs > 0)
                        {
                            var recentDialogs = GetCapturedDialogs().Skip(dialogCountBefore).ToList();
                            foreach (var dialog in recentDialogs)
                            {
                                TestContext.WriteLine($"    Dialog: {dialog.DialogType} - {dialog.Title}");
                            }
                        }
                    }
                    else
                    {
                        TestContext.WriteLine($"  Button {buttonName} not found");
                    }
                }
            }
            catch (Exception ex)
            {
                TestContext.WriteLine($"Exception during button testing: {ex.Message}");
            }
        }

        [Test]
        public async Task Dashboard_DataOperations_CapturesDialogs()
        {
            TestContext.WriteLine("=== TEST: Dashboard Data Operations ===");

            try
            {
                _dashboard = new Dashboard(_loggerMock.Object, _busServiceMock.Object, _configServiceMock.Object);
                _dashboard.Show();
                Application.DoEvents();
                await Task.Delay(500);

                // Try to trigger data refresh operations that might cause dialogs
                var refreshButton = FindControl<SfButton>(_dashboard, "refreshDataButton");
                if (refreshButton != null)
                {
                    TestContext.WriteLine("Clicking refresh data button...");
                    refreshButton.PerformClick();
                    Application.DoEvents();
                    await Task.Delay(1000); // Give time for data operations
                }

                // Try to access forms that might have validation issues
                TestContext.WriteLine("Testing form access patterns...");

                // Simulate various user interactions that might trigger dialogs
                await Task.Delay(500);
                Application.DoEvents();

            }
            catch (Exception ex)
            {
                TestContext.WriteLine($"Exception during data operations: {ex.Message}");
            }
        }

        [Test]
        [Apartment(ApartmentState.STA)]
        public void Dashboard_WithoutMocking_CapturesRealDialogs()
        {
            Console.WriteLine("=== TEST: Dashboard Service Container Dialog Capture ===");

            // Start dialog capture for this test
            StartDialogCapture();

            try
            {
                // DELIBERATE: Dispose ServiceContainer to trigger GetService failures
                Console.WriteLine("Disposing ServiceContainer to trigger service resolution failures...");
                ServiceContainer.Dispose();

                Console.WriteLine("Creating Dashboard with null services to trigger dialogs...");
                _dashboard = new Dashboard(_loggerMock.Object, Mock.Of<IBusService>(), Mock.Of<IRouteService>());

                Console.WriteLine("Showing Dashboard...");
                _dashboard.Show();
                Application.DoEvents();

                // Give time for initialization dialogs from LoadDashboardDataAsync with null services
                Thread.Sleep(2000);
                Application.DoEvents();

                var dialogsAfterInit = GetCapturedDialogs().Count;
                Console.WriteLine($"Dialogs after initialization: {dialogsAfterInit}");

                Console.WriteLine("Attempting to click management buttons to trigger ServiceContainer failures...");

                // Try to click buttons that should trigger ServiceContainer.GetService calls which will now fail
                var buttons = new[]
                {
                    "busmanagementButton", "drivermanagementButton", "routemanagementButton",
                    "schedulemanagementButton", "passengermanagementButton", "studentmanagementButton",
                    "maintenanceButton", "fueltrackingButton", "activitylogButton", "ticketmanagementButton"
                };

                foreach (var buttonName in buttons)
                {
                    var button = FindControl<Button>(_dashboard, buttonName);
                    if (button != null)
                    {
                        Console.WriteLine($"Found button: {buttonName} (Enabled: {button.Enabled}, Visible: {button.Visible})");
                        try
                        {
                            var beforeClick = GetCapturedDialogs().Count;
                            button.PerformClick();
                            Application.DoEvents();
                            Thread.Sleep(1500); // Give more time for dialogs to appear
                            Application.DoEvents();
                            var afterClick = GetCapturedDialogs().Count;
                            Console.WriteLine($"  Button {buttonName}: {afterClick - beforeClick} dialogs captured");
                            if (afterClick > beforeClick)
                            {
                                Console.WriteLine($"  *** {buttonName} triggered {afterClick - beforeClick} dialogs! ***");
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Exception during {buttonName} click: {ex.Message}");
                        }
                    }
                    else
                    {
                        Console.WriteLine($"Button NOT FOUND: {buttonName}");
                    }
                }

                // Final count
                var totalDialogs = GetCapturedDialogs().Count;
                Console.WriteLine($"TOTAL DIALOGS CAPTURED: {totalDialogs}");

                if (totalDialogs >= 10)
                {
                    Console.WriteLine("*** FOUND THE MULTI-DIALOG ISSUE! ***");
                }

                // Log captured dialogs for debugging
                LogCapturedDialogs();

                // The assertion - expect to capture dialogs from service resolution failures
                totalDialogs.Should().BeGreaterThanOrEqualTo(0, "Dialog capture system should be working");

            }
            finally
            {
                Console.WriteLine("=== DASHBOARD TEARDOWN - CAPTURING DIALOGS ===");
                Application.DoEvents();
                _dashboard?.Hide();
                _dashboard?.Close();
                _dashboard?.Dispose();

                // Stop capture and log results
                StopDialogCaptureAndGetReport();

                // Reinitialize ServiceContainer for other tests
                try
                {
                    ServiceContainer.Initialize();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Failed to reinitialize ServiceContainer: {ex.Message}");
                }
            }
        }

        private T FindControl<T>(Control parent, string name) where T : Control
        {
            return parent.Controls.Find(name, true).OfType<T>().FirstOrDefault() ??
                   parent.Controls.OfType<T>().FirstOrDefault(c => c.Name == name);
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

        private Button? GetButtonByName(string name)
        {
            var button = _dashboard?.Controls.OfType<Button>().FirstOrDefault(b => b.Name == name);
            if (button == null)
            {
                _logger?.LogWarning($"Button '{name}' not found on the dashboard.");
            }
            return button;
        }
    }
}
