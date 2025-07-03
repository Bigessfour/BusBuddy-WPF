using Bus_Buddy.Models;
using Bus_Buddy.Services;
using Microsoft.Extensions.Logging;
using Syncfusion.Windows.Forms;
using Syncfusion.Windows.Forms.Tools;
using Syncfusion.WinForms.Controls;
using System.ComponentModel.DataAnnotations;

namespace Bus_Buddy.Forms;

/// <summary>
/// Passenger Add/Edit Form for creating and editing passenger records
/// Demonstrates comprehensive data entry form with validation using Syncfusion controls
/// Works with Student entities as passengers in the transportation system
/// </summary>
public partial class PassengerEditForm : MetroForm
{
    #region Fields and Services
    private readonly ILogger<PassengerEditForm> _logger;
    private readonly IStudentService _studentService;
    private readonly IBusService _busService;
    private readonly Student? _existingStudent;
    private readonly bool _isEditMode;

    // Form result properties
    public Student? EditedStudent { get; private set; }
    public bool IsDataSaved { get; private set; }

    // Available data for dropdowns
    private List<Route> _routes = null!;

    #endregion

    #region UI Controls - Personal Information
    private Syncfusion.Windows.Forms.Tools.GradientPanel panelPersonal = null!;
    private Syncfusion.Windows.Forms.Tools.AutoLabel lblPersonalInfo = null!;

    private Syncfusion.Windows.Forms.Tools.TextBoxExt txtStudentNumber = null!;
    private Syncfusion.Windows.Forms.Tools.TextBoxExt txtStudentName = null!;
    private ComboBoxAdv cmbGrade = null!;
    private Syncfusion.Windows.Forms.Tools.TextBoxExt txtSchool = null!;
    private Syncfusion.Windows.Forms.Tools.DateTimePickerAdv dtpEnrollmentDate = null!;
    private CheckBoxAdv chkActive = null!;

    #endregion

    #region UI Controls - Contact Information
    private Syncfusion.Windows.Forms.Tools.GradientPanel panelContact = null!;
    private Syncfusion.Windows.Forms.Tools.AutoLabel lblContactInfo = null!;

    private Syncfusion.Windows.Forms.Tools.TextBoxExt txtHomeAddress = null!;
    private Syncfusion.Windows.Forms.Tools.TextBoxExt txtCity = null!;
    private Syncfusion.Windows.Forms.Tools.TextBoxExt txtState = null!;
    private Syncfusion.Windows.Forms.Tools.TextBoxExt txtZip = null!;
    private Syncfusion.Windows.Forms.Tools.TextBoxExt txtHomePhone = null!;
    private Syncfusion.Windows.Forms.Tools.TextBoxExt txtCellPhone = null!;
    private Syncfusion.Windows.Forms.Tools.TextBoxExt txtEmail = null!;

    #endregion

    #region UI Controls - Transportation Information
    private Syncfusion.Windows.Forms.Tools.GradientPanel panelTransportation = null!;
    private Syncfusion.Windows.Forms.Tools.AutoLabel lblTransportationInfo = null!;

    private ComboBoxAdv cmbAMRoute = null!;
    private Syncfusion.Windows.Forms.Tools.TextBoxExt txtAMPickupTime = null!;
    private ComboBoxAdv cmbPMRoute = null!;
    private Syncfusion.Windows.Forms.Tools.TextBoxExt txtPMPickupTime = null!;
    private Syncfusion.Windows.Forms.Tools.TextBoxExt txtBusStop = null!;
    private CheckBoxAdv chkSpecialNeeds = null!;

    #endregion

    #region UI Controls - Emergency Contact
    private Syncfusion.Windows.Forms.Tools.GradientPanel panelEmergency = null!;
    private Syncfusion.Windows.Forms.Tools.AutoLabel lblEmergencyInfo = null!;

    private Syncfusion.Windows.Forms.Tools.TextBoxExt txtEmergencyName = null!;
    private Syncfusion.Windows.Forms.Tools.TextBoxExt txtEmergencyPhone = null!;
    private Syncfusion.Windows.Forms.Tools.TextBoxExt txtEmergencyRelationship = null!;

    #endregion

    #region UI Controls - Notes and Actions
    private Syncfusion.Windows.Forms.Tools.GradientPanel panelNotes = null!;
    private Syncfusion.Windows.Forms.Tools.AutoLabel lblNotesInfo = null!;

    private Syncfusion.Windows.Forms.Tools.TextBoxExt txtNotes = null!;

    // Action buttons
    private SfButton btnSave = null!;
    private SfButton btnCancel = null!;
    private SfButton btnValidate = null!;

    #endregion

    #region Constructors

    /// <summary>
    /// Constructor for adding new passenger
    /// </summary>
    public PassengerEditForm(
        ILogger<PassengerEditForm> logger,
        IStudentService studentService,
        IBusService busService)
        : this(logger, studentService, busService, null)
    {
    }

    /// <summary>
    /// Constructor for editing existing passenger
    /// </summary>
    public PassengerEditForm(
        ILogger<PassengerEditForm> logger,
        IStudentService studentService,
        IBusService busService,
        Student? existingStudent)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _studentService = studentService ?? throw new ArgumentNullException(nameof(studentService));
        _busService = busService ?? throw new ArgumentNullException(nameof(busService));
        _existingStudent = existingStudent;
        _isEditMode = existingStudent != null;

        _logger.LogInformation("Initializing PassengerEditForm in {Mode} mode",
            _isEditMode ? "Edit" : "Add");

        InitializeComponent();
        SetupForm();
        LoadRouteData();

        if (_isEditMode && _existingStudent != null)
        {
            PopulateFormData(_existingStudent);
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
        this.Text = _isEditMode ? "Edit Passenger" : "Add New Passenger";
        this.Size = new Size(800, 700);
        this.MinimumSize = new Size(700, 600);
        this.StartPosition = FormStartPosition.CenterParent;
        this.MaximizeBox = false;
        this.ShowIcon = false;

        // Apply Syncfusion styling
        this.MetroColor = Color.FromArgb(46, 125, 185);
        this.CaptionBarColor = Color.FromArgb(46, 125, 185);
        this.CaptionForeColor = Color.White;

        _logger.LogInformation("PassengerEditForm setup completed");
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
        CreatePersonalInfoPanel();
        CreateContactInfoPanel();
        CreateTransportationInfoPanel();
        CreateEmergencyContactPanel();
        CreateNotesPanel();
        CreateActionButtons();

        // Layout panels vertically
        int yPosition = 10;
        int panelSpacing = 15;

        panelPersonal.Location = new Point(10, yPosition);
        mainPanel.Controls.Add(panelPersonal);
        yPosition += panelPersonal.Height + panelSpacing;

        panelContact.Location = new Point(10, yPosition);
        mainPanel.Controls.Add(panelContact);
        yPosition += panelContact.Height + panelSpacing;

        panelTransportation.Location = new Point(10, yPosition);
        mainPanel.Controls.Add(panelTransportation);
        yPosition += panelTransportation.Height + panelSpacing;

        panelEmergency.Location = new Point(10, yPosition);
        mainPanel.Controls.Add(panelEmergency);
        yPosition += panelEmergency.Height + panelSpacing;

        panelNotes.Location = new Point(10, yPosition);
        mainPanel.Controls.Add(panelNotes);
        yPosition += panelNotes.Height + panelSpacing;

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

    private void CreatePersonalInfoPanel()
    {
        panelPersonal = new GradientPanel();
        panelPersonal.Size = new Size(780, 180);
        panelPersonal.BackgroundColor = new Syncfusion.Drawing.BrushInfo(Color.White);
        panelPersonal.BorderStyle = BorderStyle.FixedSingle;

        lblPersonalInfo = new AutoLabel();
        lblPersonalInfo.Text = "📋 Personal Information";
        lblPersonalInfo.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
        lblPersonalInfo.ForeColor = Color.FromArgb(46, 125, 185);
        lblPersonalInfo.Location = new Point(10, 10);

        // Student Number
        var lblStudentNumber = new AutoLabel();
        lblStudentNumber.Text = "Student Number:";
        lblStudentNumber.Location = new Point(20, 45);
        txtStudentNumber = new TextBoxExt();
        txtStudentNumber.Size = new Size(150, 25);
        txtStudentNumber.Location = new Point(140, 42);

        // Student Name
        var lblStudentName = new AutoLabel();
        lblStudentName.Text = "Full Name:";
        lblStudentName.Location = new Point(320, 45);
        txtStudentName = new TextBoxExt();
        txtStudentName.Size = new Size(200, 25);
        txtStudentName.Location = new Point(390, 42);

        // Grade
        var lblGrade = new AutoLabel();
        lblGrade.Text = "Grade:";
        lblGrade.Location = new Point(20, 80);
        cmbGrade = new ComboBoxAdv();
        cmbGrade.Size = new Size(100, 25);
        cmbGrade.Location = new Point(140, 77);
        cmbGrade.Items.AddRange(new[] { "K", "1", "2", "3", "4", "5", "6", "7", "8", "9", "10", "11", "12" });

        // School
        var lblSchool = new AutoLabel();
        lblSchool.Text = "School:";
        lblSchool.Location = new Point(320, 80);
        txtSchool = new TextBoxExt();
        txtSchool.Size = new Size(200, 25);
        txtSchool.Location = new Point(390, 77);

        // Enrollment Date
        var lblEnrollmentDate = new AutoLabel();
        lblEnrollmentDate.Text = "Enrollment Date:";
        lblEnrollmentDate.Location = new Point(20, 115);
        dtpEnrollmentDate = new DateTimePickerAdv();
        dtpEnrollmentDate.Size = new Size(150, 25);
        dtpEnrollmentDate.Location = new Point(140, 112);

        // Active Status
        chkActive = new CheckBoxAdv();
        chkActive.Text = "Active Student";
        chkActive.Size = new Size(120, 25);
        chkActive.Location = new Point(320, 115);
        chkActive.Checked = true;

        panelPersonal.Controls.AddRange(new Control[] {
            lblPersonalInfo, lblStudentNumber, txtStudentNumber, lblStudentName, txtStudentName,
            lblGrade, cmbGrade, lblSchool, txtSchool, lblEnrollmentDate, dtpEnrollmentDate, chkActive
        });
    }

    private void CreateContactInfoPanel()
    {
        panelContact = new GradientPanel();
        panelContact.Size = new Size(780, 180);
        panelContact.BackgroundColor = new Syncfusion.Drawing.BrushInfo(Color.White);
        panelContact.BorderStyle = BorderStyle.FixedSingle;

        lblContactInfo = new AutoLabel();
        lblContactInfo.Text = "📞 Contact Information";
        lblContactInfo.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
        lblContactInfo.ForeColor = Color.FromArgb(46, 125, 185);
        lblContactInfo.Location = new Point(10, 10);

        // Home Address
        var lblHomeAddress = new AutoLabel();
        lblHomeAddress.Text = "Home Address:";
        lblHomeAddress.Location = new Point(20, 45);
        txtHomeAddress = new TextBoxExt();
        txtHomeAddress.Size = new Size(300, 25);
        txtHomeAddress.Location = new Point(140, 42);

        // City
        var lblCity = new AutoLabel();
        lblCity.Text = "City:";
        lblCity.Location = new Point(20, 80);
        txtCity = new TextBoxExt();
        txtCity.Size = new Size(150, 25);
        txtCity.Location = new Point(140, 77);

        // State
        var lblState = new AutoLabel();
        lblState.Text = "State:";
        lblState.Location = new Point(320, 80);
        txtState = new TextBoxExt();
        txtState.Size = new Size(60, 25);
        txtState.Location = new Point(360, 77);

        // ZIP Code
        var lblZip = new AutoLabel();
        lblZip.Text = "ZIP Code:";
        lblZip.Location = new Point(450, 80);
        txtZip = new TextBoxExt();
        txtZip.Size = new Size(100, 25);
        txtZip.Location = new Point(520, 77);

        // Home Phone
        var lblHomePhone = new AutoLabel();
        lblHomePhone.Text = "Home Phone:";
        lblHomePhone.Location = new Point(20, 115);
        txtHomePhone = new TextBoxExt();
        txtHomePhone.Size = new Size(150, 25);
        txtHomePhone.Location = new Point(140, 112);

        // Cell Phone
        var lblCellPhone = new AutoLabel();
        lblCellPhone.Text = "Cell Phone:";
        lblCellPhone.Location = new Point(320, 115);
        txtCellPhone = new TextBoxExt();
        txtCellPhone.Size = new Size(150, 25);
        txtCellPhone.Location = new Point(390, 112);

        // Email
        var lblEmail = new AutoLabel();
        lblEmail.Text = "Email:";
        lblEmail.Location = new Point(20, 150);
        txtEmail = new TextBoxExt();
        txtEmail.Size = new Size(250, 25);
        txtEmail.Location = new Point(140, 147);

        panelContact.Controls.AddRange(new Control[] {
            lblContactInfo, lblHomeAddress, txtHomeAddress, lblCity, txtCity,
            lblState, txtState, lblZip, txtZip, lblHomePhone, txtHomePhone,
            lblCellPhone, txtCellPhone, lblEmail, txtEmail
        });
    }

    private void CreateTransportationInfoPanel()
    {
        panelTransportation = new GradientPanel();
        panelTransportation.Size = new Size(780, 180);
        panelTransportation.BackgroundColor = new Syncfusion.Drawing.BrushInfo(Color.White);
        panelTransportation.BorderStyle = BorderStyle.FixedSingle;

        lblTransportationInfo = new AutoLabel();
        lblTransportationInfo.Text = "🚌 Transportation Information";
        lblTransportationInfo.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
        lblTransportationInfo.ForeColor = Color.FromArgb(46, 125, 185);
        lblTransportationInfo.Location = new Point(10, 10);

        // AM Route
        var lblAMRoute = new AutoLabel();
        lblAMRoute.Text = "AM Route:";
        lblAMRoute.Location = new Point(20, 45);
        cmbAMRoute = new ComboBoxAdv();
        cmbAMRoute.Size = new Size(150, 25);
        cmbAMRoute.Location = new Point(140, 42);

        // AM Pickup Time
        var lblAMPickupTime = new AutoLabel();
        lblAMPickupTime.Text = "AM Pickup Time:";
        lblAMPickupTime.Location = new Point(320, 45);
        txtAMPickupTime = new TextBoxExt();
        txtAMPickupTime.Size = new Size(100, 25);
        txtAMPickupTime.Location = new Point(420, 42);
        txtAMPickupTime.Text = "07:30 AM";

        // PM Route
        var lblPMRoute = new AutoLabel();
        lblPMRoute.Text = "PM Route:";
        lblPMRoute.Location = new Point(20, 80);
        cmbPMRoute = new ComboBoxAdv();
        cmbPMRoute.Size = new Size(150, 25);
        cmbPMRoute.Location = new Point(140, 77);

        // PM Pickup Time
        var lblPMPickupTime = new AutoLabel();
        lblPMPickupTime.Text = "PM Pickup Time:";
        lblPMPickupTime.Location = new Point(320, 80);
        txtPMPickupTime = new TextBoxExt();
        txtPMPickupTime.Size = new Size(100, 25);
        txtPMPickupTime.Location = new Point(420, 77);
        txtPMPickupTime.Text = "03:30 PM";

        // Bus Stop
        var lblBusStop = new AutoLabel();
        lblBusStop.Text = "Bus Stop/Location:";
        lblBusStop.Location = new Point(20, 115);
        txtBusStop = new TextBoxExt();
        txtBusStop.Size = new Size(300, 25);
        txtBusStop.Location = new Point(140, 112);

        // Special Needs
        chkSpecialNeeds = new CheckBoxAdv();
        chkSpecialNeeds.Text = "Special Transportation Needs";
        chkSpecialNeeds.Size = new Size(200, 25);
        chkSpecialNeeds.Location = new Point(20, 150);

        panelTransportation.Controls.AddRange(new Control[] {
            lblTransportationInfo, lblAMRoute, cmbAMRoute, lblAMPickupTime, txtAMPickupTime,
            lblPMRoute, cmbPMRoute, lblPMPickupTime, txtPMPickupTime,
            lblBusStop, txtBusStop, chkSpecialNeeds
        });
    }

    private void CreateEmergencyContactPanel()
    {
        panelEmergency = new GradientPanel();
        panelEmergency.Size = new Size(780, 120);
        panelEmergency.BackgroundColor = new Syncfusion.Drawing.BrushInfo(Color.White);
        panelEmergency.BorderStyle = BorderStyle.FixedSingle;

        lblEmergencyInfo = new AutoLabel();
        lblEmergencyInfo.Text = "🚨 Emergency Contact";
        lblEmergencyInfo.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
        lblEmergencyInfo.ForeColor = Color.FromArgb(46, 125, 185);
        lblEmergencyInfo.Location = new Point(10, 10);

        // Emergency Contact Name
        var lblEmergencyName = new AutoLabel();
        lblEmergencyName.Text = "Contact Name:";
        lblEmergencyName.Location = new Point(20, 45);
        txtEmergencyName = new TextBoxExt();
        txtEmergencyName.Size = new Size(200, 25);
        txtEmergencyName.Location = new Point(140, 42);

        // Emergency Phone
        var lblEmergencyPhone = new AutoLabel();
        lblEmergencyPhone.Text = "Phone Number:";
        lblEmergencyPhone.Location = new Point(380, 45);
        txtEmergencyPhone = new TextBoxExt();
        txtEmergencyPhone.Size = new Size(150, 25);
        txtEmergencyPhone.Location = new Point(480, 42);

        // Relationship
        var lblEmergencyRelationship = new AutoLabel();
        lblEmergencyRelationship.Text = "Relationship:";
        lblEmergencyRelationship.Location = new Point(20, 80);
        txtEmergencyRelationship = new TextBoxExt();
        txtEmergencyRelationship.Size = new Size(150, 25);
        txtEmergencyRelationship.Location = new Point(140, 77);

        panelEmergency.Controls.AddRange(new Control[] {
            lblEmergencyInfo, lblEmergencyName, txtEmergencyName,
            lblEmergencyPhone, txtEmergencyPhone, lblEmergencyRelationship, txtEmergencyRelationship
        });
    }

    private void CreateNotesPanel()
    {
        panelNotes = new GradientPanel();
        panelNotes.Size = new Size(780, 120);
        panelNotes.BackgroundColor = new Syncfusion.Drawing.BrushInfo(Color.White);
        panelNotes.BorderStyle = BorderStyle.FixedSingle;

        lblNotesInfo = new AutoLabel();
        lblNotesInfo.Text = "📝 Notes and Special Instructions";
        lblNotesInfo.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
        lblNotesInfo.ForeColor = Color.FromArgb(46, 125, 185);
        lblNotesInfo.Location = new Point(10, 10);

        txtNotes = new TextBoxExt();
        txtNotes.Size = new Size(750, 75);
        txtNotes.Location = new Point(20, 35);
        txtNotes.Multiline = true;
        txtNotes.ScrollBars = ScrollBars.Vertical;

        panelNotes.Controls.AddRange(new Control[] { lblNotesInfo, txtNotes });
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

    private async void LoadRouteData()
    {
        try
        {
            _logger.LogInformation("Loading route data for passenger edit form");

            var allRoutes = await _busService.GetAllRoutesAsync();
            _routes = new List<Route>();

            // Convert RouteInfo to Route objects if needed
            foreach (var routeInfo in allRoutes)
            {
                _routes.Add(new Route
                {
                    RouteId = routeInfo.RouteId,
                    RouteName = routeInfo.RouteName
                });
            }

            // Populate route dropdowns
            cmbAMRoute.Items.Clear();
            cmbPMRoute.Items.Clear();
            cmbAMRoute.Items.Add("None");
            cmbPMRoute.Items.Add("None");

            foreach (var route in _routes)
            {
                cmbAMRoute.Items.Add(route.RouteName);
                cmbPMRoute.Items.Add(route.RouteName);
            }

            cmbAMRoute.SelectedIndex = 0;
            cmbPMRoute.SelectedIndex = 0;

            _logger.LogInformation("Loaded {RouteCount} routes", _routes.Count);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading route data");
            MessageBoxAdv.Show($"Error loading route data: {ex.Message}", "Error",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void PopulateFormData(Student student)
    {
        try
        {
            _logger.LogInformation("Populating form with student data for ID {StudentId}", student.StudentId);

            // Personal Information
            txtStudentNumber.Text = student.StudentNumber ?? string.Empty;
            txtStudentName.Text = student.StudentName ?? string.Empty;
            cmbGrade.Text = student.Grade ?? string.Empty;
            txtSchool.Text = student.School ?? string.Empty;
            dtpEnrollmentDate.Value = student.EnrollmentDate ?? DateTime.Today;
            chkActive.Checked = student.Active;

            // Contact Information
            txtHomeAddress.Text = student.HomeAddress ?? string.Empty;
            txtCity.Text = student.City ?? string.Empty;
            txtState.Text = student.State ?? string.Empty;
            txtZip.Text = student.Zip ?? string.Empty;
            txtHomePhone.Text = student.HomePhone ?? string.Empty;
            txtCellPhone.Text = string.Empty; // Student model doesn't have CellPhone
            txtEmail.Text = string.Empty; // Student model doesn't have Email

            // Transportation Information
            cmbAMRoute.Text = student.AMRoute ?? "None";
            txtAMPickupTime.Text = "07:30 AM"; // Student model doesn't have AMPickupTime
            cmbPMRoute.Text = student.PMRoute ?? "None";
            txtPMPickupTime.Text = "03:30 PM"; // Student model doesn't have PMPickupTime
            txtBusStop.Text = student.BusStop ?? string.Empty;
            chkSpecialNeeds.Checked = student.SpecialNeeds;

            // Emergency Contact (using available fields)
            txtEmergencyName.Text = student.ParentGuardian ?? string.Empty;
            txtEmergencyPhone.Text = student.EmergencyPhone ?? string.Empty;
            txtEmergencyRelationship.Text = "Parent/Guardian";

            // Notes
            txtNotes.Text = student.MedicalNotes ?? string.Empty;

            _logger.LogInformation("Form populated successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error populating form data");
            MessageBoxAdv.Show($"Error loading student data: {ex.Message}", "Error",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void SetDefaultValues()
    {
        dtpEnrollmentDate.Value = DateTime.Today;
        chkActive.Checked = true;
        cmbAMRoute.SelectedIndex = 0;
        cmbPMRoute.SelectedIndex = 0;
        txtAMPickupTime.Text = "07:30 AM";
        txtPMPickupTime.Text = "03:30 PM";
        txtEmergencyRelationship.Text = "Parent/Guardian";
    }

    #endregion

    #region Validation

    private List<string> ValidateFormData()
    {
        var errors = new List<string>();

        // Required fields validation
        if (string.IsNullOrWhiteSpace(txtStudentNumber.Text))
            errors.Add("Student Number is required");

        if (string.IsNullOrWhiteSpace(txtStudentName.Text))
            errors.Add("Student Name is required");

        if (string.IsNullOrWhiteSpace(cmbGrade.Text))
            errors.Add("Grade is required");

        if (string.IsNullOrWhiteSpace(txtSchool.Text))
            errors.Add("School is required");

        // Email validation
        if (!string.IsNullOrWhiteSpace(txtEmail.Text))
        {
            try
            {
                var email = new System.Net.Mail.MailAddress(txtEmail.Text);
            }
            catch
            {
                errors.Add("Invalid email format");
            }
        }

        // Phone validation
        if (!string.IsNullOrWhiteSpace(txtHomePhone.Text))
        {
            var phonePattern = @"^\(?([0-9]{3})\)?[-.\s]?([0-9]{3})[-.\s]?([0-9]{4})$";
            if (!System.Text.RegularExpressions.Regex.IsMatch(txtHomePhone.Text, phonePattern))
                errors.Add("Invalid home phone format");
        }

        if (!string.IsNullOrWhiteSpace(txtCellPhone.Text))
        {
            var phonePattern = @"^\(?([0-9]{3})\)?[-.\s]?([0-9]{3})[-.\s]?([0-9]{4})$";
            if (!System.Text.RegularExpressions.Regex.IsMatch(txtCellPhone.Text, phonePattern))
                errors.Add("Invalid cell phone format");
        }

        // ZIP code validation
        if (!string.IsNullOrWhiteSpace(txtZip.Text))
        {
            var zipPattern = @"^\d{5}(-\d{4})?$";
            if (!System.Text.RegularExpressions.Regex.IsMatch(txtZip.Text, zipPattern))
                errors.Add("Invalid ZIP code format");
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

    private async void BtnSave_Click(object? sender, EventArgs e)
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

            var student = CreateStudentFromForm();

            if (_isEditMode && _existingStudent != null)
            {
                student.StudentId = _existingStudent.StudentId;
                var success = await _studentService.UpdateStudentAsync(student);

                if (success)
                {
                    EditedStudent = student;
                    IsDataSaved = true;
                    _logger.LogInformation("Student {StudentId} updated successfully", student.StudentId);
                    MessageBoxAdv.Show("Student updated successfully!", "Success",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
                else
                {
                    MessageBoxAdv.Show("Failed to update student. Please try again.", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                var newStudent = await _studentService.AddStudentAsync(student);

                if (newStudent != null)
                {
                    EditedStudent = newStudent;
                    IsDataSaved = true;
                    _logger.LogInformation("New student added successfully with ID {StudentId}", newStudent.StudentId);
                    MessageBoxAdv.Show("Student added successfully!", "Success",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
                else
                {
                    MessageBoxAdv.Show("Failed to add student. Please try again.", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error saving student data");
            MessageBoxAdv.Show($"Error saving student: {ex.Message}", "Error",
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

    private Student CreateStudentFromForm()
    {
        return new Student
        {
            StudentNumber = txtStudentNumber.Text?.Trim(),
            StudentName = txtStudentName.Text?.Trim() ?? string.Empty,
            Grade = cmbGrade.Text?.Trim(),
            School = txtSchool.Text?.Trim(),
            EnrollmentDate = dtpEnrollmentDate.Value,
            Active = chkActive.Checked,

            HomeAddress = txtHomeAddress.Text?.Trim(),
            City = txtCity.Text?.Trim(),
            State = txtState.Text?.Trim(),
            Zip = txtZip.Text?.Trim(),
            HomePhone = txtHomePhone.Text?.Trim(),

            AMRoute = cmbAMRoute.Text == "None" ? null : cmbAMRoute.Text?.Trim(),
            PMRoute = cmbPMRoute.Text == "None" ? null : cmbPMRoute.Text?.Trim(),
            BusStop = txtBusStop.Text?.Trim(),
            SpecialNeeds = chkSpecialNeeds.Checked,

            ParentGuardian = txtEmergencyName.Text?.Trim(),
            EmergencyPhone = txtEmergencyPhone.Text?.Trim(),

            MedicalNotes = txtNotes.Text?.Trim()
        };
    }

    #endregion
}
