using Syncfusion.Windows.Forms;
using Syncfusion.Windows.Forms.Tools;
using Syncfusion.WinForms.Controls;

namespace Bus_Buddy;

partial class Dashboard
{
    /// <summary>
    ///  Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    // Declare Syncfusion controls
    private GradientPanel mainPanel;
    private GradientPanel headerPanel;
    private GradientPanel contentPanel;
    private AutoLabel titleLabel;
    private AutoLabel subtitleLabel;

    // Management section buttons
    private SfButton busManagementButton;
    private SfButton driverManagementButton;
    private SfButton routeManagementButton;
    private SfButton scheduleManagementButton;
    private SfButton passengerManagementButton;
    private SfButton studentManagementButton;
    private SfButton maintenanceButton;
    private SfButton fuelTrackingButton;
    private SfButton activityLogButton;
    private SfButton ticketManagementButton;

    // Quick action buttons
    private SfButton refreshButton;
    private SfButton settingsButton;
    private SfButton reportsButton;

    /// <summary>
    ///  Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing)
    {
        if (disposing && (components != null))
        {
            components.Dispose();
        }
        base.Dispose(disposing);
    }

    #region Windows Form Designer generated code

    /// <summary>
    ///  Required method for Designer support - do not modify
    ///  the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
        this.components = new System.ComponentModel.Container();

        // Suspend layout for better performance during initialization
        this.SuspendLayout();

        // Initialize Syncfusion components
        this.mainPanel = new Syncfusion.Windows.Forms.Tools.GradientPanel();
        this.headerPanel = new Syncfusion.Windows.Forms.Tools.GradientPanel();
        this.contentPanel = new Syncfusion.Windows.Forms.Tools.GradientPanel();
        this.titleLabel = new Syncfusion.Windows.Forms.Tools.AutoLabel();
        this.subtitleLabel = new Syncfusion.Windows.Forms.Tools.AutoLabel();

        // Initialize management buttons
        this.busManagementButton = new Syncfusion.WinForms.Controls.SfButton();
        this.driverManagementButton = new Syncfusion.WinForms.Controls.SfButton();
        this.routeManagementButton = new Syncfusion.WinForms.Controls.SfButton();
        this.scheduleManagementButton = new Syncfusion.WinForms.Controls.SfButton();
        this.passengerManagementButton = new Syncfusion.WinForms.Controls.SfButton();
        this.studentManagementButton = new Syncfusion.WinForms.Controls.SfButton();
        this.maintenanceButton = new Syncfusion.WinForms.Controls.SfButton();
        this.fuelTrackingButton = new Syncfusion.WinForms.Controls.SfButton();
        this.activityLogButton = new Syncfusion.WinForms.Controls.SfButton();
        this.ticketManagementButton = new Syncfusion.WinForms.Controls.SfButton();

        // Initialize quick action buttons
        this.refreshButton = new Syncfusion.WinForms.Controls.SfButton();
        this.settingsButton = new Syncfusion.WinForms.Controls.SfButton();
        this.reportsButton = new Syncfusion.WinForms.Controls.SfButton();

        // Main Panel
        this.mainPanel.BorderStyle = System.Windows.Forms.BorderStyle.None;
        this.mainPanel.Dock = System.Windows.Forms.DockStyle.Fill;
        this.mainPanel.Location = new System.Drawing.Point(0, 0);
        this.mainPanel.Name = "mainPanel";
        this.mainPanel.Size = new System.Drawing.Size(1200, 800);
        this.mainPanel.TabIndex = 0;

        // Header Panel
        this.headerPanel.BorderStyle = System.Windows.Forms.BorderStyle.None;
        this.headerPanel.BackgroundColor = new Syncfusion.Drawing.BrushInfo(System.Drawing.Color.FromArgb(11, 95, 178));
        this.headerPanel.Dock = System.Windows.Forms.DockStyle.Top;
        this.headerPanel.Location = new System.Drawing.Point(0, 0);
        this.headerPanel.Name = "headerPanel";
        this.headerPanel.Size = new System.Drawing.Size(1200, 80);
        this.headerPanel.TabIndex = 1;

        // Title Label
        this.titleLabel.BackColor = System.Drawing.Color.Transparent;
        this.titleLabel.Font = new System.Drawing.Font("Segoe UI", 18F, System.Drawing.FontStyle.Bold);
        this.titleLabel.ForeColor = System.Drawing.Color.White;
        this.titleLabel.Location = new System.Drawing.Point(30, 15);
        this.titleLabel.Name = "titleLabel";
        this.titleLabel.Size = new System.Drawing.Size(200, 32);
        this.titleLabel.TabIndex = 0;
        this.titleLabel.Text = "Bus Buddy Dashboard";

        // Subtitle Label
        this.subtitleLabel.BackColor = System.Drawing.Color.Transparent;
        this.subtitleLabel.Font = new System.Drawing.Font("Segoe UI", 10F);
        this.subtitleLabel.ForeColor = System.Drawing.Color.LightGray;
        this.subtitleLabel.Location = new System.Drawing.Point(30, 50);
        this.subtitleLabel.Name = "subtitleLabel";
        this.subtitleLabel.Size = new System.Drawing.Size(300, 20);
        this.subtitleLabel.TabIndex = 1;
        this.subtitleLabel.Text = "Transportation Management System";

        // Content Panel
        this.contentPanel.BorderStyle = System.Windows.Forms.BorderStyle.None;
        this.contentPanel.Dock = System.Windows.Forms.DockStyle.Fill;
        this.contentPanel.Location = new System.Drawing.Point(0, 80);
        this.contentPanel.Name = "contentPanel";
        this.contentPanel.Size = new System.Drawing.Size(1200, 720);
        this.contentPanel.TabIndex = 2;
        this.contentPanel.Padding = new System.Windows.Forms.Padding(20);

        // Configure management buttons (3 columns, 4 rows layout)
        ConfigureManagementButton(this.busManagementButton, "Bus Management", "Manage fleet vehicles", 30, 30, 0);
        ConfigureManagementButton(this.driverManagementButton, "Driver Management", "Manage bus drivers", 270, 30, 1);
        ConfigureManagementButton(this.routeManagementButton, "Route Management", "Manage bus routes", 510, 30, 2);
        ConfigureManagementButton(this.scheduleManagementButton, "Schedule Management", "Manage bus schedules", 30, 180, 3);
        ConfigureManagementButton(this.passengerManagementButton, "Passenger Management", "Manage passengers", 270, 180, 4);
        ConfigureManagementButton(this.studentManagementButton, "Student Management", "Manage students", 510, 180, 5);
        ConfigureManagementButton(this.maintenanceButton, "Maintenance", "Vehicle maintenance records", 30, 330, 6);
        ConfigureManagementButton(this.fuelTrackingButton, "Fuel Tracking", "Track fuel consumption", 270, 330, 7);
        ConfigureManagementButton(this.activityLogButton, "Activity Log", "View system activities", 510, 330, 8);
        ConfigureManagementButton(this.ticketManagementButton, "Ticket Management", "Manage passenger tickets", 30, 480, 9);

        // Add event handlers
        this.busManagementButton.Click += new System.EventHandler(this.BusManagementButton_Click);
        this.driverManagementButton.Click += new System.EventHandler(this.DriverManagementButton_Click);
        this.routeManagementButton.Click += new System.EventHandler(this.RouteManagementButton_Click);
        this.scheduleManagementButton.Click += new System.EventHandler(this.ScheduleManagementButton_Click);
        this.passengerManagementButton.Click += new System.EventHandler(this.PassengerManagementButton_Click);
        this.studentManagementButton.Click += new System.EventHandler(this.StudentManagementButton_Click);
        this.maintenanceButton.Click += new System.EventHandler(this.MaintenanceButton_Click);
        this.fuelTrackingButton.Click += new System.EventHandler(this.FuelTrackingButton_Click);
        this.activityLogButton.Click += new System.EventHandler(this.ActivityLogButton_Click);
        this.ticketManagementButton.Click += new System.EventHandler(this.TicketManagementButton_Click);

        // Configure quick action buttons (bottom right)
        ConfigureQuickActionButton(this.refreshButton, "Refresh Data", 750, 480, 10, System.Drawing.Color.FromArgb(76, 175, 80));
        ConfigureQuickActionButton(this.reportsButton, "Reports", 870, 480, 11, System.Drawing.Color.FromArgb(156, 39, 176));
        ConfigureQuickActionButton(this.settingsButton, "Settings", 990, 480, 12, System.Drawing.Color.FromArgb(158, 158, 158));

        // Add event handlers for quick action buttons
        this.refreshButton.Click += new System.EventHandler(this.RefreshButton_Click);
        this.reportsButton.Click += new System.EventHandler(this.ReportsButton_Click);
        this.settingsButton.Click += new System.EventHandler(this.SettingsButton_Click);

        // Add controls to panels
        this.headerPanel.Controls.Add(this.titleLabel);
        this.headerPanel.Controls.Add(this.subtitleLabel);

        this.contentPanel.Controls.Add(this.busManagementButton);
        this.contentPanel.Controls.Add(this.driverManagementButton);
        this.contentPanel.Controls.Add(this.routeManagementButton);
        this.contentPanel.Controls.Add(this.scheduleManagementButton);
        this.contentPanel.Controls.Add(this.passengerManagementButton);
        this.contentPanel.Controls.Add(this.studentManagementButton);
        this.contentPanel.Controls.Add(this.maintenanceButton);
        this.contentPanel.Controls.Add(this.fuelTrackingButton);
        this.contentPanel.Controls.Add(this.activityLogButton);
        this.contentPanel.Controls.Add(this.ticketManagementButton);
        this.contentPanel.Controls.Add(this.refreshButton);
        this.contentPanel.Controls.Add(this.reportsButton);
        this.contentPanel.Controls.Add(this.settingsButton);

        this.mainPanel.Controls.Add(this.contentPanel);
        this.mainPanel.Controls.Add(this.headerPanel);

        // Form settings
        this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
        this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
        this.ClientSize = new System.Drawing.Size(1200, 800);
        this.Controls.Add(this.mainPanel);
        this.Text = "Bus Buddy - Dashboard";
        this.MinimumSize = new System.Drawing.Size(1000, 600);
        this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
        this.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);

        // Resume layout
        this.ResumeLayout(false);
    }

    private void ConfigureManagementButton(SfButton button, string title, string description, int x, int y, int tabIndex)
    {
        button.AccessibleName = title;
        button.BackColor = System.Drawing.Color.FromArgb(63, 81, 181);
        button.ForeColor = System.Drawing.Color.White;
        button.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
        button.Location = new System.Drawing.Point(x, y);
        button.Name = title.Replace(" ", "").ToLower() + "Button";
        button.Size = new System.Drawing.Size(220, 120);
        button.TabIndex = tabIndex;
        button.Text = title;
        button.UseVisualStyleBackColor = false;
    }

    private void ConfigureQuickActionButton(SfButton button, string title, int x, int y, int tabIndex, System.Drawing.Color color)
    {
        button.AccessibleName = title;
        button.BackColor = color;
        button.ForeColor = System.Drawing.Color.White;
        button.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
        button.Location = new System.Drawing.Point(x, y);
        button.Name = title.Replace(" ", "").ToLower() + "Button";
        button.Size = new System.Drawing.Size(100, 40);
        button.TabIndex = tabIndex;
        button.Text = title;
        button.UseVisualStyleBackColor = false;
    }

    #endregion
}
