using Syncfusion.Windows.Forms;
using Syncfusion.Windows.Forms.Tools;
using Syncfusion.WinForms.Controls;
using Microsoft.Extensions.Logging;
using Bus_Buddy.Services;
using Bus_Buddy.Models;
using System;
using System.ComponentModel.DataAnnotations;
using System.Windows.Forms;

namespace Bus_Buddy.Forms;

/// <summary>
/// Bus Add/Edit Form for creating and editing bus records
/// Demonstrates comprehensive data entry form with validation using Syncfusion controls
/// </summary>
public partial class BusEditForm : MetroForm
{
    private readonly ILogger<BusEditForm> _logger;
    private readonly IBusService _busService;
    private Bus? _currentBus;
    private bool _isEditMode;

    public Bus? EditedBus { get; private set; }
    public bool IsDataSaved { get; private set; }

    public BusEditForm(ILogger<BusEditForm> logger, IBusService busService, Bus? bus = null)
    {
        _logger = logger;
        _busService = busService;
        _currentBus = bus;
        _isEditMode = bus != null;

        _logger.LogInformation("Initializing Bus Edit form in {Mode} mode", _isEditMode ? "Edit" : "Add");

        InitializeComponent();
        InitializeBusEditForm();

        if (_isEditMode && _currentBus != null)
        {
            LoadBusData(_currentBus);
        }
    }

    private void InitializeBusEditForm()
    {
        // Set Syncfusion MetroForm styles
        this.MetroColor = System.Drawing.Color.FromArgb(63, 81, 181);
        this.CaptionBarColor = System.Drawing.Color.FromArgb(63, 81, 181);
        this.CaptionForeColor = System.Drawing.Color.White;
        this.Text = _isEditMode ? "Edit Bus" : "Add New Bus";

        // Enable high DPI scaling for this form
        this.AutoScaleMode = AutoScaleMode.Dpi;
        this.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);

        // Initialize form controls
        InitializeFormControls();

        _logger.LogInformation("Bus Edit form initialized successfully");
    }

    private void InitializeFormControls()
    {
        // Set up date pickers with current date as default
        purchaseDatePicker.Value = DateTime.Now;
        lastInspectionDatePicker.Value = DateTime.Now;
        insuranceExpiryDatePicker.Value = DateTime.Now.AddYears(1);

        // Set up numeric controls
        yearNumeric.Minimum = 1900;
        yearNumeric.Maximum = DateTime.Now.Year + 1;
        yearNumeric.Value = DateTime.Now.Year;

        seatingCapacityNumeric.Minimum = 1;
        seatingCapacityNumeric.Maximum = 90;
        seatingCapacityNumeric.Value = 66;

        odometerNumeric.Minimum = 0;
        odometerNumeric.Maximum = 999999;

        purchasePriceNumeric.Minimum = 0;
        purchasePriceNumeric.Maximum = 500000;
        purchasePriceNumeric.DecimalPlaces = 2;

        // Set up combo boxes
        statusComboBox.Items.AddRange(new[] { "Active", "Maintenance", "Retired", "Out of Service" });
        statusComboBox.SelectedIndex = 0;

        makeComboBox.Items.AddRange(new[] { "Blue Bird", "Thomas Built Buses", "IC Bus", "Collins Bus", "Micro Bird", "Other" });
        makeComboBox.SelectedIndex = 0;

        // Set tab order
        SetTabOrder();
    }

    private void SetTabOrder()
    {
        int tabIndex = 0;
        busNumberTextBox.TabIndex = tabIndex++;
        yearNumeric.TabIndex = tabIndex++;
        makeComboBox.TabIndex = tabIndex++;
        modelTextBox.TabIndex = tabIndex++;
        seatingCapacityNumeric.TabIndex = tabIndex++;
        vinTextBox.TabIndex = tabIndex++;
        licenseNumberTextBox.TabIndex = tabIndex++;
        lastInspectionDatePicker.TabIndex = tabIndex++;
        odometerNumeric.TabIndex = tabIndex++;
        statusComboBox.TabIndex = tabIndex++;
        purchaseDatePicker.TabIndex = tabIndex++;
        purchasePriceNumeric.TabIndex = tabIndex++;
        insurancePolicyTextBox.TabIndex = tabIndex++;
        insuranceExpiryDatePicker.TabIndex = tabIndex++;
        saveButton.TabIndex = tabIndex++;
        cancelButton.TabIndex = tabIndex++;
    }

    private void LoadBusData(Bus bus)
    {
        try
        {
            _logger.LogInformation("Loading bus data for editing: {BusNumber}", bus.BusNumber);

            busNumberTextBox.Text = bus.BusNumber;
            yearNumeric.Value = bus.Year;
            makeComboBox.Text = bus.Make;
            modelTextBox.Text = bus.Model;
            seatingCapacityNumeric.Value = bus.SeatingCapacity;
            vinTextBox.Text = bus.VINNumber;
            licenseNumberTextBox.Text = bus.LicenseNumber;

            if (bus.DateLastInspection.HasValue)
                lastInspectionDatePicker.Value = bus.DateLastInspection.Value;

            if (bus.CurrentOdometer.HasValue)
                odometerNumeric.Value = bus.CurrentOdometer.Value;

            statusComboBox.Text = bus.Status;

            if (bus.PurchaseDate.HasValue)
                purchaseDatePicker.Value = bus.PurchaseDate.Value;

            if (bus.PurchasePrice.HasValue)
                purchasePriceNumeric.Value = bus.PurchasePrice.Value;

            insurancePolicyTextBox.Text = bus.InsurancePolicyNumber ?? string.Empty;

            if (bus.InsuranceExpiryDate.HasValue)
                insuranceExpiryDatePicker.Value = bus.InsuranceExpiryDate.Value;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading bus data");
            MessageBox.Show($"Error loading bus data: {ex.Message}", "Error",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private bool ValidateForm()
    {
        var errors = new List<string>();

        // Required field validation
        if (string.IsNullOrWhiteSpace(busNumberTextBox.Text))
            errors.Add("Bus Number is required");

        if (string.IsNullOrWhiteSpace(makeComboBox.Text))
            errors.Add("Make is required");

        if (string.IsNullOrWhiteSpace(modelTextBox.Text))
            errors.Add("Model is required");

        if (string.IsNullOrWhiteSpace(vinTextBox.Text))
            errors.Add("VIN Number is required");
        else if (vinTextBox.Text.Length != 17)
            errors.Add("VIN Number must be exactly 17 characters");

        if (string.IsNullOrWhiteSpace(licenseNumberTextBox.Text))
            errors.Add("License Number is required");

        // Business logic validation
        if (yearNumeric.Value > DateTime.Now.Year + 1)
            errors.Add("Year cannot be in the future");

        if (seatingCapacityNumeric.Value < 1)
            errors.Add("Seating capacity must be at least 1");

        if (lastInspectionDatePicker.Value > DateTime.Now)
            errors.Add("Last inspection date cannot be in the future");

        if (errors.Count > 0)
        {
            MessageBox.Show($"Please correct the following errors:\n\n{string.Join("\n", errors)}",
                "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return false;
        }

        return true;
    }

    private Bus CreateBusFromForm()
    {
        var bus = _currentBus ?? new Bus();

        bus.BusNumber = busNumberTextBox.Text.Trim();
        bus.Year = (int)yearNumeric.Value;
        bus.Make = makeComboBox.Text.Trim();
        bus.Model = modelTextBox.Text.Trim();
        bus.SeatingCapacity = (int)seatingCapacityNumeric.Value;
        bus.VINNumber = vinTextBox.Text.Trim().ToUpper();
        bus.LicenseNumber = licenseNumberTextBox.Text.Trim();
        bus.DateLastInspection = lastInspectionDatePicker.Value;
        bus.CurrentOdometer = (int?)odometerNumeric.Value;
        bus.Status = statusComboBox.Text;
        bus.PurchaseDate = purchaseDatePicker.Value;
        bus.PurchasePrice = purchasePriceNumeric.Value;
        bus.InsurancePolicyNumber = string.IsNullOrWhiteSpace(insurancePolicyTextBox.Text) ?
            null : insurancePolicyTextBox.Text.Trim();
        bus.InsuranceExpiryDate = insuranceExpiryDatePicker.Value;

        return bus;
    }

    #region Button Event Handlers

    private async void SaveButton_Click(object sender, EventArgs e)
    {
        try
        {
            if (!ValidateForm())
                return;

            _logger.LogInformation("Attempting to save bus data");

            saveButton.Enabled = false;
            saveButton.Text = "Saving...";

            var bus = CreateBusFromForm();

            bool success;
            if (_isEditMode)
            {
                success = await _busService.UpdateBusEntityAsync(bus);
                _logger.LogInformation("Updated bus: {BusNumber}", bus.BusNumber);
            }
            else
            {
                await _busService.AddBusEntityAsync(bus);
                success = true;
                _logger.LogInformation("Added new bus: {BusNumber}", bus.BusNumber);
            }

            if (success || !_isEditMode)
            {
                EditedBus = bus;
                IsDataSaved = true;

                MessageBox.Show($"Bus {bus.BusNumber} has been {(_isEditMode ? "updated" : "added")} successfully.",
                    "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            else
            {
                MessageBox.Show("Failed to save bus data. Please try again.",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error saving bus data");
            MessageBox.Show($"Error saving bus data: {ex.Message}", "Error",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        finally
        {
            saveButton.Enabled = true;
            saveButton.Text = "Save";
        }
    }

    private void CancelButton_Click(object sender, EventArgs e)
    {
        try
        {
            _logger.LogInformation("Cancel button clicked");

            if (HasUnsavedChanges())
            {
                var result = MessageBox.Show("You have unsaved changes. Are you sure you want to cancel?",
                    "Unsaved Changes", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (result == DialogResult.No)
                    return;
            }

            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in cancel operation");
            MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    #endregion

    private bool HasUnsavedChanges()
    {
        if (!_isEditMode)
        {
            // For new buses, check if any data has been entered
            return !string.IsNullOrWhiteSpace(busNumberTextBox.Text) ||
                   !string.IsNullOrWhiteSpace(modelTextBox.Text) ||
                   !string.IsNullOrWhiteSpace(vinTextBox.Text) ||
                   !string.IsNullOrWhiteSpace(licenseNumberTextBox.Text);
        }

        if (_currentBus == null) return false;

        // For existing buses, compare with original data
        return busNumberTextBox.Text != _currentBus.BusNumber ||
               yearNumeric.Value != _currentBus.Year ||
               makeComboBox.Text != _currentBus.Make ||
               modelTextBox.Text != _currentBus.Model ||
               seatingCapacityNumeric.Value != _currentBus.SeatingCapacity ||
               vinTextBox.Text != _currentBus.VINNumber ||
               licenseNumberTextBox.Text != _currentBus.LicenseNumber ||
               statusComboBox.Text != _currentBus.Status;
    }

    protected override void OnFormClosing(FormClosingEventArgs e)
    {
        if (this.DialogResult == DialogResult.None && HasUnsavedChanges())
        {
            var result = MessageBox.Show("You have unsaved changes. Are you sure you want to close?",
                "Unsaved Changes", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.No)
            {
                e.Cancel = true;
                return;
            }
        }

        base.OnFormClosing(e);
    }
}
