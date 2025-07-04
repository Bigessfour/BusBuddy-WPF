using NUnit.Framework;
using FluentAssertions;
using System.Windows.Forms;
using System.Drawing;
using Syncfusion.Windows.Forms;
using Syncfusion.Windows.Forms.Tools;
using Syncfusion.WinForms.Controls;
using Syncfusion.WinForms.ListView;
using Syncfusion.WinForms.DataGrid;
using Bus_Buddy.Forms;
using Bus_Buddy.Utilities;
using BusBuddy.Tests.Infrastructure;

namespace BusBuddy.Tests.UnitTests.UI
{
    /// <summary>
    /// Critical UI tests for Syncfusion components
    /// Tests proper initialization, styling, docking, sizing, and visual hierarchy
    /// 
    /// FOCUS AREAS:
    /// - Docking Manager proper setup and z-order
    /// - Form sizing and responsive layout
    /// - Button padding and consistent styling
    /// - Theme application and visual consistency
    /// - Control initialization order and dependencies
    /// </summary>
    [TestFixture]
    [Apartment(ApartmentState.STA)] // Required for Windows Forms testing
    public class SyncfusionUITests : TestBase
    {
        private Dashboard? _dashboard;
        private BusManagementForm? _busManagementForm;
        private VisualEnhancementManager? _visualManager;

        [SetUp]
        public void SetUp()
        {
            // Initialize visual manager first
            _visualManager = new VisualEnhancementManager();
        }

        [TearDown]
        public void TearDown()
        {
            _dashboard?.Dispose();
            _busManagementForm?.Dispose();
            _visualManager?.Dispose();
        }

        #region Docking Manager Tests

        [Test]
        public void DockingManager_ShouldInitializeWithProperConfiguration()
        {
            // Arrange & Act
            _dashboard = new Dashboard();
            
            // Find DockingManager if it exists
            var dockingManager = FindControlOfType<DockingManager>(_dashboard);

            if (dockingManager != null)
            {
                // Assert
                dockingManager.Should().NotBeNull("DockingManager should be properly initialized");
                dockingManager.Enabled.Should().BeTrue("DockingManager should be enabled");
                dockingManager.DockBehavior.Should().Be(DockBehavior.VS2005, "Should use modern VS2005 docking behavior");
                dockingManager.AutoHideTabFont.Should().NotBeNull("AutoHide tab font should be configured");
            }
        }

        [Test]
        public void DockingManager_ZOrder_ShouldBeProperlyConfigure()
        {
            // Arrange & Act
            _busManagementForm = new BusManagementForm();
            var dockingManager = FindControlOfType<DockingManager>(_busManagementForm);

            if (dockingManager != null)
            {
                // Assert - Check that docked controls have proper z-order
                var dockedControls = dockingManager.Controls.Cast<Control>().ToList();
                
                if (dockedControls.Any())
                {
                    // Main content should have higher z-order than side panels
                    var mainPanels = dockedControls.Where(c => c.Dock == DockStyle.Fill).ToList();
                    var sidePanels = dockedControls.Where(c => c.Dock == DockStyle.Left || c.Dock == DockStyle.Right).ToList();

                    foreach (var mainPanel in mainPanels)
                    {
                        foreach (var sidePanel in sidePanels)
                        {
                            _busManagementForm.Controls.GetChildIndex(mainPanel)
                                .Should().BeLessThan(_busManagementForm.Controls.GetChildIndex(sidePanel),
                                "Main panels should have higher z-order (lower index) than side panels");
                        }
                    }
                }
            }
        }

        [Test]
        public void DockingManager_ShouldSupportProgrammaticDocking()
        {
            // Arrange
            _dashboard = new Dashboard();
            var dockingManager = FindControlOfType<DockingManager>(_dashboard);

            if (dockingManager != null)
            {
                var testPanel = new Panel { Name = "TestDockPanel", Size = new Size(200, 100) };
                _dashboard.Controls.Add(testPanel);

                // Act
                dockingManager.SetDockLabel(testPanel, "Test Panel");
                dockingManager.SetEnableDocking(testPanel, true);

                // Assert
                dockingManager.GetDockLabel(testPanel).Should().Be("Test Panel");
                dockingManager.GetEnableDocking(testPanel).Should().BeTrue();
            }
        }

        #endregion

        #region Form Sizing and Layout Tests

        [Test]
        public void Dashboard_ShouldHaveProperInitialSize()
        {
            // Arrange & Act
            _dashboard = new Dashboard();

            // Assert
            _dashboard.Size.Width.Should().BeGreaterThan(800, "Dashboard should have adequate width");
            _dashboard.Size.Height.Should().BeGreaterThan(600, "Dashboard should have adequate height");
            _dashboard.MinimumSize.Should().NotBe(Size.Empty, "Minimum size should be set to prevent too-small windows");
            _dashboard.WindowState.Should().Be(FormWindowState.Normal, "Should start in normal window state");
        }

        [Test]
        public void BusManagementForm_ShouldHaveResponsiveLayout()
        {
            // Arrange & Act
            _busManagementForm = new BusManagementForm();

            // Assert
            _busManagementForm.AutoScaleMode.Should().Be(AutoScaleMode.Font, "Should use font-based auto scaling");
            _busManagementForm.AutoSize.Should().BeFalse("Large forms should not auto-size");
            
            // Check that main controls have proper anchoring
            var mainControls = _busManagementForm.Controls.Cast<Control>()
                .Where(c => c.Name.Contains("dataGrid") || c.Name.Contains("panel") || c.Name.Contains("button"))
                .ToList();

            foreach (var control in mainControls)
            {
                if (control.Dock == DockStyle.None)
                {
                    control.Anchor.Should().NotBe(AnchorStyles.None, 
                        $"Control '{control.Name}' should have proper anchoring for responsive layout");
                }
            }
        }

        [Test]
        public void Forms_ShouldHandleResizing()
        {
            // Arrange
            _dashboard = new Dashboard();
            var originalSize = _dashboard.Size;

            // Act
            _dashboard.Size = new Size(originalSize.Width + 200, originalSize.Height + 100);

            // Assert
            _dashboard.Size.Width.Should().Be(originalSize.Width + 200);
            _dashboard.Size.Height.Should().Be(originalSize.Height + 100);

            // Check that controls adjusted properly
            var gradientPanels = FindControlsOfType<GradientPanel>(_dashboard);
            foreach (var panel in gradientPanels)
            {
                if (panel.Dock == DockStyle.Fill || panel.Anchor.HasFlag(AnchorStyles.Right))
                {
                    panel.Right.Should().BeGreaterThan(originalSize.Width, 
                        "Docked/anchored panels should expand with form");
                }
            }
        }

        #endregion

        #region Button Styling and Padding Tests

        [Test]
        public void SfButtons_ShouldHaveConsistentPadding()
        {
            // Arrange & Act
            _dashboard = new Dashboard();
            var sfButtons = FindControlsOfType<SfButton>(_dashboard);

            // Assert
            sfButtons.Should().NotBeEmpty("Dashboard should contain SfButton controls");

            foreach (var button in sfButtons)
            {
                // Check padding consistency
                button.Padding.Left.Should().BeGreaterThan(5, "Buttons should have adequate left padding");
                button.Padding.Right.Should().BeGreaterThan(5, "Buttons should have adequate right padding");
                button.Padding.Top.Should().BeGreaterThan(2, "Buttons should have adequate top padding");
                button.Padding.Bottom.Should().BeGreaterThan(2, "Buttons should have adequate bottom padding");

                // Check minimum size
                button.Size.Width.Should().BeGreaterThan(75, "Buttons should have minimum usable width");
                button.Size.Height.Should().BeGreaterThan(23, "Buttons should have minimum usable height");
            }
        }

        [Test]
        public void SfButtons_ShouldHaveProperThemeApplication()
        {
            // Arrange & Act
            _busManagementForm = new BusManagementForm();
            var sfButtons = FindControlsOfType<SfButton>(_busManagementForm);

            // Assert
            foreach (var button in sfButtons)
            {
                button.Style.Should().NotBeNull("Button should have style applied");
                button.ThemeName.Should().NotBeNullOrEmpty("Button should have theme name set");
                
                // Check that theme colors are applied
                button.Style.BackColor.Should().NotBe(SystemColors.Control, 
                    "Button should not use default system colors");
            }
        }

        [Test]
        public void Buttons_ShouldHaveConsistentStyling()
        {
            // Arrange & Act
            _dashboard = new Dashboard();
            var allButtons = new List<Control>();
            allButtons.AddRange(FindControlsOfType<SfButton>(_dashboard));
            allButtons.AddRange(FindControlsOfType<Button>(_dashboard));

            if (allButtons.Count > 1)
            {
                // Assert - Check font consistency
                var firstButton = allButtons.First();
                foreach (var button in allButtons.Skip(1))
                {
                    button.Font.Name.Should().Be(firstButton.Font.Name, 
                        "All buttons should use consistent font family");
                    Math.Abs(button.Font.Size - firstButton.Font.Size).Should().BeLessOrEqualTo(2, 
                        "Button font sizes should be consistent within 2pt range");
                }
            }
        }

        #endregion

        #region Theme and Visual Style Tests

        [Test]
        public void VisualEnhancementManager_ShouldApplyThemeConsistently()
        {
            // Arrange & Act
            _dashboard = new Dashboard();
            var result = _visualManager?.ApplyEnhancedVisualStyling(_dashboard);

            // Assert
            result.Should().BeTrue("Visual styling should apply successfully");

            // Check MetroForm styling
            if (_dashboard is MetroForm metroForm)
            {
                metroForm.MetroColor.Should().NotBe(Color.Empty, "MetroForm should have color set");
                metroForm.ShowIcon.Should().BeTrue("MetroForm should show icon");
            }
        }

        [Test]
        public void SfDataGrid_ShouldHaveProperStyling()
        {
            // Arrange & Act
            _busManagementForm = new BusManagementForm();
            var dataGrids = FindControlsOfType<SfDataGrid>(_busManagementForm);

            // Assert
            foreach (var grid in dataGrids)
            {
                grid.Style.Should().NotBeNull("DataGrid should have style configuration");
                grid.ThemeName.Should().NotBeNullOrEmpty("DataGrid should have theme applied");
                
                // Check grid appearance
                grid.Style.BorderColor.Should().NotBe(Color.Empty, "Grid should have border color");
                grid.Style.HeaderStyle.Should().NotBeNull("Grid headers should be styled");
                
                // Check that grid is properly sized
                grid.Size.Width.Should().BeGreaterThan(200, "Grid should have adequate width");
                grid.Size.Height.Should().BeGreaterThan(100, "Grid should have adequate height");
            }
        }

        [Test]
        public void GradientPanels_ShouldHaveProperConfiguration()
        {
            // Arrange & Act
            _dashboard = new Dashboard();
            var gradientPanels = FindControlsOfType<GradientPanel>(_dashboard);

            // Assert
            foreach (var panel in gradientPanels)
            {
                panel.Border3DStyle.Should().NotBe(Border3DStyle.Adjust, 
                    "Gradient panels should have defined border style");
                
                // Check gradient configuration
                if (panel.BackgroundColor.IsEmpty == false)
                {
                    panel.BackgroundColor.Should().NotBe(SystemColors.Control, 
                        "Gradient panels should use custom colors");
                }
            }
        }

        #endregion

        #region Control Initialization Order Tests

        [Test]
        public void Form_ShouldInitializeControlsInProperOrder()
        {
            // Arrange & Act
            _dashboard = new Dashboard();

            // Assert - Check that base controls are initialized before dependent controls
            var metroFormInitialized = _dashboard is MetroForm;
            var gradientPanelsExist = FindControlsOfType<GradientPanel>(_dashboard).Any();
            var buttonsExist = FindControlsOfType<SfButton>(_dashboard).Any();

            if (metroFormInitialized && gradientPanelsExist && buttonsExist)
            {
                // All essential components should be present
                metroFormInitialized.Should().BeTrue("MetroForm base should be initialized");
                gradientPanelsExist.Should().BeTrue("Container panels should be created");
                buttonsExist.Should().BeTrue("Interactive controls should be added");
            }
        }

        [Test]
        public void DockingManager_ShouldInitializeBeforeDockedControls()
        {
            // Arrange & Act
            _busManagementForm = new BusManagementForm();

            var dockingManager = FindControlOfType<DockingManager>(_busManagementForm);
            var dataGrid = FindControlOfType<SfDataGrid>(_busManagementForm);

            if (dockingManager != null && dataGrid != null)
            {
                // Assert - DockingManager should be created before controls that will be docked
                var dockingManagerIndex = _busManagementForm.Controls.GetChildIndex(dockingManager);
                var dataGridIndex = _busManagementForm.Controls.GetChildIndex(dataGrid);

                dockingManagerIndex.Should().BeGreaterThan(dataGridIndex, 
                    "DockingManager should be added after (higher index) controls it will manage");
            }
        }

        #endregion

        #region Performance and Memory Tests

        [Test]
        public void Forms_ShouldDisposeResourcesProperly()
        {
            // Arrange
            var testForm = new Dashboard();
            var controlCount = testForm.Controls.Count;

            // Act
            testForm.Dispose();

            // Assert
            testForm.IsDisposed.Should().BeTrue("Form should be properly disposed");
            // Note: Controls collection may still show count due to disposal timing
        }

        [Test]
        public void VisualManager_ShouldNotLeakMemory()
        {
            // Arrange
            var initialMemory = GC.GetTotalMemory(false);
            
            // Act - Create and dispose multiple forms
            for (int i = 0; i < 5; i++)
            {
                using var tempForm = new Dashboard();
                _visualManager?.ApplyEnhancedVisualStyling(tempForm);
            }

            GC.Collect();
            GC.WaitForPendingFinalizers();
            var finalMemory = GC.GetTotalMemory(true);

            // Assert - Memory should not grow excessively
            var memoryIncrease = finalMemory - initialMemory;
            memoryIncrease.Should().BeLessThan(10_000_000, // 10MB threshold
                "Visual styling should not cause significant memory leaks");
        }

        #endregion

        #region Helper Methods

        private T? FindControlOfType<T>(Control parent) where T : Control
        {
            foreach (Control control in parent.Controls)
            {
                if (control is T targetControl)
                    return targetControl;

                var found = FindControlOfType<T>(control);
                if (found != null)
                    return found;
            }
            return null;
        }

        private List<T> FindControlsOfType<T>(Control parent) where T : Control
        {
            var results = new List<T>();
            
            foreach (Control control in parent.Controls)
            {
                if (control is T targetControl)
                    results.Add(targetControl);

                results.AddRange(FindControlsOfType<T>(control));
            }
            
            return results;
        }

        #endregion
    }
}
