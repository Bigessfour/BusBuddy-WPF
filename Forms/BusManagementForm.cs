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
/// Bus Management Form for managing fleet vehicles
/// Demonstrates how model management forms integrate with the dashboard
/// </summary>
public partial class BusManagementForm : MetroForm
{
    private readonly ILogger<BusManagementForm> _logger;
    private readonly IBusService _busService;
    private List<BusInfo> _buses = new List<BusInfo>();

    public BusManagementForm(ILogger<BusManagementForm> logger, IBusService busService)
    {
        _logger = logger;
        _busService = busService;

        _logger.LogInformation("Initializing Bus Management form");
        InitializeComponent();
        InitializeBusManagement();
    }

    private void InitializeBusManagement()
    {
        // Set Syncfusion MetroForm styles
        this.MetroColor = System.Drawing.Color.FromArgb(63, 81, 181);
        this.CaptionBarColor = System.Drawing.Color.FromArgb(63, 81, 181);
        this.CaptionForeColor = System.Drawing.Color.White;
        this.Text = "Bus Management - Fleet Vehicles";

        // Enable high DPI scaling for this form
        this.AutoScaleMode = AutoScaleMode.Dpi;
        this.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);

        _logger.LogInformation("Bus Management form initialized successfully");

        // Load bus data
        LoadBusDataAsync();
    }

    private async void LoadBusDataAsync()
    {
        try
        {
            _logger.LogInformation("Loading bus data");

            // Load buses from service
            _buses = await _busService.GetAllBusesAsync();

            // Update the data grid
            busDataGrid.DataSource = _buses;

            _logger.LogInformation("Loaded {BusCount} buses", _buses.Count);

            // Update status
            statusLabel.Text = $"Loaded {_buses.Count} buses";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading bus data");
            MessageBox.Show($"Error loading bus data: {ex.Message}", "Error",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
            statusLabel.Text = "Error loading data";
        }
    }

    #region Button Event Handlers

    private void AddBusButton_Click(object sender, EventArgs e)
    {
        try
        {
            _logger.LogInformation("Add Bus button clicked");

            // Open Add Bus dialog using the service container
            var busEditForm = ServiceContainer.GetService<BusEditForm>();
            var result = busEditForm.ShowDialog(this);

            if (result == DialogResult.OK && busEditForm.IsDataSaved)
            {
                // Refresh the data grid
                LoadBusDataAsync();
                statusLabel.Text = $"Bus {busEditForm.EditedBus?.BusNumber} added successfully";
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in Add Bus");
            MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private async void EditBusButton_Click(object sender, EventArgs e)
    {
        try
        {
            _logger.LogInformation("Edit Bus button clicked");

            if (busDataGrid.SelectedRows.Count > 0)
            {
                var selectedRowIndex = busDataGrid.SelectedRows[0].Index;
                var selectedBusInfo = _buses[selectedRowIndex];

                // Get the full Bus entity from the service
                var busEntity = await _busService.GetBusEntityByIdAsync(selectedBusInfo.BusId);

                if (busEntity != null)
                {
                    // Create BusEditForm with dependency injection
                    var logger = ServiceContainer.GetService<ILogger<BusEditForm>>();
                    var busEditForm = new BusEditForm(logger, _busService, busEntity);
                    var result = busEditForm.ShowDialog(this);

                    if (result == DialogResult.OK && busEditForm.IsDataSaved)
                    {
                        // Refresh the data grid
                        LoadBusDataAsync();
                        statusLabel.Text = $"Bus {busEditForm.EditedBus?.BusNumber} updated successfully";
                    }
                }
                else
                {
                    MessageBox.Show("Could not load bus details for editing.", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("Please select a bus to edit.", "No Selection",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in Edit Bus");
            MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private async void DeleteBusButton_Click(object sender, EventArgs e)
    {
        try
        {
            _logger.LogInformation("Delete Bus button clicked");

            if (busDataGrid.SelectedRows.Count > 0)
            {
                var selectedRowIndex = busDataGrid.SelectedRows[0].Index;
                var selectedBus = _buses[selectedRowIndex];
                var result = MessageBox.Show($"Are you sure you want to delete bus {selectedBus.BusNumber}?\n\nThis action cannot be undone.",
                    "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);

                if (result == DialogResult.Yes)
                {
                    deleteBusButton.Enabled = false;
                    deleteBusButton.Text = "Deleting...";

                    bool success = await _busService.DeleteBusEntityAsync(selectedBus.BusId);

                    if (success)
                    {
                        MessageBox.Show($"Bus {selectedBus.BusNumber} has been deleted successfully.",
                            "Delete Successful", MessageBoxButtons.OK, MessageBoxIcon.Information);

                        // Refresh the data grid
                        LoadBusDataAsync();
                        statusLabel.Text = $"Bus {selectedBus.BusNumber} deleted successfully";
                    }
                    else
                    {
                        MessageBox.Show($"Failed to delete bus {selectedBus.BusNumber}. Please try again.",
                            "Delete Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            else
            {
                MessageBox.Show("Please select a bus to delete.", "No Selection",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in Delete Bus");
            MessageBox.Show($"Error deleting bus: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        finally
        {
            deleteBusButton.Enabled = true;
            deleteBusButton.Text = "Delete Bus";
        }
    }

    private async void RefreshButton_Click(object sender, EventArgs e)
    {
        try
        {
            _logger.LogInformation("Refresh button clicked");
            await Task.Run(() => LoadBusDataAsync());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error refreshing data");
            MessageBox.Show($"Error refreshing data: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
}
