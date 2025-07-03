using Syncfusion.Windows.Forms;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using Bus_Buddy.Services;
using Bus_Buddy.Forms;
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
        // Set Syncfusion MetroForm styles
        this.MetroColor = System.Drawing.Color.FromArgb(11, 95, 178);
        this.CaptionBarColor = System.Drawing.Color.FromArgb(11, 95, 178);
        this.CaptionForeColor = System.Drawing.Color.White;
        this.Text = "BusBuddy - Dashboard";

        // Enable high DPI scaling for this form
        this.AutoScaleMode = AutoScaleMode.Dpi;
        this.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);

        _logger.LogInformation("Dashboard form initialized successfully");

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

            // TODO: Update UI with loaded data

        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading dashboard data");
            MessageBox.Show($"Error loading dashboard data: {ex.Message}", "Error",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
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
            MessageBox.Show($"Error opening Bus Management: {ex.Message}", "Error",
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
            MessageBox.Show($"Error opening Driver Management: {ex.Message}", "Error",
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
            MessageBox.Show($"Error opening Route Management: {ex.Message}", "Error",
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
            MessageBox.Show($"Error opening Schedule Management: {ex.Message}", "Error",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void PassengerManagementButton_Click(object sender, EventArgs e)
    {
        try
        {
            _logger.LogInformation("Passenger Management button clicked");

            // TODO: Open Passenger Management form
            MessageBox.Show("Passenger Management functionality will be implemented here.\n\nThis will open a form to manage:\n• Passenger information\n• Boarding history\n• Passenger preferences",
                "Passenger Management", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error opening Passenger Management");
            MessageBox.Show($"Error opening Passenger Management: {ex.Message}", "Error",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void StudentManagementButton_Click(object sender, EventArgs e)
    {
        try
        {
            _logger.LogInformation("Student Management button clicked");

            // TODO: Open Student Management form
            MessageBox.Show("Student Management functionality will be implemented here.\n\nThis will open a form to manage:\n• Student information\n• School assignments\n• Transportation needs",
                "Student Management", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error opening Student Management");
            MessageBox.Show($"Error opening Student Management: {ex.Message}", "Error",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void MaintenanceButton_Click(object sender, EventArgs e)
    {
        try
        {
            _logger.LogInformation("Maintenance button clicked");

            // TODO: Open Maintenance Management form
            MessageBox.Show("Maintenance Management functionality will be implemented here.\n\nThis will open a form to manage:\n• Maintenance records\n• Service schedules\n• Repair history",
                "Maintenance Management", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error opening Maintenance Management");
            MessageBox.Show($"Error opening Maintenance Management: {ex.Message}", "Error",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void FuelTrackingButton_Click(object sender, EventArgs e)
    {
        try
        {
            _logger.LogInformation("Fuel Tracking button clicked");

            // TODO: Open Fuel Tracking form
            MessageBox.Show("Fuel Tracking functionality will be implemented here.\n\nThis will open a form to manage:\n• Fuel consumption\n• Fuel costs\n• Efficiency reports",
                "Fuel Tracking", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error opening Fuel Tracking");
            MessageBox.Show($"Error opening Fuel Tracking: {ex.Message}", "Error",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void ActivityLogButton_Click(object sender, EventArgs e)
    {
        try
        {
            _logger.LogInformation("Activity Log button clicked");

            // TODO: Open Activity Log form
            MessageBox.Show("Activity Log functionality will be implemented here.\n\nThis will open a form to view:\n• System activities\n• User actions\n• Audit trails",
                "Activity Log", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error opening Activity Log");
            MessageBox.Show($"Error opening Activity Log: {ex.Message}", "Error",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void TicketManagementButton_Click(object sender, EventArgs e)
    {
        try
        {
            _logger.LogInformation("Ticket Management button clicked");

            // TODO: Open Ticket Management form
            MessageBox.Show("Ticket Management functionality will be implemented here.\n\nThis will open a form to manage:\n• Passenger tickets\n• Ticket sales\n• Fare collection",
                "Ticket Management", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error opening Ticket Management");
            MessageBox.Show($"Error opening Ticket Management: {ex.Message}", "Error",
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

            MessageBox.Show("Data refreshed successfully", "Refresh Complete",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during refresh operation");
            MessageBox.Show($"Refresh error: {ex.Message}", "Error",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void ReportsButton_Click(object sender, EventArgs e)
    {
        try
        {
            _logger.LogInformation("Reports button clicked");

            // TODO: Open Reports form or dialog
            MessageBox.Show("Reports functionality will be implemented here.\n\nThis will provide:\n• Fleet reports\n• Financial reports\n• Performance analytics",
                "Reports", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error opening Reports");
            MessageBox.Show($"Reports error: {ex.Message}", "Error",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void SettingsButton_Click(object sender, EventArgs e)
    {
        try
        {
            _logger.LogInformation("Settings button clicked");

            // TODO: Open settings dialog
            MessageBox.Show("Settings functionality will be implemented here.\n\nThis will provide:\n• Application settings\n• Database configuration\n• User preferences",
                "Settings", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error opening settings");
            MessageBox.Show($"Settings error: {ex.Message}", "Error",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    #endregion
}
