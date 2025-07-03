using Syncfusion.WinForms.Controls;
using Syncfusion.WinForms.DataGrid;
using Syncfusion.Windows.Forms.Tools;

namespace Bus_Buddy.Forms
{
    partial class FuelManagementForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.panelHeader = new GradientPanel();
            this.labelTitle = new AutoLabel();
            this.lblTotalCost = new AutoLabel();
            this.lblTotalGallons = new AutoLabel();
            this.panelControls = new GradientPanel();
            this.btnAdd = new Syncfusion.WinForms.Controls.SfButton();
            this.btnEdit = new Syncfusion.WinForms.Controls.SfButton();
            this.btnDelete = new Syncfusion.WinForms.Controls.SfButton();
            this.btnRefresh = new Syncfusion.WinForms.Controls.SfButton();
            this.btnViewDetails = new Syncfusion.WinForms.Controls.SfButton();
            this.txtSearch = new Syncfusion.Windows.Forms.Tools.TextBoxExt();
            this.labelSearch = new AutoLabel();
            this.cmbVehicleFilter = new Syncfusion.Windows.Forms.Tools.ComboBoxAdv();
            this.labelVehicle = new AutoLabel();
            this.cmbFuelTypeFilter = new Syncfusion.Windows.Forms.Tools.ComboBoxAdv();
            this.labelFuelType = new AutoLabel();
            this.dtpStartDate = new Syncfusion.Windows.Forms.Tools.DateTimePickerAdv();
            this.labelStartDate = new AutoLabel();
            this.dtpEndDate = new Syncfusion.Windows.Forms.Tools.DateTimePickerAdv();
            this.labelEndDate = new AutoLabel();
            this.panelGrid = new GradientPanel();
            this.dataGridFuel = new Syncfusion.WinForms.DataGrid.SfDataGrid();
            this.statusLabel = new AutoLabel();
            this.panelHeader.SuspendLayout();
            this.panelControls.SuspendLayout();
            this.panelGrid.SuspendLayout();
            this.SuspendLayout();
            // 
            // panelHeader
            // 
            this.panelHeader.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(249)))), ((int)(((byte)(250)))));
            this.panelHeader.Controls.Add(this.lblTotalGallons);
            this.panelHeader.Controls.Add(this.lblTotalCost);
            this.panelHeader.Controls.Add(this.labelTitle);
            this.panelHeader.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelHeader.Location = new System.Drawing.Point(0, 0);
            this.panelHeader.Name = "panelHeader";
            this.panelHeader.Size = new System.Drawing.Size(1400, 70);
            this.panelHeader.TabIndex = 0;
            // 
            // labelTitle
            // 
            this.labelTitle.AutoSize = true;
            this.labelTitle.Font = new System.Drawing.Font("Segoe UI", 18F, System.Drawing.FontStyle.Bold);
            this.labelTitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(41)))), ((int)(((byte)(128)))), ((int)(((byte)(185)))));
            this.labelTitle.Location = new System.Drawing.Point(20, 20);
            this.labelTitle.Name = "labelTitle";
            this.labelTitle.Size = new System.Drawing.Size(287, 32);
            this.labelTitle.TabIndex = 0;
            this.labelTitle.Text = "⛽ Fuel Management";
            // 
            // lblTotalCost
            // 
            this.lblTotalCost.AutoSize = true;
            this.lblTotalCost.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.lblTotalCost.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(231)))), ((int)(((byte)(76)))), ((int)(((byte)(60)))));
            this.lblTotalCost.Location = new System.Drawing.Point(350, 15);
            this.lblTotalCost.Name = "lblTotalCost";
            this.lblTotalCost.Size = new System.Drawing.Size(112, 20);
            this.lblTotalCost.TabIndex = 1;
            this.lblTotalCost.Text = "Total Cost: $0.00";
            // 
            // lblTotalGallons
            // 
            this.lblTotalGallons.AutoSize = true;
            this.lblTotalGallons.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.lblTotalGallons.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(41)))), ((int)(((byte)(128)))), ((int)(((byte)(185)))));
            this.lblTotalGallons.Location = new System.Drawing.Point(350, 40);
            this.lblTotalGallons.Name = "lblTotalGallons";
            this.lblTotalGallons.Size = new System.Drawing.Size(127, 20);
            this.lblTotalGallons.TabIndex = 2;
            this.lblTotalGallons.Text = "Total Gallons: 0.0";
            // 
            // panelControls
            // 
            this.panelControls.BackColor = System.Drawing.Color.White;
            this.panelControls.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelControls.Controls.Add(this.labelEndDate);
            this.panelControls.Controls.Add(this.dtpEndDate);
            this.panelControls.Controls.Add(this.labelStartDate);
            this.panelControls.Controls.Add(this.dtpStartDate);
            this.panelControls.Controls.Add(this.labelFuelType);
            this.panelControls.Controls.Add(this.cmbFuelTypeFilter);
            this.panelControls.Controls.Add(this.labelVehicle);
            this.panelControls.Controls.Add(this.cmbVehicleFilter);
            this.panelControls.Controls.Add(this.labelSearch);
            this.panelControls.Controls.Add(this.txtSearch);
            this.panelControls.Controls.Add(this.btnViewDetails);
            this.panelControls.Controls.Add(this.btnRefresh);
            this.panelControls.Controls.Add(this.btnDelete);
            this.panelControls.Controls.Add(this.btnEdit);
            this.panelControls.Controls.Add(this.btnAdd);
            this.panelControls.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelControls.Location = new System.Drawing.Point(0, 70);
            this.panelControls.Name = "panelControls";
            this.panelControls.Padding = new System.Windows.Forms.Padding(20, 15, 20, 15);
            this.panelControls.Size = new System.Drawing.Size(1400, 120);
            this.panelControls.TabIndex = 1;
            // 
            // btnAdd
            // 
            this.btnAdd.Location = new System.Drawing.Point(20, 20);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(130, 40);
            this.btnAdd.Style.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(41)))), ((int)(((byte)(128)))), ((int)(((byte)(185)))));
            this.btnAdd.Style.ForeColor = System.Drawing.Color.White;
            this.btnAdd.TabIndex = 0;
            this.btnAdd.Text = "➕ Add Fuel Record";
            this.btnAdd.UseVisualStyleBackColor = false;
            this.btnAdd.Click += new System.EventHandler(this.BtnAdd_Click);
            // 
            // btnEdit
            // 
            this.btnEdit.Location = new System.Drawing.Point(160, 20);
            this.btnEdit.Name = "btnEdit";
            this.btnEdit.Size = new System.Drawing.Size(130, 40);
            this.btnEdit.Style.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(241)))), ((int)(((byte)(196)))), ((int)(((byte)(15)))));
            this.btnEdit.Style.ForeColor = System.Drawing.Color.White;
            this.btnEdit.TabIndex = 1;
            this.btnEdit.Text = "✏️ Edit Record";
            this.btnEdit.UseVisualStyleBackColor = false;
            this.btnEdit.Click += new System.EventHandler(this.BtnEdit_Click);
            // 
            // btnDelete
            // 
            this.btnDelete.Location = new System.Drawing.Point(300, 20);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(100, 40);
            this.btnDelete.Style.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(231)))), ((int)(((byte)(76)))), ((int)(((byte)(60)))));
            this.btnDelete.Style.ForeColor = System.Drawing.Color.White;
            this.btnDelete.TabIndex = 2;
            this.btnDelete.Text = "🗑️ Delete";
            this.btnDelete.UseVisualStyleBackColor = false;
            this.btnDelete.Click += new System.EventHandler(this.BtnDelete_Click);
            // 
            // btnRefresh
            // 
            this.btnRefresh.Location = new System.Drawing.Point(410, 20);
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new System.Drawing.Size(100, 40);
            this.btnRefresh.Style.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(155)))), ((int)(((byte)(89)))), ((int)(((byte)(182)))));
            this.btnRefresh.Style.ForeColor = System.Drawing.Color.White;
            this.btnRefresh.TabIndex = 3;
            this.btnRefresh.Text = "🔄 Refresh";
            this.btnRefresh.UseVisualStyleBackColor = false;
            this.btnRefresh.Click += new System.EventHandler(this.BtnRefresh_Click);
            // 
            // btnViewDetails
            // 
            this.btnViewDetails.Location = new System.Drawing.Point(520, 20);
            this.btnViewDetails.Name = "btnViewDetails";
            this.btnViewDetails.Size = new System.Drawing.Size(130, 40);
            this.btnViewDetails.Style.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(46)))), ((int)(((byte)(204)))), ((int)(((byte)(113)))));
            this.btnViewDetails.Style.ForeColor = System.Drawing.Color.White;
            this.btnViewDetails.TabIndex = 4;
            this.btnViewDetails.Text = "👁️ View Details";
            this.btnViewDetails.UseVisualStyleBackColor = false;
            this.btnViewDetails.Click += new System.EventHandler(this.BtnViewDetails_Click);
            // 
            // txtSearch
            // 
            this.txtSearch.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtSearch.Location = new System.Drawing.Point(70, 80);
            this.txtSearch.Name = "txtSearch";
            this.txtSearch.Size = new System.Drawing.Size(180, 23);
            this.txtSearch.TabIndex = 5;
            this.txtSearch.TextChanged += new System.EventHandler(this.TxtSearch_TextChanged);
            // 
            // labelSearch
            // 
            this.labelSearch.AutoSize = true;
            this.labelSearch.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.labelSearch.Location = new System.Drawing.Point(20, 82);
            this.labelSearch.Name = "labelSearch";
            this.labelSearch.Size = new System.Drawing.Size(45, 15);
            this.labelSearch.TabIndex = 6;
            this.labelSearch.Text = "Search:";
            // 
            // cmbVehicleFilter
            // 

            this.cmbVehicleFilter.Location = new System.Drawing.Point(320, 80);
            this.cmbVehicleFilter.Name = "cmbVehicleFilter";
            this.cmbVehicleFilter.Size = new System.Drawing.Size(150, 21);
            this.cmbVehicleFilter.TabIndex = 7;
            this.cmbVehicleFilter.SelectedIndexChanged += new System.EventHandler(this.CmbVehicleFilter_SelectedIndexChanged);
            // 
            // labelVehicle
            // 
            this.labelVehicle.AutoSize = true;
            this.labelVehicle.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.labelVehicle.Location = new System.Drawing.Point(270, 82);
            this.labelVehicle.Name = "labelVehicle";
            this.labelVehicle.Size = new System.Drawing.Size(47, 15);
            this.labelVehicle.TabIndex = 8;
            this.labelVehicle.Text = "Vehicle:";
            // 
            // cmbFuelTypeFilter
            // 

            this.cmbFuelTypeFilter.Location = new System.Drawing.Point(550, 80);
            this.cmbFuelTypeFilter.Name = "cmbFuelTypeFilter";
            this.cmbFuelTypeFilter.Size = new System.Drawing.Size(120, 21);
            this.cmbFuelTypeFilter.TabIndex = 9;
            this.cmbFuelTypeFilter.SelectedIndexChanged += new System.EventHandler(this.CmbFuelTypeFilter_SelectedIndexChanged);
            // 
            // labelFuelType
            // 
            this.labelFuelType.AutoSize = true;
            this.labelFuelType.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.labelFuelType.Location = new System.Drawing.Point(490, 82);
            this.labelFuelType.Name = "labelFuelType";
            this.labelFuelType.Size = new System.Drawing.Size(60, 15);
            this.labelFuelType.TabIndex = 10;
            this.labelFuelType.Text = "Fuel Type:";
            // 
            // dtpStartDate
            // 
            this.dtpStartDate.Location = new System.Drawing.Point(750, 80);
            this.dtpStartDate.Name = "dtpStartDate";
            this.dtpStartDate.Size = new System.Drawing.Size(120, 20);
            this.dtpStartDate.TabIndex = 11;
            this.dtpStartDate.Value = new System.DateTime(2025, 7, 3, 0, 0, 0, 0);
            this.dtpStartDate.ValueChanged += new System.EventHandler(this.DtpStartDate_ValueChanged);
            // 
            // labelStartDate
            // 
            this.labelStartDate.AutoSize = true;
            this.labelStartDate.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.labelStartDate.Location = new System.Drawing.Point(690, 82);
            this.labelStartDate.Name = "labelStartDate";
            this.labelStartDate.Size = new System.Drawing.Size(58, 15);
            this.labelStartDate.TabIndex = 12;
            this.labelStartDate.Text = "Start Date:";
            // 
            // dtpEndDate
            // 
            this.dtpEndDate.Location = new System.Drawing.Point(940, 80);
            this.dtpEndDate.Name = "dtpEndDate";
            this.dtpEndDate.Size = new System.Drawing.Size(120, 20);
            this.dtpEndDate.TabIndex = 13;
            this.dtpEndDate.Value = new System.DateTime(2025, 7, 3, 0, 0, 0, 0);
            this.dtpEndDate.ValueChanged += new System.EventHandler(this.DtpEndDate_ValueChanged);
            // 
            // labelEndDate
            // 
            this.labelEndDate.AutoSize = true;
            this.labelEndDate.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.labelEndDate.Location = new System.Drawing.Point(890, 82);
            this.labelEndDate.Name = "labelEndDate";
            this.labelEndDate.Size = new System.Drawing.Size(55, 15);
            this.labelEndDate.TabIndex = 14;
            this.labelEndDate.Text = "End Date:";
            // 
            // panelGrid
            // 
            this.panelGrid.BackColor = System.Drawing.Color.White;
            this.panelGrid.Controls.Add(this.dataGridFuel);
            this.panelGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelGrid.Location = new System.Drawing.Point(0, 190);
            this.panelGrid.Name = "panelGrid";
            this.panelGrid.Padding = new System.Windows.Forms.Padding(10);
            this.panelGrid.Size = new System.Drawing.Size(1400, 408);
            this.panelGrid.TabIndex = 2;
            // 
            // dataGridFuel
            // 
            this.dataGridFuel.AccessibleName = "Table";
            this.dataGridFuel.AllowEditing = false;
            this.dataGridFuel.AllowFiltering = true;
            this.dataGridFuel.AllowSorting = true;
            this.dataGridFuel.AutoSizeColumnsMode = Syncfusion.WinForms.DataGrid.Enums.AutoSizeColumnsMode.Fill;
            this.dataGridFuel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridFuel.Location = new System.Drawing.Point(10, 10);
            this.dataGridFuel.Name = "dataGridFuel";
            this.dataGridFuel.SelectionMode = Syncfusion.WinForms.DataGrid.Enums.GridSelectionMode.Single;
            this.dataGridFuel.Size = new System.Drawing.Size(1380, 388);
            this.dataGridFuel.TabIndex = 0;
            this.dataGridFuel.Text = "sfDataGrid1";
            this.dataGridFuel.CellDoubleClick += new Syncfusion.WinForms.DataGrid.Events.CellClickEventHandler(this.DataGridFuel_CellDoubleClick);
            // 
            // statusLabel
            // 
            this.statusLabel.AutoSize = true;
            this.statusLabel.BackColor = System.Drawing.Color.FromArgb(248, 249, 250);
            this.statusLabel.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.statusLabel.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.statusLabel.ForeColor = System.Drawing.Color.Gray;
            this.statusLabel.Location = new System.Drawing.Point(0, 598);
            this.statusLabel.Name = "statusLabel";
            this.statusLabel.Padding = new System.Windows.Forms.Padding(10, 2, 10, 2);
            this.statusLabel.Size = new System.Drawing.Size(1400, 22);
            this.statusLabel.TabIndex = 3;
            this.statusLabel.Text = "Ready";
            this.statusLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // FuelManagementForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1400, 620);
            this.Controls.Add(this.panelGrid);
            this.Controls.Add(this.statusLabel);
            this.Controls.Add(this.panelControls);
            this.Controls.Add(this.panelHeader);
            this.MinimumSize = new System.Drawing.Size(1200, 600);
            this.Name = "FuelManagementForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Bus Buddy - Fuel Management";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.panelHeader.ResumeLayout(false);
            this.panelHeader.PerformLayout();
            this.panelControls.ResumeLayout(false);
            this.panelControls.PerformLayout();
            this.panelGrid.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private GradientPanel panelHeader;
        private AutoLabel labelTitle;
        private AutoLabel lblTotalCost;
        private AutoLabel lblTotalGallons;
        private GradientPanel panelControls;
        private Syncfusion.WinForms.Controls.SfButton btnAdd;
        private Syncfusion.WinForms.Controls.SfButton btnEdit;
        private Syncfusion.WinForms.Controls.SfButton btnDelete;
        private Syncfusion.WinForms.Controls.SfButton btnRefresh;
        private Syncfusion.WinForms.Controls.SfButton btnViewDetails;
        private Syncfusion.Windows.Forms.Tools.TextBoxExt txtSearch;
        private AutoLabel labelSearch;
        private Syncfusion.Windows.Forms.Tools.ComboBoxAdv cmbVehicleFilter;
        private AutoLabel labelVehicle;
        private Syncfusion.Windows.Forms.Tools.ComboBoxAdv cmbFuelTypeFilter;
        private AutoLabel labelFuelType;
        private Syncfusion.Windows.Forms.Tools.DateTimePickerAdv dtpStartDate;
        private AutoLabel labelStartDate;
        private Syncfusion.Windows.Forms.Tools.DateTimePickerAdv dtpEndDate;
        private AutoLabel labelEndDate;
        private GradientPanel panelGrid;
        private Syncfusion.WinForms.DataGrid.SfDataGrid dataGridFuel;
        private AutoLabel statusLabel;
    }
}
