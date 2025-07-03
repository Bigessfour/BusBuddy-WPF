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
    /// Maintenance record edit form using Syncfusion v30.1.37 components
    /// Provides CRUD operations for maintenance data
    /// </summary>
    public partial class MaintenanceEditForm : MetroForm
    {
        private readonly ILogger<MaintenanceEditForm> _logger;
        private readonly IMaintenanceService _maintenanceService;
        private readonly IBusService _busService;

        // Syncfusion Controls
        private AutoLabel titleLabel = null!;
        private AutoLabel busLabel = null!, dateLabel = null!, odometerLabel = null!;
        private AutoLabel maintenanceLabel = null!, vendorLabel = null!, costLabel = null!;
        private AutoLabel notesLabel = null!;
        private ComboBoxAdv busComboBox = null!, maintenanceTypeComboBox = null!;
        private ComboBoxAdv vendorComboBox = null!;
        private DateTimePickerAdv dateEdit = null!;
        private TextBoxExt odometerTextBox = null!, costTextBox = null!;
        private TextBoxExt notesTextBox = null!;

        private SfButton saveButton = null!, cancelButton = null!;
        private GradientPanel mainPanel = null!;
        private TableLayoutPanel formTableLayout = null!;
        private GradientPanel buttonPanel = null!;

        // Data Management
        private Maintenance? _currentMaintenance;
        private bool _isEditMode = false;

        public Maintenance? EditedMaintenance { get; private set; }
        public bool IsDataSaved { get; private set; }

        public MaintenanceEditForm(ILogger<MaintenanceEditForm> logger, IMaintenanceService maintenanceService, IBusService busService, Maintenance? maintenance = null)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _maintenanceService = maintenanceService ?? throw new ArgumentNullException(nameof(maintenanceService));
            _busService = busService ?? throw new ArgumentNullException(nameof(busService));

            _currentMaintenance = maintenance;
            _isEditMode = maintenance != null;

            _currentMaintenance = maintenance;
            _isEditMode = maintenance != null;

            InitializeComponent();
            InitializeFormControls();
            LoadFormData();

            if (_isEditMode && _currentMaintenance != null)
            {
                LoadMaintenanceData();
            }

            _logger.LogInformation("MaintenanceEditForm initialized in {Mode} mode", _isEditMode ? "Edit" : "Add");
        }

        private void InitializeComponent()
        {
            SuspendLayout();

            // Form configuration
            Text = _isEditMode ? "Edit Maintenance Record" : "Add New Maintenance Record";
            Size = new Size(650, 500);
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            ShowIcon = false;

            // Apply Syncfusion Metro theme
            // MetroColorTable = new MetroColorTable();
            ApplyMetroTheme();

            ResumeLayout(false);
        }

        private void InitializeFormControls()
        {
            // Main panel
            mainPanel = new GradientPanel()
            {
                Dock = DockStyle.Fill,
                BorderStyle = BorderStyle.None
            };
            Controls.Add(mainPanel);

            // Title label
            titleLabel = new AutoLabel()
            {
                Text = _isEditMode ? "Edit Maintenance Record" : "Add New Maintenance Record",
                Font = new Font("Segoe UI", 16F, FontStyle.Bold),
                Location = new Point(20, 20),
                AutoSize = true,
                ForeColor = Color.FromArgb(68, 114, 196)
            };
            mainPanel.Controls.Add(titleLabel);

            // Create table layout for form fields
            formTableLayout = new TableLayoutPanel()
            {
                Location = new Point(20, 60),
                Size = new Size(580, 320),
                ColumnCount = 2,
                RowCount = 7,
                CellBorderStyle = TableLayoutPanelCellBorderStyle.None
            };

            formTableLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 150F));
            formTableLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));

            for (int i = 0; i < 7; i++)
            {
                formTableLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 45F));
            }

            mainPanel.Controls.Add(formTableLayout);

            CreateFormFields();
            CreateButtons();

            // Apply visual enhancements
            VisualEnhancementManager.ApplyEnhancedTheme(this);
        }

        private void CreateFormFields()
        {
            int row = 0;

            // Bus selection
            busLabel = new AutoLabel()
            {
                Text = "Bus:",
                Font = new Font("Segoe UI", 10F),
                Anchor = AnchorStyles.Left,
                AutoSize = true
            };
            formTableLayout.Controls.Add(busLabel, 0, row);

            busComboBox = new ComboBoxAdv()
            {
                Size = new Size(300, 25),
                DropDownStyle = ComboBoxStyle.DropDownList,
                Anchor = AnchorStyles.Left | AnchorStyles.Right
            };
            formTableLayout.Controls.Add(busComboBox, 1, row++);

            // Date
            dateLabel = new AutoLabel()
            {
                Text = "Date:",
                Font = new Font("Segoe UI", 10F),
                Anchor = AnchorStyles.Left,
                AutoSize = true
            };
            formTableLayout.Controls.Add(dateLabel, 0, row);

            dateEdit = new DateTimePickerAdv()
            {
                Size = new Size(200, 25),
                Value = DateTime.Today,
                Anchor = AnchorStyles.Left
            };
            formTableLayout.Controls.Add(dateEdit, 1, row++);

            // Odometer Reading
            odometerLabel = new AutoLabel()
            {
                Text = "Odometer:",
                Font = new Font("Segoe UI", 10F),
                Anchor = AnchorStyles.Left,
                AutoSize = true
            };
            formTableLayout.Controls.Add(odometerLabel, 0, row);

            odometerTextBox = new TextBoxExt()
            {
                Size = new Size(120, 25),
                Anchor = AnchorStyles.Left
            };
            formTableLayout.Controls.Add(odometerTextBox, 1, row++);

            // Maintenance Type
            maintenanceLabel = new AutoLabel()
            {
                Text = "Maintenance:",
                Font = new Font("Segoe UI", 10F),
                Anchor = AnchorStyles.Left,
                AutoSize = true
            };
            formTableLayout.Controls.Add(maintenanceLabel, 0, row);

            maintenanceTypeComboBox = new ComboBoxAdv()
            {
                Size = new Size(300, 25),
                Anchor = AnchorStyles.Left | AnchorStyles.Right
            };
            maintenanceTypeComboBox.Items.AddRange(new[] {
                "Tires", "Windshield", "Alignment", "Mechanical",
                "Car Wash", "Cleaning", "Accessory Install", "Oil Change",
                "Brake Service", "Engine Repair", "Transmission", "Other"
            });
            formTableLayout.Controls.Add(maintenanceTypeComboBox, 1, row++);

            // Vendor
            vendorLabel = new AutoLabel()
            {
                Text = "Vendor:",
                Font = new Font("Segoe UI", 10F),
                Anchor = AnchorStyles.Left,
                AutoSize = true
            };
            formTableLayout.Controls.Add(vendorLabel, 0, row);

            vendorComboBox = new ComboBoxAdv()
            {
                Size = new Size(300, 25),
                Anchor = AnchorStyles.Left | AnchorStyles.Right
            };
            vendorComboBox.Items.AddRange(new[] {
                "ABC Auto Repair", "Quick Lube Station", "Tire Pro",
                "Main Street Garage", "Fleet Services Inc", "Other"
            });
            formTableLayout.Controls.Add(vendorComboBox, 1, row++);

            // Cost
            costLabel = new AutoLabel()
            {
                Text = "Cost:",
                Font = new Font("Segoe UI", 10F),
                Anchor = AnchorStyles.Left,
                AutoSize = true
            };
            formTableLayout.Controls.Add(costLabel, 0, row);

            costTextBox = new TextBoxExt()
            {
                Size = new Size(120, 25),
                Anchor = AnchorStyles.Left
            };
            formTableLayout.Controls.Add(costTextBox, 1, row++);

            // Notes
            notesLabel = new AutoLabel()
            {
                Text = "Notes:",
                Font = new Font("Segoe UI", 10F),
                Anchor = AnchorStyles.Left,
                AutoSize = true
            };
            formTableLayout.Controls.Add(notesLabel, 0, row);

            notesTextBox = new TextBoxExt()
            {
                Size = new Size(400, 60),
                Multiline = true,
                ScrollBars = ScrollBars.Vertical,
                Anchor = AnchorStyles.Left | AnchorStyles.Right
            };
            formTableLayout.Controls.Add(notesTextBox, 1, row++);
        }

        private void CreateButtons()
        {
            // Button panel
            buttonPanel = new GradientPanel()
            {
                Size = new Size(580, 50),
                Location = new Point(20, 390)
            };
            mainPanel.Controls.Add(buttonPanel);

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

            buttonPanel.Controls.Add(saveButton);
            buttonPanel.Controls.Add(cancelButton);
        }

        private async void LoadFormData()
        {
            try
            {
                // Load buses for selection
                var buses = await _busService.GetAllBusesAsync();
                busComboBox.DataSource = buses.ToList();
                busComboBox.DisplayMember = "BusNumber";
                busComboBox.ValueMember = "BusId";

                _logger.LogDebug("Form data loaded successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading form data");
                MessageBox.Show("Error loading form data. Please try again.", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadMaintenanceData()
        {
            if (_currentMaintenance == null) return;

            try
            {
                busComboBox.SelectedValue = _currentMaintenance.VehicleId;
                dateEdit.Value = _currentMaintenance.Date;
                odometerTextBox.Text = _currentMaintenance.OdometerReading.ToString();
                maintenanceTypeComboBox.Text = _currentMaintenance.MaintenanceCompleted;
                vendorComboBox.Text = _currentMaintenance.Vendor;
                costTextBox.Text = _currentMaintenance.RepairCost.ToString("F2");
                notesTextBox.Text = _currentMaintenance.Notes ?? "";

                _logger.LogDebug("Maintenance data loaded for editing: ID {MaintenanceId}", _currentMaintenance.MaintenanceId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading maintenance data for editing");
            }
        }

        private async void SaveButton_Click(object? sender, EventArgs e)
        {
            try
            {
                if (!ValidateInput()) return;

                UpdateMaintenanceFromForm();

                if (_isEditMode && _currentMaintenance != null)
                {
                    EditedMaintenance = await _maintenanceService.UpdateMaintenanceRecordAsync(_currentMaintenance);
                    _logger.LogInformation("Maintenance record updated: ID {MaintenanceId}", _currentMaintenance.MaintenanceId);
                }
                else if (_currentMaintenance != null)
                {
                    EditedMaintenance = await _maintenanceService.CreateMaintenanceRecordAsync(_currentMaintenance);
                    _logger.LogInformation("New maintenance record created");
                }

                IsDataSaved = true;
                DialogResult = DialogResult.OK;
                Close();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving maintenance record");
                MessageBox.Show($"Error saving maintenance record: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void UpdateMaintenanceFromForm()
        {
            try
            {
                if (_currentMaintenance == null)
                {
                    _currentMaintenance = new Maintenance();
                }

                // Update maintenance properties
                _currentMaintenance.VehicleId = (int)busComboBox.SelectedValue;
                _currentMaintenance.Date = dateEdit.Value;
                _currentMaintenance.OdometerReading = int.Parse(odometerTextBox.Text);
                _currentMaintenance.MaintenanceCompleted = maintenanceTypeComboBox.Text;
                _currentMaintenance.Vendor = vendorComboBox.Text;
                _currentMaintenance.RepairCost = decimal.Parse(costTextBox.Text);
                _currentMaintenance.Notes = notesTextBox.Text;
                _currentMaintenance.Status = "Completed";
                _currentMaintenance.UpdatedDate = DateTime.UtcNow;
                _currentMaintenance.UpdatedBy = "System"; // TODO: Use actual user context

                if (!_isEditMode)
                {
                    _currentMaintenance.CreatedDate = DateTime.UtcNow;
                    _currentMaintenance.CreatedBy = "System"; // TODO: Use actual user context
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating maintenance from form data");
                throw;
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

            // Validate odometer reading
            if (!int.TryParse(odometerTextBox.Text, out int odometer) || odometer < 0)
            {
                MessageBoxAdv.Show(this, "Please enter a valid odometer reading.", "Validation Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                odometerTextBox.Focus();
                return false;
            }

            // Validate maintenance type
            if (string.IsNullOrWhiteSpace(maintenanceTypeComboBox.Text))
            {
                MessageBoxAdv.Show(this, "Please select or enter a maintenance type.", "Validation Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                maintenanceTypeComboBox.Focus();
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

            // Validate cost
            if (!decimal.TryParse(costTextBox.Text, out decimal cost) || cost < 0)
            {
                MessageBoxAdv.Show(this, "Please enter a valid cost amount.", "Validation Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                costTextBox.Focus();
                return false;
            }

            return true;
        }

        private void ApplyMetroTheme()
        {
            try
            {
                // Apply Syncfusion Metro styling
                BackColor = Color.FromArgb(255, 255, 255);
                VisualEnhancementManager.ApplyEnhancedTheme(this);

                _logger.LogDebug("Metro theme applied successfully");
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Error applying metro theme");
            }
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);

            // Set focus to the first input control
            if (busComboBox != null)
            {
                busComboBox.Focus();
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                // Dispose of Syncfusion controls
                titleLabel?.Dispose();
                busLabel?.Dispose();
                dateLabel?.Dispose();
                odometerLabel?.Dispose();
                maintenanceLabel?.Dispose();
                vendorLabel?.Dispose();
                costLabel?.Dispose();
                notesLabel?.Dispose();
                busComboBox?.Dispose();
                maintenanceTypeComboBox?.Dispose();
                vendorComboBox?.Dispose();
                dateEdit?.Dispose();
                odometerTextBox?.Dispose();
                costTextBox?.Dispose();
                notesTextBox?.Dispose();
                saveButton?.Dispose();
                cancelButton?.Dispose();
                mainPanel?.Dispose();
                formTableLayout?.Dispose();
                buttonPanel?.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
