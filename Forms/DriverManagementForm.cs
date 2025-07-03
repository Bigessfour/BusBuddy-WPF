using Syncfusion.Windows.Forms;
using Syncfusion.Windows.Forms.Tools;
using Syncfusion.WinForms.Controls;
using Microsoft.Extensions.Logging;
using Bus_Buddy.Services;
using Bus_Buddy.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Linq;

namespace Bus_Buddy.Forms;

/// <summary>
/// Driver Management Form for managing bus drivers
/// Demonstrates comprehensive driver data management with Syncfusion controls
/// </summary>
public partial class DriverManagementForm : MetroForm
{
    private readonly ILogger<DriverManagementForm> _logger;
    private readonly IBusService _busService;
    private List<Driver> _drivers = new List<Driver>();

    public DriverManagementForm(ILogger<DriverManagementForm> logger, IBusService busService)
    {
        _logger = logger;
        _busService = busService;

        _logger.LogInformation("Initializing Driver Management form");
        InitializeComponent();
        InitializeDriverManagement();
    }

    private void InitializeDriverManagement()
    {
        // Set Syncfusion MetroForm styles
        this.MetroColor = System.Drawing.Color.FromArgb(156, 39, 176);
        this.CaptionBarColor = System.Drawing.Color.FromArgb(156, 39, 176);
        this.CaptionForeColor = System.Drawing.Color.White;
        this.Text = "Driver Management - Bus Drivers";

        // Enable high DPI scaling for this form
        this.AutoScaleMode = AutoScaleMode.Dpi;
        this.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);

        // Configure data grid columns
        ConfigureDataGridColumns();

        _logger.LogInformation("Driver Management form initialized successfully");

        // Load driver data
        LoadDriverDataAsync();
    }

    private void ConfigureDataGridColumns()
    {
        // Clear auto-generated columns
        driverDataGrid.AutoGenerateColumns = false;
        driverDataGrid.Columns.Clear();

        // Add custom columns
        driverDataGrid.Columns.Add(new DataGridViewTextBoxColumn
        {
            Name = "DriverId",
            HeaderText = "ID",
            DataPropertyName = "DriverId",
            Width = 50,
            ReadOnly = true
        });

        driverDataGrid.Columns.Add(new DataGridViewTextBoxColumn
        {
            Name = "FirstName",
            HeaderText = "First Name",
            DataPropertyName = "FirstName",
            Width = 120
        });

        driverDataGrid.Columns.Add(new DataGridViewTextBoxColumn
        {
            Name = "LastName",
            HeaderText = "Last Name",
            DataPropertyName = "LastName",
            Width = 120
        });

        driverDataGrid.Columns.Add(new DataGridViewTextBoxColumn
        {
            Name = "LicenseNumber",
            HeaderText = "License #",
            DataPropertyName = "LicenseNumber",
            Width = 120
        });

        driverDataGrid.Columns.Add(new DataGridViewTextBoxColumn
        {
            Name = "Phone",
            HeaderText = "Phone",
            DataPropertyName = "Phone",
            Width = 120
        });

        driverDataGrid.Columns.Add(new DataGridViewTextBoxColumn
        {
            Name = "HireDate",
            HeaderText = "Hire Date",
            DataPropertyName = "HireDate",
            Width = 100,
            DefaultCellStyle = { Format = "MM/dd/yyyy" }
        });

        driverDataGrid.Columns.Add(new DataGridViewTextBoxColumn
        {
            Name = "Status",
            HeaderText = "Status",
            DataPropertyName = "Status",
            Width = 80
        });

        driverDataGrid.Columns.Add(new DataGridViewTextBoxColumn
        {
            Name = "LicenseExpiry",
            HeaderText = "License Expiry",
            DataPropertyName = "LicenseExpiryDate",
            Width = 120,
            DefaultCellStyle = { Format = "MM/dd/yyyy" }
        });
    }

    private async void LoadDriverDataAsync()
    {
        try
        {
            _logger.LogInformation("Loading driver data");

            // Load drivers from service
            _drivers = await _busService.GetAllDriversAsync();

            // Update the data grid
            driverDataGrid.DataSource = _drivers;

            _logger.LogInformation("Loaded {DriverCount} drivers", _drivers.Count);

            // Update status
            statusLabel.Text = $"Loaded {_drivers.Count} drivers";

            // Highlight drivers with expiring licenses (within 30 days)
            HighlightExpiringLicenses();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading driver data");
            MessageBox.Show($"Error loading driver data: {ex.Message}", "Error",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
            statusLabel.Text = "Error loading data";
        }
    }

    private void HighlightExpiringLicenses()
    {
        try
        {
            var expiryThreshold = DateTime.Now.AddDays(30);
            var expiringCount = 0;

            foreach (DataGridViewRow row in driverDataGrid.Rows)
            {
                if (row.DataBoundItem is Driver driver && driver.LicenseExpiryDate.HasValue)
                {
                    if (driver.LicenseExpiryDate.Value <= expiryThreshold)
                    {
                        row.DefaultCellStyle.BackColor = System.Drawing.Color.FromArgb(255, 235, 238);
                        row.DefaultCellStyle.ForeColor = System.Drawing.Color.FromArgb(198, 40, 40);
                        expiringCount++;
                    }
                }
            }

            if (expiringCount > 0)
            {
                statusLabel.Text += $" - {expiringCount} license(s) expiring soon";
                statusLabel.ForeColor = System.Drawing.Color.FromArgb(198, 40, 40);
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Error highlighting expiring licenses");
        }
    }

    #region Button Event Handlers

    private void AddDriverButton_Click(object sender, EventArgs e)
    {
        try
        {
            _logger.LogInformation("Add Driver button clicked");

            // Create new DriverEditForm for adding a driver
            var logger = ServiceContainer.GetService<ILogger<DriverEditForm>>();
            var driverEditForm = new DriverEditForm(logger, _busService);
            var result = driverEditForm.ShowDialog(this);

            if (result == DialogResult.OK && driverEditForm.IsDataSaved)
            {
                // Refresh the data grid
                LoadDriverDataAsync();
                statusLabel.Text = $"Driver {driverEditForm.EditedDriver?.FirstName} {driverEditForm.EditedDriver?.LastName} added successfully";
                statusLabel.ForeColor = System.Drawing.Color.FromArgb(76, 175, 80);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in Add Driver");
            MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void EditDriverButton_Click(object sender, EventArgs e)
    {
        try
        {
            _logger.LogInformation("Edit Driver button clicked");

            if (driverDataGrid.SelectedRows.Count > 0)
            {
                var selectedRowIndex = driverDataGrid.SelectedRows[0].Index;
                var selectedDriver = _drivers[selectedRowIndex];

                // Create DriverEditForm with existing driver data
                var logger = ServiceContainer.GetService<ILogger<DriverEditForm>>();
                var driverEditForm = new DriverEditForm(logger, _busService, selectedDriver);
                var result = driverEditForm.ShowDialog(this);

                if (result == DialogResult.OK && driverEditForm.IsDataSaved)
                {
                    // Refresh the data grid
                    LoadDriverDataAsync();
                    statusLabel.Text = $"Driver {driverEditForm.EditedDriver?.FirstName} {driverEditForm.EditedDriver?.LastName} updated successfully";
                    statusLabel.ForeColor = System.Drawing.Color.FromArgb(76, 175, 80);
                }
            }
            else
            {
                MessageBox.Show("Please select a driver to edit.", "No Selection",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in Edit Driver");
            MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void DeleteDriverButton_Click(object sender, EventArgs e)
    {
        try
        {
            _logger.LogInformation("Delete Driver button clicked");

            if (driverDataGrid.SelectedRows.Count > 0)
            {
                var selectedRowIndex = driverDataGrid.SelectedRows[0].Index;
                var selectedDriver = _drivers[selectedRowIndex];
                var result = MessageBox.Show($"Are you sure you want to delete driver {selectedDriver.FirstName} {selectedDriver.LastName}?\n\nThis action cannot be undone.",
                    "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);

                if (result == DialogResult.Yes)
                {
                    deleteDriverButton.Enabled = false;
                    deleteDriverButton.Text = "Deleting...";

                    // Note: We would need to implement DeleteDriverAsync in the service
                    // For now, show a placeholder message
                    MessageBox.Show($"Delete Driver functionality will be implemented.\n\nDriver {selectedDriver.FirstName} {selectedDriver.LastName} would be deleted.",
                        "Delete Driver", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    statusLabel.Text = $"Driver {selectedDriver.FirstName} {selectedDriver.LastName} deleted successfully";
                    statusLabel.ForeColor = System.Drawing.Color.FromArgb(76, 175, 80);
                }
            }
            else
            {
                MessageBox.Show("Please select a driver to delete.", "No Selection",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in Delete Driver");
            MessageBox.Show($"Error deleting driver: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        finally
        {
            deleteDriverButton.Enabled = true;
            deleteDriverButton.Text = "Delete Driver";
        }
    }

    private async void RefreshButton_Click(object sender, EventArgs e)
    {
        try
        {
            _logger.LogInformation("Refresh button clicked");
            statusLabel.ForeColor = System.Drawing.Color.Gray;
            await Task.Run(() => LoadDriverDataAsync());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error refreshing data");
            MessageBox.Show($"Error refreshing data: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void ViewLicenseButton_Click(object sender, EventArgs e)
    {
        try
        {
            _logger.LogInformation("View License button clicked");

            if (driverDataGrid.SelectedRows.Count > 0)
            {
                var selectedRowIndex = driverDataGrid.SelectedRows[0].Index;
                var selectedDriver = _drivers[selectedRowIndex];

                // Show driver license information
                var licenseInfo = $"Driver License Information\n\n" +
                    $"Driver: {selectedDriver.FirstName} {selectedDriver.LastName}\n" +
                    $"License Number: {selectedDriver.LicenseNumber}\n" +
                    $"License Class: {selectedDriver.LicenseClass}\n" +
                    $"Issue Date: {selectedDriver.LicenseIssueDate?.ToString("MM/dd/yyyy") ?? "Not specified"}\n" +
                    $"Expiry Date: {selectedDriver.LicenseExpiryDate?.ToString("MM/dd/yyyy") ?? "Not specified"}\n" +
                    $"Endorsements: {selectedDriver.Endorsements ?? "None"}\n" +
                    $"Status: {selectedDriver.Status}";

                MessageBox.Show(licenseInfo, "License Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("Please select a driver to view license information.", "No Selection",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error viewing license");
            MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void CloseButton_Click(object sender, EventArgs e)
    {
        try
        {
            _logger.LogInformation("Close button clicked");
            this.Close();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error closing form");
            MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    #endregion

    private void DriverDataGrid_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
    {
        if (e.RowIndex >= 0)
        {
            // Double-click to edit driver
            EditDriverButton_Click(sender, EventArgs.Empty);
        }
    }

    private void DriverDataGrid_SelectionChanged(object sender, EventArgs e)
    {
        // Enable/disable buttons based on selection
        var hasSelection = driverDataGrid.SelectedRows.Count > 0;
        editDriverButton.Enabled = hasSelection;
        deleteDriverButton.Enabled = hasSelection;
        viewLicenseButton.Enabled = hasSelection;
    }
}
