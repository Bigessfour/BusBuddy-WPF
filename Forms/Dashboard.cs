using Syncfusion.Windows.Forms;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using Bus_Buddy.Services;
using Bus_Buddy.Forms;
using Bus_Buddy.Utilities;
using System;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Bus_Buddy;

public partial class Dashboard : MetroForm
{
    private readonly ILogger<Dashboard> _logger;
    private readonly IBusService _busService;
    private readonly IConfigurationService _configService;

    public Dashboard(ILogger<Dashboard> logger, IBusService busService, IConfigurationService configService)
    {
        _logger = logger;
        _busService = busService;
        _configService = configService;

        _logger.LogInformation("Initializing Dashboard form");

        InitializeComponent();
        InitializeDashboard();
    }

    private void InitializeDashboard()
    {
        // Apply enhanced visual theme system
        VisualEnhancementManager.ApplyEnhancedTheme(this);

        // Set Syncfusion MetroForm styles with enhanced colors
        this.MetroColor = System.Drawing.Color.FromArgb(46, 125, 185);
        this.CaptionBarColor = System.Drawing.Color.FromArgb(46, 125, 185);
        this.CaptionForeColor = System.Drawing.Color.White;
        this.Text = "BusBuddy - Dashboard";

        // Enable high DPI scaling for this form
        this.AutoScaleMode = AutoScaleMode.Dpi;
        this.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);

        // Enable high-quality font rendering
        VisualEnhancementManager.EnableHighQualityFontRendering(this);

        _logger.LogInformation("Dashboard form initialized with enhanced visuals");

        // Load initial data
        LoadDashboardDataAsync();
    }

    private async void LoadDashboardDataAsync()
    {
        try
        {
            _logger.LogInformation("Loading dashboard data");

            // Load buses and routes data
            var buses = await _busService.GetAllBusesAsync();
            var routes = await _busService.GetAllRoutesAsync();

            _logger.LogInformation("Loaded {BusCount} buses and {RouteCount} routes", buses.Count, routes.Count);

            // Update UI with loaded data using Syncfusion dashboard components
            UpdateDashboardSummary(buses, routes);

        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading dashboard data");
            MessageBoxAdv.Show($"Error loading dashboard data: {ex.Message}", "Error",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    /// <summary>
    /// Update dashboard summary using Syncfusion HubTile components for visual display
    /// </summary>
    private void UpdateDashboardSummary(System.Collections.Generic.List<Bus_Buddy.Services.BusInfo> buses, System.Collections.Generic.List<Bus_Buddy.Services.RouteInfo> routes)
    {
        try
        {
            // Calculate summary statistics
            var activeBuses = buses.Count(b => b.Status == "Active");
            var inServiceBuses = buses.Count(b => b.Status == "In Service");
            var maintenanceBuses = buses.Count(b => b.Status == "Maintenance");
            var totalCapacity = buses.Sum(b => b.Capacity);

            // Update the subtitle with current statistics
            if (subtitleLabel != null)
            {
                subtitleLabel.Text = $"Fleet: {buses.Count} buses | Routes: {routes.Count} | Active: {activeBuses} | Total Capacity: {totalCapacity:N0} passengers";
            }

            // Create or update dashboard summary tiles using Syncfusion HubTile
            CreateDashboardTiles(buses.Count, routes.Count, activeBuses, inServiceBuses, maintenanceBuses, totalCapacity);

            _logger.LogInformation("Dashboard summary updated successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating dashboard summary: {Message}", ex.Message);
        }
    }

    /// <summary>
    /// Create dashboard summary tiles using Syncfusion HubTile components
    /// </summary>
    private void CreateDashboardTiles(int totalBuses, int totalRoutes, int activeBuses, int inServiceBuses, int maintenanceBuses, int totalCapacity)
    {
        try
        {
            // Clear existing summary tiles if any
            var existingTiles = contentPanel?.Controls.OfType<Syncfusion.Windows.Forms.Tools.HubTile>().ToList();
            if (existingTiles?.Any() == true)
            {
                foreach (var tile in existingTiles)
                {
                    contentPanel?.Controls.Remove(tile);
                    tile.Dispose();
                }
            }

            // Create summary information panel
            var summaryPanel = new Syncfusion.Windows.Forms.Tools.GradientPanel();
            summaryPanel.Location = new System.Drawing.Point(750, 30);
            summaryPanel.Size = new System.Drawing.Size(420, 420);
            summaryPanel.BackgroundColor = new Syncfusion.Drawing.BrushInfo(System.Drawing.Color.FromArgb(248, 249, 250));
            summaryPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;

            // Summary title
            var summaryTitle = new Syncfusion.Windows.Forms.Tools.AutoLabel();
            summaryTitle.Text = "Fleet Summary";
            summaryTitle.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold);
            summaryTitle.ForeColor = System.Drawing.Color.FromArgb(46, 125, 185);
            summaryTitle.Location = new System.Drawing.Point(20, 15);
            summaryTitle.AutoSize = true;

            // Create HubTiles for key metrics using Syncfusion v30.1.37 HubTile
            var fleetTile = CreateMetricTile("Total Fleet", totalBuses.ToString(), "Vehicles", System.Drawing.Color.FromArgb(63, 81, 181), 20, 50);
            var routesTile = CreateMetricTile("Active Routes", totalRoutes.ToString(), "Routes", System.Drawing.Color.FromArgb(76, 175, 80), 220, 50);
            var activeTile = CreateMetricTile("Active Buses", activeBuses.ToString(), "In Service", System.Drawing.Color.FromArgb(255, 152, 0), 20, 150);
            var maintenanceTile = CreateMetricTile("Maintenance", maintenanceBuses.ToString(), "In Shop", System.Drawing.Color.FromArgb(244, 67, 54), 220, 150);
            var capacityTile = CreateMetricTile("Total Capacity", totalCapacity.ToString("N0"), "Passengers", System.Drawing.Color.FromArgb(156, 39, 176), 120, 250);

            // Add summary statistics labels
            var statsLabel = new Syncfusion.Windows.Forms.Tools.AutoLabel();
            statsLabel.Text = $"Fleet Utilization: {(activeBuses > 0 ? (double)inServiceBuses / activeBuses * 100 : 0):F1}%\n" +
                             $"Maintenance Rate: {(totalBuses > 0 ? (double)maintenanceBuses / totalBuses * 100 : 0):F1}%\n" +
                             $"Average Capacity: {(totalBuses > 0 ? (double)totalCapacity / totalBuses : 0):F0} seats/bus";
            statsLabel.Font = new System.Drawing.Font("Segoe UI", 9F);
            statsLabel.ForeColor = System.Drawing.Color.FromArgb(95, 99, 104);
            statsLabel.Location = new System.Drawing.Point(20, 350);
            statsLabel.AutoSize = true;

            // Add controls to summary panel
            summaryPanel.Controls.Add(summaryTitle);
            summaryPanel.Controls.Add(fleetTile);
            summaryPanel.Controls.Add(routesTile);
            summaryPanel.Controls.Add(activeTile);
            summaryPanel.Controls.Add(maintenanceTile);
            summaryPanel.Controls.Add(capacityTile);
            summaryPanel.Controls.Add(statsLabel);

            // Add summary panel to content panel
            contentPanel?.Controls.Add(summaryPanel);

            _logger.LogInformation("Dashboard tiles created successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating dashboard tiles: {Message}", ex.Message);
        }
    }

    /// <summary>
    /// Create a metric tile using Syncfusion HubTile for dashboard display
    /// </summary>
    private Syncfusion.Windows.Forms.Tools.HubTile CreateMetricTile(string title, string value, string subtitle, System.Drawing.Color color, int x, int y)
    {
        var tile = new Syncfusion.Windows.Forms.Tools.HubTile();

        // Configure HubTile properties according to Syncfusion v30.1.37 documentation
        tile.Banner.Text = title;
        tile.Title.Text = value;
        tile.Title.TextColor = System.Drawing.Color.White;
        tile.Title.Font = new System.Drawing.Font("Segoe UI", 16F, System.Drawing.FontStyle.Bold);
        tile.Body.Text = subtitle;
        tile.Body.TextColor = System.Drawing.Color.WhiteSmoke;
        tile.Body.Font = new System.Drawing.Font("Segoe UI", 9F);

        // Set tile appearance
        tile.Size = new System.Drawing.Size(180, 80);
        tile.Location = new System.Drawing.Point(x, y);
        tile.BackColor = color;

        // Enable visual enhancements
        tile.TileType = Syncfusion.Windows.Forms.Tools.HubTileType.DefaultTile;
        tile.ImageTransitionSpeed = 3000;
        tile.RotationTransitionSpeed = 2000;

        return tile;
    }

    #region Button Event Handlers

    // Management Module Event Handlers
    private void BusManagementButton_Click(object sender, EventArgs e)
    {
        try
        {
            _logger.LogInformation("Bus Management button clicked");

            // Open Bus Management form using the service container
            var busManagementForm = ServiceContainer.GetService<BusManagementForm>();
            busManagementForm.ShowDialog(this);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error opening Bus Management");
            MessageBoxAdv.Show($"Error opening Bus Management: {ex.Message}", "Error",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void DriverManagementButton_Click(object sender, EventArgs e)
    {
        try
        {
            _logger.LogInformation("Driver Management button clicked");

            // Open Driver Management form using the service container
            var driverManagementForm = ServiceContainer.GetService<DriverManagementForm>();
            driverManagementForm.ShowDialog(this);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error opening Driver Management");
            MessageBoxAdv.Show($"Error opening Driver Management: {ex.Message}", "Error",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void RouteManagementButton_Click(object sender, EventArgs e)
    {
        try
        {
            _logger.LogInformation("Route Management button clicked");

            // Open Route Management form using the service container
            var routeManagementForm = ServiceContainer.GetService<RouteManagementForm>();
            routeManagementForm.ShowDialog(this);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error opening Route Management");
            MessageBoxAdv.Show($"Error opening Route Management: {ex.Message}", "Error",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void ScheduleManagementButton_Click(object sender, EventArgs e)
    {
        try
        {
            _logger.LogInformation("Schedule Management button clicked");

            // Open Schedule Management form
            var scheduleForm = ServiceContainer.GetService<ScheduleManagementForm>();
            scheduleForm.ShowDialog();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error opening Schedule Management");
            MessageBoxAdv.Show($"Error opening Schedule Management: {ex.Message}", "Error",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void PassengerManagementButton_Click(object sender, EventArgs e)
    {
        try
        {
            _logger.LogInformation("Passenger Management button clicked");

            // Open Passenger Management form using the service container
            var passengerManagementForm = ServiceContainer.GetService<Bus_Buddy.Forms.PassengerManagementForm>();
            passengerManagementForm.ShowDialog(this);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error opening Passenger Management");
            MessageBoxAdv.Show($"Error opening Passenger Management: {ex.Message}", "Error",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void StudentManagementButton_Click(object sender, EventArgs e)
    {
        try
        {
            _logger.LogInformation("Student Management button clicked");

            // Open Student Management form using the service container
            var studentManagementForm = ServiceContainer.GetService<Bus_Buddy.Forms.StudentManagementForm>();
            studentManagementForm.ShowDialog(this);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error opening Student Management");
            MessageBoxAdv.Show($"Error opening Student Management: {ex.Message}", "Error",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void MaintenanceButton_Click(object sender, EventArgs e)
    {
        try
        {
            _logger.LogInformation("Maintenance button clicked");

            // Open Maintenance Management form using the service container
            var maintenanceForm = ServiceContainer.GetService<Bus_Buddy.Forms.MaintenanceManagementForm>();
            maintenanceForm.ShowDialog(this);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error opening Maintenance Management");
            MessageBoxAdv.Show($"Error opening Maintenance Management: {ex.Message}", "Error",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void FuelTrackingButton_Click(object sender, EventArgs e)
    {
        try
        {
            _logger.LogInformation("Fuel Tracking button clicked");

            // Open Fuel Management form using the service container
            var fuelForm = ServiceContainer.GetService<Bus_Buddy.Forms.FuelManagementForm>();
            fuelForm.ShowDialog(this);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error opening Fuel Tracking");
            MessageBoxAdv.Show($"Error opening Fuel Tracking: {ex.Message}", "Error",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void ActivityLogButton_Click(object sender, EventArgs e)
    {
        try
        {
            _logger.LogInformation("Activity Log button clicked");

            // TODO: Open Activity Log form
            MessageBoxAdv.Show("Activity Log functionality will be implemented here.\n\nThis will open a form to view:\n• System activities\n• User actions\n• Audit trails",
                "Activity Log", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error opening Activity Log");
            MessageBoxAdv.Show($"Error opening Activity Log: {ex.Message}", "Error",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void TicketManagementButton_Click(object sender, EventArgs e)
    {
        try
        {
            _logger.LogInformation("Ticket Management button clicked");

            // Open Ticket Management form using the service container
            var ticketForm = ServiceContainer.GetService<Bus_Buddy.Forms.TicketManagementForm>();
            ticketForm.ShowDialog(this);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error opening Ticket Management");
            MessageBoxAdv.Show($"Error opening Ticket Management: {ex.Message}", "Error",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    // Quick Action Event Handlers
    private async void RefreshButton_Click(object sender, EventArgs e)
    {
        try
        {
            _logger.LogInformation("Refresh button clicked");

            // Reload dashboard data
            await Task.Run(() => LoadDashboardDataAsync());

            MessageBoxAdv.Show("Data refreshed successfully", "Refresh Complete",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during refresh operation");
            MessageBoxAdv.Show($"Refresh error: {ex.Message}", "Error",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void ReportsButton_Click(object sender, EventArgs e)
    {
        try
        {
            _logger.LogInformation("Reports button clicked");

            // TODO: Open Reports form or dialog
            MessageBoxAdv.Show("Reports functionality will be implemented here.\n\nThis will provide:\n• Fleet reports\n• Financial reports\n• Performance analytics",
                "Reports", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error opening Reports");
            MessageBoxAdv.Show($"Reports error: {ex.Message}", "Error",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void SettingsButton_Click(object sender, EventArgs e)
    {
        try
        {
            _logger.LogInformation("Settings button clicked");

            // TODO: Open settings dialog
            MessageBoxAdv.Show("Settings functionality will be implemented here.\n\nThis will provide:\n• Application settings\n• Database configuration\n• User preferences",
                "Settings", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error opening settings");
            MessageBoxAdv.Show($"Settings error: {ex.Message}", "Error",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    /// <summary>
    /// Opens the Enhanced Grid Layout Demo form to showcase improved Syncfusion grid formatting
    /// </summary>
    private void OpenEnhancedGridDemo()
    {
        try
        {
            _logger.LogInformation("Opening Enhanced Grid Layout Demo");

            var demoForm = new EnhancedGridDemoForm();
            demoForm.Show(); // Use Show() instead of ShowDialog() to allow multiple instances
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error opening Enhanced Grid Demo");
            MessageBoxAdv.Show($"Error opening Enhanced Grid Demo: {ex.Message}", "Error",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    /// <summary>
    /// Opens the Visual Enhancement Showcase to demonstrate comprehensive visual improvements
    /// </summary>
    private void OpenVisualEnhancementShowcase()
    {
        try
        {
            _logger.LogInformation("Opening Visual Enhancement Showcase");

            var showcaseForm = ServiceContainer.GetService<VisualEnhancementShowcaseForm>();
            showcaseForm.Show(); // Use Show() to allow multiple instances
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error opening Visual Enhancement Showcase");
            MessageBoxAdv.Show($"Error opening Visual Enhancement Showcase: {ex.Message}", "Error",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    #endregion
}
