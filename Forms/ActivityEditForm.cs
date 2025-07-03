using Bus_Buddy.Models;
using Bus_Buddy.Services;
using Microsoft.Extensions.Logging;
using Syncfusion.WinForms.Controls;
using System.ComponentModel;

namespace Bus_Buddy.Forms;

/// <summary>
/// Form for adding/editing activities using Syncfusion components
/// </summary>
public partial class ActivityEditForm : SfForm
{
    private readonly IActivityService _activityService;
    private readonly IBusService _busService;
    private readonly ILogger _logger;
    private Activity? _activity;

    // Controls
    private Label labelTitle = null!;
    private Label labelActivityDate = null!;
    private DateTimePicker datePickerActivityDate = null!;
    private Label labelActivityType = null!;
    private ComboBox comboBoxActivityType = null!;
    private Label labelVehicle = null!;
    private ComboBox comboBoxVehicle = null!;
    private Label labelRoute = null!;
    private ComboBox comboBoxRoute = null!;
    private Label labelDriver = null!;
    private ComboBox comboBoxDriver = null!;
    private Label labelStartTime = null!;
    private DateTimePicker timePickerStart = null!;
    private Label labelEndTime = null!;
    private DateTimePicker timePickerEnd = null!;
    private Label labelStudentsCount = null!;
    private NumericUpDown numericStudentsCount = null!;
    private Label labelStartOdometer = null!;
    private NumericUpDown numericStartOdometer = null!;
    private Label labelEndOdometer = null!;
    private NumericUpDown numericEndOdometer = null!;
    private Label labelNotes = null!;
    private TextBox textBoxNotes = null!;
    private SfButton sfButtonSave = null!;
    private SfButton sfButtonCancel = null!;

    public ActivityEditForm(
        IActivityService activityService,
        IBusService busService,
        ILogger logger,
        Activity? activity = null)
    {
        _activityService = activityService;
        _busService = busService;
        _logger = logger;
        _activity = activity;

        InitializeComponent();
        LoadComboBoxData();

        if (_activity != null)
        {
            LoadActivityData();
        }
    }

    private void InitializeComponent()
    {
        SuspendLayout();

        // Form settings
        Text = _activity == null ? "Add Activity - Bus Buddy" : "Edit Activity - Bus Buddy";
        Size = new Size(600, 700);
        StartPosition = FormStartPosition.CenterParent;
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;

        // Apply Syncfusion styling
        Style.TitleBar.BackColor = Color.FromArgb(46, 204, 113);
        Style.TitleBar.ForeColor = Color.White;

        int yPos = 20;
        const int leftMargin = 20;
        const int labelWidth = 120;
        const int controlWidth = 200;
        const int spacing = 35;

        // Title
        labelTitle = new Label
        {
            Text = _activity == null ? "Add New Activity" : "Edit Activity",
            Font = new Font("Segoe UI", 14F, FontStyle.Bold),
            ForeColor = Color.FromArgb(46, 204, 113),
            Location = new Point(leftMargin, yPos),
            Size = new Size(400, 25)
        };
        Controls.Add(labelTitle);
        yPos += 40;

        // Activity Date
        labelActivityDate = new Label
        {
            Text = "Activity Date:",
            Location = new Point(leftMargin, yPos + 3),
            Size = new Size(labelWidth, 20),
            Font = new Font("Segoe UI", 9F)
        };
        Controls.Add(labelActivityDate);

        datePickerActivityDate = new DateTimePicker
        {
            Location = new Point(leftMargin + labelWidth + 10, yPos),
            Size = new Size(controlWidth, 23),
            Format = DateTimePickerFormat.Short,
            Value = DateTime.Today
        };
        Controls.Add(datePickerActivityDate);
        yPos += spacing;

        // Activity Type
        labelActivityType = new Label
        {
            Text = "Activity Type:",
            Location = new Point(leftMargin, yPos + 3),
            Size = new Size(labelWidth, 20),
            Font = new Font("Segoe UI", 9F)
        };
        Controls.Add(labelActivityType);

        comboBoxActivityType = new ComboBox
        {
            Location = new Point(leftMargin + labelWidth + 10, yPos),
            Size = new Size(controlWidth, 23),
            DropDownStyle = ComboBoxStyle.DropDownList
        };
        comboBoxActivityType.Items.AddRange(new[] { "Morning", "Afternoon", "Field Trip" });
        Controls.Add(comboBoxActivityType);
        yPos += spacing;

        // Vehicle
        labelVehicle = new Label
        {
            Text = "Vehicle:",
            Location = new Point(leftMargin, yPos + 3),
            Size = new Size(labelWidth, 20),
            Font = new Font("Segoe UI", 9F)
        };
        Controls.Add(labelVehicle);

        comboBoxVehicle = new ComboBox
        {
            Location = new Point(leftMargin + labelWidth + 10, yPos),
            Size = new Size(controlWidth, 23),
            DropDownStyle = ComboBoxStyle.DropDownList,
            DisplayMember = "BusNumber",
            ValueMember = "BusId"
        };
        Controls.Add(comboBoxVehicle);
        yPos += spacing;

        // Route
        labelRoute = new Label
        {
            Text = "Route:",
            Location = new Point(leftMargin, yPos + 3),
            Size = new Size(labelWidth, 20),
            Font = new Font("Segoe UI", 9F)
        };
        Controls.Add(labelRoute);

        comboBoxRoute = new ComboBox
        {
            Location = new Point(leftMargin + labelWidth + 10, yPos),
            Size = new Size(controlWidth, 23),
            DropDownStyle = ComboBoxStyle.DropDownList,
            DisplayMember = "RouteName",
            ValueMember = "RouteId"
        };
        Controls.Add(comboBoxRoute);
        yPos += spacing;

        // Driver
        labelDriver = new Label
        {
            Text = "Driver:",
            Location = new Point(leftMargin, yPos + 3),
            Size = new Size(labelWidth, 20),
            Font = new Font("Segoe UI", 9F)
        };
        Controls.Add(labelDriver);

        comboBoxDriver = new ComboBox
        {
            Location = new Point(leftMargin + labelWidth + 10, yPos),
            Size = new Size(controlWidth, 23),
            DropDownStyle = ComboBoxStyle.DropDownList,
            DisplayMember = "DriverName",
            ValueMember = "DriverId"
        };
        Controls.Add(comboBoxDriver);
        yPos += spacing;

        // Start Time
        labelStartTime = new Label
        {
            Text = "Start Time:",
            Location = new Point(leftMargin, yPos + 3),
            Size = new Size(labelWidth, 20),
            Font = new Font("Segoe UI", 9F)
        };
        Controls.Add(labelStartTime);

        timePickerStart = new DateTimePicker
        {
            Location = new Point(leftMargin + labelWidth + 10, yPos),
            Size = new Size(controlWidth, 23),
            Format = DateTimePickerFormat.Time,
            ShowUpDown = true,
            Value = DateTime.Today.AddHours(7)
        };
        Controls.Add(timePickerStart);
        yPos += spacing;

        // End Time
        labelEndTime = new Label
        {
            Text = "End Time:",
            Location = new Point(leftMargin, yPos + 3),
            Size = new Size(labelWidth, 20),
            Font = new Font("Segoe UI", 9F)
        };
        Controls.Add(labelEndTime);

        timePickerEnd = new DateTimePicker
        {
            Location = new Point(leftMargin + labelWidth + 10, yPos),
            Size = new Size(controlWidth, 23),
            Format = DateTimePickerFormat.Time,
            ShowUpDown = true,
            Value = DateTime.Today.AddHours(8)
        };
        Controls.Add(timePickerEnd);
        yPos += spacing;

        // Students Count
        labelStudentsCount = new Label
        {
            Text = "Students Count:",
            Location = new Point(leftMargin, yPos + 3),
            Size = new Size(labelWidth, 20),
            Font = new Font("Segoe UI", 9F)
        };
        Controls.Add(labelStudentsCount);

        numericStudentsCount = new NumericUpDown
        {
            Location = new Point(leftMargin + labelWidth + 10, yPos),
            Size = new Size(controlWidth, 23),
            Minimum = 0,
            Maximum = 99,
            Value = 0
        };
        Controls.Add(numericStudentsCount);
        yPos += spacing;

        // Start Odometer
        labelStartOdometer = new Label
        {
            Text = "Start Odometer:",
            Location = new Point(leftMargin, yPos + 3),
            Size = new Size(labelWidth, 20),
            Font = new Font("Segoe UI", 9F)
        };
        Controls.Add(labelStartOdometer);

        numericStartOdometer = new NumericUpDown
        {
            Location = new Point(leftMargin + labelWidth + 10, yPos),
            Size = new Size(controlWidth, 23),
            Minimum = 0,
            Maximum = 999999,
            DecimalPlaces = 0
        };
        Controls.Add(numericStartOdometer);
        yPos += spacing;

        // End Odometer
        labelEndOdometer = new Label
        {
            Text = "End Odometer:",
            Location = new Point(leftMargin, yPos + 3),
            Size = new Size(labelWidth, 20),
            Font = new Font("Segoe UI", 9F)
        };
        Controls.Add(labelEndOdometer);

        numericEndOdometer = new NumericUpDown
        {
            Location = new Point(leftMargin + labelWidth + 10, yPos),
            Size = new Size(controlWidth, 23),
            Minimum = 0,
            Maximum = 999999,
            DecimalPlaces = 0
        };
        Controls.Add(numericEndOdometer);
        yPos += spacing;

        // Notes
        labelNotes = new Label
        {
            Text = "Notes:",
            Location = new Point(leftMargin, yPos + 3),
            Size = new Size(labelWidth, 20),
            Font = new Font("Segoe UI", 9F)
        };
        Controls.Add(labelNotes);

        textBoxNotes = new TextBox
        {
            Location = new Point(leftMargin + labelWidth + 10, yPos),
            Size = new Size(controlWidth + 150, 80),
            Multiline = true,
            ScrollBars = ScrollBars.Vertical
        };
        Controls.Add(textBoxNotes);
        yPos += 100;

        // Buttons
        sfButtonSave = new SfButton
        {
            Text = "Save",
            Location = new Point(leftMargin + labelWidth + 10, yPos),
            Size = new Size(100, 35),
            Style = { BackColor = Color.FromArgb(46, 204, 113), ForeColor = Color.White }
        };
        sfButtonSave.Click += SfButtonSave_Click;
        Controls.Add(sfButtonSave);

        sfButtonCancel = new SfButton
        {
            Text = "Cancel",
            Location = new Point(leftMargin + labelWidth + 120, yPos),
            Size = new Size(100, 35),
            Style = { BackColor = Color.FromArgb(231, 76, 60), ForeColor = Color.White }
        };
        sfButtonCancel.Click += SfButtonCancel_Click;
        Controls.Add(sfButtonCancel);

        ResumeLayout(false);
    }

    private async void LoadComboBoxData()
    {
        try
        {
            // Load buses
            var buses = await _busService.GetAllBusesAsync();
            comboBoxVehicle.DataSource = buses.Where(b => b.Status == "Active").ToList();

            // Load routes (you'll need to implement IRouteService)
            // var routes = await _routeService.GetAllRoutesAsync();
            // comboBoxRoute.DataSource = routes.Where(r => r.Active).ToList();

            // Load drivers (you'll need to implement IDriverService)
            // var drivers = await _driverService.GetAllDriversAsync();
            // comboBoxDriver.DataSource = drivers.Where(d => d.Active).ToList();

            _logger.LogInformation("Combo box data loaded successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading combo box data");
            MessageBox.Show($"Error loading data: {ex.Message}", "Error",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void LoadActivityData()
    {
        if (_activity == null) return;

        try
        {
            datePickerActivityDate.Value = _activity.ActivityDate;
            comboBoxActivityType.SelectedItem = _activity.ActivityType;

            if (_activity.VehicleId > 0)
                comboBoxVehicle.SelectedValue = _activity.VehicleId;

            if (_activity.RouteId > 0)
                comboBoxRoute.SelectedValue = _activity.RouteId;

            if (_activity.DriverId > 0)
                comboBoxDriver.SelectedValue = _activity.DriverId;

            if (_activity.StartTime.HasValue)
                timePickerStart.Value = DateTime.Today.Add(_activity.StartTime.Value);

            if (_activity.EndTime.HasValue)
                timePickerEnd.Value = DateTime.Today.Add(_activity.EndTime.Value);

            numericStudentsCount.Value = _activity.StudentsCount ?? 0;
            numericStartOdometer.Value = _activity.StartOdometer ?? 0;
            numericEndOdometer.Value = _activity.EndOdometer ?? 0;
            textBoxNotes.Text = _activity.Notes ?? string.Empty;

            _logger.LogInformation("Activity data loaded for editing");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading activity data");
            MessageBox.Show($"Error loading activity data: {ex.Message}", "Error",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private async void SfButtonSave_Click(object? sender, EventArgs e)
    {
        try
        {
            if (!ValidateInput())
                return;

            var activity = _activity ?? new Activity();

            activity.ActivityDate = datePickerActivityDate.Value.Date;
            activity.ActivityType = comboBoxActivityType.SelectedItem?.ToString() ?? string.Empty;
            activity.VehicleId = (int)(comboBoxVehicle.SelectedValue ?? 0);
            activity.RouteId = (int)(comboBoxRoute.SelectedValue ?? 0);
            activity.DriverId = (int)(comboBoxDriver.SelectedValue ?? 0);
            activity.StartTime = timePickerStart.Value.TimeOfDay;
            activity.EndTime = timePickerEnd.Value.TimeOfDay;
            activity.StudentsCount = (int)numericStudentsCount.Value;
            activity.StartOdometer = (int)numericStartOdometer.Value;
            activity.EndOdometer = (int)numericEndOdometer.Value;
            activity.Notes = textBoxNotes.Text.Trim();

            if (_activity == null)
            {
                await _activityService.CreateActivityAsync(activity);
                MessageBox.Show("Activity created successfully!", "Success",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                await _activityService.UpdateActivityAsync(activity);
                MessageBox.Show("Activity updated successfully!", "Success",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

            DialogResult = DialogResult.OK;
            Close();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error saving activity");
            MessageBox.Show($"Error saving activity: {ex.Message}", "Error",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void SfButtonCancel_Click(object? sender, EventArgs e)
    {
        DialogResult = DialogResult.Cancel;
        Close();
    }

    private bool ValidateInput()
    {
        if (string.IsNullOrEmpty(comboBoxActivityType.SelectedItem?.ToString()))
        {
            MessageBox.Show("Please select an activity type.", "Validation Error",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
            comboBoxActivityType.Focus();
            return false;
        }

        if (comboBoxVehicle.SelectedValue == null)
        {
            MessageBox.Show("Please select a vehicle.", "Validation Error",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
            comboBoxVehicle.Focus();
            return false;
        }

        if (comboBoxRoute.SelectedValue == null)
        {
            MessageBox.Show("Please select a route.", "Validation Error",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
            comboBoxRoute.Focus();
            return false;
        }

        if (comboBoxDriver.SelectedValue == null)
        {
            MessageBox.Show("Please select a driver.", "Validation Error",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
            comboBoxDriver.Focus();
            return false;
        }

        if (timePickerEnd.Value <= timePickerStart.Value)
        {
            MessageBox.Show("End time must be after start time.", "Validation Error",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
            timePickerEnd.Focus();
            return false;
        }

        if (numericEndOdometer.Value <= numericStartOdometer.Value)
        {
            MessageBox.Show("End odometer must be greater than start odometer.", "Validation Error",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
            numericEndOdometer.Focus();
            return false;
        }

        return true;
    }
}
