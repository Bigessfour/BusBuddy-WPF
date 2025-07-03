using Bus_Buddy.Models;
using Bus_Buddy.Services;
using Bus_Buddy.Utilities;
using Microsoft.Extensions.Logging;
using Syncfusion.Windows.Forms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Syncfusion.WinForms.DataGrid;
using Syncfusion.WinForms.DataGrid.Enums;
using Syncfusion.WinForms.DataGrid.Events;
using Microsoft.Extensions.DependencyInjection;

namespace Bus_Buddy.Forms
{
    public partial class TicketManagementForm : Form
    {
        private readonly ILogger<TicketManagementForm> _logger;
        private readonly IServiceProvider _serviceProvider;
        private BindingList<TicketViewModel> _tickets = new();
        private List<Route> _allRoutes = new();
        private bool _isLoading = false;

        public TicketManagementForm(ILogger<TicketManagementForm> logger, IServiceProvider serviceProvider)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));

            InitializeComponent();

            // Configure form for full screen and enhanced layout
            SyncfusionLayoutManager.ConfigureFormForFullScreen(this);

            // Configure the data grid with enhanced layout manager  
            SyncfusionLayoutManager.ConfigureTicketManagementGrid(dataGridTickets);

            InitializeDataGrid();
            InitializeFilters();
            LoadDataAsync();
        }

        private void InitializeDataGrid()
        {
            try
            {
                _tickets = new BindingList<TicketViewModel>();
                dataGridTickets.DataSource = _tickets;

                // Wait for data source to be set, then configure columns with enhanced alignment
                dataGridTickets.DataSourceChanged += (sender, e) =>
                {
                    ConfigureTicketGridColumns();
                };

                _logger.LogInformation("Ticket management data grid initialized successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error initializing ticket management data grid");
                MessageBoxAdv.Show($"Error initializing data grid: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ConfigureTicketGridColumns()
        {
            try
            {
                // Configure column headers and alignment using the layout manager
                SyncfusionLayoutManager.ConfigureColumnAlignment(dataGridTickets, "Id",
                    System.Windows.Forms.HorizontalAlignment.Center, null, 80);
                SyncfusionLayoutManager.ConfigureColumnAlignment(dataGridTickets, "StudentName",
                    System.Windows.Forms.HorizontalAlignment.Left, null, 150);
                SyncfusionLayoutManager.ConfigureColumnAlignment(dataGridTickets, "RouteName",
                    System.Windows.Forms.HorizontalAlignment.Left, null, 120);
                SyncfusionLayoutManager.ConfigureColumnAlignment(dataGridTickets, "TravelDate",
                    System.Windows.Forms.HorizontalAlignment.Center, "MM/dd/yyyy", 100);
                SyncfusionLayoutManager.ConfigureColumnAlignment(dataGridTickets, "IssuedDate",
                    System.Windows.Forms.HorizontalAlignment.Center, "MM/dd/yyyy HH:mm", 130);
                SyncfusionLayoutManager.ConfigureColumnAlignment(dataGridTickets, "TicketType",
                    System.Windows.Forms.HorizontalAlignment.Center, null, 100);
                SyncfusionLayoutManager.ConfigureColumnAlignment(dataGridTickets, "Price",
                    System.Windows.Forms.HorizontalAlignment.Right, "C2", 80);
                SyncfusionLayoutManager.ConfigureColumnAlignment(dataGridTickets, "Status",
                    System.Windows.Forms.HorizontalAlignment.Center, null, 80);
                SyncfusionLayoutManager.ConfigureColumnAlignment(dataGridTickets, "PaymentMethod",
                    System.Windows.Forms.HorizontalAlignment.Center, null, 100);
                SyncfusionLayoutManager.ConfigureColumnAlignment(dataGridTickets, "QRCode",
                    System.Windows.Forms.HorizontalAlignment.Center, null, 100);

                // Set custom header texts
                if (dataGridTickets.Columns["Id"] != null)
                    dataGridTickets.Columns["Id"].HeaderText = "Ticket ID";
                if (dataGridTickets.Columns["StudentName"] != null)
                    dataGridTickets.Columns["StudentName"].HeaderText = "Student Name";
                if (dataGridTickets.Columns["RouteName"] != null)
                    dataGridTickets.Columns["RouteName"].HeaderText = "Route";
                if (dataGridTickets.Columns["TravelDate"] != null)
                    dataGridTickets.Columns["TravelDate"].HeaderText = "Travel Date";
                if (dataGridTickets.Columns["IssuedDate"] != null)
                    dataGridTickets.Columns["IssuedDate"].HeaderText = "Issued Date";
                if (dataGridTickets.Columns["TicketType"] != null)
                    dataGridTickets.Columns["TicketType"].HeaderText = "Ticket Type";
                if (dataGridTickets.Columns["Price"] != null)
                    dataGridTickets.Columns["Price"].HeaderText = "Price";
                if (dataGridTickets.Columns["Status"] != null)
                    dataGridTickets.Columns["Status"].HeaderText = "Status";
                if (dataGridTickets.Columns["PaymentMethod"] != null)
                    dataGridTickets.Columns["PaymentMethod"].HeaderText = "Payment";
                if (dataGridTickets.Columns["QRCode"] != null)
                    dataGridTickets.Columns["QRCode"].HeaderText = "QR Code";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error configuring ticket grid columns");
            }
        }

        private void InitializeFilters()
        {
            try
            {
                // Status filter
                cmbStatusFilter.Items.Add("All Statuses");
                cmbStatusFilter.Items.Add("Valid");
                cmbStatusFilter.Items.Add("Used");
                cmbStatusFilter.Items.Add("Expired");
                cmbStatusFilter.Items.Add("Cancelled");
                cmbStatusFilter.SelectedIndex = 0;

                // Date filter (default to today)
                dtpDateFilter.Value = DateTime.Today;

                _logger.LogInformation("Ticket management filters initialized");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error initializing filters");
                MessageBoxAdv.Show($"Error initializing filters: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void LoadDataAsync()
        {
            if (_isLoading) return;

            try
            {
                _isLoading = true;
                statusLabel.Text = "Loading tickets...";

                await LoadRoutesAsync();
                LoadTickets();

                statusLabel.Text = $"Loaded {_tickets.Count} tickets";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading ticket management data");
                MessageBoxAdv.Show($"Error loading data: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                statusLabel.Text = "Error loading data";
            }
            finally
            {
                _isLoading = false;
            }
        }

        private async Task LoadRoutesAsync()
        {
            try
            {
                using var scope = _serviceProvider.CreateScope();
                var routeService = scope.ServiceProvider.GetRequiredService<IRouteService>();

                _allRoutes = (await routeService.GetAllActiveRoutesAsync()).ToList();

                cmbRouteFilter.Items.Clear();
                cmbRouteFilter.Items.Add("All Routes");

                foreach (var route in _allRoutes)
                {
                    cmbRouteFilter.Items.Add($"{route.RouteId} - {route.RouteName}");
                }

                cmbRouteFilter.SelectedIndex = 0;

                _logger.LogInformation($"Loaded {_allRoutes.Count} routes for ticket management");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading routes for ticket management");
                throw;
            }
        }

        private void LoadTickets()
        {
            try
            {
                // Since we don't have a ticket service yet, let's create mock data
                var mockTickets = GenerateMockTickets();

                _tickets.Clear();
                foreach (var ticket in mockTickets)
                {
                    _tickets.Add(ticket);
                }

                UpdateSummaryLabels();

                _logger.LogInformation($"Loaded {_tickets.Count} tickets");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading tickets");
                throw;
            }
        }

        private List<TicketViewModel> GenerateMockTickets()
        {
            var random = new Random();
            var tickets = new List<TicketViewModel>();
            var ticketTypes = new[] { "Single", "Round Trip", "Daily Pass", "Weekly Pass", "Monthly Pass" };
            var statuses = new[] { "Valid", "Used", "Expired", "Cancelled" };
            var paymentMethods = new[] { "Cash", "Card", "Mobile Pay", "Student Account" };

            for (int i = 1; i <= 50; i++)
            {
                var route = _allRoutes?.FirstOrDefault() ?? new Route { RouteId = 1, RouteName = "Main Route" };
                var travelDate = DateTime.Today.AddDays(random.Next(-30, 30));
                var ticketType = ticketTypes[random.Next(ticketTypes.Length)];
                var status = statuses[random.Next(statuses.Length)];

                tickets.Add(new TicketViewModel
                {
                    Id = i,
                    StudentName = $"Student {i:000}",
                    RouteName = $"{route.RouteId} - {route.RouteName}",
                    TravelDate = travelDate,
                    IssuedDate = DateTime.Now.AddDays(-random.Next(0, 30)).AddHours(-random.Next(0, 24)),
                    TicketType = ticketType,
                    Price = ticketType switch
                    {
                        "Single" => 2.50m,
                        "Round Trip" => 4.50m,
                        "Daily Pass" => 8.00m,
                        "Weekly Pass" => 35.00m,
                        "Monthly Pass" => 120.00m,
                        _ => 2.50m
                    },
                    Status = status,
                    PaymentMethod = paymentMethods[random.Next(paymentMethods.Length)],
                    QRCode = $"QR{i:0000}{random.Next(1000, 9999)}"
                });
            }

            return tickets.OrderByDescending(t => t.IssuedDate).ToList();
        }

        private void UpdateSummaryLabels()
        {
            try
            {
                var validTickets = _tickets.Where(t => t.Status == "Valid" || t.Status == "Used").ToList();
                var totalRevenue = validTickets.Sum(t => t.Price);
                var ticketsSold = validTickets.Count;

                lblTotalRevenue.Text = $"Total Revenue: {totalRevenue:C2}";
                lblTicketsSold.Text = $"Tickets Sold: {ticketsSold:N0}";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating summary labels");
            }
        }

        private void ApplyFilters()
        {
            if (_isLoading || _tickets == null) return;

            try
            {
                var filteredTickets = _tickets.AsEnumerable();

                // Search filter
                if (!string.IsNullOrWhiteSpace(txtSearch.Text))
                {
                    var searchTerm = txtSearch.Text.ToLower();
                    filteredTickets = filteredTickets.Where(t =>
                        t.StudentName.ToLower().Contains(searchTerm) ||
                        t.RouteName.ToLower().Contains(searchTerm) ||
                        t.QRCode.ToLower().Contains(searchTerm) ||
                        t.TicketType.ToLower().Contains(searchTerm));
                }

                // Route filter
                if (cmbRouteFilter.SelectedIndex > 0)
                {
                    var selectedRoute = cmbRouteFilter.SelectedItem.ToString();
                    filteredTickets = filteredTickets.Where(t => t.RouteName == selectedRoute);
                }

                // Status filter
                if (cmbStatusFilter.SelectedIndex > 0)
                {
                    var selectedStatus = cmbStatusFilter.SelectedItem.ToString();
                    filteredTickets = filteredTickets.Where(t => t.Status == selectedStatus);
                }

                // Date filter
                var filterDate = dtpDateFilter.Value.Date;
                filteredTickets = filteredTickets.Where(t => t.TravelDate.Date >= filterDate);

                // Update data source
                var newList = new BindingList<TicketViewModel>(filteredTickets.ToList());
                dataGridTickets.DataSource = newList;

                statusLabel.Text = $"Showing {newList.Count} of {_tickets.Count} tickets";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error applying filters");
                MessageBoxAdv.Show($"Error applying filters: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnAdd_Click(object sender, EventArgs e)
        {
            try
            {
                MessageBoxAdv.Show("Ticket Sale Form will be implemented here.", "Coming Soon",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                _logger.LogInformation("Add ticket button clicked");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in add ticket");
                MessageBoxAdv.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnEdit_Click(object sender, EventArgs e)
        {
            try
            {
                if (dataGridTickets.SelectedItem is TicketViewModel selectedTicket)
                {
                    MessageBoxAdv.Show($"Edit Ticket Form for Ticket ID: {selectedTicket.Id} will be implemented here.",
                        "Coming Soon", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    _logger.LogInformation($"Edit ticket button clicked for ticket {selectedTicket.Id}");
                }
                else
                {
                    MessageBoxAdv.Show("Please select a ticket to edit.", "No Selection",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in edit ticket");
                MessageBoxAdv.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnDelete_Click(object sender, EventArgs e)
        {
            try
            {
                if (dataGridTickets.SelectedItem is TicketViewModel selectedTicket)
                {
                    var result = MessageBoxAdv.Show($"Are you sure you want to cancel ticket {selectedTicket.Id}?",
                        "Confirm Cancellation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                    if (result == DialogResult.Yes)
                    {
                        selectedTicket.Status = "Cancelled";
                        dataGridTickets.Refresh();
                        UpdateSummaryLabels();
                        _logger.LogInformation($"Ticket {selectedTicket.Id} cancelled");
                        MessageBoxAdv.Show("Ticket cancelled successfully.", "Success",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
                else
                {
                    MessageBoxAdv.Show("Please select a ticket to cancel.", "No Selection",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error cancelling ticket");
                MessageBoxAdv.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnRefresh_Click(object sender, EventArgs e)
        {
            try
            {
                LoadDataAsync();
                _logger.LogInformation("Ticket data refreshed");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error refreshing ticket data");
                MessageBoxAdv.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnViewDetails_Click(object sender, EventArgs e)
        {
            try
            {
                if (dataGridTickets.SelectedItem is TicketViewModel selectedTicket)
                {
                    var details = $"Ticket Details:\n\n" +
                                $"Ticket ID: {selectedTicket.Id}\n" +
                                $"Student: {selectedTicket.StudentName}\n" +
                                $"Route: {selectedTicket.RouteName}\n" +
                                $"Travel Date: {selectedTicket.TravelDate:MM/dd/yyyy}\n" +
                                $"Issued: {selectedTicket.IssuedDate:MM/dd/yyyy HH:mm}\n" +
                                $"Type: {selectedTicket.TicketType}\n" +
                                $"Price: {selectedTicket.Price:C2}\n" +
                                $"Status: {selectedTicket.Status}\n" +
                                $"Payment: {selectedTicket.PaymentMethod}\n" +
                                $"QR Code: {selectedTicket.QRCode}";

                    MessageBoxAdv.Show(details, "Ticket Details", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    _logger.LogInformation($"Viewed details for ticket {selectedTicket.Id}");
                }
                else
                {
                    MessageBoxAdv.Show("Please select a ticket to view details.", "No Selection",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error viewing ticket details");
                MessageBoxAdv.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnPrintTicket_Click(object sender, EventArgs e)
        {
            try
            {
                if (dataGridTickets.SelectedItem is TicketViewModel selectedTicket)
                {
                    MessageBoxAdv.Show($"Print functionality for Ticket {selectedTicket.Id} will be implemented here.",
                        "Coming Soon", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    _logger.LogInformation($"Print ticket button clicked for ticket {selectedTicket.Id}");
                }
                else
                {
                    MessageBoxAdv.Show("Please select a ticket to print.", "No Selection",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error printing ticket");
                MessageBoxAdv.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void TxtSearch_TextChanged(object sender, EventArgs e)
        {
            ApplyFilters();
        }

        private void CmbRouteFilter_SelectedIndexChanged(object sender, EventArgs e)
        {
            ApplyFilters();
        }

        private void CmbStatusFilter_SelectedIndexChanged(object sender, EventArgs e)
        {
            ApplyFilters();
        }

        private void DtpDateFilter_ValueChanged(object sender, EventArgs e)
        {
            ApplyFilters();
        }

        private void DataGridTickets_CellDoubleClick(object sender, CellClickEventArgs e)
        {
            BtnViewDetails_Click(sender, e);
        }
    }

    // TicketViewModel class for data binding
    public class TicketViewModel
    {
        public int Id { get; set; }
        public string StudentName { get; set; } = string.Empty;
        public string RouteName { get; set; } = string.Empty;
        public DateTime TravelDate { get; set; }
        public DateTime IssuedDate { get; set; }
        public string TicketType { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string Status { get; set; } = string.Empty;
        public string PaymentMethod { get; set; } = string.Empty;
        public string QRCode { get; set; } = string.Empty;
    }
}
