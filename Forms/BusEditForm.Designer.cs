using Syncfusion.Windows.Forms;
using Syncfusion.Windows.Forms.Tools;
using Syncfusion.WinForms.Controls;
using System.Windows.Forms;

namespace Bus_Buddy.Forms;

partial class BusEditForm
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
    private Syncfusion.Windows.Forms.Tools.GradientPanel basicInfoGroupBox;
    private Syncfusion.Windows.Forms.Tools.GradientPanel detailsGroupBox;
    private Syncfusion.Windows.Forms.Tools.GradientPanel financialGroupBox;

    // Basic Information Controls
    private AutoLabel busNumberLabel;
    private Syncfusion.Windows.Forms.Tools.TextBoxExt busNumberTextBox;
    private AutoLabel yearLabel;
    private Syncfusion.Windows.Forms.Tools.IntegerTextBox yearNumeric;
    private AutoLabel makeLabel;
    private Syncfusion.Windows.Forms.Tools.ComboBoxAdv makeComboBox;
    private AutoLabel modelLabel;
    private Syncfusion.Windows.Forms.Tools.TextBoxExt modelTextBox;
    private AutoLabel seatingCapacityLabel;
    private Syncfusion.Windows.Forms.Tools.IntegerTextBox seatingCapacityNumeric;

    // Details Controls
    private AutoLabel vinLabel;
    private Syncfusion.Windows.Forms.Tools.TextBoxExt vinTextBox;
    private AutoLabel licenseNumberLabel;
    private Syncfusion.Windows.Forms.Tools.TextBoxExt licenseNumberTextBox;
    private AutoLabel lastInspectionLabel;
    private Syncfusion.Windows.Forms.Tools.DateTimePickerAdv lastInspectionDatePicker;
    private AutoLabel odometerLabel;
    private Syncfusion.Windows.Forms.Tools.IntegerTextBox odometerNumeric;
    private AutoLabel statusLabel;
    private Syncfusion.Windows.Forms.Tools.ComboBoxAdv statusComboBox;

    // Financial Controls
    private AutoLabel purchaseDateLabel;
    private Syncfusion.Windows.Forms.Tools.DateTimePickerAdv purchaseDatePicker;
    private AutoLabel purchasePriceLabel;
    private Syncfusion.Windows.Forms.Tools.CurrencyTextBox purchasePriceNumeric;
    private AutoLabel insurancePolicyLabel;
    private Syncfusion.Windows.Forms.Tools.TextBoxExt insurancePolicyTextBox;
    private AutoLabel insuranceExpiryLabel;
    private Syncfusion.Windows.Forms.Tools.DateTimePickerAdv insuranceExpiryDatePicker;

    // Action buttons
    private SfButton saveButton;
    private SfButton cancelButton;

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

        // Initialize group boxes
        this.basicInfoGroupBox = new Syncfusion.Windows.Forms.Tools.GradientPanel();
        this.detailsGroupBox = new Syncfusion.Windows.Forms.Tools.GradientPanel();
        this.financialGroupBox = new Syncfusion.Windows.Forms.Tools.GradientPanel();

        // Initialize basic information controls
        this.busNumberLabel = new Syncfusion.Windows.Forms.Tools.AutoLabel();
        this.busNumberTextBox = new Syncfusion.Windows.Forms.Tools.TextBoxExt();
        this.yearLabel = new Syncfusion.Windows.Forms.Tools.AutoLabel();
        this.yearNumeric = new Syncfusion.Windows.Forms.Tools.IntegerTextBox();
        this.makeLabel = new Syncfusion.Windows.Forms.Tools.AutoLabel();
        this.makeComboBox = new Syncfusion.Windows.Forms.Tools.ComboBoxAdv();
        this.modelLabel = new Syncfusion.Windows.Forms.Tools.AutoLabel();
        this.modelTextBox = new Syncfusion.Windows.Forms.Tools.TextBoxExt();
        this.seatingCapacityLabel = new Syncfusion.Windows.Forms.Tools.AutoLabel();
        this.seatingCapacityNumeric = new Syncfusion.Windows.Forms.Tools.IntegerTextBox();

        // Initialize details controls
        this.vinLabel = new Syncfusion.Windows.Forms.Tools.AutoLabel();
        this.vinTextBox = new Syncfusion.Windows.Forms.Tools.TextBoxExt();
        this.licenseNumberLabel = new Syncfusion.Windows.Forms.Tools.AutoLabel();
        this.licenseNumberTextBox = new Syncfusion.Windows.Forms.Tools.TextBoxExt();
        this.lastInspectionLabel = new Syncfusion.Windows.Forms.Tools.AutoLabel();
        this.lastInspectionDatePicker = new Syncfusion.Windows.Forms.Tools.DateTimePickerAdv();
        this.odometerLabel = new Syncfusion.Windows.Forms.Tools.AutoLabel();
        this.odometerNumeric = new Syncfusion.Windows.Forms.Tools.IntegerTextBox();
        this.statusLabel = new Syncfusion.Windows.Forms.Tools.AutoLabel();
        this.statusComboBox = new Syncfusion.Windows.Forms.Tools.ComboBoxAdv();

        // Initialize financial controls
        this.purchaseDateLabel = new Syncfusion.Windows.Forms.Tools.AutoLabel();
        this.purchaseDatePicker = new Syncfusion.Windows.Forms.Tools.DateTimePickerAdv();
        this.purchasePriceLabel = new Syncfusion.Windows.Forms.Tools.AutoLabel();
        this.purchasePriceNumeric = new Syncfusion.Windows.Forms.Tools.CurrencyTextBox();
        this.insurancePolicyLabel = new Syncfusion.Windows.Forms.Tools.AutoLabel();
        this.insurancePolicyTextBox = new Syncfusion.Windows.Forms.Tools.TextBoxExt();
        this.insuranceExpiryLabel = new Syncfusion.Windows.Forms.Tools.AutoLabel();
        this.insuranceExpiryDatePicker = new Syncfusion.Windows.Forms.Tools.DateTimePickerAdv();

        // Initialize buttons
        this.saveButton = new Syncfusion.WinForms.Controls.SfButton();
        this.cancelButton = new Syncfusion.WinForms.Controls.SfButton();

        // Main Panel
        this.mainPanel.BorderStyle = System.Windows.Forms.BorderStyle.None;
        this.mainPanel.Dock = System.Windows.Forms.DockStyle.Fill;
        this.mainPanel.Location = new System.Drawing.Point(0, 0);
        this.mainPanel.Name = "mainPanel";
        this.mainPanel.Size = new System.Drawing.Size(800, 650);
        this.mainPanel.TabIndex = 0;

        // Header Panel with enhanced Office2016 styling
        this.headerPanel.BorderStyle = System.Windows.Forms.BorderStyle.None;
        this.headerPanel.BackgroundColor = new Syncfusion.Drawing.BrushInfo(Color.FromArgb(52, 152, 219));
        this.headerPanel.Dock = System.Windows.Forms.DockStyle.Top;
        this.headerPanel.Location = new System.Drawing.Point(0, 0);
        this.headerPanel.Name = "headerPanel";
        this.headerPanel.Size = new System.Drawing.Size(800, 50);
        this.headerPanel.TabIndex = 1;

        // Title Label
        this.titleLabel.BackColor = System.Drawing.Color.Transparent;
        this.titleLabel.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold);
        this.titleLabel.ForeColor = System.Drawing.Color.White;
        this.titleLabel.Location = new System.Drawing.Point(20, 15);
        this.titleLabel.Name = "titleLabel";
        this.titleLabel.Size = new System.Drawing.Size(200, 25);
        this.titleLabel.TabIndex = 0;
        this.titleLabel.Text = "Bus Information";

        // Button Panel
        this.buttonPanel.BorderStyle = System.Windows.Forms.BorderStyle.None;
        this.buttonPanel.Dock = System.Windows.Forms.DockStyle.Bottom;
        this.buttonPanel.Location = new System.Drawing.Point(0, 590);
        this.buttonPanel.Name = "buttonPanel";
        this.buttonPanel.Size = new System.Drawing.Size(800, 60);
        this.buttonPanel.TabIndex = 2;
        this.buttonPanel.Padding = new System.Windows.Forms.Padding(10);

        // Content Panel
        this.contentPanel.BorderStyle = System.Windows.Forms.BorderStyle.None;
        this.contentPanel.Dock = System.Windows.Forms.DockStyle.Fill;
        this.contentPanel.Location = new System.Drawing.Point(0, 50);
        this.contentPanel.Name = "contentPanel";
        this.contentPanel.Size = new System.Drawing.Size(800, 540);
        this.contentPanel.TabIndex = 3;
        this.contentPanel.Padding = new System.Windows.Forms.Padding(20);

        // Basic Information Group Panel with Office2016 styling
        this.basicInfoGroupBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
        this.basicInfoGroupBox.Location = new System.Drawing.Point(20, 20);
        this.basicInfoGroupBox.Name = "basicInfoGroupBox";
        this.basicInfoGroupBox.Size = new System.Drawing.Size(360, 180);
        this.basicInfoGroupBox.TabIndex = 0;
        this.basicInfoGroupBox.BackgroundColor = new Syncfusion.Drawing.BrushInfo(Color.FromArgb(248, 249, 250));

        // Details Group Panel with Office2016 styling
        this.detailsGroupBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
        this.detailsGroupBox.Location = new System.Drawing.Point(400, 20);
        this.detailsGroupBox.Name = "detailsGroupBox";
        this.detailsGroupBox.Size = new System.Drawing.Size(360, 180);
        this.detailsGroupBox.TabIndex = 1;
        this.detailsGroupBox.BackgroundColor = new Syncfusion.Drawing.BrushInfo(Color.FromArgb(248, 249, 250));

        // Financial Group Panel with Office2016 styling
        this.financialGroupBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
        this.financialGroupBox.Location = new System.Drawing.Point(20, 220);
        this.financialGroupBox.Name = "financialGroupBox";
        this.financialGroupBox.Size = new System.Drawing.Size(740, 120);
        this.financialGroupBox.TabIndex = 2;
        this.financialGroupBox.BackgroundColor = new Syncfusion.Drawing.BrushInfo(Color.FromArgb(248, 249, 250));

        // Configure Basic Information Controls
        ConfigureBasicInfoControls();

        // Configure Details Controls
        ConfigureDetailsControls();

        // Configure Financial Controls
        ConfigureFinancialControls();

        // Configure action buttons with Office2016 colors
        ConfigureActionButton(this.saveButton, "Save", 590, 15, 0, Color.FromArgb(76, 175, 80));
        ConfigureActionButton(this.cancelButton, "Cancel", 700, 15, 1, Color.FromArgb(158, 158, 158));

        // Add event handlers
        this.saveButton.Click += new System.EventHandler(this.SaveButton_Click);
        this.cancelButton.Click += new System.EventHandler(this.CancelButton_Click);

        // Add controls to panels
        this.headerPanel.Controls.Add(this.titleLabel);

        this.contentPanel.Controls.Add(this.basicInfoGroupBox);
        this.contentPanel.Controls.Add(this.detailsGroupBox);
        this.contentPanel.Controls.Add(this.financialGroupBox);

        this.buttonPanel.Controls.Add(this.saveButton);
        this.buttonPanel.Controls.Add(this.cancelButton);

        this.mainPanel.Controls.Add(this.contentPanel);
        this.mainPanel.Controls.Add(this.buttonPanel);
        this.mainPanel.Controls.Add(this.headerPanel);

        // Form settings
        this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
        this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
        this.ClientSize = new System.Drawing.Size(800, 650);
        this.Controls.Add(this.mainPanel);
        this.Text = "Bus Information";
        this.MinimumSize = new System.Drawing.Size(800, 650);
        this.MaximumSize = new System.Drawing.Size(800, 650);
        this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
        this.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
        this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
        this.MaximizeBox = false;

        // Resume layout
        this.ResumeLayout(false);
    }

    private void ConfigureBasicInfoControls()
    {
        // Bus Number
        this.busNumberLabel.Font = new System.Drawing.Font("Segoe UI", 9F);
        this.busNumberLabel.Location = new System.Drawing.Point(15, 25);
        this.busNumberLabel.Size = new System.Drawing.Size(80, 20);
        this.busNumberLabel.Text = "Bus Number:";

        this.busNumberTextBox.Location = new System.Drawing.Point(105, 25);
        this.busNumberTextBox.Size = new System.Drawing.Size(120, 23);
        this.busNumberTextBox.Font = new System.Drawing.Font("Segoe UI", 9F);

        // Year
        this.yearLabel.Font = new System.Drawing.Font("Segoe UI", 9F);
        this.yearLabel.Location = new System.Drawing.Point(240, 25);
        this.yearLabel.Size = new System.Drawing.Size(40, 20);
        this.yearLabel.Text = "Year:";

        this.yearNumeric.Location = new System.Drawing.Point(285, 25);
        this.yearNumeric.Size = new System.Drawing.Size(70, 23);
        this.yearNumeric.Font = new System.Drawing.Font("Segoe UI", 9F);

        // Make
        this.makeLabel.Font = new System.Drawing.Font("Segoe UI", 9F);
        this.makeLabel.Location = new System.Drawing.Point(15, 55);
        this.makeLabel.Size = new System.Drawing.Size(40, 20);
        this.makeLabel.Text = "Make:";

        this.makeComboBox.Location = new System.Drawing.Point(105, 55);
        this.makeComboBox.Size = new System.Drawing.Size(150, 23);
        this.makeComboBox.Font = new System.Drawing.Font("Segoe UI", 9F);
        this.makeComboBox.Style = Syncfusion.Windows.Forms.VisualStyle.Office2016Colorful;

        // Model
        this.modelLabel.Font = new System.Drawing.Font("Segoe UI", 9F);
        this.modelLabel.Location = new System.Drawing.Point(15, 85);
        this.modelLabel.Size = new System.Drawing.Size(45, 20);
        this.modelLabel.Text = "Model:";

        this.modelTextBox.Location = new System.Drawing.Point(105, 85);
        this.modelTextBox.Size = new System.Drawing.Size(200, 23);
        this.modelTextBox.Font = new System.Drawing.Font("Segoe UI", 9F);

        // Seating Capacity
        this.seatingCapacityLabel.Font = new System.Drawing.Font("Segoe UI", 9F);
        this.seatingCapacityLabel.Location = new System.Drawing.Point(15, 115);
        this.seatingCapacityLabel.Size = new System.Drawing.Size(85, 20);
        this.seatingCapacityLabel.Text = "Seat Capacity:";

        this.seatingCapacityNumeric.Location = new System.Drawing.Point(105, 115);
        this.seatingCapacityNumeric.Size = new System.Drawing.Size(80, 23);
        this.seatingCapacityNumeric.Font = new System.Drawing.Font("Segoe UI", 9F);

        // Add controls to basic info group
        this.basicInfoGroupBox.Controls.Add(this.busNumberLabel);
        this.basicInfoGroupBox.Controls.Add(this.busNumberTextBox);
        this.basicInfoGroupBox.Controls.Add(this.yearLabel);
        this.basicInfoGroupBox.Controls.Add(this.yearNumeric);
        this.basicInfoGroupBox.Controls.Add(this.makeLabel);
        this.basicInfoGroupBox.Controls.Add(this.makeComboBox);
        this.basicInfoGroupBox.Controls.Add(this.modelLabel);
        this.basicInfoGroupBox.Controls.Add(this.modelTextBox);
        this.basicInfoGroupBox.Controls.Add(this.seatingCapacityLabel);
        this.basicInfoGroupBox.Controls.Add(this.seatingCapacityNumeric);
    }

    private void ConfigureDetailsControls()
    {
        // VIN Number
        this.vinLabel.Font = new System.Drawing.Font("Segoe UI", 9F);
        this.vinLabel.Location = new System.Drawing.Point(15, 25);
        this.vinLabel.Size = new System.Drawing.Size(80, 20);
        this.vinLabel.Text = "VIN Number:";

        this.vinTextBox.Location = new System.Drawing.Point(105, 25);
        this.vinTextBox.Size = new System.Drawing.Size(200, 23);
        this.vinTextBox.Font = new System.Drawing.Font("Segoe UI", 9F);

        // License Number
        this.licenseNumberLabel.Font = new System.Drawing.Font("Segoe UI", 9F);
        this.licenseNumberLabel.Location = new System.Drawing.Point(15, 55);
        this.licenseNumberLabel.Size = new System.Drawing.Size(85, 20);
        this.licenseNumberLabel.Text = "License Number:";

        this.licenseNumberTextBox.Location = new System.Drawing.Point(105, 55);
        this.licenseNumberTextBox.Size = new System.Drawing.Size(150, 23);
        this.licenseNumberTextBox.Font = new System.Drawing.Font("Segoe UI", 9F);

        // Last Inspection
        this.lastInspectionLabel.Font = new System.Drawing.Font("Segoe UI", 9F);
        this.lastInspectionLabel.Location = new System.Drawing.Point(15, 85);
        this.lastInspectionLabel.Size = new System.Drawing.Size(85, 20);
        this.lastInspectionLabel.Text = "Last Inspection:";

        this.lastInspectionDatePicker.Location = new System.Drawing.Point(105, 85);
        this.lastInspectionDatePicker.Size = new System.Drawing.Size(120, 23);
        this.lastInspectionDatePicker.Font = new System.Drawing.Font("Segoe UI", 9F);
        this.lastInspectionDatePicker.Style = Syncfusion.Windows.Forms.VisualStyle.Office2016Colorful;

        // Odometer
        this.odometerLabel.Font = new System.Drawing.Font("Segoe UI", 9F);
        this.odometerLabel.Location = new System.Drawing.Point(15, 115);
        this.odometerLabel.Size = new System.Drawing.Size(70, 20);
        this.odometerLabel.Text = "Odometer:";

        this.odometerNumeric.Location = new System.Drawing.Point(105, 115);
        this.odometerNumeric.Size = new System.Drawing.Size(100, 23);
        this.odometerNumeric.Font = new System.Drawing.Font("Segoe UI", 9F);

        // Status
        this.statusLabel.Font = new System.Drawing.Font("Segoe UI", 9F);
        this.statusLabel.Location = new System.Drawing.Point(15, 145);
        this.statusLabel.Size = new System.Drawing.Size(45, 20);
        this.statusLabel.Text = "Status:";

        this.statusComboBox.Location = new System.Drawing.Point(105, 145);
        this.statusComboBox.Size = new System.Drawing.Size(120, 23);
        this.statusComboBox.Font = new System.Drawing.Font("Segoe UI", 9F);
        this.statusComboBox.Style = Syncfusion.Windows.Forms.VisualStyle.Office2016Colorful;

        // Add controls to details group
        this.detailsGroupBox.Controls.Add(this.vinLabel);
        this.detailsGroupBox.Controls.Add(this.vinTextBox);
        this.detailsGroupBox.Controls.Add(this.licenseNumberLabel);
        this.detailsGroupBox.Controls.Add(this.licenseNumberTextBox);
        this.detailsGroupBox.Controls.Add(this.lastInspectionLabel);
        this.detailsGroupBox.Controls.Add(this.lastInspectionDatePicker);
        this.detailsGroupBox.Controls.Add(this.odometerLabel);
        this.detailsGroupBox.Controls.Add(this.odometerNumeric);
        this.detailsGroupBox.Controls.Add(this.statusLabel);
        this.detailsGroupBox.Controls.Add(this.statusComboBox);
    }

    private void ConfigureFinancialControls()
    {
        // Purchase Date
        this.purchaseDateLabel.Font = new System.Drawing.Font("Segoe UI", 9F);
        this.purchaseDateLabel.Location = new System.Drawing.Point(15, 25);
        this.purchaseDateLabel.Size = new System.Drawing.Size(90, 20);
        this.purchaseDateLabel.Text = "Purchase Date:";

        this.purchaseDatePicker.Location = new System.Drawing.Point(115, 25);
        this.purchaseDatePicker.Size = new System.Drawing.Size(120, 23);
        this.purchaseDatePicker.Font = new System.Drawing.Font("Segoe UI", 9F);
        this.purchaseDatePicker.Style = Syncfusion.Windows.Forms.VisualStyle.Office2016Colorful;

        // Purchase Price
        this.purchasePriceLabel.Font = new System.Drawing.Font("Segoe UI", 9F);
        this.purchasePriceLabel.Location = new System.Drawing.Point(255, 25);
        this.purchasePriceLabel.Size = new System.Drawing.Size(90, 20);
        this.purchasePriceLabel.Text = "Purchase Price:";

        this.purchasePriceNumeric.Location = new System.Drawing.Point(355, 25);
        this.purchasePriceNumeric.Size = new System.Drawing.Size(120, 23);
        this.purchasePriceNumeric.Font = new System.Drawing.Font("Segoe UI", 9F);

        // Insurance Policy
        this.insurancePolicyLabel.Font = new System.Drawing.Font("Segoe UI", 9F);
        this.insurancePolicyLabel.Location = new System.Drawing.Point(15, 55);
        this.insurancePolicyLabel.Size = new System.Drawing.Size(95, 20);
        this.insurancePolicyLabel.Text = "Insurance Policy:";

        this.insurancePolicyTextBox.Location = new System.Drawing.Point(115, 55);
        this.insurancePolicyTextBox.Size = new System.Drawing.Size(200, 23);
        this.insurancePolicyTextBox.Font = new System.Drawing.Font("Segoe UI", 9F);

        // Insurance Expiry
        this.insuranceExpiryLabel.Font = new System.Drawing.Font("Segoe UI", 9F);
        this.insuranceExpiryLabel.Location = new System.Drawing.Point(335, 55);
        this.insuranceExpiryLabel.Size = new System.Drawing.Size(100, 20);
        this.insuranceExpiryLabel.Text = "Insurance Expiry:";

        this.insuranceExpiryDatePicker.Location = new System.Drawing.Point(445, 55);
        this.insuranceExpiryDatePicker.Size = new System.Drawing.Size(120, 23);
        this.insuranceExpiryDatePicker.Font = new System.Drawing.Font("Segoe UI", 9F);
        this.insuranceExpiryDatePicker.Style = Syncfusion.Windows.Forms.VisualStyle.Office2016Colorful;

        // Add controls to financial group
        this.financialGroupBox.Controls.Add(this.purchaseDateLabel);
        this.financialGroupBox.Controls.Add(this.purchaseDatePicker);
        this.financialGroupBox.Controls.Add(this.purchasePriceLabel);
        this.financialGroupBox.Controls.Add(this.purchasePriceNumeric);
        this.financialGroupBox.Controls.Add(this.insurancePolicyLabel);
        this.financialGroupBox.Controls.Add(this.insurancePolicyTextBox);
        this.financialGroupBox.Controls.Add(this.insuranceExpiryLabel);
        this.financialGroupBox.Controls.Add(this.insuranceExpiryDatePicker);
    }

    private void ConfigureActionButton(SfButton button, string title, int x, int y, int tabIndex, Color color)
    {
        button.AccessibleName = title;
        button.BackColor = color;
        button.ForeColor = Color.White;
        button.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
        button.Location = new Point(x, y);
        button.Name = title.Replace(" ", "").ToLower() + "Button";
        button.Size = new Size(90, 35);
        button.TabIndex = tabIndex;
        button.Text = title;
        button.UseVisualStyleBackColor = false;

        // Add hover effects for better user experience
        button.MouseEnter += (s, e) =>
        {
            if (button.Enabled)
                button.BackColor = Color.FromArgb(Math.Min(255, color.R + 20),
                                                Math.Min(255, color.G + 20),
                                                Math.Min(255, color.B + 20));
        };
        button.MouseLeave += (s, e) =>
        {
            if (button.Enabled)
                button.BackColor = color;
        };
    }

    #endregion
}
