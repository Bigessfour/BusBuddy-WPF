using NUnit.Framework;
using FluentAssertions;
using System.Drawing;
using System.Windows.Forms;
using Syncfusion.Windows.Forms;
using Syncfusion.Windows.Forms.Tools;
using Bus_Buddy.Utilities;

namespace BusBuddy.Tests.UnitTests.UI
{
    /// <summary>
    /// Tests for Syncfusion Background Bleeding Fix
    /// Validates that the exact issue you described is resolved
    /// </summary>
    [TestFixture]
    public class SyncfusionBackgroundFixTests
    {
        private MetroForm _testForm = null!;

        [SetUp]
        public void SetUp()
        {
            // Create test form similar to Dashboard
            _testForm = new MetroForm
            {
                Size = new Size(800, 600),
                BackColor = Color.Transparent // Start with problematic transparent background
            };

            // Add a main panel like Dashboard
            var mainPanel = new GradientPanel
            {
                Dock = DockStyle.Fill,
                Name = "mainPanel"
            };

            // Add content panel like Dashboard
            var contentPanel = new GradientPanel
            {
                Name = "contentPanel",
                Size = new Size(600, 400),
                Location = new Point(50, 100)
            };

            _testForm.Controls.Add(mainPanel);
            mainPanel.Controls.Add(contentPanel);
        }

        [TearDown]
        public void TearDown()
        {
            _testForm?.Dispose();
        }

        [Test]
        public void FixDashboardBackground_ShouldEliminateTransparentBackground()
        {
            // Arrange
            _testForm.BackColor.Should().Be(Color.Transparent); // Verify problem exists

            // Act
            SyncfusionBackgroundFix.FixDashboardBackground(_testForm);

            // Assert
            _testForm.BackColor.Should().NotBe(Color.Transparent);
            _testForm.BackColor.Should().Be(Color.FromArgb(240, 246, 252));
        }

        [Test]
        public void FixDashboardBackground_ShouldConfigureMetroFormProperties()
        {
            // Act
            SyncfusionBackgroundFix.FixDashboardBackground(_testForm);

            // Assert
            _testForm.MetroColor.Should().Be(Color.FromArgb(11, 95, 178));
            _testForm.BorderColor.Should().Be(Color.FromArgb(11, 95, 178));
            _testForm.CaptionBarColor.Should().Be(Color.FromArgb(11, 95, 178));
            _testForm.CaptionForeColor.Should().Be(Color.White);
        }

        [Test]
        public void FixDashboardBackground_ShouldConfigureMainPanelBackground()
        {
            // Arrange
            var mainPanel = _testForm.Controls[0] as GradientPanel;
            mainPanel!.BackgroundColor.IsEmpty.Should().BeTrue(); // Verify problem exists

            // Act
            SyncfusionBackgroundFix.FixDashboardBackground(_testForm);

            // Assert
            mainPanel.BackgroundColor.Should().NotBeNull();
            mainPanel.BorderStyle.Should().Be(BorderStyle.None);
        }

        [Test]
        public void FixDashboardBackground_ShouldConfigureContentPanelBackground()
        {
            // Arrange
            var mainPanel = _testForm.Controls[0] as GradientPanel;
            var contentPanel = mainPanel!.Controls[0] as GradientPanel;

            // Act
            SyncfusionBackgroundFix.FixDashboardBackground(_testForm);

            // Assert
            contentPanel!.BackgroundColor.Should().NotBeNull();
            contentPanel.BorderStyle.Should().Be(BorderStyle.None);
            contentPanel.Padding.All.Should().Be(20); // Should add padding to prevent edge bleeding
        }

        [Test]
        public void ValidateBackgroundFix_ShouldReturnTrue_WhenFixApplied()
        {
            // Act
            SyncfusionBackgroundFix.FixDashboardBackground(_testForm);
            var isValid = SyncfusionBackgroundFix.ValidateBackgroundFix(_testForm);

            // Assert
            isValid.Should().BeTrue();
        }

        [Test]
        public void ValidateBackgroundFix_ShouldReturnFalse_WhenFormBackgroundTransparent()
        {
            // Arrange
            _testForm.BackColor = Color.Transparent;

            // Act
            var isValid = SyncfusionBackgroundFix.ValidateBackgroundFix(_testForm);

            // Assert
            isValid.Should().BeFalse();
        }

        [Test]
        public void FixButtonBackgrounds_ShouldEnsureButtonsHaveSolidColors()
        {
            // Arrange
            var button = new Syncfusion.WinForms.Controls.SfButton
            {
                Text = "Bus Management"
            };
            button.Style.BackColor = Color.Transparent; // Problematic transparent background
            _testForm.Controls.Add(button);

            // Act
            SyncfusionBackgroundFix.FixButtonBackgrounds(_testForm);

            // Assert
            button.Style.BackColor.Should().NotBe(Color.Transparent);
            button.Style.BackColor.A.Should().Be(255); // Should be fully opaque
            button.Style.BackColor.Should().Be(Color.FromArgb(63, 81, 181)); // Blue for management buttons
        }

        [Test]
        public void FixButtonBackgrounds_ShouldApplyCorrectColorsBasedOnButtonText()
        {
            // Arrange
            var managementButton = new Syncfusion.WinForms.Controls.SfButton { Text = "Bus Management" };
            var refreshButton = new Syncfusion.WinForms.Controls.SfButton { Text = "Refresh Data" };
            var reportsButton = new Syncfusion.WinForms.Controls.SfButton { Text = "Reports" };

            managementButton.Style.BackColor = Color.Transparent;
            refreshButton.Style.BackColor = Color.Transparent;
            reportsButton.Style.BackColor = Color.Transparent;

            _testForm.Controls.AddRange(new Control[] { managementButton, refreshButton, reportsButton });

            // Act
            SyncfusionBackgroundFix.FixButtonBackgrounds(_testForm);

            // Assert
            managementButton.Style.BackColor.Should().Be(Color.FromArgb(63, 81, 181)); // Blue for management
            refreshButton.Style.BackColor.Should().Be(Color.FromArgb(76, 175, 80)); // Green for refresh
            reportsButton.Style.BackColor.Should().Be(Color.FromArgb(156, 39, 176)); // Purple for reports
        }
    }
}
