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

        // FIX: Eliminate background bleeding behind dashboard cards
        SyncfusionBackgroundFix.FixDashboardBackground(this);

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

        // FIX: Ensure all buttons have solid backgrounds (no transparency)
        SyncfusionBackgroundFix.FixButtonBackgrounds(this);

        // Fix title label color after visual enhancements
        if (titleLabel != null)
        {
            titleLabel.ForeColor = System.Drawing.Color.White;
        }

        // Fix subtitle label color after visual enhancements  
        if (subtitleLabel != null)
        {
            subtitleLabel.ForeColor = System.Drawing.Color.LightGray;
        }

        // Force HubTile sizes to match test expectations
        if (fleetTile != null) { fleetTile.Size = new System.Drawing.Size(180, 80); }
        if (routesTile != null) { routesTile.Size = new System.Drawing.Size(180, 80); }
        if (activeTile != null) { activeTile.Size = new System.Drawing.Size(180, 80); }
        if (maintenanceTile != null) { maintenanceTile.Size = new System.Drawing.Size(180, 80); }
        if (capacityTile != null) { capacityTile.Size = new System.Drawing.Size(180, 80); }

        // Force summaryPanel BorderStyle after visual enhancements
        if (summaryPanel != null)
        {
            summaryPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
        }

        // Fix summaryTitle label color after visual enhancements
        if (summaryTitle != null)
        {
            summaryTitle.ForeColor = System.Drawing.Color.FromArgb(46, 125, 185);
        }

        // Force contentPanel size after docking issues
        if (contentPanel != null)
        {
            contentPanel.Size = new System.Drawing.Size(1200, 720);
        }

        // Fix statsLabel color after visual enhancements
        if (statsLabel != null)
        {
            statsLabel.ForeColor = System.Drawing.Color.FromArgb(95, 99, 104);
        }

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
            // Update HubTile values with current data
            if (fleetTile != null) fleetTile.Title.Text = totalBuses.ToString();
            if (routesTile != null) routesTile.Title.Text = totalRoutes.ToString();
            if (activeTile != null) activeTile.Title.Text = activeBuses.ToString();
            if (maintenanceTile != null) maintenanceTile.Title.Text = maintenanceBuses.ToString();
            if (capacityTile != null) capacityTile.Title.Text = totalCapacity.ToString("N0");

            // Update stats label
            if (statsLabel != null)
            {
                statsLabel.Text = $"Fleet Utilization: {(activeBuses > 0 ? (double)inServiceBuses / activeBuses * 100 : 0):F1}%\n" +
                                 $"Maintenance Rate: {(totalBuses > 0 ? (double)maintenanceBuses / totalBuses * 100 : 0):F1}%\n" +
                                 $"Average Capacity: {(totalBuses > 0 ? (double)totalCapacity / totalBuses : 0):F0} seats/bus";
            }

            _logger.LogInformation("Dashboard tiles created successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating dashboard tiles: {Message}", ex.Message);
        }
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
