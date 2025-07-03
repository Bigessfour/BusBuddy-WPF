using Bus_Buddy.Models;
using Bus_Buddy.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Syncfusion.Drawing;
using Syncfusion.Windows.Forms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Bus_Buddy.Forms
{
    public partial class TicketEditForm : MetroForm
    {
        private readonly ILogger<TicketEditForm> _logger;
        private readonly IServiceProvider _serviceProvider;
        private Ticket? _currentTicket;
        private List<Student> _students = new();
        private List<Route> _routes = new();
        private bool _isLoading = false;
        private bool _isEditMode = false;

        public Ticket? CurrentTicket => _currentTicket;

        public TicketEditForm(ILogger<TicketEditForm> logger, IServiceProvider serviceProvider, Ticket? ticket = null)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            _currentTicket = ticket;
            _isEditMode = ticket != null;

            InitializeComponent();
            SetupFormAppearance();
            LoadDataAsync();
        }

        private void SetupFormAppearance()
        {
            // Configure basic form appearance
            this.ShowIcon = false;
            this.StartPosition = FormStartPosition.CenterParent;

            // Set form title
            this.Text = _isEditMode ? "Edit Ticket" : "New Ticket";

            // Configure form size and behavior
            this.Size = new Size(600, 700);
            this.MinimumSize = new Size(580, 650);
            this.MaximizeBox = false;

            // Configure controls appearance
            ConfigureControlStyles();
        }

        private void ConfigureControlStyles()
        {
            // Configure text boxes
            foreach (Control control in this.Controls)
            {
                if (control is TextBox textBox)
                {
                    textBox.Font = new Font("Segoe UI", 10F);
                    textBox.BackColor = Color.White;
                    textBox.BorderStyle = BorderStyle.FixedSingle;
                }
                else if (control is ComboBox comboBox)
                {
                    comboBox.Font = new Font("Segoe UI", 10F);
                    comboBox.BackColor = Color.White;
                    comboBox.FlatStyle = FlatStyle.Flat;
                }
                else if (control is DateTimePicker dateTimePicker)
                {
                    dateTimePicker.Font = new Font("Segoe UI", 10F);
                    dateTimePicker.CalendarMonthBackground = Color.White;
                }
                else if (control is NumericUpDown numericUpDown)
                {
                    numericUpDown.Font = new Font("Segoe UI", 10F);
                    numericUpDown.BackColor = Color.White;
                    numericUpDown.BorderStyle = BorderStyle.FixedSingle;
                }
                else if (control is Label label && !label.Name.StartsWith("lbl"))
                {
                    label.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
                    label.ForeColor = ColorTranslator.FromHtml("#2c3e50");
                }
            }
        }

        private async void LoadDataAsync()
        {
            if (_isLoading) return;

            try
            {
                _isLoading = true;

                await LoadStudentsAsync();
                await LoadRoutesAsync();

                if (_isEditMode && _currentTicket != null)
                {
                    PopulateFormWithTicketData();
                }
                else
                {
                    SetDefaultValues();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading ticket edit form data");
                MessageBoxAdv.Show($"Error loading data: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                _isLoading = false;
            }
        }

        private async Task LoadStudentsAsync()
        {
            try
            {
                using var scope = _serviceProvider.CreateScope();
                var studentService = scope.ServiceProvider.GetRequiredService<IStudentService>();

                _students = (await studentService.GetActiveStudentsAsync()).ToList();

                cmbStudent.Items.Clear();
                cmbStudent.Items.Add("Select Student...");

                foreach (var student in _students.OrderBy(s => s.FullName))
                {
                    cmbStudent.Items.Add($"{student.FullName} ({student.StudentNumber})");
                }

                cmbStudent.SelectedIndex = 0;
                _logger.LogInformation($"Loaded {_students.Count} students for ticket form");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading students for ticket form");
                throw;
            }
        }

        private Task LoadRoutesAsync()
        {
            try
            {
                // TODO: Replace with actual IRouteService when implemented
                // For now, create mock routes from existing data
                _routes = new List<Route>
                {
                    new Route { RouteId = 1, RouteName = "Route 1" },
                    new Route { RouteId = 2, RouteName = "Route 2" },
                    new Route { RouteId = 3, RouteName = "Route 3" }
                };

                cmbRoute.Items.Clear();
                cmbRoute.Items.Add("Select Route...");

                foreach (var route in _routes.OrderBy(r => r.RouteName))
                {
                    cmbRoute.Items.Add($"{route.RouteName} (ID: {route.RouteId})");
                }

                cmbRoute.SelectedIndex = 0;
                _logger.LogInformation($"Loaded {_routes.Count} mock routes for ticket form");
                return Task.CompletedTask;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading routes for ticket form");
                throw;
            }
        }

        private void SetDefaultValues()
        {
            try
            {
                dtpTravelDate.Value = DateTime.Today.AddDays(1); // Default to tomorrow
                dtpIssuedDate.Value = DateTime.Now;

                // Set default ticket type
                cmbTicketType.Items.Clear();
                cmbTicketType.Items.AddRange(new string[]
                {
                    "Single", "Round Trip", "Daily Pass", "Weekly Pass", "Monthly Pass"
                });
                cmbTicketType.SelectedIndex = 0;

                // Set default payment method
                cmbPaymentMethod.Items.Clear();
                cmbPaymentMethod.Items.AddRange(new string[]
                {
                    "Cash", "Card", "Mobile Pay", "Student Account"
                });
                cmbPaymentMethod.SelectedIndex = 0;

                // Set default status
                cmbStatus.Items.Clear();
                cmbStatus.Items.AddRange(new string[]
                {
                    "Valid", "Used", "Expired", "Cancelled"
                });
                cmbStatus.SelectedIndex = 0;

                // Set default price
                UpdatePriceBasedOnTicketType();

                chkIsRefundable.Checked = true;
                txtQRCode.Text = "Auto-generated";
                txtQRCode.ReadOnly = true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error setting default values");
            }
        }

        private void PopulateFormWithTicketData()
        {
            if (_currentTicket == null) return;

            try
            {
                // Find and select student
                var studentIndex = _students.FindIndex(s => s.StudentId == _currentTicket.StudentId);
                if (studentIndex >= 0)
                {
                    cmbStudent.SelectedIndex = studentIndex + 1; // +1 for "Select Student..." item
                }

                // Find and select route
                var routeIndex = _routes.FindIndex(r => r.RouteId == _currentTicket.RouteId);
                if (routeIndex >= 0)
                {
                    cmbRoute.SelectedIndex = routeIndex + 1; // +1 for "Select Route..." item
                }

                // Set dates
                dtpTravelDate.Value = _currentTicket.TravelDate;
                dtpIssuedDate.Value = _currentTicket.IssuedDate;

                // Set ticket type
                SetDefaultValues(); // Initialize combo boxes first
                cmbTicketType.SelectedItem = _currentTicket.TicketType;

                // Set other fields
                numPrice.Value = _currentTicket.Price;
                cmbStatus.SelectedItem = _currentTicket.Status;
                cmbPaymentMethod.SelectedItem = _currentTicket.PaymentMethod;
                txtQRCode.Text = _currentTicket.QRCode;
                txtNotes.Text = _currentTicket.Notes ?? string.Empty;

                // Set optional fields
                if (_currentTicket.ValidFrom.HasValue)
                    dtpValidFrom.Value = _currentTicket.ValidFrom.Value;
                if (_currentTicket.ValidUntil.HasValue)
                    dtpValidUntil.Value = _currentTicket.ValidUntil.Value;

                chkIsRefundable.Checked = _currentTicket.IsRefundable;

                if (_currentTicket.RefundAmount.HasValue)
                    numRefundAmount.Value = _currentTicket.RefundAmount.Value;

                if (_currentTicket.UsedDate.HasValue)
                {
                    dtpUsedDate.Value = _currentTicket.UsedDate.Value;
                    dtpUsedDate.Enabled = true;
                }

                txtUsedByDriver.Text = _currentTicket.UsedByDriver ?? string.Empty;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error populating form with ticket data");
            }
        }

        private void UpdatePriceBasedOnTicketType()
        {
            if (cmbTicketType.SelectedItem == null) return;

            try
            {
                var ticketType = cmbTicketType.SelectedItem.ToString();
                decimal price = ticketType switch
                {
                    "Single" => 2.50m,
                    "Round Trip" => 4.50m,
                    "Daily Pass" => 8.00m,
                    "Weekly Pass" => 35.00m,
                    "Monthly Pass" => 120.00m,
                    _ => 2.50m
                };

                numPrice.Value = price;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating price based on ticket type");
            }
        }

        private async Task<bool> ValidateFormAsync()
        {
            var errors = new List<string>();

            try
            {
                // Validate student selection
                if (cmbStudent.SelectedIndex <= 0)
                    errors.Add("Please select a student");

                // Validate route selection
                if (cmbRoute.SelectedIndex <= 0)
                    errors.Add("Please select a route");

                // Validate travel date
                if (dtpTravelDate.Value.Date < DateTime.Today && !_isEditMode)
                    errors.Add("Travel date cannot be in the past");

                // Validate price
                if (numPrice.Value <= 0)
                    errors.Add("Price must be greater than 0");

                // Validate ticket type
                if (cmbTicketType.SelectedItem == null)
                    errors.Add("Please select a ticket type");

                // Validate payment method
                if (cmbPaymentMethod.SelectedItem == null)
                    errors.Add("Please select a payment method");

                // Additional validation using service
                if (errors.Count == 0)
                {
                    var ticket = CreateTicketFromForm();
                    using var scope = _serviceProvider.CreateScope();
                    var ticketService = scope.ServiceProvider.GetRequiredService<ITicketService>();
                    var serviceErrors = await ticketService.ValidateTicketAsync(ticket);
                    errors.AddRange(serviceErrors);
                }

                if (errors.Count > 0)
                {
                    var message = "Please correct the following errors:\n\n" + string.Join("\n", errors);
                    MessageBoxAdv.Show(message, "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error validating ticket form");
                MessageBoxAdv.Show($"Error during validation: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        private Ticket CreateTicketFromForm()
        {
            var ticket = _currentTicket ?? new Ticket();

            // Get selected student
            if (cmbStudent.SelectedIndex > 0)
            {
                ticket.StudentId = _students[cmbStudent.SelectedIndex - 1].StudentId;
            }

            // Get selected route
            if (cmbRoute.SelectedIndex > 0)
            {
                ticket.RouteId = _routes[cmbRoute.SelectedIndex - 1].RouteId;
            }

            ticket.TravelDate = dtpTravelDate.Value.Date;
            ticket.IssuedDate = dtpIssuedDate.Value;
            ticket.TicketType = cmbTicketType.SelectedItem?.ToString() ?? "Single";
            ticket.Price = numPrice.Value;
            ticket.Status = cmbStatus.SelectedItem?.ToString() ?? "Valid";
            ticket.PaymentMethod = cmbPaymentMethod.SelectedItem?.ToString() ?? "Cash";
            ticket.Notes = txtNotes.Text.Trim();
            ticket.IsRefundable = chkIsRefundable.Checked;

            if (numRefundAmount.Value > 0)
                ticket.RefundAmount = numRefundAmount.Value;

            if (dtpUsedDate.Enabled && dtpUsedDate.Value != DateTime.MinValue)
                ticket.UsedDate = dtpUsedDate.Value;

            ticket.UsedByDriver = txtUsedByDriver.Text.Trim();

            // Set validity period if not in edit mode
            if (!_isEditMode)
            {
                ticket.SetValidityPeriod();
            }
            else
            {
                ticket.ValidFrom = dtpValidFrom.Value;
                ticket.ValidUntil = dtpValidUntil.Value;
            }

            return ticket;
        }

        private async void BtnSave_Click(object sender, EventArgs e)
        {
            if (!await ValidateFormAsync()) return;

            try
            {
                using var scope = _serviceProvider.CreateScope();
                var ticketService = scope.ServiceProvider.GetRequiredService<ITicketService>();

                var ticket = CreateTicketFromForm();

                if (_isEditMode)
                {
                    var success = await ticketService.UpdateTicketAsync(ticket);
                    if (success)
                    {
                        _currentTicket = ticket;
                        _logger.LogInformation($"Updated ticket {ticket.TicketId}");
                        MessageBoxAdv.Show("Ticket updated successfully!", "Success",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                        this.DialogResult = DialogResult.OK;
                        this.Close();
                    }
                    else
                    {
                        MessageBoxAdv.Show("Failed to update ticket.", "Error",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else
                {
                    _currentTicket = await ticketService.CreateTicketAsync(ticket);
                    _logger.LogInformation($"Created new ticket {_currentTicket.TicketId}");
                    MessageBoxAdv.Show("Ticket created successfully!", "Success",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving ticket");
                MessageBoxAdv.Show($"Error saving ticket: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void CmbTicketType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!_isLoading)
            {
                UpdatePriceBasedOnTicketType();
            }
        }

        private void CmbStatus_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbStatus.SelectedItem?.ToString() == "Used")
            {
                dtpUsedDate.Value = DateTime.Now;
                dtpUsedDate.Enabled = true;
            }
            else
            {
                dtpUsedDate.Enabled = false;
            }
        }

        private void ChkIsRefundable_CheckedChanged(object sender, EventArgs e)
        {
            numRefundAmount.Enabled = chkIsRefundable.Checked;
            if (!chkIsRefundable.Checked)
            {
                numRefundAmount.Value = 0;
            }
        }

        private void BtnGenerateQR_Click(object sender, EventArgs e)
        {
            try
            {
                var random = new Random();
                var qrCode = $"BT{random.Next(1000, 9999)}{DateTime.Now:yyMMddHHmm}";
                txtQRCode.Text = qrCode;
                txtQRCode.ReadOnly = false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating QR code");
                MessageBoxAdv.Show($"Error generating QR code: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
