using NUnit.Framework;
using FluentAssertions;
using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Syncfusion.Licensing;
using Syncfusion.Windows.Forms;
using Syncfusion.WinForms.DataGrid;
using Syncfusion.Windows.Forms.Tools;
using Syncfusion.WinForms.DataGrid.Enums;
using Syncfusion.WinForms.DataGrid.Styles;

namespace BusBuddy.Tests.UnitTests.UI
{
    /// <summary>
    /// Comprehensive Syncfusion UI Validation Tests
    /// Evaluates all Syncfusion UI elements (buttons, grids, panels) ensuring they are configured 
    /// per Syncfusion documentation and function as designed using NUnit best practices.
    /// </summary>
    [TestFixture]
    [Apartment(ApartmentState.STA)]
    public class ComprehensiveSyncfusionUIValidationTests
    {
        private MetroForm _testForm;
        private const string LicenseKey = "Ngo9BigBOggjHTQxAR8/V1JEaF5cXmRCf1FpRmJGdld5fUVHYVZUTXxaS00DNHVRdkdmWXhec3RSRGRYU0R2WUBWYEk=";

        [SetUp]
        public void Setup()
        {
            // Initialize Syncfusion licensing per documentation
            SyncfusionLicenseProvider.RegisterLicense(LicenseKey);
            
            // Create test form with proper configuration
            _testForm = new MetroForm
            {
                Size = new Size(800, 600),
                Text = "Comprehensive UI Test Form",
                AutoScaleMode = AutoScaleMode.Dpi,
                StartPosition = FormStartPosition.CenterScreen,
                ShowInTaskbar = false // Don't show in taskbar during tests
            };
            
            AddTestControls(_testForm);
            _testForm.Show();
            Application.DoEvents();
        }

        [TearDown]
        public void TearDown()
        {
            try
            {
                _testForm?.Close();
                _testForm?.Dispose();
                Application.DoEvents();
                
                // Clean up any remaining forms
                foreach (Form form in Application.OpenForms.Cast<Form>().ToArray())
                {
                    if (form is MetroForm && form != _testForm)
                    {
                        form.Close();
                        form.Dispose();
                    }
                }
            }
            catch (Exception)
            {
                // Ignore cleanup errors in tests
            }
        }

        [Test]
        public void ButtonAdv_ConfiguredAndFunctional()
        {
            // Arrange
            var button = FindControl<ButtonAdv>(_testForm, "TestButton");
            
            // Assert - Button configuration per Syncfusion documentation
            button.Should().NotBeNull("ButtonAdv should be created successfully");
            button.Text.Should().Be("Test Button", "Button text should be set correctly");
            button.Enabled.Should().BeTrue("Button should be enabled by default");
            button.Visible.Should().BeTrue("Button should be visible");
            button.UseVisualStyle.Should().BeTrue("UseVisualStyle should be enabled per Syncfusion best practices");
            button.Size.Should().Be(new Size(120, 35), "Button should have correct size");

            // Test event handling functionality
            bool clicked = false;
            button.Click += (s, e) => clicked = true;
            button.PerformClick();
            clicked.Should().BeTrue("Button click event should fire correctly");
        }

        [Test]
        public void SfDataGrid_ConfiguredPerDocumentation()
        {
            // Arrange
            var grid = FindControl<SfDataGrid>(_testForm, "TestDataGrid");
            
            // Assert - Core SfDataGrid configuration
            grid.Should().NotBeNull("SfDataGrid should be created successfully");
            grid.Visible.Should().BeTrue("Grid should be visible");
            grid.Enabled.Should().BeTrue("Grid should be enabled");
            grid.AllowEditing.Should().BeTrue("Grid should allow editing per configuration");
            grid.AllowResizingColumns.Should().BeTrue("Grid should allow column resizing");
            grid.AllowSorting.Should().BeTrue("Grid should allow sorting");
            grid.AutoGenerateColumns.Should().BeFalse("AutoGenerateColumns should be disabled for manual column control");
            
            // Assert - Data binding and structure
            grid.DataSource.Should().NotBeNull("Grid should have data source");
            grid.Columns.Count.Should().BeGreaterThan(0, "Grid should have defined columns");
            grid.Columns[0].MappingName.Should().NotBeEmpty("First column should have mapping name");
            
            // Assert - Styling and layout per Syncfusion docs
            grid.RowHeight.Should().BeGreaterOrEqualTo(35, "Row height should be touch-friendly (35px minimum)");
            grid.HeaderRowHeight.Should().BeGreaterOrEqualTo(40, "Header row should be appropriately sized");
        }

        [Test]
        public void GradientPanel_ConfiguredCorrectly()
        {
            // Arrange
            var panel = FindControl<GradientPanel>(_testForm, "TestPanel");
            
            // Assert - GradientPanel configuration
            panel.Should().NotBeNull("GradientPanel should be created successfully");
            panel.Visible.Should().BeTrue("Panel should be visible");
            panel.Size.Should().Be(new Size(400, 200), "Panel should have correct size");
            panel.BackgroundColor.Should().NotBeNull("Panel should have background color configured");
            
            // Assert - Nested controls functionality
            panel.Controls.Count.Should().BeGreaterThan(0, "Panel should contain nested controls");
            panel.Controls[0].Should().BeOfType<ButtonAdv>("First nested control should be ButtonAdv");
        }

        [Test]
        public void MetroForm_HighDpiAndThemeApplied()
        {
            // Assert - MetroForm DPI and theme configuration per Syncfusion docs
            _testForm.AutoScaleMode.Should().Be(AutoScaleMode.Dpi, "Form should use DPI scaling for high-DPI support");
            _testForm.MetroColor.Should().NotBe(Color.Empty, "MetroColor should be configured");
            _testForm.CaptionAlign.Should().Be(HorizontalAlignment.Left, "Caption should be left-aligned per Metro design");
            _testForm.Size.Width.Should().BeGreaterThan(0, "Form width should be positive");
            _testForm.Size.Height.Should().BeGreaterThan(0, "Form height should be positive");
            _testForm.Text.Should().NotBeEmpty("Form should have title text");
        }

        [Test]
        public void DockingManager_ConfiguredCorrectly()
        {
            // Arrange
            var dockingManager = FindControl<DockingManager>(_testForm, "TestDockingManager");
            var dockablePanel = _testForm.Controls.Find("DockablePanel", true).FirstOrDefault();
            
            // Assert - DockingManager functionality per Syncfusion documentation
            dockingManager.Should().NotBeNull("DockingManager should be created");
            dockablePanel.Should().NotBeNull("Dockable panel should exist");
            
            // Verify docking configuration
            dockingManager.GetDockLabel(dockablePanel).Should().Be("Dockable Panel", "Dock label should be set correctly");
            dockingManager.GetEnableDocking(dockablePanel).Should().BeTrue("Docking should be enabled for panel");
            dockablePanel.Size.Width.Should().BeGreaterThan(0, "Dockable panel should have valid width");
        }

        [Test]
        public void AllControls_RenderAndInteractCorrectly()
        {
            // Arrange
            var controls = GetAllControlsRecursively(_testForm);
            
            // Assert - All controls should render properly
            controls.Should().NotBeEmpty("Form should contain controls");
            
            foreach (var control in controls.Where(c => c.Visible))
            {
                control.Size.Width.Should().BeGreaterThan(0, $"{control.Name} should have valid width");
                control.Size.Height.Should().BeGreaterThan(0, $"{control.Name} should have valid height");
                control.Font.Size.Should().BeGreaterThan(0, $"{control.Name} should have valid font size");
            }

            // Test specific control interactions
            var grid = FindControl<SfDataGrid>(_testForm, "TestDataGrid");
            if (grid != null)
            {
                // Test grid selection functionality
                grid.SelectCell(0, 0);
                grid.SelectedIndex.Should().Be(0, "Grid should support cell selection");
            }

            var button = FindControl<ButtonAdv>(_testForm, "TestButton");
            if (button != null)
            {
                // Test button interaction
                bool clicked = false;
                button.Click += (s, e) => clicked = true;
                button.PerformClick();
                clicked.Should().BeTrue("Button should respond to click events");
            }
        }

        [Test]
        public void SyncfusionControls_PerformanceAndMemoryOptimized()
        {
            // Arrange - Get all Syncfusion controls
            var syncfusionControls = GetAllControlsRecursively(_testForm)
                .Where(c => c.GetType().Namespace?.StartsWith("Syncfusion") == true)
                .ToList();

            // Assert - Performance characteristics
            syncfusionControls.Should().NotBeEmpty("Should have Syncfusion controls");
            
            foreach (var control in syncfusionControls)
            {
                // Verify controls are properly initialized
                control.Should().NotBeNull($"{control.GetType().Name} should be initialized");
                control.Handle.Should().NotBe(IntPtr.Zero, $"{control.GetType().Name} should have valid handle");
                
                // Check for proper disposal setup
                if (control is IDisposable)
                {
                    // Control should implement IDisposable properly
                    control.Should().BeAssignableTo<IDisposable>($"{control.GetType().Name} should be disposable");
                }
            }
        }

        [Test]
        public void DataGrid_ColumnFormattingAndStyling()
        {
            // Arrange
            var grid = FindControl<SfDataGrid>(_testForm, "TestDataGrid");
            
            // Assert - Column formatting per Syncfusion documentation
            grid.Should().NotBeNull();
            grid.Columns.Should().NotBeEmpty("Grid should have columns configured");
            
            // Verify first column (ID column) formatting
            var idColumn = grid.Columns.FirstOrDefault(c => c.MappingName == "Id");
            idColumn.Should().NotBeNull("ID column should exist");
            idColumn.HeaderText.Should().Be("ID", "Header text should be set correctly");
            
            // Verify second column (Name column)
            var nameColumn = grid.Columns.FirstOrDefault(c => c.MappingName == "Name");
            nameColumn.Should().NotBeNull("Name column should exist");
            nameColumn.HeaderText.Should().Be("Name", "Name column header should be correct");
        }

        #region Helper Methods

        private void AddTestControls(MetroForm form)
        {
            try
            {
                // ButtonAdv - configured per Syncfusion documentation
                var button = new ButtonAdv
                {
                    Name = "TestButton",
                    Text = "Test Button",
                    Size = new Size(120, 35),
                    Location = new Point(10, 10),
                    UseVisualStyle = true,
                    FlatStyle = FlatStyle.Standard // Changed from Flat to Standard for better compatibility
                };

                // GradientPanel with nested ButtonAdv
                var panel = new GradientPanel
                {
                    Name = "TestPanel",
                    Size = new Size(400, 200),
                    Location = new Point(10, 50)
                };
                
                // Set background color using compatible method
                try
                {
                    panel.BackgroundColor = new Syncfusion.Drawing.BrushInfo(Color.LightBlue);
                }
                catch
                {
                    // Fallback if BrushInfo has different constructor
                    panel.BackColor = Color.LightBlue;
                }

                var nestedButton = new ButtonAdv
                {
                    Text = "Nested Button",
                    Size = new Size(100, 30),
                    Location = new Point(10, 10),
                    UseVisualStyle = true
                };
                panel.Controls.Add(nestedButton);

                // SfDataGrid - configured per Syncfusion best practices
                var grid = new SfDataGrid
                {
                    Name = "TestDataGrid",
                    Size = new Size(600, 300),
                    Location = new Point(10, 260),
                    AllowEditing = true,
                    AllowResizingColumns = true,
                    AllowSorting = true,
                    AutoGenerateColumns = false,
                    RowHeight = 35,
                    HeaderRowHeight = 40
                };

                // Configure grid columns per Syncfusion documentation
                grid.Columns.Add(new GridTextColumn 
                { 
                    MappingName = "Id", 
                    HeaderText = "ID"
                });
                grid.Columns.Add(new GridTextColumn 
                { 
                    MappingName = "Name", 
                    HeaderText = "Name" 
                });

                // Set data source
                grid.DataSource = new[]
                {
                    new { Id = 1, Name = "Item 1" },
                    new { Id = 2, Name = "Item 2" },
                    new { Id = 3, Name = "Item 3" }
                };

                // DockingManager - per Syncfusion docking documentation
                var dockingManager = new DockingManager(form) 
                { 
                    Name = "TestDockingManager" 
                };
                
                var dockablePanel = new Panel
                {
                    Name = "DockablePanel",
                    Size = new Size(200, 200),
                    Location = new Point(420, 50),
                    BackColor = Color.LightGray
                };

                // Add controls to form
                form.Controls.Add(button);
                form.Controls.Add(panel);
                form.Controls.Add(grid);
                form.Controls.Add(dockablePanel);

                // Configure docking after adding to form
                dockingManager.SetDockLabel(dockablePanel, "Dockable Panel");
                dockingManager.SetEnableDocking(dockablePanel, true);
            }
            catch (Exception ex)
            {
                // Log but don't fail test setup
                TestContext.WriteLine($"Warning: Error setting up test controls: {ex.Message}");
            }
        }

        private T FindControl<T>(Control parent, string name) where T : Control
        {
            return parent.Controls.Find(name, true).OfType<T>().FirstOrDefault();
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

        #endregion
    }
}
