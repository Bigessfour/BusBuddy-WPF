using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Syncfusion.Windows.Forms;
using Syncfusion.Windows.Forms.Tools;
using Syncfusion.WinForms.Controls;
using Bus_Buddy.Models;
using Bus_Buddy.Services;

namespace Bus_Buddy.Forms
{
    public partial class EnhancedScheduleManagementForm : SfForm
    {
        private readonly IScheduleService _scheduleService;
        private readonly IBusService _busService;
        private readonly IActivityService _activityService;

        private DataGridView _scheduleGrid = null!;
        private SfButton _addButton = null!;
        private SfButton _editButton = null!;
        private SfButton _deleteButton = null!;
        private SfButton _refreshButton = null!;
        private DateTimePicker _startDatePicker = null!;
        private DateTimePicker _endDatePicker = null!;
        private Label _dateRangeLabel = null!;
        private Panel _toolbarPanel = null!;
        private Panel _gridPanel = null!;

        public EnhancedScheduleManagementForm(
            IScheduleService scheduleService,
            IBusService busService,
            IActivityService activityService)
        {
            _scheduleService = scheduleService;
            _busService = busService;
            _activityService = activityService;

            InitializeComponent();
            SetupLayout();
            ConfigureDataGrid();
            _ = LoadScheduleData(); // Fire and forget for constructor
        }

        private void InitializeComponent()
        {
            this.Text = "Enhanced Schedule Management";
            this.Size = new Size(1200, 800);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.WindowState = FormWindowState.Maximized;

            // Apply Syncfusion styling
            this.Style.TitleBar.Height = 40;
            this.Style.TitleBar.BackColor = Color.FromArgb(0, 120, 215);
            this.Style.TitleBar.ForeColor = Color.White;
            this.Style.TitleBar.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
        }

        private void SetupLayout()
        {
            // Create toolbar panel
            _toolbarPanel = new Panel
            {
                Height = 80,
                Dock = DockStyle.Top,
                BackColor = Color.FromArgb(250, 250, 250),
                Padding = new Padding(10)
            };

            // Create grid panel
            _gridPanel = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(10)
            };

            CreateToolbarControls();

            this.Controls.Add(_gridPanel);
            this.Controls.Add(_toolbarPanel);
        }

        private void CreateToolbarControls()
        {
            // Date range label
            _dateRangeLabel = new Label
            {
                Text = "Date Range:",
                Location = new Point(10, 15),
                Size = new Size(80, 23),
                TextAlign = ContentAlignment.MiddleLeft
            };

            // Start date picker
            _startDatePicker = new DateTimePicker
            {
                Location = new Point(100, 12),
                Size = new Size(120, 30),
                Value = DateTime.Today.AddMonths(-1),
                Format = DateTimePickerFormat.Short
            };
            _startDatePicker.ValueChanged += DatePicker_ValueChanged;

            // End date picker
            _endDatePicker = new DateTimePicker
            {
                Location = new Point(230, 12),
                Size = new Size(120, 30),
                Value = DateTime.Today.AddMonths(1),
                Format = DateTimePickerFormat.Short
            };
            _endDatePicker.ValueChanged += DatePicker_ValueChanged;

            // Add Button
            _addButton = new SfButton
            {
                Text = "Add Schedule",
                Size = new Size(120, 35),
                Location = new Point(10, 45),
                Style = { BackColor = Color.FromArgb(0, 120, 215), ForeColor = Color.White }
            };
            _addButton.Click += AddButton_Click;

            // Edit Button
            _editButton = new SfButton
            {
                Text = "Edit Schedule",
                Size = new Size(120, 35),
                Location = new Point(140, 45),
                Style = { BackColor = Color.FromArgb(76, 175, 80), ForeColor = Color.White }
            };
            _editButton.Click += EditButton_Click;

            // Delete Button
            _deleteButton = new SfButton
            {
                Text = "Delete Schedule",
                Size = new Size(120, 35),
                Location = new Point(270, 45),
                Style = { BackColor = Color.FromArgb(244, 67, 54), ForeColor = Color.White }
            };
            _deleteButton.Click += DeleteButton_Click;

            // Refresh Button
            _refreshButton = new SfButton
            {
                Text = "Refresh",
                Size = new Size(100, 35),
                Location = new Point(400, 45),
                Style = { BackColor = Color.FromArgb(96, 125, 139), ForeColor = Color.White }
            };
            _refreshButton.Click += RefreshButton_Click;

            _toolbarPanel.Controls.AddRange(new Control[]
            {
                _dateRangeLabel, _startDatePicker, _endDatePicker,
                _addButton, _editButton, _deleteButton, _refreshButton
            });
        }

        private void ConfigureDataGrid()
        {
            _scheduleGrid = new DataGridView
            {
                Dock = DockStyle.Fill,
                AutoGenerateColumns = false,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.None,
                ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize,
                RowHeadersVisible = false
            };

            // Configure columns
            _scheduleGrid.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "Date",
                DataPropertyName = "ActivityDate",
                Width = 100,
                DefaultCellStyle = { Format = "dd/MM/yyyy" }
            });

            _scheduleGrid.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "Activity",
                DataPropertyName = "ActivityType",
                Width = 120
            });

            _scheduleGrid.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "Route",
                DataPropertyName = "Route.RouteName",
                Width = 150
            });

            _scheduleGrid.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "Start Time",
                DataPropertyName = "StartTime",
                Width = 100,
                DefaultCellStyle = { Format = @"hh\:mm" }
            });

            _scheduleGrid.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "End Time",
                DataPropertyName = "EndTime",
                Width = 100,
                DefaultCellStyle = { Format = @"hh\:mm" }
            });

            _scheduleGrid.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "Students",
                DataPropertyName = "StudentsCount",
                Width = 80
            });

            _scheduleGrid.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "Bus Number",
                DataPropertyName = "Vehicle.BusNumber",
                Width = 100
            });

            _scheduleGrid.Columns.Add(new DataGridViewTextBoxColumn
            {
                HeaderText = "Driver",
                DataPropertyName = "Driver.DriverName",
                Width = 120
            });

            // Style the grid
            _scheduleGrid.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(0, 120, 215);
            _scheduleGrid.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            _scheduleGrid.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            _scheduleGrid.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(248, 248, 248);

            _scheduleGrid.SelectionChanged += ScheduleGrid_SelectionChanged;
            _scheduleGrid.CellDoubleClick += ScheduleGrid_CellDoubleClick;

            _gridPanel.Controls.Add(_scheduleGrid);
        }

        private async Task LoadScheduleData()
        {
            try
            {
                var activities = await _activityService.GetActivitiesByDateRangeAsync(
                    _startDatePicker.Value.Date,
                    _endDatePicker.Value.Date);

                _scheduleGrid.DataSource = activities.ToList();
                _scheduleGrid.Refresh();

                // Update button states
                UpdateButtonStates();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading schedule data: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void UpdateButtonStates()
        {
            var hasSelection = _scheduleGrid.SelectedRows.Count > 0;
            _editButton.Enabled = hasSelection;
            _deleteButton.Enabled = hasSelection;
        }

        // Event Handlers
        private void AddButton_Click(object? sender, EventArgs e)
        {
            // For now, show a message that this needs to be implemented with the correct form
            MessageBox.Show("Add Activity functionality needs to be implemented with ActivityEditForm constructor requirements.",
                "Not Implemented", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void EditButton_Click(object? sender, EventArgs e)
        {
            if (_scheduleGrid.SelectedRows.Count > 0)
            {
                var selectedActivity = (Activity)_scheduleGrid.SelectedRows[0].DataBoundItem;
                MessageBox.Show($"Edit Activity functionality needs to be implemented.\nSelected: {selectedActivity.ActivityType} on {selectedActivity.ActivityDate:dd/MM/yyyy}",
                    "Not Implemented", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private async void DeleteButton_Click(object? sender, EventArgs e)
        {
            if (_scheduleGrid.SelectedRows.Count > 0)
            {
                var selectedActivity = (Activity)_scheduleGrid.SelectedRows[0].DataBoundItem;
                var result = MessageBox.Show(
                    $"Are you sure you want to delete the activity for {selectedActivity.ActivityType} on {selectedActivity.ActivityDate:dd/MM/yyyy}?",
                    "Confirm Delete",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    try
                    {
                        await _activityService.DeleteActivityAsync(selectedActivity.ActivityId);
                        await LoadScheduleData();
                        MessageBox.Show("Activity deleted successfully.", "Success",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error deleting activity: {ex.Message}", "Error",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private async void RefreshButton_Click(object? sender, EventArgs e)
        {
            await LoadScheduleData();
        }

        private async void DatePicker_ValueChanged(object? sender, EventArgs e)
        {
            await LoadScheduleData();
        }

        private void ScheduleGrid_SelectionChanged(object? sender, EventArgs e)
        {
            UpdateButtonStates();
        }

        private void ScheduleGrid_CellDoubleClick(object? sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && _scheduleGrid.Rows[e.RowIndex].DataBoundItem is Activity selectedActivity)
            {
                MessageBox.Show($"Edit Activity functionality needs to be implemented.\nSelected: {selectedActivity.ActivityType} on {selectedActivity.ActivityDate:dd/MM/yyyy}",
                    "Not Implemented", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            UpdateButtonStates();
        }
    }
}
