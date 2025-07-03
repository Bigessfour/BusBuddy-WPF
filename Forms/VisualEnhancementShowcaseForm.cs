using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Syncfusion.Windows.Forms;
using Syncfusion.Windows.Forms.Tools;
using Syncfusion.WinForms.Controls;
using Syncfusion.WinForms.DataGrid;
using Bus_Buddy.Utilities;
using Microsoft.Extensions.Logging;

namespace Bus_Buddy.Forms
{
    /// <summary>
    /// Visual Enhancement Showcase Form
    /// Demonstrates the complete visual enhancement system for BusBuddy_Syncfusion
    /// Features: Enhanced themes, anti-aliased text, sharp grid lines, and optimized rendering
    /// </summary>
    public partial class VisualEnhancementShowcaseForm : MetroForm
    {
        private TableLayoutPanel mainTableLayout = null!;
        private GradientPanel headerPanel = null!;
        private GradientPanel demonstrationPanel = null!;
        private GradientPanel comparisonPanel = null!;
        private GradientPanel controlPanel = null!;
        private GradientPanel statusPanel = null!;

        private AutoLabel titleLabel = null!;
        private AutoLabel enhancedLabel = null!;
        private AutoLabel standardLabel = null!;
        private AutoLabel statusLabel = null!;
        private AutoLabel diagnosticsLabel = null!;

        private SfDataGrid enhancedGrid = null!;
        private SfDataGrid standardGrid = null!;

        private SfButton btnToggleTheme = null!;
        private SfButton btnRefreshData = null!;
        private SfButton btnShowDiagnostics = null!;
        private SfButton btnExportComparison = null!;
        private SfButton btnClose = null!;

        private BindingList<VehicleComparisonData> demoData = null!;
        private readonly ILogger<VisualEnhancementShowcaseForm>? _logger;
        private bool isEnhancedMode = true;

        public VisualEnhancementShowcaseForm(ILogger<VisualEnhancementShowcaseForm>? logger = null)
        {
            _logger = logger;
            _logger?.LogInformation("Initializing Visual Enhancement Showcase");

            InitializeComponent();
            InitializeShowcaseForm();
            LoadComparisonData();
        }

        private void InitializeComponent()
        {
            // Apply enhanced visual theme immediately
            VisualEnhancementManager.ApplyEnhancedTheme(this);

            this.Text = "🎨 Visual Enhancement Showcase - BusBuddy Syncfusion v30.1.37";
            this.Size = new Size(1800, 1200);
            this.BackColor = Color.FromArgb(248, 249, 250);

            // Initialize main responsive layout
            mainTableLayout = SyncfusionLayoutManager.CreateResponsiveTableLayout(1, 5);
            mainTableLayout.RowStyles.Clear();
            mainTableLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 80));        // Header
            mainTableLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));       // Enhanced demo
            mainTableLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));       // Standard comparison
            mainTableLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 80));       // Controls
            mainTableLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 100));      // Status & diagnostics

            InitializePanels();
            InitializeControls();
            InitializeGrids();

            // Add panels to layout
            mainTableLayout.Controls.Add(headerPanel, 0, 0);
            mainTableLayout.Controls.Add(demonstrationPanel, 0, 1);
            mainTableLayout.Controls.Add(comparisonPanel, 0, 2);
            mainTableLayout.Controls.Add(controlPanel, 0, 3);
            mainTableLayout.Controls.Add(statusPanel, 0, 4);

            this.Controls.Add(mainTableLayout);
        }

        private void InitializePanels()
        {
            // Header Panel - Enhanced theme showcase
            headerPanel = new GradientPanel();
            VisualEnhancementManager.ApplyChartEnhancements(headerPanel);
            SyncfusionLayoutManager.ConfigureGradientPanel(headerPanel,
                Color.FromArgb(46, 125, 185));
            headerPanel.Dock = DockStyle.Fill;
            headerPanel.Padding = new Padding(20);

            // Enhanced Demonstration Panel
            demonstrationPanel = new GradientPanel();
            SyncfusionLayoutManager.ConfigureGradientPanel(demonstrationPanel, Color.White);
            demonstrationPanel.Dock = DockStyle.Fill;
            demonstrationPanel.Padding = new Padding(10);
            demonstrationPanel.BorderStyle = BorderStyle.FixedSingle;

            // Standard Comparison Panel
            comparisonPanel = new GradientPanel();
            SyncfusionLayoutManager.ConfigureGradientPanel(comparisonPanel,
                Color.FromArgb(252, 252, 252));
            comparisonPanel.Dock = DockStyle.Fill;
            comparisonPanel.Padding = new Padding(10);
            comparisonPanel.BorderStyle = BorderStyle.FixedSingle;

            // Control Panel
            controlPanel = new GradientPanel();
            SyncfusionLayoutManager.ConfigureGradientPanel(controlPanel,
                Color.FromArgb(245, 245, 245));
            controlPanel.Dock = DockStyle.Fill;
            controlPanel.Padding = new Padding(15);

            // Status Panel
            statusPanel = new GradientPanel();
            SyncfusionLayoutManager.ConfigureGradientPanel(statusPanel,
                Color.FromArgb(240, 245, 251));
            statusPanel.Dock = DockStyle.Fill;
            statusPanel.Padding = new Padding(10);
        }

        private void InitializeControls()
        {
            // Enhanced title with improved typography
            titleLabel = new AutoLabel
            {
                Text = "🎨 Visual Enhancement System Demonstration",
                Font = new Font("Segoe UI", 18F, FontStyle.Bold),
                ForeColor = Color.White,
                AutoSize = true,
                Location = new Point(0, 20)
            };

            // Section labels with enhanced styling
            enhancedLabel = new AutoLabel
            {
                Text = "✨ ENHANCED VISUALS - Anti-aliased text, sharp lines, optimal contrast",
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                ForeColor = Color.FromArgb(46, 125, 185),
                AutoSize = true,
                Location = new Point(10, 5)
            };

            standardLabel = new AutoLabel
            {
                Text = "📊 STANDARD COMPARISON - Default Syncfusion styling",
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                ForeColor = Color.FromArgb(158, 158, 158),
                AutoSize = true,
                Location = new Point(10, 5)
            };

            statusLabel = new AutoLabel
            {
                Text = "Ready - Visual enhancement system loaded",
                Font = new Font("Segoe UI", 9F),
                ForeColor = Color.FromArgb(76, 175, 80),
                AutoSize = true,
                Location = new Point(5, 5)
            };

            diagnosticsLabel = new AutoLabel
            {
                Text = "Click 'Show Diagnostics' to view detailed enhancement information",
                Font = new Font("Segoe UI", 8.5F),
                ForeColor = Color.FromArgb(95, 99, 104),
                AutoSize = true,
                MaximumSize = new Size(1400, 60),
                Location = new Point(5, 25)
            };

            // Enhanced button styling
            btnToggleTheme = new SfButton();
            VisualEnhancementManager.ApplyEnhancedButtonStyling(btnToggleTheme,
                Color.FromArgb(142, 68, 173));
            btnToggleTheme.Text = "Toggle Enhancement";
            btnToggleTheme.Size = new Size(150, 40);
            btnToggleTheme.Location = new Point(20, 20);
            btnToggleTheme.Click += BtnToggleTheme_Click;

            btnRefreshData = new SfButton();
            VisualEnhancementManager.ApplyEnhancedButtonStyling(btnRefreshData,
                Color.FromArgb(76, 175, 80));
            btnRefreshData.Text = "Refresh Data";
            btnRefreshData.Size = new Size(130, 40);
            btnRefreshData.Location = new Point(190, 20);
            btnRefreshData.Click += BtnRefreshData_Click;

            btnShowDiagnostics = new SfButton();
            VisualEnhancementManager.ApplyEnhancedButtonStyling(btnShowDiagnostics,
                Color.FromArgb(33, 150, 243));
            btnShowDiagnostics.Text = "Show Diagnostics";
            btnShowDiagnostics.Size = new Size(140, 40);
            btnShowDiagnostics.Location = new Point(340, 20);
            btnShowDiagnostics.Click += BtnShowDiagnostics_Click;

            btnExportComparison = new SfButton();
            VisualEnhancementManager.ApplyEnhancedButtonStyling(btnExportComparison,
                Color.FromArgb(255, 152, 0));
            btnExportComparison.Text = "Export Analysis";
            btnExportComparison.Size = new Size(130, 40);
            btnExportComparison.Location = new Point(500, 20);
            btnExportComparison.Click += BtnExportComparison_Click;

            btnClose = new SfButton();
            VisualEnhancementManager.ApplyEnhancedButtonStyling(btnClose,
                Color.FromArgb(158, 158, 158));
            btnClose.Text = "Close";
            btnClose.Size = new Size(100, 40);
            btnClose.Location = new Point(650, 20);
            btnClose.Click += BtnClose_Click;

            // Add controls to panels
            headerPanel.Controls.Add(titleLabel);
            demonstrationPanel.Controls.Add(enhancedLabel);
            comparisonPanel.Controls.Add(standardLabel);
            controlPanel.Controls.AddRange(new Control[]
            {
                btnToggleTheme, btnRefreshData, btnShowDiagnostics,
                btnExportComparison, btnClose
            });
            statusPanel.Controls.AddRange(new Control[] { statusLabel, diagnosticsLabel });
        }

        private void InitializeGrids()
        {
            // Enhanced Grid - Full visual enhancement system
            enhancedGrid = new SfDataGrid();
            VisualEnhancementManager.ApplyEnhancedGridVisuals(enhancedGrid);
            enhancedGrid.Dock = DockStyle.Fill;
            enhancedGrid.Margin = new Padding(0, 25, 0, 0);

            // Standard Grid - Default Syncfusion styling for comparison
            standardGrid = new SfDataGrid();
            SyncfusionLayoutManager.ConfigureSfDataGrid(standardGrid, true, false); // Disable enhancements
            standardGrid.Dock = DockStyle.Fill;
            standardGrid.Margin = new Padding(0, 25, 0, 0);

            // Configure both grids for comparison data
            enhancedGrid.DataSourceChanged += (sender, e) => ConfigureEnhancedGridColumns();
            standardGrid.DataSourceChanged += (sender, e) => ConfigureStandardGridColumns();

            demonstrationPanel.Controls.Add(enhancedGrid);
            comparisonPanel.Controls.Add(standardGrid);
        }

        private void InitializeShowcaseForm()
        {
            // Enhanced MetroForm properties
            this.MetroColor = Color.FromArgb(46, 125, 185);
            this.CaptionBarColor = Color.FromArgb(46, 125, 185);
            this.CaptionForeColor = Color.White;
            this.ShowIcon = false;
            this.MaximizeBox = true;
            this.MinimizeBox = true;
            this.StartPosition = FormStartPosition.CenterScreen;

            // Enable high-quality font rendering for entire form
            VisualEnhancementManager.EnableHighQualityFontRendering(this);

            _logger?.LogInformation("Visual Enhancement Showcase form initialized");
        }

        private void LoadComparisonData()
        {
            demoData = new BindingList<VehicleComparisonData>();
            var random = new Random();

            var routes = new[] { "Downtown Express", "University Loop", "Shopping Center", "Airport Shuttle" };
            var statuses = new[] { "In Service", "Maintenance", "Out of Service", "Standby" };
            var drivers = new[] { "John Smith", "Sarah Johnson", "Mike Davis", "Lisa Wilson", "Tom Brown" };

            // Generate comprehensive comparison data
            for (int i = 1; i <= 150; i++)
            {
                demoData.Add(new VehicleComparisonData
                {
                    VehicleId = i,
                    VehicleNumber = $"BUS-{i:D3}",
                    Route = routes[random.Next(routes.Length)],
                    Driver = drivers[random.Next(drivers.Length)],
                    Capacity = random.Next(25, 65),
                    CurrentLoad = random.Next(0, 60),
                    Status = statuses[random.Next(statuses.Length)],
                    LastService = DateTime.Now.AddDays(-random.Next(1, 180)),
                    NextService = DateTime.Now.AddDays(random.Next(1, 90)),
                    Mileage = random.Next(15000, 200000),
                    FuelEfficiency = Math.Round(8.0 + (random.NextDouble() * 4.0), 2),
                    IsActive = random.Next(0, 2) == 1,
                    Revenue = Math.Round(random.Next(100, 800) * 1.25m, 2),
                    MaintenanceCost = Math.Round(random.Next(50, 500) * 0.85m, 2),
                    Notes = GenerateNote(i, random)
                });
            }

            // Apply data to both grids
            enhancedGrid.DataSource = demoData;
            standardGrid.DataSource = demoData;

            statusLabel.Text = $"Loaded {demoData.Count} vehicles for visual comparison";
            statusLabel.ForeColor = Color.FromArgb(76, 175, 80);

            _logger?.LogInformation("Loaded {Count} vehicles for visual comparison", demoData.Count);
        }

        private static string GenerateNote(int vehicleId, Random random)
        {
            var notes = new[]
            {
                "Excellent condition, regular maintenance",
                "Minor wear, scheduled for inspection",
                "Recently serviced, optimal performance",
                "Requires attention for minor repairs",
                "Fleet vehicle with standard operations"
            };
            return notes[random.Next(notes.Length)];
        }

        private void ConfigureEnhancedGridColumns()
        {
            // Configure enhanced grid with optimal column formatting
            var columnConfigs = new Dictionary<string, (HorizontalAlignment alignment, string? format, int width)>
            {
                ["VehicleId"] = (HorizontalAlignment.Center, null, 70),
                ["VehicleNumber"] = (HorizontalAlignment.Center, null, 110),
                ["Route"] = (HorizontalAlignment.Left, null, 140),
                ["Driver"] = (HorizontalAlignment.Left, null, 120),
                ["Capacity"] = (HorizontalAlignment.Center, null, 80),
                ["CurrentLoad"] = (HorizontalAlignment.Center, null, 90),
                ["Status"] = (HorizontalAlignment.Center, null, 100),
                ["LastService"] = (HorizontalAlignment.Center, "MM/dd/yyyy", 110),
                ["NextService"] = (HorizontalAlignment.Center, "MM/dd/yyyy", 110),
                ["Mileage"] = (HorizontalAlignment.Right, "N0", 90),
                ["FuelEfficiency"] = (HorizontalAlignment.Right, "F2", 100),
                ["IsActive"] = (HorizontalAlignment.Center, null, 70),
                ["Revenue"] = (HorizontalAlignment.Right, "C2", 100),
                ["MaintenanceCost"] = (HorizontalAlignment.Right, "C2", 120),
                ["Notes"] = (HorizontalAlignment.Left, null, 200)
            };

            foreach (var config in columnConfigs)
            {
                SyncfusionLayoutManager.ConfigureColumnAlignment(enhancedGrid,
                    config.Key, config.Value.alignment, config.Value.format, config.Value.width);
            }

            // Set enhanced header texts
            var headerTexts = new Dictionary<string, string>
            {
                ["VehicleId"] = "ID",
                ["VehicleNumber"] = "Vehicle #",
                ["Route"] = "Route Assignment",
                ["Driver"] = "Driver",
                ["Capacity"] = "Capacity",
                ["CurrentLoad"] = "Current Load",
                ["Status"] = "Status",
                ["LastService"] = "Last Service",
                ["NextService"] = "Next Service",
                ["Mileage"] = "Mileage",
                ["FuelEfficiency"] = "MPG",
                ["IsActive"] = "Active",
                ["Revenue"] = "Revenue",
                ["MaintenanceCost"] = "Maintenance",
                ["Notes"] = "Notes"
            };

            foreach (var header in headerTexts)
            {
                if (enhancedGrid.Columns[header.Key] != null)
                    enhancedGrid.Columns[header.Key].HeaderText = header.Value;
            }
        }

        private void ConfigureStandardGridColumns()
        {
            // Apply minimal configuration to show contrast with enhanced grid
            if (standardGrid.Columns["VehicleId"] != null)
                standardGrid.Columns["VehicleId"].Width = 70;
            if (standardGrid.Columns["VehicleNumber"] != null)
                standardGrid.Columns["VehicleNumber"].Width = 110;
        }

        #region Event Handlers

        private void BtnToggleTheme_Click(object? sender, EventArgs e)
        {
            try
            {
                isEnhancedMode = !isEnhancedMode;

                if (isEnhancedMode)
                {
                    VisualEnhancementManager.ApplyEnhancedTheme(this);
                    VisualEnhancementManager.ApplyEnhancedGridVisuals(enhancedGrid);
                    btnToggleTheme.Text = "Disable Enhancement";
                    statusLabel.Text = "Enhanced visual mode enabled";
                    statusLabel.ForeColor = Color.FromArgb(76, 175, 80);
                }
                else
                {
                    // Apply standard theme
                    SyncfusionLayoutManager.ApplyGridStyling(enhancedGrid);
                    btnToggleTheme.Text = "Enable Enhancement";
                    statusLabel.Text = "Standard visual mode enabled";
                    statusLabel.ForeColor = Color.FromArgb(255, 152, 0);
                }

                _logger?.LogInformation("Visual enhancement toggled: {Mode}", isEnhancedMode ? "Enhanced" : "Standard");
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Error toggling visual enhancement");
                statusLabel.Text = $"Error toggling enhancement: {ex.Message}";
                statusLabel.ForeColor = Color.FromArgb(244, 67, 54);
            }
        }

        private void BtnRefreshData_Click(object? sender, EventArgs e)
        {
            try
            {
                LoadComparisonData();
                statusLabel.Text = "Data refreshed with current visual enhancements";
                statusLabel.ForeColor = Color.FromArgb(76, 175, 80);
                _logger?.LogInformation("Comparison data refreshed");
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Error refreshing data");
                statusLabel.Text = $"Refresh error: {ex.Message}";
                statusLabel.ForeColor = Color.FromArgb(244, 67, 54);
            }
        }

        private void BtnShowDiagnostics_Click(object? sender, EventArgs e)
        {
            try
            {
                var diagnostics = VisualEnhancementManager.GetVisualEnhancementDiagnostics(this);

                var diagnosticsForm = new Form
                {
                    Text = "Visual Enhancement Diagnostics",
                    Size = new Size(600, 500),
                    StartPosition = FormStartPosition.CenterParent,
                    ShowIcon = false,
                    MaximizeBox = false,
                    MinimizeBox = false
                };

                var textBox = new TextBox
                {
                    Multiline = true,
                    ReadOnly = true,
                    Dock = DockStyle.Fill,
                    ScrollBars = ScrollBars.Vertical,
                    Font = new Font("Consolas", 9F),
                    Text = diagnostics
                };

                diagnosticsForm.Controls.Add(textBox);
                diagnosticsForm.ShowDialog(this);

                diagnosticsLabel.Text = "Diagnostics: Form optimized with enhanced visuals enabled";
                _logger?.LogInformation("Diagnostics displayed");
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Error showing diagnostics");
                statusLabel.Text = $"Diagnostics error: {ex.Message}";
                statusLabel.ForeColor = Color.FromArgb(244, 67, 54);
            }
        }

        private void BtnExportComparison_Click(object? sender, EventArgs e)
        {
            try
            {
                // Demonstrate export capability
                MessageBoxAdv.Show(this,
                    "Visual Enhancement Comparison Export\n\n" +
                    "This feature would export:\n" +
                    "• Enhanced vs Standard visual comparison\n" +
                    "• Performance metrics\n" +
                    "• Visual quality analysis\n" +
                    "• Optimization recommendations\n\n" +
                    "Export formats: PDF, Excel, Image comparison",
                    "Export Analysis", MessageBoxButtons.OK, MessageBoxIcon.Information);

                statusLabel.Text = "Export analysis feature demonstrated";
                _logger?.LogInformation("Export analysis demonstrated");
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Error in export analysis");
                statusLabel.Text = $"Export error: {ex.Message}";
                statusLabel.ForeColor = Color.FromArgb(244, 67, 54);
            }
        }

        private void BtnClose_Click(object? sender, EventArgs e)
        {
            try
            {
                _logger?.LogInformation("Visual Enhancement Showcase closing");
                this.Close();
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Error closing showcase");
            }
        }

        #endregion
    }

    /// <summary>
    /// Comprehensive data class for visual comparison demonstration
    /// </summary>
    public class VehicleComparisonData
    {
        public int VehicleId { get; set; }
        public string VehicleNumber { get; set; } = string.Empty;
        public string Route { get; set; } = string.Empty;
        public string Driver { get; set; } = string.Empty;
        public int Capacity { get; set; }
        public int CurrentLoad { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime LastService { get; set; }
        public DateTime NextService { get; set; }
        public int Mileage { get; set; }
        public double FuelEfficiency { get; set; }
        public bool IsActive { get; set; }
        public decimal Revenue { get; set; }
        public decimal MaintenanceCost { get; set; }
        public string Notes { get; set; } = string.Empty;
    }
}
