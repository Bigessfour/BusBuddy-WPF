using Syncfusion.WinForms.Controls;
using Syncfusion.WinForms.DataGrid;
using Syncfusion.Windows.Forms.Tools;

namespace Bus_Buddy.Forms
{
    partial class TicketManagementForm
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
            this.lblTotalRevenue = new AutoLabel();
            this.lblTicketsSold = new AutoLabel();
            this.panelControls = new GradientPanel();
            this.btnAdd = new Syncfusion.WinForms.Controls.SfButton();
            this.btnEdit = new Syncfusion.WinForms.Controls.SfButton();
            this.btnDelete = new Syncfusion.WinForms.Controls.SfButton();
            this.btnRefresh = new Syncfusion.WinForms.Controls.SfButton();
            this.btnViewDetails = new Syncfusion.WinForms.Controls.SfButton();
            this.btnPrintTicket = new Syncfusion.WinForms.Controls.SfButton();
            this.txtSearch = new Syncfusion.Windows.Forms.Tools.TextBoxExt();
            this.labelSearch = new AutoLabel();
            this.cmbRouteFilter = new Syncfusion.Windows.Forms.Tools.ComboBoxAdv();
            this.labelRoute = new AutoLabel();
            this.cmbStatusFilter = new Syncfusion.Windows.Forms.Tools.ComboBoxAdv();
            this.labelStatus = new AutoLabel();
            this.dtpDateFilter = new Syncfusion.Windows.Forms.Tools.DateTimePickerAdv();
            this.labelDateFilter = new AutoLabel();
            this.panelGrid = new GradientPanel();
            this.dataGridTickets = new Syncfusion.WinForms.DataGrid.SfDataGrid();
            this.statusStrip = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.panelHeader.SuspendLayout();
            this.panelControls.SuspendLayout();
            this.panelGrid.SuspendLayout();
            this.statusStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // panelHeader
            // 
            this.panelHeader.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(249)))), ((int)(((byte)(250)))));
            this.panelHeader.Controls.Add(this.lblTicketsSold);
            this.panelHeader.Controls.Add(this.lblTotalRevenue);
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
            this.labelTitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(142)))), ((int)(((byte)(68)))), ((int)(((byte)(173)))));
            this.labelTitle.Location = new System.Drawing.Point(20, 20);
            this.labelTitle.Name = "labelTitle";
            this.labelTitle.Size = new System.Drawing.Size(298, 32);
            this.labelTitle.TabIndex = 0;
            this.labelTitle.Text = "🎫 Ticket Management";
            // 
            // lblTotalRevenue
            // 
            this.lblTotalRevenue.AutoSize = true;
            this.lblTotalRevenue.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.lblTotalRevenue.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(46)))), ((int)(((byte)(204)))), ((int)(((byte)(113)))));
            this.lblTotalRevenue.Location = new System.Drawing.Point(350, 15);
            this.lblTotalRevenue.Name = "lblTotalRevenue";
            this.lblTotalRevenue.Size = new System.Drawing.Size(139, 20);
            this.lblTotalRevenue.TabIndex = 1;
            this.lblTotalRevenue.Text = "Total Revenue: $0.00";
            // 
            // lblTicketsSold
            // 
            this.lblTicketsSold.AutoSize = true;
            this.lblTicketsSold.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.lblTicketsSold.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(142)))), ((int)(((byte)(68)))), ((int)(((byte)(173)))));
            this.lblTicketsSold.Location = new System.Drawing.Point(350, 40);
            this.lblTicketsSold.Name = "lblTicketsSold";
            this.lblTicketsSold.Size = new System.Drawing.Size(119, 20);
            this.lblTicketsSold.TabIndex = 2;
            this.lblTicketsSold.Text = "Tickets Sold: 0";
            // 
            // panelControls
            // 
            this.panelControls.BackColor = System.Drawing.Color.White;
            this.panelControls.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelControls.Controls.Add(this.labelDateFilter);
            this.panelControls.Controls.Add(this.dtpDateFilter);
            this.panelControls.Controls.Add(this.labelStatus);
            this.panelControls.Controls.Add(this.cmbStatusFilter);
            this.panelControls.Controls.Add(this.labelRoute);
            this.panelControls.Controls.Add(this.cmbRouteFilter);
            this.panelControls.Controls.Add(this.labelSearch);
            this.panelControls.Controls.Add(this.txtSearch);
            this.panelControls.Controls.Add(this.btnPrintTicket);
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
            this.btnAdd.Size = new System.Drawing.Size(120, 40);
            this.btnAdd.Style.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(142)))), ((int)(((byte)(68)))), ((int)(((byte)(173)))));
            this.btnAdd.Style.ForeColor = System.Drawing.Color.White;
            this.btnAdd.TabIndex = 0;
            this.btnAdd.Text = "➕ Sell Ticket";
            this.btnAdd.UseVisualStyleBackColor = false;
            this.btnAdd.Click += new System.EventHandler(this.BtnAdd_Click);
            // 
            // btnEdit
            // 
            this.btnEdit.Location = new System.Drawing.Point(150, 20);
            this.btnEdit.Name = "btnEdit";
            this.btnEdit.Size = new System.Drawing.Size(120, 40);
            this.btnEdit.Style.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(241)))), ((int)(((byte)(196)))), ((int)(((byte)(15)))));
            this.btnEdit.Style.ForeColor = System.Drawing.Color.White;
            this.btnEdit.TabIndex = 1;
            this.btnEdit.Text = "✏️ Edit Ticket";
            this.btnEdit.UseVisualStyleBackColor = false;
            this.btnEdit.Click += new System.EventHandler(this.BtnEdit_Click);
            // 
            // btnDelete
            // 
            this.btnDelete.Location = new System.Drawing.Point(280, 20);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(100, 40);
            this.btnDelete.Style.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(231)))), ((int)(((byte)(76)))), ((int)(((byte)(60)))));
            this.btnDelete.Style.ForeColor = System.Drawing.Color.White;
            this.btnDelete.TabIndex = 2;
            this.btnDelete.Text = "🗑️ Cancel";
            this.btnDelete.UseVisualStyleBackColor = false;
            this.btnDelete.Click += new System.EventHandler(this.BtnDelete_Click);
            // 
            // btnRefresh
            // 
            this.btnRefresh.Location = new System.Drawing.Point(390, 20);
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
            this.btnViewDetails.Location = new System.Drawing.Point(500, 20);
            this.btnViewDetails.Name = "btnViewDetails";
            this.btnViewDetails.Size = new System.Drawing.Size(120, 40);
            this.btnViewDetails.Style.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(52)))), ((int)(((byte)(152)))), ((int)(((byte)(219)))));
            this.btnViewDetails.Style.ForeColor = System.Drawing.Color.White;
            this.btnViewDetails.TabIndex = 4;
            this.btnViewDetails.Text = "👁️ View Details";
            this.btnViewDetails.UseVisualStyleBackColor = false;
            this.btnViewDetails.Click += new System.EventHandler(this.BtnViewDetails_Click);
            // 
            // btnPrintTicket
            // 
            this.btnPrintTicket.Location = new System.Drawing.Point(630, 20);
            this.btnPrintTicket.Name = "btnPrintTicket";
            this.btnPrintTicket.Size = new System.Drawing.Size(120, 40);
            this.btnPrintTicket.Style.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(230)))), ((int)(((byte)(126)))), ((int)(((byte)(34)))));
            this.btnPrintTicket.Style.ForeColor = System.Drawing.Color.White;
            this.btnPrintTicket.TabIndex = 5;
            this.btnPrintTicket.Text = "🖨️ Print Ticket";
            this.btnPrintTicket.UseVisualStyleBackColor = false;
            this.btnPrintTicket.Click += new System.EventHandler(this.BtnPrintTicket_Click);
            // 
            // txtSearch
            // 
            this.txtSearch.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtSearch.Location = new System.Drawing.Point(70, 80);
            this.txtSearch.Name = "txtSearch";
            this.txtSearch.Size = new System.Drawing.Size(180, 23);
            this.txtSearch.TabIndex = 6;
            this.txtSearch.TextChanged += new System.EventHandler(this.TxtSearch_TextChanged);
            // 
            // labelSearch
            // 
            this.labelSearch.AutoSize = true;
            this.labelSearch.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.labelSearch.Location = new System.Drawing.Point(20, 82);
            this.labelSearch.Name = "labelSearch";
            this.labelSearch.Size = new System.Drawing.Size(45, 15);
            this.labelSearch.TabIndex = 7;
            this.labelSearch.Text = "Search:";
            // 
            // cmbRouteFilter
            // 
            this.cmbRouteFilter.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbRouteFilter.Location = new System.Drawing.Point(320, 80);
            this.cmbRouteFilter.Name = "cmbRouteFilter";
            this.cmbRouteFilter.Size = new System.Drawing.Size(150, 21);
            this.cmbRouteFilter.TabIndex = 8;
            this.cmbRouteFilter.SelectedIndexChanged += new System.EventHandler(this.CmbRouteFilter_SelectedIndexChanged);
            // 
            // labelRoute
            // 
            this.labelRoute.AutoSize = true;
            this.labelRoute.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.labelRoute.Location = new System.Drawing.Point(270, 82);
            this.labelRoute.Name = "labelRoute";
            this.labelRoute.Size = new System.Drawing.Size(40, 15);
            this.labelRoute.TabIndex = 9;
            this.labelRoute.Text = "Route:";
            // 
            // cmbStatusFilter
            // 
            this.cmbStatusFilter.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbStatusFilter.Location = new System.Drawing.Point(540, 80);
            this.cmbStatusFilter.Name = "cmbStatusFilter";
            this.cmbStatusFilter.Size = new System.Drawing.Size(120, 21);
            this.cmbStatusFilter.TabIndex = 10;
            this.cmbStatusFilter.SelectedIndexChanged += new System.EventHandler(this.CmbStatusFilter_SelectedIndexChanged);
            // 
            // labelStatus
            // 
            this.labelStatus.AutoSize = true;
            this.labelStatus.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.labelStatus.Location = new System.Drawing.Point(490, 82);
            this.labelStatus.Name = "labelStatus";
            this.labelStatus.Size = new System.Drawing.Size(42, 15);
            this.labelStatus.TabIndex = 11;
            this.labelStatus.Text = "Status:";
            // 
            // dtpDateFilter
            // 
            this.dtpDateFilter.Location = new System.Drawing.Point(740, 80);
            this.dtpDateFilter.Name = "dtpDateFilter";
            this.dtpDateFilter.Size = new System.Drawing.Size(150, 20);
            this.dtpDateFilter.TabIndex = 12;
            this.dtpDateFilter.Value = new System.DateTime(2025, 7, 3, 0, 0, 0, 0);
            this.dtpDateFilter.ValueChanged += new System.EventHandler(this.DtpDateFilter_ValueChanged);
            // 
            // labelDateFilter
            // 
            this.labelDateFilter.AutoSize = true;
            this.labelDateFilter.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.labelDateFilter.Location = new System.Drawing.Point(680, 82);
            this.labelDateFilter.Name = "labelDateFilter";
            this.labelDateFilter.Size = new System.Drawing.Size(58, 15);
            this.labelDateFilter.TabIndex = 13;
            this.labelDateFilter.Text = "Date From:";
            // 
            // panelGrid
            // 
            this.panelGrid.BackColor = System.Drawing.Color.White;
            this.panelGrid.Controls.Add(this.dataGridTickets);
            this.panelGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelGrid.Location = new System.Drawing.Point(0, 190);
            this.panelGrid.Name = "panelGrid";
            this.panelGrid.Padding = new System.Windows.Forms.Padding(10);
            this.panelGrid.Size = new System.Drawing.Size(1400, 408);
            this.panelGrid.TabIndex = 2;
            // 
            // dataGridTickets
            // 
            this.dataGridTickets.AccessibleName = "Table";
            this.dataGridTickets.AllowEditing = false;
            this.dataGridTickets.AllowFiltering = true;
            this.dataGridTickets.AllowSorting = true;
            this.dataGridTickets.AutoSizeColumnsMode = Syncfusion.WinForms.DataGrid.Enums.AutoSizeColumnsMode.Fill;
            this.dataGridTickets.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridTickets.Location = new System.Drawing.Point(10, 10);
            this.dataGridTickets.Name = "dataGridTickets";
            this.dataGridTickets.SelectionMode = Syncfusion.WinForms.DataGrid.Enums.GridSelectionMode.Single;
            this.dataGridTickets.Size = new System.Drawing.Size(1380, 388);
            this.dataGridTickets.TabIndex = 0;
            this.dataGridTickets.Text = "sfDataGrid1";
            this.dataGridTickets.CellDoubleClick += new Syncfusion.WinForms.DataGrid.Events.CellClickEventHandler(this.DataGridTickets_CellDoubleClick);
            // 
            // statusStrip
            // 
            this.statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel});
            this.statusStrip.Location = new System.Drawing.Point(0, 598);
            this.statusStrip.Name = "statusStrip";
            this.statusStrip.Size = new System.Drawing.Size(1400, 22);
            this.statusStrip.TabIndex = 3;
            this.statusStrip.Text = "statusStrip1";
            // 
            // toolStripStatusLabel
            // 
            this.toolStripStatusLabel.Name = "toolStripStatusLabel";
            this.toolStripStatusLabel.Size = new System.Drawing.Size(39, 17);
            this.toolStripStatusLabel.Text = "Ready";
            // 
            // TicketManagementForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1400, 620);
            this.Controls.Add(this.panelGrid);
            this.Controls.Add(this.statusStrip);
            this.Controls.Add(this.panelControls);
            this.Controls.Add(this.panelHeader);
            this.MinimumSize = new System.Drawing.Size(1200, 600);
            this.Name = "TicketManagementForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Bus Buddy - Ticket Management";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.panelHeader.ResumeLayout(false);
            this.panelHeader.PerformLayout();
            this.panelControls.ResumeLayout(false);
            this.panelControls.PerformLayout();
            this.panelGrid.ResumeLayout(false);
            this.statusStrip.ResumeLayout(false);
            this.statusStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private GradientPanel panelHeader;
        private AutoLabel labelTitle;
        private AutoLabel lblTotalRevenue;
        private AutoLabel lblTicketsSold;
        private GradientPanel panelControls;
        private Syncfusion.WinForms.Controls.SfButton btnAdd;
        private Syncfusion.WinForms.Controls.SfButton btnEdit;
        private Syncfusion.WinForms.Controls.SfButton btnDelete;
        private Syncfusion.WinForms.Controls.SfButton btnRefresh;
        private Syncfusion.WinForms.Controls.SfButton btnViewDetails;
        private Syncfusion.WinForms.Controls.SfButton btnPrintTicket;
        private Syncfusion.Windows.Forms.Tools.TextBoxExt txtSearch;
        private AutoLabel labelSearch;
        private Syncfusion.Windows.Forms.Tools.ComboBoxAdv cmbRouteFilter;
        private AutoLabel labelRoute;
        private Syncfusion.Windows.Forms.Tools.ComboBoxAdv cmbStatusFilter;
        private AutoLabel labelStatus;
        private Syncfusion.Windows.Forms.Tools.DateTimePickerAdv dtpDateFilter;
        private AutoLabel labelDateFilter;
        private GradientPanel panelGrid;
        private Syncfusion.WinForms.DataGrid.SfDataGrid dataGridTickets;
        private System.Windows.Forms.StatusStrip statusStrip;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel;
    }
}
