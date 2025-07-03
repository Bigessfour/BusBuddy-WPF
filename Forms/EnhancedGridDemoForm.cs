using Bus_Buddy.Utilities;
using Syncfusion.Windows.Forms;
using Syncfusion.Windows.Forms.Tools;
using Syncfusion.WinForms.Controls;
using Syncfusion.WinForms.DataGrid;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace Bus_Buddy.Forms
{
    /// <summary>
    /// Enhanced Grid Demonstration Form showcasing Syncfusion Layout Manager improvements
    /// Demonstrates proper grid alignment, full-screen display, and responsive design
    /// </summary>
    public partial class EnhancedGridDemoForm : MetroForm
    {
        private TableLayoutPanel mainTableLayout = null!;
        private GradientPanel headerPanel = null!;
        private GradientPanel controlPanel = null!;
        private GradientPanel gridPanel = null!;
        private GradientPanel statusPanel = null!;

        private AutoLabel titleLabel = null!;
        private AutoLabel instructionsLabel = null!;
        private AutoLabel statusLabel = null!;

        private SfDataGrid demoGrid = null!;
        private SfButton btnToggleFullScreen = null!;
        private SfButton btnRefreshData = null!;
        private SfButton btnConfigureColumns = null!;
        private SfButton btnExportData = null!;
        private SfButton btnClose = null!;

        private BindingList<GridDemoData> demoData = null!;
        private bool isFullScreen = false;

        public EnhancedGridDemoForm()
        {
            InitializeComponent();
            InitializeDemoForm();
            LoadDemoData();
        }

        private void InitializeComponent()
        {
            // Form configuration with full screen support
            SyncfusionLayoutManager.ConfigureFormForFullScreen(this);

            this.Text = "Enhanced Grid Layout Demo - Syncfusion v30.1.37";
            this.Size = new Size(1600, 1000);
            this.BackColor = Color.FromArgb(248, 249, 250);

            // Apply Syncfusion theme
            try
            {
                Syncfusion.Windows.Forms.SkinManager.SetVisualStyle(this,
                    Syncfusion.Windows.Forms.VisualTheme.Office2016Colorful);
            }
            catch
            {
                // Fallback if theme fails to load
            }

            // Initialize main table layout for responsive design
            mainTableLayout = SyncfusionLayoutManager.CreateResponsiveTableLayout(1, 4);
            mainTableLayout.RowStyles.Clear();
            mainTableLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, SyncfusionLayoutManager.HEADER_HEIGHT));
            mainTableLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, SyncfusionLayoutManager.CONTROL_PANEL_HEIGHT));
            mainTableLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            mainTableLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, SyncfusionLayoutManager.STATUS_BAR_HEIGHT));

            // Initialize panels
            InitializePanels();
            InitializeControls();
            InitializeGrid();

            // Add panels to table layout
            mainTableLayout.Controls.Add(headerPanel, 0, 0);
            mainTableLayout.Controls.Add(controlPanel, 0, 1);
            mainTableLayout.Controls.Add(gridPanel, 0, 2);
            mainTableLayout.Controls.Add(statusPanel, 0, 3);

            this.Controls.Add(mainTableLayout);
        }

        private void InitializePanels()
        {
            // Header Panel
            headerPanel = new GradientPanel();
            SyncfusionLayoutManager.ConfigureGradientPanel(headerPanel, SyncfusionLayoutManager.PRIMARY_COLOR);
            headerPanel.Dock = DockStyle.Fill;
            headerPanel.Padding = new Padding(SyncfusionLayoutManager.STANDARD_PADDING);

            // Control Panel
            controlPanel = new GradientPanel();
            SyncfusionLayoutManager.ConfigureGradientPanel(controlPanel, Color.White);
            controlPanel.Dock = DockStyle.Fill;
            controlPanel.Padding = new Padding(SyncfusionLayoutManager.STANDARD_PADDING);
            controlPanel.BorderStyle = BorderStyle.FixedSingle;

            // Grid Panel
            gridPanel = new GradientPanel();
            SyncfusionLayoutManager.ConfigureGradientPanel(gridPanel, Color.White);
            gridPanel.Dock = DockStyle.Fill;
            gridPanel.Padding = new Padding(SyncfusionLayoutManager.CONTROL_SPACING);

            // Status Panel
            statusPanel = new GradientPanel();
            SyncfusionLayoutManager.ConfigureGradientPanel(statusPanel, SyncfusionLayoutManager.HEADER_BACKGROUND);
            statusPanel.Dock = DockStyle.Fill;
            statusPanel.Padding = new Padding(SyncfusionLayoutManager.CONTROL_SPACING);
        }

        private void InitializeControls()
        {
            // Title Label
            titleLabel = new AutoLabel
            {
                Text = "🚌 Enhanced Grid Layout Demonstration",
                Font = new Font("Segoe UI", 16F, FontStyle.Bold),
                ForeColor = Color.White,
                AutoSize = true,
                Location = new Point(0, 15)
            };

            // Instructions Label
            instructionsLabel = new AutoLabel
            {
                Text = "This demo showcases proper Syncfusion grid alignment, full-screen display, and responsive layout management using the SyncfusionLayoutManager utility.",
                Font = new Font("Segoe UI", 9F),
                ForeColor = SyncfusionLayoutManager.CELL_TEXT_COLOR,
                AutoSize = true,
                MaximumSize = new Size(1200, 0),
                Location = new Point(5, 5)
            };

            // Status Label
            statusLabel = new AutoLabel
            {
                Text = "Ready - Grid optimized for full-screen display",
                Font = new Font("Segoe UI", 9F),
                ForeColor = SyncfusionLayoutManager.CELL_TEXT_COLOR,
                AutoSize = true,
                Location = new Point(5, 5)
            };

            // Initialize buttons
            btnToggleFullScreen = new SfButton();
            SyncfusionLayoutManager.ConfigureSfButton(btnToggleFullScreen, "Toggle Full Screen",
                SyncfusionLayoutManager.PRIMARY_COLOR, new Point(20, 40), 0);
            btnToggleFullScreen.Click += BtnToggleFullScreen_Click;

            btnRefreshData = new SfButton();
            SyncfusionLayoutManager.ConfigureSfButton(btnRefreshData, "Refresh Data",
                SyncfusionLayoutManager.SUCCESS_COLOR, new Point(160, 40), 1);
            btnRefreshData.Click += BtnRefreshData_Click;

            btnConfigureColumns = new SfButton();
            SyncfusionLayoutManager.ConfigureSfButton(btnConfigureColumns, "Configure Columns",
                SyncfusionLayoutManager.INFO_COLOR, new Point(300, 40), 2);
            btnConfigureColumns.Click += BtnConfigureColumns_Click;

            btnExportData = new SfButton();
            SyncfusionLayoutManager.ConfigureSfButton(btnExportData, "Export Data",
                SyncfusionLayoutManager.WARNING_COLOR, new Point(440, 40), 3);
            btnExportData.Click += BtnExportData_Click;

            btnClose = new SfButton();
            SyncfusionLayoutManager.ConfigureSfButton(btnClose, "Close",
                Color.FromArgb(158, 158, 158), new Point(580, 40), 4);
            btnClose.Click += BtnClose_Click;

            // Add controls to panels
            headerPanel.Controls.Add(titleLabel);
            controlPanel.Controls.AddRange(new Control[]
            {
                instructionsLabel, btnToggleFullScreen, btnRefreshData,
                btnConfigureColumns, btnExportData, btnClose
            });
            statusPanel.Controls.Add(statusLabel);
        }

        private void InitializeGrid()
        {
            demoGrid = new SfDataGrid();

            // Configure grid using the enhanced layout manager
            SyncfusionLayoutManager.ConfigureSfDataGrid(demoGrid, true);
            demoGrid.Dock = DockStyle.Fill;

            // Add custom configuration for demo data
            demoGrid.DataSourceChanged += (sender, e) =>
            {
                ConfigureDemoGridColumns();
            };

            gridPanel.Controls.Add(demoGrid);
        }

        private void InitializeDemoForm()
        {
            // Set MetroForm specific properties
            this.MetroColor = SyncfusionLayoutManager.PRIMARY_COLOR;
            this.CaptionBarColor = SyncfusionLayoutManager.PRIMARY_COLOR;
            this.CaptionForeColor = Color.White;
            this.ShowIcon = false;
            this.MaximizeBox = true;
            this.MinimizeBox = true;
        }

        private void LoadDemoData()
        {
            demoData = new BindingList<GridDemoData>();

            // Generate sample data to demonstrate grid capabilities
            var random = new Random();
            var routes = new[] { "Route A - Downtown", "Route B - University", "Route C - Mall", "Route D - Airport" };
            var statuses = new[] { "Active", "Maintenance", "Out of Service", "Reserved" };
            var drivers = new[] { "John Smith", "Mary Johnson", "Robert Davis", "Sarah Wilson", "Michael Brown" };

            for (int i = 1; i <= 100; i++)
            {
                demoData.Add(new GridDemoData
                {
                    VehicleId = i,
                    VehicleNumber = $"BUS-{i:D3}",
                    Route = routes[random.Next(routes.Length)],
                    Driver = drivers[random.Next(drivers.Length)],
                    Capacity = random.Next(20, 60),
                    CurrentPassengers = random.Next(0, 50),
                    Status = statuses[random.Next(statuses.Length)],
                    LastMaintenance = DateTime.Now.AddDays(-random.Next(1, 365)),
                    NextMaintenance = DateTime.Now.AddDays(random.Next(1, 90)),
                    Mileage = random.Next(10000, 150000),
                    FuelLevel = random.Next(10, 100),
                    IsActive = random.Next(0, 2) == 1,
                    Revenue = random.Next(50, 500) * 1.5m,
                    Notes = $"Sample note for vehicle {i}"
                });
            }

            demoGrid.DataSource = demoData;
            statusLabel.Text = $"Loaded {demoData.Count} vehicles - Grid optimized for full-screen display";
        }

        private void ConfigureDemoGridColumns()
        {
            try
            {
                // Configure column alignment and formatting using the layout manager
                SyncfusionLayoutManager.ConfigureColumnAlignment(demoGrid, "VehicleId",
                    HorizontalAlignment.Center, null, 80);
                SyncfusionLayoutManager.ConfigureColumnAlignment(demoGrid, "VehicleNumber",
                    HorizontalAlignment.Center, null, 100);
                SyncfusionLayoutManager.ConfigureColumnAlignment(demoGrid, "Route",
                    HorizontalAlignment.Left, null, 150);
                SyncfusionLayoutManager.ConfigureColumnAlignment(demoGrid, "Driver",
                    HorizontalAlignment.Left, null, 120);
                SyncfusionLayoutManager.ConfigureColumnAlignment(demoGrid, "Capacity",
                    HorizontalAlignment.Center, null, 80);
                SyncfusionLayoutManager.ConfigureColumnAlignment(demoGrid, "CurrentPassengers",
                    HorizontalAlignment.Center, null, 100);
                SyncfusionLayoutManager.ConfigureColumnAlignment(demoGrid, "Status",
                    HorizontalAlignment.Center, null, 100);
                SyncfusionLayoutManager.ConfigureColumnAlignment(demoGrid, "LastMaintenance",
                    HorizontalAlignment.Center, "MM/dd/yyyy", 120);
                SyncfusionLayoutManager.ConfigureColumnAlignment(demoGrid, "NextMaintenance",
                    HorizontalAlignment.Center, "MM/dd/yyyy", 120);
                SyncfusionLayoutManager.ConfigureColumnAlignment(demoGrid, "Mileage",
                    HorizontalAlignment.Right, "N0", 100);
                SyncfusionLayoutManager.ConfigureColumnAlignment(demoGrid, "FuelLevel",
                    HorizontalAlignment.Center, "P0", 80);
                SyncfusionLayoutManager.ConfigureColumnAlignment(demoGrid, "IsActive",
                    HorizontalAlignment.Center, null, 80);
                SyncfusionLayoutManager.ConfigureColumnAlignment(demoGrid, "Revenue",
                    HorizontalAlignment.Right, "C2", 100);

                // Set custom header texts
                var columns = new Dictionary<string, string>
                {
                    ["VehicleId"] = "ID",
                    ["VehicleNumber"] = "Vehicle #",
                    ["Route"] = "Assigned Route",
                    ["Driver"] = "Driver Name",
                    ["Capacity"] = "Capacity",
                    ["CurrentPassengers"] = "Current Load",
                    ["Status"] = "Status",
                    ["LastMaintenance"] = "Last Service",
                    ["NextMaintenance"] = "Next Service",
                    ["Mileage"] = "Mileage",
                    ["FuelLevel"] = "Fuel %",
                    ["IsActive"] = "Active",
                    ["Revenue"] = "Revenue",
                    ["Notes"] = "Notes"
                };

                foreach (var kvp in columns)
                {
                    if (demoGrid.Columns[kvp.Key] != null)
                        demoGrid.Columns[kvp.Key].HeaderText = kvp.Value;
                }
            }
            catch (Exception ex)
            {
                statusLabel.Text = $"Error configuring columns: {ex.Message}";
            }
        }

        #region Button Event Handlers

        private void BtnToggleFullScreen_Click(object? sender, EventArgs e)
        {
            if (isFullScreen)
            {
                this.WindowState = FormWindowState.Normal;
                this.FormBorderStyle = FormBorderStyle.Sizable;
                btnToggleFullScreen.Text = "Enter Full Screen";
                statusLabel.Text = "Windowed mode - Grid adapts to window size";
            }
            else
            {
                this.WindowState = FormWindowState.Maximized;
                this.FormBorderStyle = FormBorderStyle.None;
                btnToggleFullScreen.Text = "Exit Full Screen";
                statusLabel.Text = "Full screen mode - Grid optimized for maximum display";
            }
            isFullScreen = !isFullScreen;
        }

        private void BtnRefreshData_Click(object? sender, EventArgs e)
        {
            LoadDemoData();
            statusLabel.Text = "Data refreshed with enhanced grid formatting";
        }

        private void BtnConfigureColumns_Click(object? sender, EventArgs e)
        {
            // Demonstrate column auto-sizing
            demoGrid.AutoSizeController.ResetAutoSizeWidthForAllColumns();
            demoGrid.AutoSizeController.Refresh();
            statusLabel.Text = "Columns auto-sized for optimal display";
        }

        private void BtnExportData_Click(object? sender, EventArgs e)
        {
            try
            {
                // Demonstrate export functionality
                MessageBoxAdv.Show("Export functionality would be implemented here.\n\nThe grid is properly formatted and ready for export to Excel, PDF, or other formats.",
                    "Export Demo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                statusLabel.Text = "Export functionality demonstrated";
            }
            catch (Exception ex)
            {
                statusLabel.Text = $"Export error: {ex.Message}";
            }
        }

        private void BtnClose_Click(object? sender, EventArgs e)
        {
            this.Close();
        }

        #endregion
    }

    /// <summary>
    /// Demo data class for grid demonstration
    /// </summary>
    public class GridDemoData
    {
        public int VehicleId { get; set; }
        public string VehicleNumber { get; set; } = string.Empty;
        public string Route { get; set; } = string.Empty;
        public string Driver { get; set; } = string.Empty;
        public int Capacity { get; set; }
        public int CurrentPassengers { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime LastMaintenance { get; set; }
        public DateTime NextMaintenance { get; set; }
        public int Mileage { get; set; }
        public int FuelLevel { get; set; }
        public bool IsActive { get; set; }
        public decimal Revenue { get; set; }
        public string Notes { get; set; } = string.Empty;
    }
}
