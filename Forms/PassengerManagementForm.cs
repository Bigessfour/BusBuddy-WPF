using System;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Extensions.Logging;
using Syncfusion.Windows.Forms;
using Syncfusion.Windows.Forms.Tools;
using Syncfusion.WinForms.Controls;
using Syncfusion.WinForms.DataGrid;
using Bus_Buddy.Services;
using Bus_Buddy.Utilities;

namespace Bus_Buddy.Forms
{
    /// <summary>
    /// Passenger Management Form using Syncfusion Windows Forms v30.1.37
    /// Manages passenger information, boarding history, and preferences
    /// </summary>
    public partial class PassengerManagementForm : MetroForm
    {
        private readonly ILogger<PassengerManagementForm>? _logger;
        private readonly IBusService _busService;

        // Syncfusion Controls
        private TableLayoutPanel mainTableLayout = null!;
        private GradientPanel headerPanel = null!;
        private GradientPanel searchPanel = null!;
        private GradientPanel gridPanel = null!;
        private GradientPanel buttonPanel = null!;

        private AutoLabel titleLabel = null!;
        private AutoLabel searchLabel = null!;
        private TextBox searchTextBox = null!;
        private SfButton searchButton = null!;
        private SfButton clearButton = null!;

        private SfDataGrid passengersGrid = null!;

        private SfButton addButton = null!;
        private SfButton editButton = null!;
        private SfButton deleteButton = null!;
        private SfButton refreshButton = null!;
        private SfButton closeButton = null!;

        private BindingList<PassengerInfo> _passengers = null!;
        private PassengerInfo? _selectedPassenger;

        public PassengerManagementForm(ILogger<PassengerManagementForm>? logger, IBusService busService)
        {
            _logger = logger;
            _busService = busService;

            _logger?.LogInformation("Initializing Passenger Management form");

            InitializeComponent();
            InitializeForm();
            LoadPassengersAsync();
        }

        private void InitializeComponent()
        {
            // Apply enhanced visual theme
            VisualEnhancementManager.ApplyEnhancedTheme(this);

            this.Text = "Passenger Management - BusBuddy";
            this.Size = new Size(1200, 700);
            this.MinimumSize = new Size(800, 500);
            this.StartPosition = FormStartPosition.CenterParent;

            // Initialize main responsive layout
            mainTableLayout = SyncfusionLayoutManager.CreateResponsiveTableLayout(1, 4);
            mainTableLayout.RowStyles.Clear();
            mainTableLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 70));  // Header
            mainTableLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 60));  // Search
            mainTableLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F)); // Grid
            mainTableLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 60));  // Buttons

            InitializePanels();
            InitializeControls();
            InitializeGrid();

            // Add panels to layout
            mainTableLayout.Controls.Add(headerPanel, 0, 0);
            mainTableLayout.Controls.Add(searchPanel, 0, 1);
            mainTableLayout.Controls.Add(gridPanel, 0, 2);
            mainTableLayout.Controls.Add(buttonPanel, 0, 3);

            this.Controls.Add(mainTableLayout);
        }

        private void InitializePanels()
        {
            // Header Panel
            headerPanel = new GradientPanel();
            SyncfusionLayoutManager.ConfigureGradientPanel(headerPanel, Color.FromArgb(46, 125, 185));
            headerPanel.Dock = DockStyle.Fill;

            // Search Panel
            searchPanel = new GradientPanel();
            SyncfusionLayoutManager.ConfigureGradientPanel(searchPanel, Color.FromArgb(248, 249, 250));
            searchPanel.Dock = DockStyle.Fill;
            searchPanel.Padding = new Padding(10, 10, 10, 5);

            // Grid Panel
            gridPanel = new GradientPanel();
            SyncfusionLayoutManager.ConfigureGradientPanel(gridPanel, Color.White);
            gridPanel.Dock = DockStyle.Fill;
            gridPanel.Padding = new Padding(10);

            // Button Panel
            buttonPanel = new GradientPanel();
            SyncfusionLayoutManager.ConfigureGradientPanel(buttonPanel, Color.FromArgb(248, 249, 250));
            buttonPanel.Dock = DockStyle.Fill;
            buttonPanel.Padding = new Padding(10);
        }

        private void InitializeControls()
        {
            // Title Label
            titleLabel = new AutoLabel
            {
                Text = "🚌 Passenger Management",
                Font = new Font("Segoe UI", 16F, FontStyle.Bold),
                ForeColor = Color.White,
                AutoSize = true,
                Location = new Point(20, 20)
            };

            // Search Controls
            searchLabel = new AutoLabel
            {
                Text = "Search Passengers:",
                Font = new Font("Segoe UI", 10F),
                ForeColor = Color.FromArgb(33, 37, 41),
                AutoSize = true,
                Location = new Point(0, 8)
            };

            searchTextBox = new TextBox
            {
                Location = new Point(140, 5),
                Size = new Size(300, 30),
                Font = new Font("Segoe UI", 9F),
                PlaceholderText = "Enter passenger name, ID, or contact info..."
            };

            searchButton = new SfButton();
            VisualEnhancementManager.ApplyEnhancedButtonStyling(searchButton, Color.FromArgb(40, 167, 69));
            searchButton.Text = "Search";
            searchButton.Size = new Size(80, 30);
            searchButton.Location = new Point(450, 5);
            searchButton.Click += SearchButton_Click;

            clearButton = new SfButton();
            VisualEnhancementManager.ApplyEnhancedButtonStyling(clearButton, Color.FromArgb(108, 117, 125));
            clearButton.Text = "Clear";
            clearButton.Size = new Size(70, 30);
            clearButton.Location = new Point(540, 5);
            clearButton.Click += ClearButton_Click;

            // Action Buttons
            addButton = new SfButton();
            VisualEnhancementManager.ApplyEnhancedButtonStyling(addButton, Color.FromArgb(40, 167, 69));
            addButton.Text = "Add Passenger";
            addButton.Size = new Size(120, 35);
            addButton.Location = new Point(20, 10);
            addButton.Click += AddButton_Click;

            editButton = new SfButton();
            VisualEnhancementManager.ApplyEnhancedButtonStyling(editButton, Color.FromArgb(255, 193, 7));
            editButton.Text = "Edit";
            editButton.Size = new Size(80, 35);
            editButton.Location = new Point(150, 10);
            editButton.Click += EditButton_Click;

            deleteButton = new SfButton();
            VisualEnhancementManager.ApplyEnhancedButtonStyling(deleteButton, Color.FromArgb(220, 53, 69));
            deleteButton.Text = "Delete";
            deleteButton.Size = new Size(80, 35);
            deleteButton.Location = new Point(240, 10);
            deleteButton.Click += DeleteButton_Click;

            refreshButton = new SfButton();
            VisualEnhancementManager.ApplyEnhancedButtonStyling(refreshButton, Color.FromArgb(23, 162, 184));
            refreshButton.Text = "Refresh";
            refreshButton.Size = new Size(80, 35);
            refreshButton.Location = new Point(330, 10);
            refreshButton.Click += RefreshButton_Click;

            closeButton = new SfButton();
            VisualEnhancementManager.ApplyEnhancedButtonStyling(closeButton, Color.FromArgb(108, 117, 125));
            closeButton.Text = "Close";
            closeButton.Size = new Size(80, 35);
            closeButton.Location = new Point(450, 10);
            closeButton.Click += CloseButton_Click;

            // Add controls to panels
            headerPanel.Controls.Add(titleLabel);
            searchPanel.Controls.AddRange(new Control[] { searchLabel, searchTextBox, searchButton, clearButton });
            buttonPanel.Controls.AddRange(new Control[] { addButton, editButton, deleteButton, refreshButton, closeButton });
        }

        private void InitializeGrid()
        {
            // Initialize SfDataGrid with enhanced visuals
            passengersGrid = new SfDataGrid();
            VisualEnhancementManager.ApplyEnhancedGridVisuals(passengersGrid);
            passengersGrid.Dock = DockStyle.Fill;
            passengersGrid.AllowEditing = false;
            passengersGrid.AllowDeleting = false;
            passengersGrid.SelectionMode = Syncfusion.WinForms.DataGrid.Enums.GridSelectionMode.Single;
            passengersGrid.SelectionChanged += PassengersGrid_SelectionChanged;

            // Configure grid columns when data source changes
            passengersGrid.DataSourceChanged += (sender, e) => ConfigureGridColumns();

            gridPanel.Controls.Add(passengersGrid);
        }

        private void InitializeForm()
        {
            // Set MetroForm properties
            this.MetroColor = Color.FromArgb(46, 125, 185);
            this.CaptionBarColor = Color.FromArgb(46, 125, 185);
            this.CaptionForeColor = Color.White;
            this.ShowIcon = false;

            // Enable high-quality font rendering
            VisualEnhancementManager.EnableHighQualityFontRendering(this);

            _logger?.LogInformation("Passenger Management form initialized");
        }

        private void LoadPassengersAsync()
        {
            try
            {
                _logger?.LogInformation("Loading passengers data");

                // For now, create sample data since passenger service doesn't exist yet
                _passengers = new BindingList<PassengerInfo>(CreateSamplePassengers());

                passengersGrid.DataSource = _passengers;

                _logger?.LogInformation("Loaded {Count} passengers", _passengers.Count);
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Error loading passengers");
                MessageBoxAdv.Show($"Error loading passengers: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private System.Collections.Generic.List<PassengerInfo> CreateSamplePassengers()
        {
            var passengers = new System.Collections.Generic.List<PassengerInfo>();
            var random = new Random();
            var firstNames = new[] { "John", "Jane", "Michael", "Sarah", "David", "Emily", "Robert", "Jessica", "William", "Ashley" };
            var lastNames = new[] { "Smith", "Johnson", "Williams", "Brown", "Jones", "Garcia", "Miller", "Davis", "Rodriguez", "Martinez" };

            for (int i = 1; i <= 50; i++)
            {
                var firstName = firstNames[random.Next(firstNames.Length)];
                var lastName = lastNames[random.Next(lastNames.Length)];

                passengers.Add(new PassengerInfo
                {
                    PassengerId = i,
                    FirstName = firstName,
                    LastName = lastName,
                    Email = $"{firstName.ToLower()}.{lastName.ToLower()}@email.com",
                    Phone = $"({random.Next(100, 999)}) {random.Next(100, 999)}-{random.Next(1000, 9999)}",
                    Address = $"{random.Next(100, 9999)} Main Street",
                    City = "Sample City",
                    State = "ST",
                    ZipCode = $"{random.Next(10000, 99999)}",
                    IsActive = random.Next(0, 10) > 1, // 90% active
                    RegistrationDate = DateTime.Now.AddDays(-random.Next(1, 365)),
                    LastRide = DateTime.Now.AddDays(-random.Next(1, 30)),
                    TotalRides = random.Next(1, 100)
                });
            }

            return passengers;
        }

        private void ConfigureGridColumns()
        {
            if (passengersGrid.Columns.Count == 0) return;

            // Configure column alignments and formatting
            SyncfusionLayoutManager.ConfigureColumnAlignment(passengersGrid, "PassengerId", HorizontalAlignment.Center, null, 70);
            SyncfusionLayoutManager.ConfigureColumnAlignment(passengersGrid, "FirstName", HorizontalAlignment.Left, null, 100);
            SyncfusionLayoutManager.ConfigureColumnAlignment(passengersGrid, "LastName", HorizontalAlignment.Left, null, 100);
            SyncfusionLayoutManager.ConfigureColumnAlignment(passengersGrid, "Email", HorizontalAlignment.Left, null, 200);
            SyncfusionLayoutManager.ConfigureColumnAlignment(passengersGrid, "Phone", HorizontalAlignment.Center, null, 120);
            SyncfusionLayoutManager.ConfigureColumnAlignment(passengersGrid, "City", HorizontalAlignment.Left, null, 100);
            SyncfusionLayoutManager.ConfigureColumnAlignment(passengersGrid, "State", HorizontalAlignment.Center, null, 60);
            SyncfusionLayoutManager.ConfigureColumnAlignment(passengersGrid, "IsActive", HorizontalAlignment.Center, null, 70);
            SyncfusionLayoutManager.ConfigureColumnAlignment(passengersGrid, "RegistrationDate", HorizontalAlignment.Center, "MM/dd/yyyy", 110);
            SyncfusionLayoutManager.ConfigureColumnAlignment(passengersGrid, "LastRide", HorizontalAlignment.Center, "MM/dd/yyyy", 100);
            SyncfusionLayoutManager.ConfigureColumnAlignment(passengersGrid, "TotalRides", HorizontalAlignment.Right, null, 80);

            // Set header texts
            if (passengersGrid.Columns["PassengerId"] != null)
                passengersGrid.Columns["PassengerId"].HeaderText = "ID";
            if (passengersGrid.Columns["FirstName"] != null)
                passengersGrid.Columns["FirstName"].HeaderText = "First Name";
            if (passengersGrid.Columns["LastName"] != null)
                passengersGrid.Columns["LastName"].HeaderText = "Last Name";
            if (passengersGrid.Columns["IsActive"] != null)
                passengersGrid.Columns["IsActive"].HeaderText = "Active";
            if (passengersGrid.Columns["RegistrationDate"] != null)
                passengersGrid.Columns["RegistrationDate"].HeaderText = "Registered";
            if (passengersGrid.Columns["LastRide"] != null)
                passengersGrid.Columns["LastRide"].HeaderText = "Last Ride";
            if (passengersGrid.Columns["TotalRides"] != null)
                passengersGrid.Columns["TotalRides"].HeaderText = "Total Rides";

            // Hide address details in main view
            if (passengersGrid.Columns["Address"] != null)
                passengersGrid.Columns["Address"].Visible = false;
            if (passengersGrid.Columns["ZipCode"] != null)
                passengersGrid.Columns["ZipCode"].Visible = false;
        }

        #region Event Handlers

        private void PassengersGrid_SelectionChanged(object? sender, EventArgs e)
        {
            _selectedPassenger = passengersGrid.SelectedItem as PassengerInfo;

            // Enable/disable buttons based on selection
            editButton.Enabled = _selectedPassenger != null;
            deleteButton.Enabled = _selectedPassenger != null;
        }

        private void SearchButton_Click(object? sender, EventArgs e)
        {
            try
            {
                var searchTerm = searchTextBox.Text?.Trim().ToLower();
                if (string.IsNullOrEmpty(searchTerm))
                {
                    passengersGrid.DataSource = _passengers;
                    return;
                }

                var filteredPassengers = _passengers.Where(p =>
                    p.FirstName.ToLower().Contains(searchTerm) ||
                    p.LastName.ToLower().Contains(searchTerm) ||
                    p.Email.ToLower().Contains(searchTerm) ||
                    p.Phone.Contains(searchTerm) ||
                    p.PassengerId.ToString().Contains(searchTerm)
                ).ToList();

                passengersGrid.DataSource = new BindingList<PassengerInfo>(filteredPassengers);

                _logger?.LogInformation("Search completed. Found {Count} passengers", filteredPassengers.Count);
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Error during search");
                MessageBoxAdv.Show($"Search error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ClearButton_Click(object? sender, EventArgs e)
        {
            searchTextBox.Text = string.Empty;
            passengersGrid.DataSource = _passengers;
            _logger?.LogInformation("Search cleared");
        }

        private void AddButton_Click(object? sender, EventArgs e)
        {
            try
            {
                _logger?.LogInformation("Add passenger button clicked");

                // Get services from ServiceContainer
                var logger = ServiceContainer.GetService<ILogger<PassengerEditForm>>();
                var studentService = ServiceContainer.GetService<IStudentService>();

                if (logger == null || studentService == null)
                {
                    MessageBoxAdv.Show("Required services not available", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                using var addForm = new PassengerEditForm(logger, studentService, _busService);

                if (addForm.ShowDialog() == DialogResult.OK && addForm.IsDataSaved)
                {
                    // Refresh the grid to show the new passenger
                    LoadPassengersAsync();
                    _logger?.LogInformation("New passenger added successfully");
                }
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Error opening add passenger form");
                MessageBoxAdv.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void EditButton_Click(object? sender, EventArgs e)
        {
            try
            {
                if (_selectedPassenger == null)
                {
                    MessageBoxAdv.Show("Please select a passenger to edit.", "No Selection", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                _logger?.LogInformation("Edit passenger button clicked for passenger {PassengerId}", _selectedPassenger.PassengerId);

                // Get services from ServiceContainer
                var logger = ServiceContainer.GetService<ILogger<PassengerEditForm>>();
                var studentService = ServiceContainer.GetService<IStudentService>();

                if (logger == null || studentService == null)
                {
                    MessageBoxAdv.Show("Required services not available", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Get the actual Student entity by ID
                var student = studentService.GetStudentByIdAsync(_selectedPassenger.PassengerId).Result;

                if (student == null)
                {
                    MessageBoxAdv.Show("Student record not found", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                using var editForm = new PassengerEditForm(logger, studentService, _busService, student);

                if (editForm.ShowDialog() == DialogResult.OK && editForm.IsDataSaved)
                {
                    // Refresh the grid to show updated passenger data
                    LoadPassengersAsync();
                    _logger?.LogInformation("Passenger {PassengerId} updated successfully", _selectedPassenger.PassengerId);
                }
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Error opening edit passenger form");
                MessageBoxAdv.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void DeleteButton_Click(object? sender, EventArgs e)
        {
            try
            {
                if (_selectedPassenger == null)
                {
                    MessageBoxAdv.Show("Please select a passenger to delete.", "No Selection", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var result = MessageBoxAdv.Show(
                    $"Are you sure you want to delete passenger '{_selectedPassenger.FirstName} {_selectedPassenger.LastName}'?\n\nThis action cannot be undone.",
                    "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    _passengers.Remove(_selectedPassenger);
                    _logger?.LogInformation("Passenger {PassengerId} deleted", _selectedPassenger.PassengerId);
                    MessageBoxAdv.Show("Passenger deleted successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Error deleting passenger");
                MessageBoxAdv.Show($"Error deleting passenger: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void RefreshButton_Click(object? sender, EventArgs e)
        {
            try
            {
                _logger?.LogInformation("Refresh button clicked");
                searchTextBox.Text = string.Empty;
                await Task.Run(() => LoadPassengersAsync());
                MessageBoxAdv.Show("Data refreshed successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Error refreshing data");
                MessageBoxAdv.Show($"Error refreshing data: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CloseButton_Click(object? sender, EventArgs e)
        {
            this.Close();
        }

        #endregion
    }

    /// <summary>
    /// Passenger information data class
    /// </summary>
    public class PassengerInfo
    {
        public int PassengerId { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string State { get; set; } = string.Empty;
        public string ZipCode { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public DateTime RegistrationDate { get; set; }
        public DateTime LastRide { get; set; }
        public int TotalRides { get; set; }
    }
}
