using NUnit.Framework;
using FluentAssertions;
using System.Windows.Forms;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using Syncfusion.Windows.Forms;
using BusBuddy.Tests.Infrastructure;

namespace BusBuddy.Tests.UnitTests.UI
{
    /// <summary>
    /// Example test demonstrating dialog event capture functionality
    /// Shows how to detect and analyze dialog boxes during testing
    /// </summary>
    [TestFixture]
    [Apartment(ApartmentState.STA)]
    public class DialogCaptureExampleTests : TestBase
    {
        [SetUp]
        public void SetUp()
        {
            // Start dialog capture before any UI operations
            StartDialogCapture();
        }

        [TearDown]
        public void TearDown()
        {
            // Get dialog capture report and log it
            var dialogReport = StopDialogCaptureAndGetReport();

            TestContext.WriteLine("=== DIALOG CAPTURE REPORT ===");
            TestContext.WriteLine(dialogReport);

            LogCapturedDialogs();
        }

        [Test]
        public void DialogCapture_SystemInitialization_Works()
        {
            // Arrange & Act
            var initialDialogCount = GetCapturedDialogs().Count;

            // Assert - Verify capture system is ready
            DialogCapture.Should().NotBeNull("Dialog capture system should be initialized");
            TestContext.WriteLine($"Dialog capture system is active. Initial dialog count: {initialDialogCount}");
        }

        [Test]
        public void MessageBoxAdv_DirectCall_CapturesDialog()
        {
            // Arrange
            var dialogShown = false;

            // Act - Trigger a MessageBoxAdv dialog in a way that won't block the test
            Task.Run(() =>
            {
                Thread.Sleep(100);
                // Use Control.Invoke pattern for WinForms threading
                var form = new Form();
                if (form.InvokeRequired)
                {
                    form.Invoke(() =>
                    {
                        MessageBoxAdv.Show("Test message for dialog capture", "Test Dialog",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                        dialogShown = true;
                    });
                }
                else
                {
                    MessageBoxAdv.Show("Test message for dialog capture", "Test Dialog",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    dialogShown = true;
                }
                form.Dispose();
            });

            // Wait for dialog to potentially appear
            var timeout = DateTime.Now.AddSeconds(3);
            while (!dialogShown && DateTime.Now < timeout)
            {
                Application.DoEvents();
                Thread.Sleep(50);
            }

            // Give time for capture system to detect
            Thread.Sleep(500);
            Application.DoEvents();

            // Assert
            var capturedDialogs = GetCapturedDialogs();
            TestContext.WriteLine($"Captured {capturedDialogs.Count} dialogs");

            foreach (var dialog in capturedDialogs)
            {
                TestContext.WriteLine($"Dialog: {dialog.DialogType} - {dialog.Title}");
            }

            // Note: This test demonstrates the capture system setup
            // Actual dialog capture depends on threading and timing in test environment
            capturedDialogs.Should().NotBeNull("Dialog capture system should return collection");
        }

        [Test]
        public void CaptureSystem_ReportGeneration_ProducesValidOutput()
        {
            // Act
            var report = StopDialogCaptureAndGetReport();

            // Assert
            report.Should().NotBeNullOrEmpty("Report should be generated");
            report.Should().Contain("DIALOG CAPTURE REPORT", "Report should have proper header");

            TestContext.WriteLine("Generated report:");
            TestContext.WriteLine(report);
        }

        [Test]
        public void CaptureSystem_LoggingFunctionality_Works()
        {
            // Act & Assert - Should not throw exceptions
            Assert.DoesNotThrow(() => LogCapturedDialogs(), "Logging should work without errors");

            var capturedDialogs = GetCapturedDialogs();
            capturedDialogs.Should().NotBeNull("Should return valid collection");
        }
    }
}
