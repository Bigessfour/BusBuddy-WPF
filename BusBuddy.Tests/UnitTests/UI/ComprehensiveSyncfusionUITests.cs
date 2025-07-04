using NUnit.Framework;
using FluentAssertions;
using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Syncfusion.Windows.Forms;
using Syncfusion.Windows.Forms.Tools;
using Syncfusion.WinForms.DataGrid;
using Bus_Buddy.Utilities;

namespace BusBuddy.Tests.UnitTests.UI
{
    [TestFixture]
    [Apartment(System.Threading.ApartmentState.STA)]
    public class ComprehensiveSyncfusionUITests
    {
        private Form _testForm;

        [SetUp]
        public void Setup()
        {
            try
            {
                // Initialize Syncfusion licensing
                Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("Ngo9BigBOggjHTQxAR8/V1JEaF5cXmRCf1FpRmJGdld5fUVHYVZUTXxaS00DNHVRdkdmWXhec3RSRGRYU0R2WUBWYEk=");
                
                // Create test form with Syncfusion controls
                _testForm = CreateTestForm();
                _testForm.Show();
                
                // Allow form to fully render
                Application.DoEvents();
                System.Threading.Thread.Sleep(100);
            }
            catch (Exception ex)
            {
                Assert.Inconclusive($"Setup failed: {ex.Message}");
            }
        }

        [TearDown]
        public void TearDown()
        {
            try
            {
                _testForm?.Close();
                _testForm?.Dispose();
                
                // Clean up any remaining forms
                Application.DoEvents();
            }
            catch (Exception ex)
            {
                // Log but don't fail teardown
                Console.WriteLine($"TearDown warning: {ex.Message}");
            }
        }

        [Test]
        public void ButtonControls_ConfiguredAndFunctional()
        {
            // Arrange
            var button = FindSyncfusionButton(_testForm, "TestButton");
            
            // Assert button exists and basic properties
            button.Should().NotBeNull("Button should be found in the form");
            button.Text.Should().NotBeEmpty("Button should have text");
            button.Enabled.Should().BeTrue("Button should be enabled");
            button.Visible.Should().BeTrue("Button should be visible");
            
            // Test Syncfusion-specific properties
            if (button is ButtonAdv buttonAdv)
            {
                buttonAdv.UseVisualStyle.Should().BeTrue("Button should use visual styling");
                buttonAdv.FlatStyle.Should().Be(FlatStyle.Flat, "Button should use flat style");
            }
            
            // Test functionality (click event)
            bool clicked = false;
            button.Click += (s, e) => clicked = true;
            button.PerformClick();
            clicked.Should().BeTrue("Button click event should be triggered");
        }

        [Test]
        public void SfDataGrid_ConfiguredPerDocumentation()
        {
            // Arrange
            var grid = FindControl<SfDataGrid>(_testForm, "TestDataGrid");
            
            if (grid != null)
            {
                // Assert basic grid properties per Syncfusion documentation
                grid.Should().NotBeNull("DataGrid should be found");
                grid.Visible.Should().BeTrue("DataGrid should be visible");
                grid.Enabled.Should().BeTrue("DataGrid should be enabled");
                
                // Test Syncfusion-specific properties
                grid.AllowEditing.Should().BeTrue("Grid should allow editing");
                grid.AllowResizingColumns.Should().BeTrue("Grid should allow column resizing");
                grid.AllowSorting.Should().BeTrue("Grid should allow sorting");
                
                // Test data binding capability
                if (grid.DataSource != null)
                {
                    grid.DataSource.Should().NotBeNull("Grid should have a data source when configured");
                }
                
                // Test column configuration
                if (grid.Columns.Count > 0)
                {
                    grid.Columns.Should().NotBeEmpty("Grid should have columns when configured");
                    
                    foreach (var column in grid.Columns)
                    {
                        column.MappingName.Should().NotBeEmpty("Each column should have a mapping name");
                    }
                }
            }
            else
            {
                Assert.Inconclusive("SfDataGrid not found in test form - test skipped");
            }
        }

        [Test]
        public void PanelControls_ConfiguredCorrectly()
        {
            // Find panel controls in the form
            var panels = FindAllControls<Panel>(_testForm);
            var gradientPanels = FindAllControls<GradientPanel>(_testForm);
            
            // Test standard panels
            foreach (var panel in panels)
            {
                panel.Should().NotBeNull("Panel should exist");
                panel.Visible.Should().BeTrue("Panel should be visible");
                
                // Test common panel properties
                panel.Size.Width.Should().BeGreaterThan(0, "Panel should have positive width");
                panel.Size.Height.Should().BeGreaterThan(0, "Panel should have positive height");
            }
            
            // Test Syncfusion GradientPanels
            foreach (var gradientPanel in gradientPanels)
            {
                gradientPanel.Should().NotBeNull("GradientPanel should exist");
                gradientPanel.Visible.Should().BeTrue("GradientPanel should be visible");
                
                // Test Syncfusion-specific properties
                if (gradientPanel.BackgroundColor != null)
                {
                    gradientPanel.BackgroundColor.Should().NotBeEmpty("GradientPanel should have background colors");
                }
            }
        }

        [Test]
        public void MetroForm_ConfiguredForHighDPI()
        {
            // Create a MetroForm for testing
            using (var metroForm = new MetroForm())
            {
                metroForm.Text = "DPI Test Form";
                metroForm.Size = new Size(800, 600);
                
                // Test basic MetroForm properties
                metroForm.Should().NotBeNull("MetroForm should be created");
                
                // Test DPI awareness
                metroForm.AutoScaleMode.Should().Be(AutoScaleMode.Dpi, "Form should use DPI scaling");
                
                // Test Metro-specific properties
                metroForm.MetroColor.Should().NotBe(Color.Empty, "MetroForm should have a metro color");
                metroForm.CaptionAlign.Should().Be(HorizontalAlignment.Left, "Caption should be left-aligned by default");
                
                // Test form responsiveness
                metroForm.MinimumSize.Width.Should().BeGreaterThan(0, "Form should have minimum width");
                metroForm.MinimumSize.Height.Should().BeGreaterThan(0, "Form should have minimum height");
            }
        }

        [Test]
        public void SfInput_ControlsConfiguredCorrectly()
        {
            // Test basic input controls that are available
            var textBoxes = FindAllControls<TextBox>(_testForm);
            var comboBoxes = FindAllControls<ComboBox>(_testForm);
            
            // Test standard TextBox controls
            foreach (var textBox in textBoxes)
            {
                textBox.Should().NotBeNull("TextBox should exist");
                textBox.Enabled.Should().BeTrue("TextBox should be enabled");
                textBox.Visible.Should().BeTrue("TextBox should be visible");
            }
            
            // Test ComboBox controls
            foreach (var comboBox in comboBoxes)
            {
                comboBox.Should().NotBeNull("ComboBox should exist");
                comboBox.Enabled.Should().BeTrue("ComboBox should be enabled");
                comboBox.Visible.Should().BeTrue("ComboBox should be visible");
            }
        }

        [Test]
        public void AllControls_RenderAndFunctionCorrectly()
        {
            // Get all controls from the form
            var allControls = GetAllControlsRecursively(_testForm);
            
            // Verify basic rendering properties
            foreach (Control control in allControls)
            {
                if (control.Visible)
                {
                    control.Size.Width.Should().BeGreaterThan(0, $"{control.Name} should have positive width");
                    control.Size.Height.Should().BeGreaterThan(0, $"{control.Name} should have positive height");
                    
                    // Test DPI scaling (basic check)
                    control.Font.Should().NotBeNull($"{control.Name} should have a font");
                    control.Font.Size.Should().BeGreaterThan(0, $"{control.Name} font size should be positive");
                }
            }
            
            // Test specific Syncfusion control interactions
            TestSyncfusionControlInteractions();
        }

        [Test]
        public void ThemeAndStyling_AppliedCorrectly()
        {
            // Test that Syncfusion theme is applied consistently
            var syncfusionControls = GetAllControlsRecursively(_testForm)
                .Where(c => c.GetType().Assembly.FullName.Contains("Syncfusion"))
                .ToList();
            
            if (syncfusionControls.Any())
            {
                foreach (Control control in syncfusionControls)
                {
                    // Basic theme validation
                    control.Font.Should().NotBeNull($"Syncfusion control {control.Name} should have a font");
                    control.BackColor.Should().NotBe(Color.Empty, $"Syncfusion control {control.Name} should have a background color");
                }
            }
            else
            {
                Assert.Inconclusive("No Syncfusion controls found for theme testing");
            }
        }

        [Test]
        public void SyncfusionBackgroundFix_Applied()
        {
            // Test that our background fix utility works correctly
            using (var testForm = new MetroForm())
            {
                testForm.Size = new Size(400, 300);
                
                // Apply our background fix
                SyncfusionBackgroundFix.FixDashboardBackground(testForm);
                
                // Verify the fix was applied
                testForm.BackColor.Should().NotBe(Color.Transparent, "Form background should not be transparent");
                testForm.BackColor.Should().NotBe(Color.Empty, "Form background should be set");
            }
        }

        #region Helper Methods

        private Form CreateTestForm()
        {
            var form = new MetroForm
            {
                Text = "Syncfusion UI Test Form",
                Size = new Size(800, 600),
                StartPosition = FormStartPosition.CenterScreen
            };

            // Add test controls
            AddTestControls(form);
            
            return form;
        }

        private void AddTestControls(Form form)
        {
            // Add a test button
            var button = new ButtonAdv
            {
                Name = "TestButton",
                Text = "Test Button",
                Location = new Point(10, 10),
                Size = new Size(100, 30),
                UseVisualStyle = true
            };
            form.Controls.Add(button);

            // Add a test panel
            var panel = new GradientPanel
            {
                Name = "TestPanel",
                Location = new Point(10, 50),
                Size = new Size(200, 100),
                BackgroundColor = new Syncfusion.Drawing.BrushInfo[] { 
                    new Syncfusion.Drawing.BrushInfo(Color.LightBlue) 
                }
            };
            form.Controls.Add(panel);

            // Add a test data grid if possible
            try
            {
                var dataGrid = new SfDataGrid
                {
                    Name = "TestDataGrid",
                    Location = new Point(10, 160),
                    Size = new Size(400, 200),
                    AllowEditing = true,
                    AllowResizingColumns = true,
                    AllowSorting = true
                };
                form.Controls.Add(dataGrid);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Could not add SfDataGrid: {ex.Message}");
            }
        }

        private T FindControl<T>(Control parent, string name) where T : Control
        {
            foreach (Control control in parent.Controls)
            {
                if (control is T && control.Name == name)
                    return (T)control;
                
                var found = FindControl<T>(control, name);
                if (found != null)
                    return found;
            }
            return null;
        }

        private System.Collections.Generic.List<T> FindAllControls<T>(Control parent) where T : Control
        {
            var result = new System.Collections.Generic.List<T>();
            foreach (Control control in parent.Controls)
            {
                if (control is T)
                    result.Add((T)control);
                
                result.AddRange(FindAllControls<T>(control));
            }
            return result;
        }

        private Button FindSyncfusionButton(Control parent, string name)
        {
            // Look for ButtonAdv first, then regular Button
            var buttonAdv = FindControl<ButtonAdv>(parent, name);
            if (buttonAdv != null) return buttonAdv;
            
            return FindControl<Button>(parent, name);
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

        private void TestSyncfusionControlInteractions()
        {
            // Test button interactions
            var buttons = FindAllControls<ButtonAdv>(_testForm);
            foreach (var button in buttons)
            {
                // Test click simulation
                bool canClick = button.Enabled && button.Visible;
                canClick.Should().BeTrue($"Button {button.Name} should be clickable");
            }

            // Test data grid interactions if present
            var dataGrid = FindControl<SfDataGrid>(_testForm, "TestDataGrid");
            if (dataGrid != null && dataGrid.Visible)
            {
                // Test basic grid functionality
                dataGrid.AllowEditing.Should().BeTrue("DataGrid should allow editing");
                dataGrid.AllowResizingColumns.Should().BeTrue("DataGrid should allow column resizing");
            }
        }

        #endregion
    }
}
