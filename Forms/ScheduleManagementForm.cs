using Bus_Buddy.Models;
using Bus_Buddy.Services;
using Microsoft.Extensions.Logging;
using Syncfusion.WinForms.Controls;
using Syncfusion.Windows.Forms.Schedule;
using Syncfusion.Schedule;
using System.ComponentModel;

namespace Bus_Buddy.Forms;

/// <summary>
/// Schedule Management Form - Scaffolded implementation using Syncfusion Schedule
/// This is a step-by-step implementation that incrementally builds complexity
/// </summary>
public partial class ScheduleManagementForm : SfForm
{
    #region Fields and Services
    private readonly IActivityService _activityService;
    private readonly IBusService _busService;
    private readonly ILogger<ScheduleManagementForm> _logger;

    // Data Collections
    private BindingList<Activity> _activities = null!;

    // UI Components
    private Panel panelHeader = null!;
    private Panel panelControls = null!;
    private Panel panelSchedule = null!;
    private Label labelTitle = null!;

    // Step 1: Basic Schedule Control
    private ScheduleControl scheduleControl = null!;

    // Step 2: Control Buttons (to be added)
    private SfButton btnAddActivity = null!;
    private SfButton btnEditActivity = null!;
    private SfButton btnDeleteActivity = null!;
    private SfButton btnRefresh = null!;

    // Step 3: View Controls (to be added)
    private ComboBox cmbViewType = null!;
    private DateTimePicker dtpNavigate = null!;

    #endregion

    #region Constructor
    public ScheduleManagementForm(
        IActivityService activityService,
        IBusService busService,
        ILogger<ScheduleManagementForm> logger)
    {
        _activityService = activityService;
        _busService = busService;
        _logger = logger;

        _logger.LogInformation("Initializing Schedule Management Form - Scaffolded Version");

        InitializeDataCollections();
        InitializeComponent();
        SetupLayout();
        InitializeScheduleControl();

        // Load data asynchronously after form is shown
        Load += async (s, e) => await LoadInitialDataAsync();
    }
    #endregion

    #region Step 1: Basic Setup
    private void InitializeDataCollections()
    {
        _activities = new BindingList<Activity>();
    }

    private void InitializeComponent()
    {
        SuspendLayout();

        // Form Configuration
        Text = "Bus Buddy - Activity Schedule Management";
        Size = new Size(1400, 900);
        StartPosition = FormStartPosition.CenterScreen;
        MinimumSize = new Size(1000, 600);
        WindowState = FormWindowState.Maximized;

        // Syncfusion Styling
        Style.TitleBar.BackColor = Color.FromArgb(46, 204, 113);
        Style.TitleBar.ForeColor = Color.White;
        Style.BackColor = Color.FromArgb(248, 249, 250);

        ResumeLayout(false);
        _logger.LogInformation("Form basic initialization completed");
    }

    private void SetupLayout()
    {
        try
        {
            CreateHeaderPanel();
            CreateControlsPanel();
            CreateSchedulePanel();

            _logger.LogInformation("Layout setup completed");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error setting up layout");
            ShowError($"Layout setup failed: {ex.Message}");
        }
    }
    #endregion

    #region Step 2: Panel Creation
    private void CreateHeaderPanel()
    {
        panelHeader = new Panel
        {
            Height = 70,
            Dock = DockStyle.Top,
            BackColor = Color.FromArgb(248, 249, 250),
            Padding = new Padding(20, 15, 20, 15)
        };

        labelTitle = new Label
        {
            Text = "📅 Activity Schedule Management",
            Font = new Font("Segoe UI", 18F, FontStyle.Bold),
            ForeColor = Color.FromArgb(46, 204, 113),
            AutoSize = true,
            Location = new Point(20, 20)
        };

        panelHeader.Controls.Add(labelTitle);
        Controls.Add(panelHeader);
    }

    private void CreateControlsPanel()
    {
        panelControls = new Panel
        {
            Height = 80,
            Dock = DockStyle.Top,
            BackColor = Color.White,
            BorderStyle = BorderStyle.FixedSingle,
            Padding = new Padding(20, 15, 20, 15)
        };

        SetupControlButtons();
        Controls.Add(panelControls);
    }

    private void CreateSchedulePanel()
    {
        panelSchedule = new Panel
        {
            Dock = DockStyle.Fill,
            BackColor = Color.White,
            Padding = new Padding(10)
        };

        Controls.Add(panelSchedule);
    }
    #endregion

    #region Step 3: Schedule Control Initialization
    private void InitializeScheduleControl()
    {
        try
        {
            _logger.LogInformation("Initializing Syncfusion Schedule Control");

            // Create the schedule control
            scheduleControl = new ScheduleControl
            {
                Dock = DockStyle.Fill,
                ScheduleType = ScheduleViewType.Month
            };

            // Configure basic properties
            ConfigureScheduleProperties();

            // Setup event handlers
            SetupScheduleEvents();

            // Add to container
            panelSchedule.Controls.Add(scheduleControl);

            _logger.LogInformation("Schedule control initialized successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to initialize schedule control");
            ShowError($"Schedule control initialization failed: {ex.Message}");
        }
    }

    private void ConfigureScheduleProperties()
    {
        // Basic schedule configuration
        scheduleControl.Culture = System.Globalization.CultureInfo.CurrentCulture;

        // Set basic appearance properties if available
        try
        {
            // Try to set basic properties that might be available
            if (scheduleControl.GetType().GetProperty("ShowNavigationPane") != null)
            {
                scheduleControl.GetType().GetProperty("ShowNavigationPane")?.SetValue(scheduleControl, true);
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Some schedule properties not available in this version");
        }
    }

    private void SetupScheduleEvents()
    {
        try
        {
            // Basic event handlers for schedule interactions
            scheduleControl.MouseClick += ScheduleControl_MouseClick;

            _logger.LogInformation("Schedule event handlers configured");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error configuring schedule events");
        }
    }
    #endregion

    #region Step 4: Control Buttons Setup
    private void SetupControlButtons()
    {
        // Add Activity Button
        btnAddActivity = new SfButton
        {
            Text = "➕ Add Activity",
            Size = new Size(130, 40),
            Location = new Point(20, 15),
            Style =
            {
                BackColor = Color.FromArgb(46, 204, 113),
                ForeColor = Color.White
            }
        };
        btnAddActivity.Click += BtnAddActivity_Click;

        // Edit Activity Button  
        btnEditActivity = new SfButton
        {
            Text = "✏️ Edit Activity",
            Size = new Size(130, 40),
            Location = new Point(160, 15),
            Style =
            {
                BackColor = Color.FromArgb(241, 196, 15),
                ForeColor = Color.White
            }
        };
        btnEditActivity.Click += BtnEditActivity_Click;

        // Delete Activity Button
        btnDeleteActivity = new SfButton
        {
            Text = "🗑️ Delete",
            Size = new Size(100, 40),
            Location = new Point(300, 15),
            Style =
            {
                BackColor = Color.FromArgb(231, 76, 60),
                ForeColor = Color.White
            }
        };
        btnDeleteActivity.Click += BtnDeleteActivity_Click;

        // Refresh Button
        btnRefresh = new SfButton
        {
            Text = "🔄 Refresh",
            Size = new Size(100, 40),
            Location = new Point(410, 15),
            Style =
            {
                BackColor = Color.FromArgb(155, 89, 182),
                ForeColor = Color.White
            }
        };
        btnRefresh.Click += BtnRefresh_Click;

        // View Type Selector
        var lblView = new Label
        {
            Text = "View:",
            Location = new Point(530, 25),
            Size = new Size(40, 20),
            Font = new Font("Segoe UI", 9F)
        };

        cmbViewType = new ComboBox
        {
            Location = new Point(575, 20),
            Size = new Size(100, 30),
            DropDownStyle = ComboBoxStyle.DropDownList
        };
        cmbViewType.Items.AddRange(new[] { "Day", "Week", "Month" });
        cmbViewType.SelectedIndex = 2; // Default to Month
        cmbViewType.SelectedIndexChanged += CmbViewType_SelectedIndexChanged;

        // Date Navigator
        var lblNavigate = new Label
        {
            Text = "Go to:",
            Location = new Point(690, 25),
            Size = new Size(45, 20),
            Font = new Font("Segoe UI", 9F)
        };

        dtpNavigate = new DateTimePicker
        {
            Location = new Point(740, 20),
            Size = new Size(150, 30),
            Value = DateTime.Today
        };
        dtpNavigate.ValueChanged += DtpNavigate_ValueChanged;

        // Add all controls to panel
        panelControls.Controls.AddRange(new Control[]
        {
            btnAddActivity, btnEditActivity, btnDeleteActivity, btnRefresh,
            lblView, cmbViewType, lblNavigate, dtpNavigate
        });
    }
    #endregion

    #region Step 5: Data Loading and Management
    private async Task LoadInitialDataAsync()
    {
        try
        {
            _logger.LogInformation("Loading initial activity data");

            // Load activities for current month and surrounding dates
            var startDate = DateTime.Today.AddMonths(-1).AddDays(-DateTime.Today.Day + 1);
            var endDate = DateTime.Today.AddMonths(2);

            var activities = await _activityService.GetActivitiesByDateRangeAsync(startDate, endDate);

            _activities.Clear();
            foreach (var activity in activities)
            {
                _activities.Add(activity);
                AddActivityToSchedule(activity);
            }

            scheduleControl.Refresh();
            _logger.LogInformation("Loaded {Count} activities into schedule", _activities.Count);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading initial data");
            ShowError($"Failed to load activity data: {ex.Message}");
        }
    }

    private void AddActivityToSchedule(Activity activity)
    {
        try
        {
            // For now, we'll add activities to a simple collection
            // and display them in a basic way until we determine the correct API
            _logger.LogInformation("Activity {ActivityId} would be displayed: {ActivityType} on {Date}",
                activity.ActivityId, activity.ActivityType, activity.ActivityDate);

            // TODO: Implement proper appointment creation once we determine the correct API
            // The schedule control needs to be properly configured with the right data source
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding activity {ActivityId} to schedule", activity.ActivityId);
        }
    }

    private Brush GetActivityTypeColor(string? activityType)
    {
        return activityType?.ToLower() switch
        {
            "morning" => Brushes.LightBlue,
            "afternoon" => Brushes.LightGreen,
            "field trip" => Brushes.LightYellow,
            _ => Brushes.LightGray
        };
    }
    #endregion

    #region Step 6: Event Handlers  
    private void BtnAddActivity_Click(object? sender, EventArgs e)
    {
        try
        {
            _logger.LogInformation("Add activity requested");
            // TODO: Open ActivityEditForm for new activity
            ShowInfo("Add Activity functionality will be implemented in the next step");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in add activity");
            ShowError($"Add activity failed: {ex.Message}");
        }
    }

    private void BtnEditActivity_Click(object? sender, EventArgs e)
    {
        try
        {
            _logger.LogInformation("Edit activity requested");
            // TODO: Get selected appointment and open edit form
            ShowInfo("Edit Activity functionality will be implemented in the next step");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in edit activity");
            ShowError($"Edit activity failed: {ex.Message}");
        }
    }

    private void BtnDeleteActivity_Click(object? sender, EventArgs e)
    {
        try
        {
            _logger.LogInformation("Delete activity requested");
            // TODO: Delete selected appointment
            ShowInfo("Delete Activity functionality will be implemented in the next step");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in delete activity");
            ShowError($"Delete activity failed: {ex.Message}");
        }
    }

    private async void BtnRefresh_Click(object? sender, EventArgs e)
    {
        try
        {
            // Clear existing activities and reload
            _activities.Clear();

            await LoadInitialDataAsync();
            ShowInfo("Schedule refreshed successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error refreshing schedule");
            ShowError($"Refresh failed: {ex.Message}");
        }
    }

    private void CmbViewType_SelectedIndexChanged(object? sender, EventArgs e)
    {
        try
        {
            var selectedView = cmbViewType.SelectedItem?.ToString();
            scheduleControl.ScheduleType = selectedView switch
            {
                "Day" => ScheduleViewType.Day,
                "Week" => ScheduleViewType.Week,
                "Month" => ScheduleViewType.Month,
                _ => ScheduleViewType.Month
            };

            _logger.LogInformation("Schedule view changed to {View}", selectedView);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error changing schedule view");
            ShowError($"View change failed: {ex.Message}");
        }
    }

    private void DtpNavigate_ValueChanged(object? sender, EventArgs e)
    {
        try
        {
            // TODO: Find correct property to navigate schedule date
            // scheduleControl.CalendarDate = dtpNavigate.Value;
            _logger.LogInformation("Schedule navigated to {Date}", dtpNavigate.Value);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error navigating schedule");
            ShowError($"Navigation failed: {ex.Message}");
        }
    }

    private void ScheduleControl_MouseClick(object? sender, MouseEventArgs e)
    {
        try
        {
            // Handle schedule clicks for context menus or selection
            _logger.LogDebug("Schedule clicked at {Location}", e.Location);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error handling schedule click");
        }
    }
    #endregion

    #region Utility Methods
    private void ShowError(string message)
    {
        _logger.LogError("Schedule Management Error: {Message}", message);
        MessageBox.Show(message, "Schedule Management Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
    }

    private void ShowInfo(string message)
    {
        _logger.LogInformation("Schedule Management Info: {Message}", message);
        MessageBox.Show(message, "Schedule Management", MessageBoxButtons.OK, MessageBoxIcon.Information);
    }

    private void ShowSuccess(string message)
    {
        _logger.LogInformation("Schedule Management Success: {Message}", message);
        MessageBox.Show(message, "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
    }
    #endregion
}
