using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Syncfusion.Windows.Forms;
using Syncfusion.Windows.Forms.Tools;
using Syncfusion.WinForms.Controls;
using Bus_Buddy.Data.UnitOfWork;
using Bus_Buddy.Models;
using Bus_Buddy.Services;
using Bus_Buddy.Utilities;

namespace Bus_Buddy.Forms
{
    /// <summary>
    /// Fuel record edit form using Syncfusion v30.1.37 components
    /// Provides CRUD operations for fuel consumption data
    /// </summary>
    public partial class FuelEditForm : MetroForm
    {
        private readonly ILogger<FuelEditForm> _logger;
        private readonly IFuelService _fuelService;
        private readonly IBusService _busService;

        // Syncfusion Controls
        private AutoLabel titleLabel = null!;
        private AutoLabel busLabel = null!, driverLabel = null!, dateLabel = null!;
        private AutoLabel volumeLabel = null!, costLabel = null!, vendorLabel = null!;
        private AutoLabel notesLabel = null!;
        private ComboBoxAdv busComboBox = null!, driverComboBox = null!;
        private ComboBoxAdv vendorComboBox = null!;
        private DateTimePickerAdv dateEdit = null!;
        private TextBoxExt volumeTextBox = null!, costTextBox = null!;
        private TextBoxExt notesTextBox = null!;
        private SfButton saveButton = null!, cancelButton = null!;
        private GradientPanel mainPanel = null!, buttonPanel = null!;
        private TableLayoutPanel formLayoutPanel = null!;

        // Data
        private Fuel? _currentFuel;
        private bool _isEditMode;

        public Fuel? EditedFuel { get; private set; }
        public bool IsDataSaved { get; private set; }

        public FuelEditForm(ILogger<FuelEditForm> logger, IFuelService fuelService, IBusService busService, Fuel? fuel = null)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _fuelService = fuelService ?? throw new ArgumentNullException(nameof(fuelService));
            _busService = busService ?? throw new ArgumentNullException(nameof(busService));
            _currentFuel = fuel;
            _isEditMode = fuel != null;

            InitializeComponent();
            SetupForm();
            SetupControls();
            LoadComboBoxData();

            _logger.LogInformation("FuelEditForm initialized successfully");
        }

        private void InitializeComponent()
        {
            SuspendLayout();

            // Form properties
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(500, 600);
            MinimumSize = new Size(450, 550);
            MaximumSize = new Size(600, 700);
            Name = "FuelEditForm";
            Text = "Fuel Record";
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;

            ResumeLayout(false);
        }

        private void SetupForm()
        {
            // Apply Syncfusion visual enhancements
            VisualEnhancementManager.ApplyEnhancedTheme(this);
            VisualEnhancementManager.EnableHighQualityFontRendering(this);

            // Configure MetroForm properties
            MetroColor = Color.FromArgb(67, 126, 231);
            CaptionBarColor = Color.FromArgb(31, 31, 31);
            CaptionForeColor = Color.White;
            BorderColor = Color.FromArgb(67, 126, 231);

            _logger.LogDebug("Form visual enhancements applied");
        }

        private void SetupControls()
        {
            try
            {
                // Create main panel
                mainPanel = new GradientPanel()
                {
                    Dock = DockStyle.Fill,
                    BackgroundColor = new Syncfusion.Drawing.BrushInfo(Color.FromArgb(250, 250, 250))
                };

                // Create title label
                titleLabel = new AutoLabel()
                {
                    Text = _isEditMode ? "Edit Fuel Record" : "Add New Fuel Record",
                    Font = new Font("Segoe UI", 14F, FontStyle.Bold),
                    ForeColor = Color.FromArgb(31, 31, 31),
                    Location = new Point(20, 20),
                    Size = new Size(300, 25)
                };

                // Create form layout panel
                formLayoutPanel = new TableLayoutPanel()
                {
                    Location = new Point(20, 60),
                    Size = new Size(440, 450),
                    ColumnCount = 2,
                    RowCount = 7,
                    AutoSize = false
                };

                // Configure column widths
                formLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 120));
                formLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));

                // Configure row heights
                for (int i = 0; i < 7; i++)
                {
                    formLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 60));
                }

                CreateFormControls();
                PopulateFormLayout();
                CreateButtonPanel();

                // Add controls to main panel
                mainPanel.Controls.AddRange(new Control[] {
                    titleLabel, formLayoutPanel, buttonPanel
                });

                Controls.Add(mainPanel);

                _logger.LogDebug("Controls setup completed");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error setting up controls");
                throw;
            }
        }

        private void CreateFormControls()
        {
            // Bus selection
            busLabel = new AutoLabel()
            {
                Text = "Bus:",
                Font = new Font("Segoe UI", 10F),
                ForeColor = Color.FromArgb(31, 31, 31),
                TextAlign = ContentAlignment.MiddleRight,
                Dock = DockStyle.Fill
            };

            busComboBox = new ComboBoxAdv()
            {
                DisplayMember = "BusNumber",
                ValueMember = "Id",
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Segoe UI", 10F),
                Dock = DockStyle.Fill,
                Margin = new Padding(5)
            };

            // Driver selection
            driverLabel = new AutoLabel()
            {
                Text = "Driver:",
                Font = new Font("Segoe UI", 10F),
                ForeColor = Color.FromArgb(31, 31, 31),
                TextAlign = ContentAlignment.MiddleRight,
                Dock = DockStyle.Fill
            };

            driverComboBox = new ComboBoxAdv()
            {
                DisplayMember = "FullName",
                ValueMember = "Id",
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Segoe UI", 10F),
                Dock = DockStyle.Fill,
                Margin = new Padding(5)
            };

            // Date selection
            dateLabel = new AutoLabel()
            {
                Text = "Date:",
                Font = new Font("Segoe UI", 10F),
                ForeColor = Color.FromArgb(31, 31, 31),
                TextAlign = ContentAlignment.MiddleRight,
                Dock = DockStyle.Fill
            };

            dateEdit = new DateTimePickerAdv()
            {
                Font = new Font("Segoe UI", 10F),
                Value = DateTime.Now,
                Format = DateTimePickerFormat.Short,
                Dock = DockStyle.Fill,
                Margin = new Padding(5)
            };

            // Volume input
            volumeLabel = new AutoLabel()
            {
                Text = "Volume (L):",
                Font = new Font("Segoe UI", 10F),
                ForeColor = Color.FromArgb(31, 31, 31),
                TextAlign = ContentAlignment.MiddleRight,
                Dock = DockStyle.Fill
            };

            volumeTextBox = new TextBoxExt()
            {
                Font = new Font("Segoe UI", 10F),
                Dock = DockStyle.Fill,
                Margin = new Padding(5)
            };

            // Cost input
            costLabel = new AutoLabel()
            {
                Text = "Cost:",
                Font = new Font("Segoe UI", 10F),
                ForeColor = Color.FromArgb(31, 31, 31),
                TextAlign = ContentAlignment.MiddleRight,
                Dock = DockStyle.Fill
            };

            costTextBox = new TextBoxExt()
            {
                Font = new Font("Segoe UI", 10F),
                Dock = DockStyle.Fill,
                Margin = new Padding(5)
            };

            // Vendor input
            vendorLabel = new AutoLabel()
            {
                Text = "Vendor:",
                Font = new Font("Segoe UI", 10F),
                ForeColor = Color.FromArgb(31, 31, 31),
                TextAlign = ContentAlignment.MiddleRight,
                Dock = DockStyle.Fill
            };

            vendorComboBox = new ComboBoxAdv()
            {
                Font = new Font("Segoe UI", 10F),
                Dock = DockStyle.Fill,
                Margin = new Padding(5)
            };

            // Notes input
            notesLabel = new AutoLabel()
            {
                Text = "Notes:",
                Font = new Font("Segoe UI", 10F),
                ForeColor = Color.FromArgb(31, 31, 31),
                TextAlign = ContentAlignment.MiddleRight,
                Dock = DockStyle.Top
            };

            notesTextBox = new TextBoxExt()
            {
                Font = new Font("Segoe UI", 10F),
                Multiline = true,
                ScrollBars = ScrollBars.Vertical,
                Dock = DockStyle.Fill,
                Margin = new Padding(5)
            };
        }

        private void PopulateFormLayout()
        {
            // Add controls to layout panel
            formLayoutPanel.Controls.Add(busLabel, 0, 0);
            formLayoutPanel.Controls.Add(busComboBox, 1, 0);

            formLayoutPanel.Controls.Add(driverLabel, 0, 1);
            formLayoutPanel.Controls.Add(driverComboBox, 1, 1);

            formLayoutPanel.Controls.Add(dateLabel, 0, 2);
            formLayoutPanel.Controls.Add(dateEdit, 1, 2);

            formLayoutPanel.Controls.Add(volumeLabel, 0, 3);
            formLayoutPanel.Controls.Add(volumeTextBox, 1, 3);

            formLayoutPanel.Controls.Add(costLabel, 0, 4);
            formLayoutPanel.Controls.Add(costTextBox, 1, 4);

            formLayoutPanel.Controls.Add(vendorLabel, 0, 5);
            formLayoutPanel.Controls.Add(vendorComboBox, 1, 5);

            formLayoutPanel.Controls.Add(notesLabel, 0, 6);
            formLayoutPanel.Controls.Add(notesTextBox, 1, 6);
        }

        private void CreateButtonPanel()
        {
            buttonPanel = new GradientPanel()
            {
                Location = new Point(20, 530),
                Size = new Size(440, 50),
                BackgroundColor = new Syncfusion.Drawing.BrushInfo(Color.FromArgb(245, 245, 245))
            };

            // Save button
            saveButton = new SfButton()
            {
                Text = _isEditMode ? "Update" : "Save",
                Size = new Size(100, 35),
                Location = new Point(240, 8),
                BackColor = Color.FromArgb(46, 125, 50),
                ForeColor = Color.White
            };
            saveButton.Click += SaveButton_Click;
            VisualEnhancementManager.ApplyEnhancedButtonStyling(saveButton, Color.FromArgb(46, 125, 50));

            // Cancel button
            cancelButton = new SfButton()
            {
                Text = "Cancel",
                Size = new Size(80, 35),
                Location = new Point(350, 8)
            };
            cancelButton.Click += CancelButton_Click;
            VisualEnhancementManager.ApplyEnhancedButtonStyling(cancelButton, Color.FromArgb(108, 117, 125));

            buttonPanel.Controls.AddRange(new Control[] { saveButton, cancelButton });
        }

        private async void LoadComboBoxData()
        {
            try
            {
                // Load buses
                var buses = await _busService.GetAllBusesAsync();
                busComboBox.DataSource = buses.ToList();
                busComboBox.DisplayMember = "BusNumber";
                busComboBox.ValueMember = "BusId";

                // Load common vendors
                var vendors = new[]
                {
                    "Shell", "BP", "Caltex", "Total", "Mobil", "Petron", "Other"
                };
                vendorComboBox.DataSource = vendors;

                _logger.LogDebug("Combo box data loaded successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading combo box data");
                MessageBoxAdv.Show(this,
                    "Error loading form data. Please check the logs for details.",
                    "Data Loading Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void LoadFuelData()
        {
            if (_currentFuel == null) return;

            try
            {
                busComboBox.SelectedValue = _currentFuel.VehicleFueledId;
                dateEdit.Value = _currentFuel.FuelDate;
                volumeTextBox.Text = _currentFuel.Gallons?.ToString("F2") ?? "";
                costTextBox.Text = _currentFuel.TotalCost?.ToString("F2") ?? "";
                vendorComboBox.Text = _currentFuel.FuelLocation ?? "";
                notesTextBox.Text = _currentFuel.Notes ?? "";
                // odometerTextBox.Text = _currentFuel.VehicleOdometerReading.ToString();
                // fuelTypeComboBox.Text = _currentFuel.FuelType ?? "";

                _logger.LogDebug("Fuel data loaded for editing: ID {FuelId}", _currentFuel.FuelId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading fuel data for editing");
            }
        }

        private async void SaveButton_Click(object? sender, EventArgs e)
        {
            try
            {
                if (!ValidateInput())
                    return;

                if (_currentFuel == null)
                {
                    _currentFuel = new Fuel();
                }

                // Update fuel properties
                _currentFuel.VehicleFueledId = (int)busComboBox.SelectedValue;
                _currentFuel.FuelDate = dateEdit.Value;
                _currentFuel.Gallons = decimal.Parse(volumeTextBox.Text);
                _currentFuel.TotalCost = decimal.Parse(costTextBox.Text);
                _currentFuel.FuelLocation = vendorComboBox.Text;
                _currentFuel.Notes = notesTextBox.Text;
                // Set required fields with defaults for now
                _currentFuel.VehicleOdometerReading = 0; // TODO: Add odometer field
                _currentFuel.FuelType = "Gasoline"; // TODO: Add fuel type field

                if (_isEditMode && _currentFuel != null)
                {
                    EditedFuel = await _fuelService.UpdateFuelRecordAsync(_currentFuel);
                    _logger.LogInformation("Fuel record updated: ID {FuelId}", _currentFuel.FuelId);
                }
                else if (_currentFuel != null)
                {
                    EditedFuel = await _fuelService.CreateFuelRecordAsync(_currentFuel);
                    _logger.LogInformation("New fuel record created");
                }

                IsDataSaved = true;

                MessageBoxAdv.Show(this,
                    _isEditMode ? "Fuel record updated successfully!" : "Fuel record created successfully!",
                    "Success",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                DialogResult = DialogResult.OK;
                Close();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving fuel record");
                MessageBoxAdv.Show(this,
                    "Error saving fuel record. Please check the logs for details.",
                    "Save Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void CancelButton_Click(object? sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private bool ValidateInput()
        {
            // Validate bus selection
            if (busComboBox.SelectedValue == null)
            {
                MessageBoxAdv.Show(this, "Please select a bus.", "Validation Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                busComboBox.Focus();
                return false;
            }

            // Validate driver selection
            if (driverComboBox.SelectedValue == null)
            {
                MessageBoxAdv.Show(this, "Please select a driver.", "Validation Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                driverComboBox.Focus();
                return false;
            }

            // Validate volume
            if (!decimal.TryParse(volumeTextBox.Text, out decimal volume) || volume <= 0)
            {
                MessageBoxAdv.Show(this, "Please enter a valid volume greater than 0.", "Validation Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                volumeTextBox.Focus();
                return false;
            }

            // Validate cost
            if (!decimal.TryParse(costTextBox.Text, out decimal cost) || cost <= 0)
            {
                MessageBoxAdv.Show(this, "Please enter a valid cost greater than 0.", "Validation Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                costTextBox.Focus();
                return false;
            }

            // Validate vendor
            if (string.IsNullOrWhiteSpace(vendorComboBox.Text))
            {
                MessageBoxAdv.Show(this, "Please select or enter a vendor.", "Validation Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                vendorComboBox.Focus();
                return false;
            }

            return true;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                // Dispose Syncfusion components
                titleLabel?.Dispose();
                busLabel?.Dispose();
                driverLabel?.Dispose();
                dateLabel?.Dispose();
                volumeLabel?.Dispose();
                costLabel?.Dispose();
                vendorLabel?.Dispose();
                notesLabel?.Dispose();
                busComboBox?.Dispose();
                driverComboBox?.Dispose();
                vendorComboBox?.Dispose();
                dateEdit?.Dispose();
                volumeTextBox?.Dispose();
                costTextBox?.Dispose();
                notesTextBox?.Dispose();
                saveButton?.Dispose();
                cancelButton?.Dispose();
                mainPanel?.Dispose();
                buttonPanel?.Dispose();
                formLayoutPanel?.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
