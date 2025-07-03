using Bus_Buddy.Models;
using Bus_Buddy.Services;
using Microsoft.Extensions.Logging;
using Syncfusion.Windows.Forms;
using Syncfusion.Windows.Forms.Tools;
using Syncfusion.WinForms.Controls;
using System.ComponentModel.DataAnnotations;

namespace Bus_Buddy.Forms;

/// <summary>
/// Route Add/Edit Form for creating and editing route records
/// Demonstrates comprehensive data entry form with validation using Syncfusion controls
/// Works with Route entities for daily route management
/// </summary>
public partial class RouteEditForm : MetroForm
{
    #region Fields and Services
    private readonly ILogger<RouteEditForm> _logger;
    private readonly IBusService _busService;
    private readonly Route? _existingRoute;
    private readonly bool _isEditMode;

    // Form result properties
    public Route? EditedRoute { get; private set; }
    public bool IsDataSaved { get; private set; }

    // Available data for dropdowns
    private List<Bus> _buses = null!;
    private List<Driver> _drivers = null!;

    #endregion

    #region UI Controls - Route Information
    private Syncfusion.Windows.Forms.Tools.GradientPanel panelRoute = null!;
    private Syncfusion.Windows.Forms.Tools.AutoLabel lblRouteInfo = null!;

    private Syncfusion.Windows.Forms.Tools.DateTimePickerAdv dtpDate = null!;
    private Syncfusion.Windows.Forms.Tools.TextBoxExt txtRouteName = null!;
    private Syncfusion.Windows.Forms.Tools.TextBoxExt txtDescription = null!;
    private CheckBoxAdv chkIsActive = null!;

    #endregion

    #region UI Controls - AM Route Information
    private Syncfusion.Windows.Forms.Tools.GradientPanel panelAMRoute = null!;
    private Syncfusion.Windows.Forms.Tools.AutoLabel lblAMRouteInfo = null!;

    private ComboBoxAdv cmbAMVehicle = null!;
    private Syncfusion.Windows.Forms.Tools.TextBoxExt txtAMBeginMiles = null!;
    private Syncfusion.Windows.Forms.Tools.TextBoxExt txtAMEndMiles = null!;
    private Syncfusion.Windows.Forms.Tools.TextBoxExt txtAMRiders = null!;
    private ComboBoxAdv cmbAMDriver = null!;

    #endregion

    #region UI Controls - PM Route Information
    private Syncfusion.Windows.Forms.Tools.GradientPanel panelPMRoute = null!;
    private Syncfusion.Windows.Forms.Tools.AutoLabel lblPMRouteInfo = null!;

    private ComboBoxAdv cmbPMVehicle = null!;
    private Syncfusion.Windows.Forms.Tools.TextBoxExt txtPMBeginMiles = null!;
    private Syncfusion.Windows.Forms.Tools.TextBoxExt txtPMEndMiles = null!;
    private Syncfusion.Windows.Forms.Tools.TextBoxExt txtPMRiders = null!;
    private ComboBoxAdv cmbPMDriver = null!;

    #endregion

    #region UI Controls - Actions
    private SfButton btnSave = null!;
    private SfButton btnCancel = null!;
    private SfButton btnValidate = null!;

    #endregion

    #region Constructors

    /// <summary>
    /// Constructor for adding new route
    /// </summary>
    public RouteEditForm(
        ILogger<RouteEditForm> logger,
        IBusService busService)
        : this(logger, busService, null)
    {
    }

    /// <summary>
    /// Constructor for editing existing route
    /// </summary>
    public RouteEditForm(
        ILogger<RouteEditForm> logger,
        IBusService busService,
        Route? existingRoute)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _busService = busService ?? throw new ArgumentNullException(nameof(busService));
        _existingRoute = existingRoute;
        _isEditMode = existingRoute != null;

        _logger.LogInformation("Initializing RouteEditForm in {Mode} mode",
            _isEditMode ? "Edit" : "Add");

        InitializeComponent();
        SetupForm();
        LoadDataAsync();

        if (_isEditMode && _existingRoute != null)
        {
            PopulateFormData(_existingRoute);
        }
        else
        {
            SetDefaultValues();
        }
    }

    #endregion

    #region Form Setup

    private void SetupForm()
    {
        // Configure MetroForm properties
        this.Text = _isEditMode ? "Edit Route" : "Add New Route";
        this.Size = new Size(800, 600);
        this.MinimumSize = new Size(700, 500);
        this.StartPosition = FormStartPosition.CenterParent;
        this.MaximizeBox = false;
        this.ShowIcon = false;

        // Apply Syncfusion styling
        this.MetroColor = Color.FromArgb(46, 125, 185);
        this.CaptionBarColor = Color.FromArgb(46, 125, 185);
        this.CaptionForeColor = Color.White;

        _logger.LogInformation("RouteEditForm setup completed");
    }

    private void InitializeComponent()
    {
        this.SuspendLayout();

        // Main layout with scrollable content
        var mainPanel = new Panel();
        mainPanel.Dock = DockStyle.Fill;
        mainPanel.AutoScroll = true;
        mainPanel.BackColor = Color.FromArgb(250, 250, 250);

        // Create panels
        CreateRouteInfoPanel();
        CreateAMRoutePanel();
        CreatePMRoutePanel();
        CreateActionButtons();

        // Layout panels vertically
        int yPosition = 10;
        int panelSpacing = 15;

        panelRoute.Location = new Point(10, yPosition);
        mainPanel.Controls.Add(panelRoute);
        yPosition += panelRoute.Height + panelSpacing;

        panelAMRoute.Location = new Point(10, yPosition);
        mainPanel.Controls.Add(panelAMRoute);
        yPosition += panelAMRoute.Height + panelSpacing;

        panelPMRoute.Location = new Point(10, yPosition);
        mainPanel.Controls.Add(panelPMRoute);
        yPosition += panelPMRoute.Height + panelSpacing;

        // Add action buttons at bottom
        var buttonPanel = new Panel();
        buttonPanel.Size = new Size(780, 60);
        buttonPanel.Location = new Point(10, yPosition);
        buttonPanel.BackColor = Color.FromArgb(248, 249, 250);

        btnValidate.Location = new Point(450, 15);
        btnSave.Location = new Point(550, 15);
        btnCancel.Location = new Point(650, 15);

        buttonPanel.Controls.AddRange(new Control[] { btnValidate, btnSave, btnCancel });
        mainPanel.Controls.Add(buttonPanel);

        this.Controls.Add(mainPanel);
        this.ResumeLayout(false);
    }

    #endregion

    #region Panel Creation

    private void CreateRouteInfoPanel()
    {
        panelRoute = new GradientPanel();
        panelRoute.Size = new Size(780, 150);
        panelRoute.BackgroundColor = new Syncfusion.Drawing.BrushInfo(Color.White);
        panelRoute.BorderStyle = BorderStyle.FixedSingle;

        lblRouteInfo = new AutoLabel();
        lblRouteInfo.Text = "🚌 Route Information";
        lblRouteInfo.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
        lblRouteInfo.ForeColor = Color.FromArgb(46, 125, 185);
        lblRouteInfo.Location = new Point(10, 10);

        // Date
        var lblDate = new AutoLabel();
        lblDate.Text = "Date:";
        lblDate.Location = new Point(20, 45);
        dtpDate = new DateTimePickerAdv();
        dtpDate.Size = new Size(150, 25);
        dtpDate.Location = new Point(140, 42);

        // Route Name
        var lblRouteName = new AutoLabel();
        lblRouteName.Text = "Route Name:";
        lblRouteName.Location = new Point(320, 45);
        txtRouteName = new TextBoxExt();
        txtRouteName.Size = new Size(200, 25);
        txtRouteName.Location = new Point(420, 42);

        // Description
        var lblDescription = new AutoLabel();
        lblDescription.Text = "Description:";
        lblDescription.Location = new Point(20, 80);
        txtDescription = new TextBoxExt();
        txtDescription.Size = new Size(400, 25);
        txtDescription.Location = new Point(140, 77);

        // Active Status
        chkIsActive = new CheckBoxAdv();
        chkIsActive.Text = "Active Route";
        chkIsActive.Size = new Size(120, 25);
        chkIsActive.Location = new Point(20, 115);
        chkIsActive.Checked = true;

        panelRoute.Controls.AddRange(new Control[] {
            lblRouteInfo, lblDate, dtpDate, lblRouteName, txtRouteName,
            lblDescription, txtDescription, chkIsActive
        });
    }

    private void CreateAMRoutePanel()
    {
        panelAMRoute = new GradientPanel();
        panelAMRoute.Size = new Size(780, 180);
        panelAMRoute.BackgroundColor = new Syncfusion.Drawing.BrushInfo(Color.White);
        panelAMRoute.BorderStyle = BorderStyle.FixedSingle;

        lblAMRouteInfo = new AutoLabel();
        lblAMRouteInfo.Text = "🌅 AM Route Details";
        lblAMRouteInfo.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
        lblAMRouteInfo.ForeColor = Color.FromArgb(46, 125, 185);
        lblAMRouteInfo.Location = new Point(10, 10);

        // AM Vehicle
        var lblAMVehicle = new AutoLabel();
        lblAMVehicle.Text = "AM Vehicle:";
        lblAMVehicle.Location = new Point(20, 45);
        cmbAMVehicle = new ComboBoxAdv();
        cmbAMVehicle.Size = new Size(200, 25);
        cmbAMVehicle.Location = new Point(140, 42);

        // AM Driver
        var lblAMDriver = new AutoLabel();
        lblAMDriver.Text = "AM Driver:";
        lblAMDriver.Location = new Point(380, 45);
        cmbAMDriver = new ComboBoxAdv();
        cmbAMDriver.Size = new Size(200, 25);
        cmbAMDriver.Location = new Point(460, 42);

        // AM Begin Miles
        var lblAMBeginMiles = new AutoLabel();
        lblAMBeginMiles.Text = "Begin Miles:";
        lblAMBeginMiles.Location = new Point(20, 80);
        txtAMBeginMiles = new TextBoxExt();
        txtAMBeginMiles.Size = new Size(100, 25);
        txtAMBeginMiles.Location = new Point(140, 77);

        // AM End Miles
        var lblAMEndMiles = new AutoLabel();
        lblAMEndMiles.Text = "End Miles:";
        lblAMEndMiles.Location = new Point(260, 80);
        txtAMEndMiles = new TextBoxExt();
        txtAMEndMiles.Size = new Size(100, 25);
        txtAMEndMiles.Location = new Point(340, 77);

        // AM Riders
        var lblAMRiders = new AutoLabel();
        lblAMRiders.Text = "Riders:";
        lblAMRiders.Location = new Point(460, 80);
        txtAMRiders = new TextBoxExt();
        txtAMRiders.Size = new Size(80, 25);
        txtAMRiders.Location = new Point(520, 77);

        panelAMRoute.Controls.AddRange(new Control[] {
            lblAMRouteInfo, lblAMVehicle, cmbAMVehicle, lblAMDriver, cmbAMDriver,
            lblAMBeginMiles, txtAMBeginMiles, lblAMEndMiles, txtAMEndMiles,
            lblAMRiders, txtAMRiders
        });
    }

    private void CreatePMRoutePanel()
    {
        panelPMRoute = new GradientPanel();
        panelPMRoute.Size = new Size(780, 180);
        panelPMRoute.BackgroundColor = new Syncfusion.Drawing.BrushInfo(Color.White);
        panelPMRoute.BorderStyle = BorderStyle.FixedSingle;

        lblPMRouteInfo = new AutoLabel();
        lblPMRouteInfo.Text = "🌆 PM Route Details";
        lblPMRouteInfo.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
        lblPMRouteInfo.ForeColor = Color.FromArgb(46, 125, 185);
        lblPMRouteInfo.Location = new Point(10, 10);

        // PM Vehicle
        var lblPMVehicle = new AutoLabel();
        lblPMVehicle.Text = "PM Vehicle:";
        lblPMVehicle.Location = new Point(20, 45);
        cmbPMVehicle = new ComboBoxAdv();
        cmbPMVehicle.Size = new Size(200, 25);
        cmbPMVehicle.Location = new Point(140, 42);

        // PM Driver
        var lblPMDriver = new AutoLabel();
        lblPMDriver.Text = "PM Driver:";
        lblPMDriver.Location = new Point(380, 45);
        cmbPMDriver = new ComboBoxAdv();
        cmbPMDriver.Size = new Size(200, 25);
        cmbPMDriver.Location = new Point(460, 42);

        // PM Begin Miles
        var lblPMBeginMiles = new AutoLabel();
        lblPMBeginMiles.Text = "Begin Miles:";
        lblPMBeginMiles.Location = new Point(20, 80);
        txtPMBeginMiles = new TextBoxExt();
        txtPMBeginMiles.Size = new Size(100, 25);
        txtPMBeginMiles.Location = new Point(140, 77);

        // PM End Miles
        var lblPMEndMiles = new AutoLabel();
        lblPMEndMiles.Text = "End Miles:";
        lblPMEndMiles.Location = new Point(260, 80);
        txtPMEndMiles = new TextBoxExt();
        txtPMEndMiles.Size = new Size(100, 25);
        txtPMEndMiles.Location = new Point(340, 77);

        // PM Riders
        var lblPMRiders = new AutoLabel();
        lblPMRiders.Text = "Riders:";
        lblPMRiders.Location = new Point(460, 80);
        txtPMRiders = new TextBoxExt();
        txtPMRiders.Size = new Size(80, 25);
        txtPMRiders.Location = new Point(520, 77);

        panelPMRoute.Controls.AddRange(new Control[] {
            lblPMRouteInfo, lblPMVehicle, cmbPMVehicle, lblPMDriver, cmbPMDriver,
            lblPMBeginMiles, txtPMBeginMiles, lblPMEndMiles, txtPMEndMiles,
            lblPMRiders, txtPMRiders
        });
    }

    private void CreateActionButtons()
    {
        // Validate Button
        btnValidate = new SfButton();
        btnValidate.Text = "Validate";
        btnValidate.Size = new Size(80, 30);
        btnValidate.Style.BackColor = Color.FromArgb(255, 193, 7);
        btnValidate.Style.ForeColor = Color.Black;
        btnValidate.Click += BtnValidate_Click;

        // Save Button
        btnSave = new SfButton();
        btnSave.Text = "Save";
        btnSave.Size = new Size(80, 30);
        btnSave.Style.BackColor = Color.FromArgb(40, 167, 69);
        btnSave.Style.ForeColor = Color.White;
        btnSave.Click += BtnSave_Click;

        // Cancel Button
        btnCancel = new SfButton();
        btnCancel.Text = "Cancel";
        btnCancel.Size = new Size(80, 30);
        btnCancel.Style.BackColor = Color.FromArgb(108, 117, 125);
        btnCancel.Style.ForeColor = Color.White;
        btnCancel.Click += BtnCancel_Click;
    }

    #endregion

    #region Data Loading and Population

    private async void LoadDataAsync()
    {
        try
        {
            _logger.LogInformation("Loading data for route edit form");

            // Load buses
            var busEntities = await _busService.GetAllBusEntitiesAsync();
            _buses = busEntities ?? new List<Bus>();

            // Load drivers
            var driverEntities = await _busService.GetAllDriversAsync();
            _drivers = driverEntities ?? new List<Driver>();

            // Populate vehicle dropdowns
            PopulateVehicleDropdowns();
            PopulateDriverDropdowns();

            _logger.LogInformation("Loaded {BusCount} buses and {DriverCount} drivers",
                _buses.Count, _drivers.Count);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading data for route edit form");
            MessageBoxAdv.Show($"Error loading data: {ex.Message}", "Error",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void PopulateVehicleDropdowns()
    {
        cmbAMVehicle.Items.Clear();
        cmbPMVehicle.Items.Clear();
        cmbAMVehicle.Items.Add("None");
        cmbPMVehicle.Items.Add("None");

        foreach (var bus in _buses)
        {
            var displayText = $"{bus.BusNumber} - {bus.Make} {bus.Model}";
            cmbAMVehicle.Items.Add(new { Text = displayText, Value = bus.VehicleId });
            cmbPMVehicle.Items.Add(new { Text = displayText, Value = bus.VehicleId });
        }

        cmbAMVehicle.SelectedIndex = 0;
        cmbPMVehicle.SelectedIndex = 0;
    }

    private void PopulateDriverDropdowns()
    {
        cmbAMDriver.Items.Clear();
        cmbPMDriver.Items.Clear();
        cmbAMDriver.Items.Add("None");
        cmbPMDriver.Items.Add("None");

        foreach (var driver in _drivers)
        {
            var displayText = $"{driver.DriverName}";
            cmbAMDriver.Items.Add(new { Text = displayText, Value = driver.DriverId });
            cmbPMDriver.Items.Add(new { Text = displayText, Value = driver.DriverId });
        }

        cmbAMDriver.SelectedIndex = 0;
        cmbPMDriver.SelectedIndex = 0;
    }

    private void PopulateFormData(Route route)
    {
        try
        {
            _logger.LogInformation("Populating form with route data for ID {RouteId}", route.RouteId);

            // Route Information
            dtpDate.Value = route.Date;
            txtRouteName.Text = route.RouteName ?? string.Empty;
            txtDescription.Text = route.Description ?? string.Empty;
            chkIsActive.Checked = route.IsActive;

            // AM Route Information
            SelectVehicleInDropdown(cmbAMVehicle, route.AMVehicleId);
            txtAMBeginMiles.Text = route.AMBeginMiles?.ToString("F2") ?? string.Empty;
            txtAMEndMiles.Text = route.AMEndMiles?.ToString("F2") ?? string.Empty;
            txtAMRiders.Text = route.AMRiders?.ToString() ?? string.Empty;
            SelectDriverInDropdown(cmbAMDriver, route.AMDriverId);

            // PM Route Information
            SelectVehicleInDropdown(cmbPMVehicle, route.PMVehicleId);
            txtPMBeginMiles.Text = route.PMBeginMiles?.ToString("F2") ?? string.Empty;
            txtPMEndMiles.Text = route.PMEndMiles?.ToString("F2") ?? string.Empty;
            txtPMRiders.Text = route.PMRiders?.ToString() ?? string.Empty;
            SelectDriverInDropdown(cmbPMDriver, route.PMDriverId);

            _logger.LogInformation("Form populated successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error populating form data");
            MessageBoxAdv.Show($"Error loading route data: {ex.Message}", "Error",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void SelectVehicleInDropdown(ComboBoxAdv dropdown, int? vehicleId)
    {
        if (vehicleId.HasValue)
        {
            for (int i = 1; i < dropdown.Items.Count; i++) // Skip "None" at index 0
            {
                if (dropdown.Items[i] is { } item &&
                    item.GetType().GetProperty("Value")?.GetValue(item) is int value &&
                    value == vehicleId.Value)
                {
                    dropdown.SelectedIndex = i;
                    return;
                }
            }
        }
        dropdown.SelectedIndex = 0; // Default to "None"
    }

    private void SelectDriverInDropdown(ComboBoxAdv dropdown, int? driverId)
    {
        if (driverId.HasValue)
        {
            for (int i = 1; i < dropdown.Items.Count; i++) // Skip "None" at index 0
            {
                if (dropdown.Items[i] is { } item &&
                    item.GetType().GetProperty("Value")?.GetValue(item) is int value &&
                    value == driverId.Value)
                {
                    dropdown.SelectedIndex = i;
                    return;
                }
            }
        }
        dropdown.SelectedIndex = 0; // Default to "None"
    }

    private void SetDefaultValues()
    {
        dtpDate.Value = DateTime.Today;
        chkIsActive.Checked = true;
        cmbAMVehicle.SelectedIndex = 0;
        cmbPMVehicle.SelectedIndex = 0;
        cmbAMDriver.SelectedIndex = 0;
        cmbPMDriver.SelectedIndex = 0;
    }

    #endregion

    #region Validation

    private List<string> ValidateFormData()
    {
        var errors = new List<string>();

        // Required fields validation
        if (string.IsNullOrWhiteSpace(txtRouteName.Text))
            errors.Add("Route Name is required");

        // Miles validation
        if (!string.IsNullOrWhiteSpace(txtAMBeginMiles.Text))
        {
            if (!decimal.TryParse(txtAMBeginMiles.Text, out var amBeginMiles) || amBeginMiles < 0)
                errors.Add("AM Begin Miles must be a valid positive number");
        }

        if (!string.IsNullOrWhiteSpace(txtAMEndMiles.Text))
        {
            if (!decimal.TryParse(txtAMEndMiles.Text, out var amEndMiles) || amEndMiles < 0)
                errors.Add("AM End Miles must be a valid positive number");
            else if (!string.IsNullOrWhiteSpace(txtAMBeginMiles.Text) &&
                     decimal.TryParse(txtAMBeginMiles.Text, out var amBegin) &&
                     amEndMiles < amBegin)
                errors.Add("AM End Miles must be greater than or equal to AM Begin Miles");
        }

        if (!string.IsNullOrWhiteSpace(txtPMBeginMiles.Text))
        {
            if (!decimal.TryParse(txtPMBeginMiles.Text, out var pmBeginMiles) || pmBeginMiles < 0)
                errors.Add("PM Begin Miles must be a valid positive number");
        }

        if (!string.IsNullOrWhiteSpace(txtPMEndMiles.Text))
        {
            if (!decimal.TryParse(txtPMEndMiles.Text, out var pmEndMiles) || pmEndMiles < 0)
                errors.Add("PM End Miles must be a valid positive number");
            else if (!string.IsNullOrWhiteSpace(txtPMBeginMiles.Text) &&
                     decimal.TryParse(txtPMBeginMiles.Text, out var pmBegin) &&
                     pmEndMiles < pmBegin)
                errors.Add("PM End Miles must be greater than or equal to PM Begin Miles");
        }

        // Riders validation
        if (!string.IsNullOrWhiteSpace(txtAMRiders.Text))
        {
            if (!int.TryParse(txtAMRiders.Text, out var amRiders) || amRiders < 0 || amRiders > 100)
                errors.Add("AM Riders must be a number between 0 and 100");
        }

        if (!string.IsNullOrWhiteSpace(txtPMRiders.Text))
        {
            if (!int.TryParse(txtPMRiders.Text, out var pmRiders) || pmRiders < 0 || pmRiders > 100)
                errors.Add("PM Riders must be a number between 0 and 100");
        }

        return errors;
    }

    #endregion

    #region Event Handlers

    private void BtnValidate_Click(object? sender, EventArgs e)
    {
        try
        {
            var errors = ValidateFormData();

            if (errors.Count == 0)
            {
                MessageBoxAdv.Show("All data is valid!", "Validation Success",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                var errorMessage = "Please correct the following errors:\n\n" + string.Join("\n", errors);
                MessageBoxAdv.Show(errorMessage, "Validation Errors",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during validation");
            MessageBoxAdv.Show($"Validation error: {ex.Message}", "Error",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void BtnSave_Click(object? sender, EventArgs e)
    {
        try
        {
            var errors = ValidateFormData();
            if (errors.Count > 0)
            {
                var errorMessage = "Please correct the following errors:\n\n" + string.Join("\n", errors);
                MessageBoxAdv.Show(errorMessage, "Validation Errors",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var route = CreateRouteFromForm();

            if (_isEditMode && _existingRoute != null)
            {
                route.RouteId = _existingRoute.RouteId;
                // Note: For now, we'll just mark as saved since there's no IRouteService
                // This should be replaced with actual service call when route service is implemented
                EditedRoute = route;
                IsDataSaved = true;
                _logger.LogInformation("Route {RouteId} updated successfully", route.RouteId);
                MessageBoxAdv.Show("Route updated successfully!", "Success",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            else
            {
                // Note: For now, we'll just assign a temporary ID since there's no IRouteService
                // This should be replaced with actual service call when route service is implemented
                route.RouteId = new Random().Next(1000, 9999);
                EditedRoute = route;
                IsDataSaved = true;
                _logger.LogInformation("New route added successfully with ID {RouteId}", route.RouteId);
                MessageBoxAdv.Show("Route added successfully!", "Success",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error saving route data");
            MessageBoxAdv.Show($"Error saving route: {ex.Message}", "Error",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void BtnCancel_Click(object? sender, EventArgs e)
    {
        this.DialogResult = DialogResult.Cancel;
        this.Close();
    }

    #endregion

    #region Helper Methods

    private Route CreateRouteFromForm()
    {
        var route = new Route
        {
            Date = dtpDate.Value,
            RouteName = txtRouteName.Text?.Trim() ?? string.Empty,
            Description = txtDescription.Text?.Trim(),
            IsActive = chkIsActive.Checked
        };

        // AM Route Information
        route.AMVehicleId = GetSelectedVehicleId(cmbAMVehicle);
        if (decimal.TryParse(txtAMBeginMiles.Text, out var amBeginMiles))
            route.AMBeginMiles = amBeginMiles;
        if (decimal.TryParse(txtAMEndMiles.Text, out var amEndMiles))
            route.AMEndMiles = amEndMiles;
        if (int.TryParse(txtAMRiders.Text, out var amRiders))
            route.AMRiders = amRiders;
        route.AMDriverId = GetSelectedDriverId(cmbAMDriver);

        // PM Route Information
        route.PMVehicleId = GetSelectedVehicleId(cmbPMVehicle);
        if (decimal.TryParse(txtPMBeginMiles.Text, out var pmBeginMiles))
            route.PMBeginMiles = pmBeginMiles;
        if (decimal.TryParse(txtPMEndMiles.Text, out var pmEndMiles))
            route.PMEndMiles = pmEndMiles;
        if (int.TryParse(txtPMRiders.Text, out var pmRiders))
            route.PMRiders = pmRiders;
        route.PMDriverId = GetSelectedDriverId(cmbPMDriver);

        return route;
    }

    private int? GetSelectedVehicleId(ComboBoxAdv dropdown)
    {
        if (dropdown.SelectedIndex > 0 && dropdown.SelectedItem != null)
        {
            var item = dropdown.SelectedItem;
            var value = item.GetType().GetProperty("Value")?.GetValue(item);
            if (value is int vehicleId)
                return vehicleId;
        }
        return null;
    }

    private int? GetSelectedDriverId(ComboBoxAdv dropdown)
    {
        if (dropdown.SelectedIndex > 0 && dropdown.SelectedItem != null)
        {
            var item = dropdown.SelectedItem;
            var value = item.GetType().GetProperty("Value")?.GetValue(item);
            if (value is int driverId)
                return driverId;
        }
        return null;
    }

    #endregion
}
