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
/// Route Management Form for managing bus routes
/// Demonstrates route planning and management functionality
/// </summary>
public partial class RouteManagementForm : MetroForm
{
    private readonly ILogger<RouteManagementForm> _logger;
    private readonly IBusService _busService;
    private List<Route> _routes = new List<Route>();

    public RouteManagementForm(ILogger<RouteManagementForm> logger, IBusService busService)
    {
        _logger = logger;
        _busService = busService;

        _logger.LogInformation("Initializing Route Management form");
        InitializeComponent();
        InitializeRouteManagement();
    }

    private void InitializeRouteManagement()
    {
        // Set Syncfusion MetroForm styles
        this.MetroColor = System.Drawing.Color.FromArgb(255, 87, 34);
        this.CaptionBarColor = System.Drawing.Color.FromArgb(255, 87, 34);
        this.CaptionForeColor = System.Drawing.Color.White;
        this.Text = "Route Management - Bus Routes";

        // Enable high DPI scaling for this form
        this.AutoScaleMode = AutoScaleMode.Dpi;
        this.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);

        // Configure data grid columns
        ConfigureDataGridColumns();

        _logger.LogInformation("Route Management form initialized successfully");

        // Load route data
        LoadRouteDataAsync();
    }

    private void ConfigureDataGridColumns()
    {
        // Clear auto-generated columns
        routeDataGrid.AutoGenerateColumns = false;
        routeDataGrid.Columns.Clear();

        // Add custom columns
        routeDataGrid.Columns.Add(new DataGridViewTextBoxColumn
        {
            Name = "RouteId",
            HeaderText = "Route ID",
            DataPropertyName = "RouteId",
            Width = 80,
            ReadOnly = true
        });

        routeDataGrid.Columns.Add(new DataGridViewTextBoxColumn
        {
            Name = "RouteName",
            HeaderText = "Route Name",
            DataPropertyName = "RouteName",
            Width = 200
        });

        routeDataGrid.Columns.Add(new DataGridViewTextBoxColumn
        {
            Name = "Description",
            HeaderText = "Description",
            DataPropertyName = "Description",
            Width = 300
        });

        routeDataGrid.Columns.Add(new DataGridViewTextBoxColumn
        {
            Name = "AMBus",
            HeaderText = "AM Bus",
            DataPropertyName = "AMVehicle.BusNumber",
            Width = 100
        });

        routeDataGrid.Columns.Add(new DataGridViewTextBoxColumn
        {
            Name = "PMBus",
            HeaderText = "PM Bus",
            DataPropertyName = "PMVehicle.BusNumber",
            Width = 100
        });

        routeDataGrid.Columns.Add(new DataGridViewTextBoxColumn
        {
            Name = "AMDriver",
            HeaderText = "AM Driver",
            DataPropertyName = "AMDriver.DriverName",
            Width = 150
        });

        routeDataGrid.Columns.Add(new DataGridViewTextBoxColumn
        {
            Name = "PMDriver",
            HeaderText = "PM Driver",
            DataPropertyName = "PMDriver.DriverName",
            Width = 150
        });

        routeDataGrid.Columns.Add(new DataGridViewCheckBoxColumn
        {
            Name = "IsActive",
            HeaderText = "Active",
            DataPropertyName = "IsActive",
            Width = 60
        });
    }

    private async void LoadRouteDataAsync()
    {
        try
        {
            _logger.LogInformation("Loading route data");

            // Load routes from service
            _routes = await _busService.GetAllRouteEntitiesAsync();

            // Update the data grid
            routeDataGrid.DataSource = _routes;

            _logger.LogInformation("Loaded {RouteCount} routes", _routes.Count);

            // Update status
            statusLabel.Text = $"Loaded {_routes.Count} routes";

            // Highlight inactive routes
            HighlightInactiveRoutes();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading route data");
            MessageBox.Show($"Error loading route data: {ex.Message}", "Error",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
            statusLabel.Text = "Error loading data";
        }
    }

    private void HighlightInactiveRoutes()
    {
        try
        {
            var inactiveCount = 0;

            foreach (DataGridViewRow row in routeDataGrid.Rows)
            {
                if (row.DataBoundItem is Route route && !route.IsActive)
                {
                    row.DefaultCellStyle.BackColor = System.Drawing.Color.FromArgb(250, 250, 250);
                    row.DefaultCellStyle.ForeColor = System.Drawing.Color.Gray;
                    inactiveCount++;
                }
            }

            if (inactiveCount > 0)
            {
                statusLabel.Text += $" - {inactiveCount} inactive route(s)";
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Error highlighting inactive routes");
        }
    }

    #region Button Event Handlers

    private void AddRouteButton_Click(object sender, EventArgs e)
    {
        try
        {
            _logger.LogInformation("Add Route button clicked");

            MessageBox.Show("Add Route functionality will be implemented here.\n\nThis will open a form to create a new bus route including:\n• Route planning\n• Stop assignments\n• Bus and driver assignments",
                "Add Route", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in Add Route");
            MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void EditRouteButton_Click(object sender, EventArgs e)
    {
        try
        {
            _logger.LogInformation("Edit Route button clicked");

            if (routeDataGrid.SelectedRows.Count > 0)
            {
                var selectedRowIndex = routeDataGrid.SelectedRows[0].Index;
                var selectedRoute = _routes[selectedRowIndex];

                MessageBox.Show($"Edit Route functionality will be implemented here.\n\nSelected Route: {selectedRoute.RouteName}\nDescription: {selectedRoute.Description}",
                    "Edit Route", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("Please select a route to edit.", "No Selection",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in Edit Route");
            MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void DeleteRouteButton_Click(object sender, EventArgs e)
    {
        try
        {
            _logger.LogInformation("Delete Route button clicked");

            if (routeDataGrid.SelectedRows.Count > 0)
            {
                var selectedRowIndex = routeDataGrid.SelectedRows[0].Index;
                var selectedRoute = _routes[selectedRowIndex];
                var result = MessageBox.Show($"Are you sure you want to delete route '{selectedRoute.RouteName}'?\n\nThis action cannot be undone.",
                    "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);

                if (result == DialogResult.Yes)
                {
                    MessageBox.Show($"Delete Route functionality will be implemented here.\n\nRoute '{selectedRoute.RouteName}' would be deleted.",
                        "Delete Route", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            else
            {
                MessageBox.Show("Please select a route to delete.", "No Selection",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in Delete Route");
            MessageBox.Show($"Error deleting route: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void ViewStopsButton_Click(object sender, EventArgs e)
    {
        try
        {
            _logger.LogInformation("View Stops button clicked");

            if (routeDataGrid.SelectedRows.Count > 0)
            {
                var selectedRowIndex = routeDataGrid.SelectedRows[0].Index;
                var selectedRoute = _routes[selectedRowIndex];

                // Show route stops information
                var stopsInfo = $"Route Stops for '{selectedRoute.RouteName}'\n\n" +
                    $"Route ID: {selectedRoute.RouteId}\n" +
                    $"Description: {selectedRoute.Description}\n" +
                    $"Status: {(selectedRoute.IsActive ? "Active" : "Inactive")}\n\n" +
                    "This will show detailed stop information including:\n" +
                    "• Stop locations and addresses\n" +
                    "• Pickup times\n" +
                    "• Student assignments\n" +
                    "• Special instructions";

                MessageBox.Show(stopsInfo, "Route Stops", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("Please select a route to view stops.", "No Selection",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error viewing route stops");
            MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private async void RefreshButton_Click(object sender, EventArgs e)
    {
        try
        {
            _logger.LogInformation("Refresh button clicked");
            statusLabel.ForeColor = System.Drawing.Color.Gray;
            await Task.Run(() => LoadRouteDataAsync());
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

    private void RouteDataGrid_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
    {
        if (e.RowIndex >= 0)
        {
            // Double-click to edit route
            EditRouteButton_Click(sender, EventArgs.Empty);
        }
    }

    private void RouteDataGrid_SelectionChanged(object sender, EventArgs e)
    {
        // Enable/disable buttons based on selection
        var hasSelection = routeDataGrid.SelectedRows.Count > 0;
        editRouteButton.Enabled = hasSelection;
        deleteRouteButton.Enabled = hasSelection;
        viewStopsButton.Enabled = hasSelection;
    }
}
