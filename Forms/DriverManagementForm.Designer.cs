using Syncfusion.Windows.Forms;
using Syncfusion.Windows.Forms.Tools;
using Syncfusion.WinForms.Controls;
using System.Windows.Forms;

namespace Bus_Buddy.Forms;

partial class DriverManagementForm
{
    /// <summary>
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    // Declare Syncfusion controls
    private GradientPanel mainPanel;
    private GradientPanel headerPanel;
    private GradientPanel contentPanel;
    private GradientPanel buttonPanel;
    private AutoLabel titleLabel;
    private AutoLabel statusLabel;

    // Data grid for displaying drivers
    private DataGridView driverDataGrid;

    // Action buttons
    private SfButton addDriverButton;
    private SfButton editDriverButton;
    private SfButton deleteDriverButton;
    private SfButton viewLicenseButton;
    private SfButton refreshButton;
    private SfButton closeButton;

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
        this.components = new System.ComponentModel.Container();

        // Suspend layout for better performance during initialization
        this.SuspendLayout();

        // Initialize Syncfusion components
        this.mainPanel = new Syncfusion.Windows.Forms.Tools.GradientPanel();
        this.headerPanel = new Syncfusion.Windows.Forms.Tools.GradientPanel();
        this.contentPanel = new Syncfusion.Windows.Forms.Tools.GradientPanel();
        this.buttonPanel = new Syncfusion.Windows.Forms.Tools.GradientPanel();
        this.titleLabel = new Syncfusion.Windows.Forms.Tools.AutoLabel();
        this.statusLabel = new Syncfusion.Windows.Forms.Tools.AutoLabel();

        // Initialize data grid
        this.driverDataGrid = new System.Windows.Forms.DataGridView();
        ((System.ComponentModel.ISupportInitialize)(this.driverDataGrid)).BeginInit();

        // Initialize buttons
        this.addDriverButton = new Syncfusion.WinForms.Controls.SfButton();
        this.editDriverButton = new Syncfusion.WinForms.Controls.SfButton();
        this.deleteDriverButton = new Syncfusion.WinForms.Controls.SfButton();
        this.viewLicenseButton = new Syncfusion.WinForms.Controls.SfButton();
        this.refreshButton = new Syncfusion.WinForms.Controls.SfButton();
        this.closeButton = new Syncfusion.WinForms.Controls.SfButton();

        // Main Panel
        this.mainPanel.BorderStyle = System.Windows.Forms.BorderStyle.None;
        this.mainPanel.Dock = System.Windows.Forms.DockStyle.Fill;
        this.mainPanel.Location = new System.Drawing.Point(0, 0);
        this.mainPanel.Name = "mainPanel";
        this.mainPanel.Size = new System.Drawing.Size(1100, 700);
        this.mainPanel.TabIndex = 0;

        // Header Panel
        this.headerPanel.BorderStyle = System.Windows.Forms.BorderStyle.None;
        this.headerPanel.BackgroundColor = new Syncfusion.Drawing.BrushInfo(System.Drawing.Color.FromArgb(156, 39, 176));
        this.headerPanel.Dock = System.Windows.Forms.DockStyle.Top;
        this.headerPanel.Location = new System.Drawing.Point(0, 0);
        this.headerPanel.Name = "headerPanel";
        this.headerPanel.Size = new System.Drawing.Size(1100, 60);
        this.headerPanel.TabIndex = 1;

        // Title Label
        this.titleLabel.BackColor = System.Drawing.Color.Transparent;
        this.titleLabel.Font = new System.Drawing.Font("Segoe UI", 16F, System.Drawing.FontStyle.Bold);
        this.titleLabel.ForeColor = System.Drawing.Color.White;
        this.titleLabel.Location = new System.Drawing.Point(20, 15);
        this.titleLabel.Name = "titleLabel";
        this.titleLabel.Size = new System.Drawing.Size(300, 30);
        this.titleLabel.TabIndex = 0;
        this.titleLabel.Text = "Driver Management - Bus Drivers";

        // Button Panel
        this.buttonPanel.BorderStyle = System.Windows.Forms.BorderStyle.None;
        this.buttonPanel.Dock = System.Windows.Forms.DockStyle.Bottom;
        this.buttonPanel.Location = new System.Drawing.Point(0, 640);
        this.buttonPanel.Name = "buttonPanel";
        this.buttonPanel.Size = new System.Drawing.Size(1100, 60);
        this.buttonPanel.TabIndex = 2;
        this.buttonPanel.Padding = new System.Windows.Forms.Padding(10);

        // Content Panel
        this.contentPanel.BorderStyle = System.Windows.Forms.BorderStyle.None;
        this.contentPanel.Dock = System.Windows.Forms.DockStyle.Fill;
        this.contentPanel.Location = new System.Drawing.Point(0, 60);
        this.contentPanel.Name = "contentPanel";
        this.contentPanel.Size = new System.Drawing.Size(1100, 580);
        this.contentPanel.TabIndex = 3;
        this.contentPanel.Padding = new System.Windows.Forms.Padding(20);

        // Status Label
        this.statusLabel.BackColor = System.Drawing.Color.Transparent;
        this.statusLabel.Font = new System.Drawing.Font("Segoe UI", 9F);
        this.statusLabel.ForeColor = System.Drawing.Color.Gray;
        this.statusLabel.Location = new System.Drawing.Point(30, 20);
        this.statusLabel.Name = "statusLabel";
        this.statusLabel.Size = new System.Drawing.Size(300, 20);
        this.statusLabel.TabIndex = 1;
        this.statusLabel.Text = "Loading...";

        // Driver Data Grid
        this.driverDataGrid.AllowUserToAddRows = false;
        this.driverDataGrid.AllowUserToDeleteRows = false;
        this.driverDataGrid.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
        this.driverDataGrid.BackgroundColor = System.Drawing.Color.White;
        this.driverDataGrid.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
        this.driverDataGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
        this.driverDataGrid.ColumnHeadersHeight = 30;
        this.driverDataGrid.Dock = System.Windows.Forms.DockStyle.Fill;
        this.driverDataGrid.Location = new System.Drawing.Point(20, 40);
        this.driverDataGrid.MultiSelect = false;
        this.driverDataGrid.Name = "driverDataGrid";
        this.driverDataGrid.ReadOnly = true;
        this.driverDataGrid.RowHeadersVisible = false;
        this.driverDataGrid.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
        this.driverDataGrid.Size = new System.Drawing.Size(1060, 520);
        this.driverDataGrid.TabIndex = 0;
        this.driverDataGrid.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.DriverDataGrid_CellDoubleClick);
        this.driverDataGrid.SelectionChanged += new System.EventHandler(this.DriverDataGrid_SelectionChanged);

        // Configure action buttons
        ConfigureActionButton(this.addDriverButton, "Add Driver", 20, 15, 0, System.Drawing.Color.FromArgb(76, 175, 80));
        ConfigureActionButton(this.editDriverButton, "Edit Driver", 140, 15, 1, System.Drawing.Color.FromArgb(255, 152, 0));
        ConfigureActionButton(this.deleteDriverButton, "Delete Driver", 260, 15, 2, System.Drawing.Color.FromArgb(244, 67, 54));
        ConfigureActionButton(this.viewLicenseButton, "View License", 380, 15, 3, System.Drawing.Color.FromArgb(156, 39, 176));
        ConfigureActionButton(this.refreshButton, "Refresh", 500, 15, 4, System.Drawing.Color.FromArgb(33, 150, 243));
        ConfigureActionButton(this.closeButton, "Close", 980, 15, 5, System.Drawing.Color.FromArgb(158, 158, 158));

        // Initially disable edit/delete/view buttons until selection is made
        this.editDriverButton.Enabled = false;
        this.deleteDriverButton.Enabled = false;
        this.viewLicenseButton.Enabled = false;

        // Add event handlers
        this.addDriverButton.Click += new System.EventHandler(this.AddDriverButton_Click);
        this.editDriverButton.Click += new System.EventHandler(this.EditDriverButton_Click);
        this.deleteDriverButton.Click += new System.EventHandler(this.DeleteDriverButton_Click);
        this.viewLicenseButton.Click += new System.EventHandler(this.ViewLicenseButton_Click);
        this.refreshButton.Click += new System.EventHandler(this.RefreshButton_Click);
        this.closeButton.Click += new System.EventHandler(this.CloseButton_Click);

        // Add controls to panels
        this.headerPanel.Controls.Add(this.titleLabel);

        this.contentPanel.Controls.Add(this.driverDataGrid);
        this.contentPanel.Controls.Add(this.statusLabel);

        this.buttonPanel.Controls.Add(this.addDriverButton);
        this.buttonPanel.Controls.Add(this.editDriverButton);
        this.buttonPanel.Controls.Add(this.deleteDriverButton);
        this.buttonPanel.Controls.Add(this.viewLicenseButton);
        this.buttonPanel.Controls.Add(this.refreshButton);
        this.buttonPanel.Controls.Add(this.closeButton);

        this.mainPanel.Controls.Add(this.contentPanel);
        this.mainPanel.Controls.Add(this.buttonPanel);
        this.mainPanel.Controls.Add(this.headerPanel);

        // Form settings
        this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
        this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
        this.ClientSize = new System.Drawing.Size(1100, 700);
        this.Controls.Add(this.mainPanel);
        this.Text = "Driver Management";
        this.MinimumSize = new System.Drawing.Size(900, 600);
        this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
        this.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);

        // Complete data grid initialization
        ((System.ComponentModel.ISupportInitialize)(this.driverDataGrid)).EndInit();

        // Resume layout
        this.ResumeLayout(false);
    }

    private void ConfigureActionButton(SfButton button, string title, int x, int y, int tabIndex, System.Drawing.Color color)
    {
        button.AccessibleName = title;
        button.BackColor = color;
        button.ForeColor = System.Drawing.Color.White;
        button.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
        button.Location = new System.Drawing.Point(x, y);
        button.Name = title.Replace(" ", "").ToLower() + "Button";
        button.Size = new System.Drawing.Size(100, 30);
        button.TabIndex = tabIndex;
        button.Text = title;
        button.UseVisualStyleBackColor = false;
    }

    #endregion
}
