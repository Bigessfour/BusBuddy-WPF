namespace Bus_Buddy.Forms
{
    partial class TicketEditForm
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
            this.lblStudent = new System.Windows.Forms.Label();
            this.cmbStudent = new System.Windows.Forms.ComboBox();
            this.lblRoute = new System.Windows.Forms.Label();
            this.cmbRoute = new System.Windows.Forms.ComboBox();
            this.lblTravelDate = new System.Windows.Forms.Label();
            this.dtpTravelDate = new System.Windows.Forms.DateTimePicker();
            this.lblIssuedDate = new System.Windows.Forms.Label();
            this.dtpIssuedDate = new System.Windows.Forms.DateTimePicker();
            this.lblTicketType = new System.Windows.Forms.Label();
            this.cmbTicketType = new System.Windows.Forms.ComboBox();
            this.lblPrice = new System.Windows.Forms.Label();
            this.numPrice = new System.Windows.Forms.NumericUpDown();
            this.lblStatus = new System.Windows.Forms.Label();
            this.cmbStatus = new System.Windows.Forms.ComboBox();
            this.lblPaymentMethod = new System.Windows.Forms.Label();
            this.cmbPaymentMethod = new System.Windows.Forms.ComboBox();
            this.lblQRCode = new System.Windows.Forms.Label();
            this.txtQRCode = new System.Windows.Forms.TextBox();
            this.btnGenerateQR = new System.Windows.Forms.Button();
            this.lblNotes = new System.Windows.Forms.Label();
            this.txtNotes = new System.Windows.Forms.TextBox();
            this.lblValidFrom = new System.Windows.Forms.Label();
            this.dtpValidFrom = new System.Windows.Forms.DateTimePicker();
            this.lblValidUntil = new System.Windows.Forms.Label();
            this.dtpValidUntil = new System.Windows.Forms.DateTimePicker();
            this.chkIsRefundable = new System.Windows.Forms.CheckBox();
            this.lblRefundAmount = new System.Windows.Forms.Label();
            this.numRefundAmount = new System.Windows.Forms.NumericUpDown();
            this.lblUsedDate = new System.Windows.Forms.Label();
            this.dtpUsedDate = new System.Windows.Forms.DateTimePicker();
            this.lblUsedByDriver = new System.Windows.Forms.Label();
            this.txtUsedByDriver = new System.Windows.Forms.TextBox();
            this.btnSave = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.numPrice)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numRefundAmount)).BeginInit();
            this.SuspendLayout();
            // 
            // lblStudent
            // 
            this.lblStudent.AutoSize = true;
            this.lblStudent.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.lblStudent.Location = new System.Drawing.Point(30, 30);
            this.lblStudent.Name = "lblStudent";
            this.lblStudent.Size = new System.Drawing.Size(55, 15);
            this.lblStudent.TabIndex = 0;
            this.lblStudent.Text = "Student:";
            // 
            // cmbStudent
            // 
            this.cmbStudent.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbStudent.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.cmbStudent.FormattingEnabled = true;
            this.cmbStudent.Location = new System.Drawing.Point(30, 50);
            this.cmbStudent.Name = "cmbStudent";
            this.cmbStudent.Size = new System.Drawing.Size(520, 25);
            this.cmbStudent.TabIndex = 1;
            // 
            // lblRoute
            // 
            this.lblRoute.AutoSize = true;
            this.lblRoute.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.lblRoute.Location = new System.Drawing.Point(30, 90);
            this.lblRoute.Name = "lblRoute";
            this.lblRoute.Size = new System.Drawing.Size(42, 15);
            this.lblRoute.TabIndex = 2;
            this.lblRoute.Text = "Route:";
            // 
            // cmbRoute
            // 
            this.cmbRoute.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbRoute.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.cmbRoute.FormattingEnabled = true;
            this.cmbRoute.Location = new System.Drawing.Point(30, 110);
            this.cmbRoute.Name = "cmbRoute";
            this.cmbRoute.Size = new System.Drawing.Size(520, 25);
            this.cmbRoute.TabIndex = 3;
            // 
            // lblTravelDate
            // 
            this.lblTravelDate.AutoSize = true;
            this.lblTravelDate.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.lblTravelDate.Location = new System.Drawing.Point(30, 150);
            this.lblTravelDate.Name = "lblTravelDate";
            this.lblTravelDate.Size = new System.Drawing.Size(76, 15);
            this.lblTravelDate.TabIndex = 4;
            this.lblTravelDate.Text = "Travel Date:";
            // 
            // dtpTravelDate
            // 
            this.dtpTravelDate.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.dtpTravelDate.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpTravelDate.Location = new System.Drawing.Point(30, 170);
            this.dtpTravelDate.Name = "dtpTravelDate";
            this.dtpTravelDate.Size = new System.Drawing.Size(250, 25);
            this.dtpTravelDate.TabIndex = 5;
            // 
            // lblIssuedDate
            // 
            this.lblIssuedDate.AutoSize = true;
            this.lblIssuedDate.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.lblIssuedDate.Location = new System.Drawing.Point(300, 150);
            this.lblIssuedDate.Name = "lblIssuedDate";
            this.lblIssuedDate.Size = new System.Drawing.Size(76, 15);
            this.lblIssuedDate.TabIndex = 6;
            this.lblIssuedDate.Text = "Issued Date:";
            // 
            // dtpIssuedDate
            // 
            this.dtpIssuedDate.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.dtpIssuedDate.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpIssuedDate.CustomFormat = "MM/dd/yyyy HH:mm";
            this.dtpIssuedDate.Location = new System.Drawing.Point(300, 170);
            this.dtpIssuedDate.Name = "dtpIssuedDate";
            this.dtpIssuedDate.Size = new System.Drawing.Size(250, 25);
            this.dtpIssuedDate.TabIndex = 7;
            // 
            // lblTicketType
            // 
            this.lblTicketType.AutoSize = true;
            this.lblTicketType.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.lblTicketType.Location = new System.Drawing.Point(30, 210);
            this.lblTicketType.Name = "lblTicketType";
            this.lblTicketType.Size = new System.Drawing.Size(77, 15);
            this.lblTicketType.TabIndex = 8;
            this.lblTicketType.Text = "Ticket Type:";
            // 
            // cmbTicketType
            // 
            this.cmbTicketType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbTicketType.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.cmbTicketType.FormattingEnabled = true;
            this.cmbTicketType.Location = new System.Drawing.Point(30, 230);
            this.cmbTicketType.Name = "cmbTicketType";
            this.cmbTicketType.Size = new System.Drawing.Size(250, 25);
            this.cmbTicketType.TabIndex = 9;
            this.cmbTicketType.SelectedIndexChanged += new System.EventHandler(this.CmbTicketType_SelectedIndexChanged);
            // 
            // lblPrice
            // 
            this.lblPrice.AutoSize = true;
            this.lblPrice.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.lblPrice.Location = new System.Drawing.Point(300, 210);
            this.lblPrice.Name = "lblPrice";
            this.lblPrice.Size = new System.Drawing.Size(38, 15);
            this.lblPrice.TabIndex = 10;
            this.lblPrice.Text = "Price:";
            // 
            // numPrice
            // 
            this.numPrice.DecimalPlaces = 2;
            this.numPrice.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.numPrice.Location = new System.Drawing.Point(300, 230);
            this.numPrice.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.numPrice.Name = "numPrice";
            this.numPrice.Size = new System.Drawing.Size(250, 25);
            this.numPrice.TabIndex = 11;
            // 
            // lblStatus
            // 
            this.lblStatus.AutoSize = true;
            this.lblStatus.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.lblStatus.Location = new System.Drawing.Point(30, 270);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(45, 15);
            this.lblStatus.TabIndex = 12;
            this.lblStatus.Text = "Status:";
            // 
            // cmbStatus
            // 
            this.cmbStatus.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbStatus.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.cmbStatus.FormattingEnabled = true;
            this.cmbStatus.Location = new System.Drawing.Point(30, 290);
            this.cmbStatus.Name = "cmbStatus";
            this.cmbStatus.Size = new System.Drawing.Size(250, 25);
            this.cmbStatus.TabIndex = 13;
            this.cmbStatus.SelectedIndexChanged += new System.EventHandler(this.CmbStatus_SelectedIndexChanged);
            // 
            // lblPaymentMethod
            // 
            this.lblPaymentMethod.AutoSize = true;
            this.lblPaymentMethod.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.lblPaymentMethod.Location = new System.Drawing.Point(300, 270);
            this.lblPaymentMethod.Name = "lblPaymentMethod";
            this.lblPaymentMethod.Size = new System.Drawing.Size(110, 15);
            this.lblPaymentMethod.TabIndex = 14;
            this.lblPaymentMethod.Text = "Payment Method:";
            // 
            // cmbPaymentMethod
            // 
            this.cmbPaymentMethod.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbPaymentMethod.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.cmbPaymentMethod.FormattingEnabled = true;
            this.cmbPaymentMethod.Location = new System.Drawing.Point(300, 290);
            this.cmbPaymentMethod.Name = "cmbPaymentMethod";
            this.cmbPaymentMethod.Size = new System.Drawing.Size(250, 25);
            this.cmbPaymentMethod.TabIndex = 15;
            // 
            // lblQRCode
            // 
            this.lblQRCode.AutoSize = true;
            this.lblQRCode.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.lblQRCode.Location = new System.Drawing.Point(30, 330);
            this.lblQRCode.Name = "lblQRCode";
            this.lblQRCode.Size = new System.Drawing.Size(59, 15);
            this.lblQRCode.TabIndex = 16;
            this.lblQRCode.Text = "QR Code:";
            // 
            // txtQRCode
            // 
            this.txtQRCode.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.txtQRCode.Location = new System.Drawing.Point(30, 350);
            this.txtQRCode.Name = "txtQRCode";
            this.txtQRCode.Size = new System.Drawing.Size(400, 25);
            this.txtQRCode.TabIndex = 17;
            // 
            // btnGenerateQR
            // 
            this.btnGenerateQR.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.btnGenerateQR.Location = new System.Drawing.Point(440, 350);
            this.btnGenerateQR.Name = "btnGenerateQR";
            this.btnGenerateQR.Size = new System.Drawing.Size(110, 25);
            this.btnGenerateQR.TabIndex = 18;
            this.btnGenerateQR.Text = "Generate QR";
            this.btnGenerateQR.UseVisualStyleBackColor = true;
            this.btnGenerateQR.Click += new System.EventHandler(this.BtnGenerateQR_Click);
            // 
            // lblNotes
            // 
            this.lblNotes.AutoSize = true;
            this.lblNotes.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.lblNotes.Location = new System.Drawing.Point(30, 390);
            this.lblNotes.Name = "lblNotes";
            this.lblNotes.Size = new System.Drawing.Size(43, 15);
            this.lblNotes.TabIndex = 19;
            this.lblNotes.Text = "Notes:";
            // 
            // txtNotes
            // 
            this.txtNotes.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.txtNotes.Location = new System.Drawing.Point(30, 410);
            this.txtNotes.Multiline = true;
            this.txtNotes.Name = "txtNotes";
            this.txtNotes.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtNotes.Size = new System.Drawing.Size(520, 60);
            this.txtNotes.TabIndex = 20;
            // 
            // lblValidFrom
            // 
            this.lblValidFrom.AutoSize = true;
            this.lblValidFrom.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.lblValidFrom.Location = new System.Drawing.Point(30, 480);
            this.lblValidFrom.Name = "lblValidFrom";
            this.lblValidFrom.Size = new System.Drawing.Size(74, 15);
            this.lblValidFrom.TabIndex = 21;
            this.lblValidFrom.Text = "Valid From:";
            // 
            // dtpValidFrom
            // 
            this.dtpValidFrom.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.dtpValidFrom.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpValidFrom.Location = new System.Drawing.Point(30, 500);
            this.dtpValidFrom.Name = "dtpValidFrom";
            this.dtpValidFrom.Size = new System.Drawing.Size(250, 25);
            this.dtpValidFrom.TabIndex = 22;
            // 
            // lblValidUntil
            // 
            this.lblValidUntil.AutoSize = true;
            this.lblValidUntil.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.lblValidUntil.Location = new System.Drawing.Point(300, 480);
            this.lblValidUntil.Name = "lblValidUntil";
            this.lblValidUntil.Size = new System.Drawing.Size(71, 15);
            this.lblValidUntil.TabIndex = 23;
            this.lblValidUntil.Text = "Valid Until:";
            // 
            // dtpValidUntil
            // 
            this.dtpValidUntil.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.dtpValidUntil.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpValidUntil.Location = new System.Drawing.Point(300, 500);
            this.dtpValidUntil.Name = "dtpValidUntil";
            this.dtpValidUntil.Size = new System.Drawing.Size(250, 25);
            this.dtpValidUntil.TabIndex = 24;
            // 
            // chkIsRefundable
            // 
            this.chkIsRefundable.AutoSize = true;
            this.chkIsRefundable.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.chkIsRefundable.Location = new System.Drawing.Point(30, 540);
            this.chkIsRefundable.Name = "chkIsRefundable";
            this.chkIsRefundable.Size = new System.Drawing.Size(95, 19);
            this.chkIsRefundable.TabIndex = 25;
            this.chkIsRefundable.Text = "Refundable";
            this.chkIsRefundable.UseVisualStyleBackColor = true;
            this.chkIsRefundable.CheckedChanged += new System.EventHandler(this.ChkIsRefundable_CheckedChanged);
            // 
            // lblRefundAmount
            // 
            this.lblRefundAmount.AutoSize = true;
            this.lblRefundAmount.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.lblRefundAmount.Location = new System.Drawing.Point(300, 540);
            this.lblRefundAmount.Name = "lblRefundAmount";
            this.lblRefundAmount.Size = new System.Drawing.Size(103, 15);
            this.lblRefundAmount.TabIndex = 26;
            this.lblRefundAmount.Text = "Refund Amount:";
            // 
            // numRefundAmount
            // 
            this.numRefundAmount.DecimalPlaces = 2;
            this.numRefundAmount.Enabled = false;
            this.numRefundAmount.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.numRefundAmount.Location = new System.Drawing.Point(300, 560);
            this.numRefundAmount.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.numRefundAmount.Name = "numRefundAmount";
            this.numRefundAmount.Size = new System.Drawing.Size(250, 25);
            this.numRefundAmount.TabIndex = 27;
            // 
            // lblUsedDate
            // 
            this.lblUsedDate.AutoSize = true;
            this.lblUsedDate.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.lblUsedDate.Location = new System.Drawing.Point(30, 590);
            this.lblUsedDate.Name = "lblUsedDate";
            this.lblUsedDate.Size = new System.Drawing.Size(69, 15);
            this.lblUsedDate.TabIndex = 28;
            this.lblUsedDate.Text = "Used Date:";
            // 
            // dtpUsedDate
            // 
            this.dtpUsedDate.Enabled = false;
            this.dtpUsedDate.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.dtpUsedDate.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpUsedDate.CustomFormat = "MM/dd/yyyy HH:mm";
            this.dtpUsedDate.Location = new System.Drawing.Point(30, 610);
            this.dtpUsedDate.Name = "dtpUsedDate";
            this.dtpUsedDate.Size = new System.Drawing.Size(250, 25);
            this.dtpUsedDate.TabIndex = 29;
            // 
            // lblUsedByDriver
            // 
            this.lblUsedByDriver.AutoSize = true;
            this.lblUsedByDriver.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.lblUsedByDriver.Location = new System.Drawing.Point(300, 590);
            this.lblUsedByDriver.Name = "lblUsedByDriver";
            this.lblUsedByDriver.Size = new System.Drawing.Size(93, 15);
            this.lblUsedByDriver.TabIndex = 30;
            this.lblUsedByDriver.Text = "Used By Driver:";
            // 
            // txtUsedByDriver
            // 
            this.txtUsedByDriver.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.txtUsedByDriver.Location = new System.Drawing.Point(300, 610);
            this.txtUsedByDriver.Name = "txtUsedByDriver";
            this.txtUsedByDriver.Size = new System.Drawing.Size(250, 25);
            this.txtUsedByDriver.TabIndex = 31;
            // 
            // btnSave
            // 
            this.btnSave.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(46)))), ((int)(((byte)(204)))), ((int)(((byte)(113)))));
            this.btnSave.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSave.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnSave.ForeColor = System.Drawing.Color.White;
            this.btnSave.Location = new System.Drawing.Point(350, 650);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(100, 35);
            this.btnSave.TabIndex = 32;
            this.btnSave.Text = "Save";
            this.btnSave.UseVisualStyleBackColor = false;
            this.btnSave.Click += new System.EventHandler(this.BtnSave_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(231)))), ((int)(((byte)(76)))), ((int)(((byte)(60)))));
            this.btnCancel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCancel.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnCancel.ForeColor = System.Drawing.Color.White;
            this.btnCancel.Location = new System.Drawing.Point(460, 650);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(100, 35);
            this.btnCancel.TabIndex = 33;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = false;
            this.btnCancel.Click += new System.EventHandler(this.BtnCancel_Click);
            // 
            // TicketEditForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(584, 701);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.txtUsedByDriver);
            this.Controls.Add(this.lblUsedByDriver);
            this.Controls.Add(this.dtpUsedDate);
            this.Controls.Add(this.lblUsedDate);
            this.Controls.Add(this.numRefundAmount);
            this.Controls.Add(this.lblRefundAmount);
            this.Controls.Add(this.chkIsRefundable);
            this.Controls.Add(this.dtpValidUntil);
            this.Controls.Add(this.lblValidUntil);
            this.Controls.Add(this.dtpValidFrom);
            this.Controls.Add(this.lblValidFrom);
            this.Controls.Add(this.txtNotes);
            this.Controls.Add(this.lblNotes);
            this.Controls.Add(this.btnGenerateQR);
            this.Controls.Add(this.txtQRCode);
            this.Controls.Add(this.lblQRCode);
            this.Controls.Add(this.cmbPaymentMethod);
            this.Controls.Add(this.lblPaymentMethod);
            this.Controls.Add(this.cmbStatus);
            this.Controls.Add(this.lblStatus);
            this.Controls.Add(this.numPrice);
            this.Controls.Add(this.lblPrice);
            this.Controls.Add(this.cmbTicketType);
            this.Controls.Add(this.lblTicketType);
            this.Controls.Add(this.dtpIssuedDate);
            this.Controls.Add(this.lblIssuedDate);
            this.Controls.Add(this.dtpTravelDate);
            this.Controls.Add(this.lblTravelDate);
            this.Controls.Add(this.cmbRoute);
            this.Controls.Add(this.lblRoute);
            this.Controls.Add(this.cmbStudent);
            this.Controls.Add(this.lblStudent);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "TicketEditForm";
            this.Text = "Ticket Editor";
            ((System.ComponentModel.ISupportInitialize)(this.numPrice)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numRefundAmount)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblStudent;
        private System.Windows.Forms.ComboBox cmbStudent;
        private System.Windows.Forms.Label lblRoute;
        private System.Windows.Forms.ComboBox cmbRoute;
        private System.Windows.Forms.Label lblTravelDate;
        private System.Windows.Forms.DateTimePicker dtpTravelDate;
        private System.Windows.Forms.Label lblIssuedDate;
        private System.Windows.Forms.DateTimePicker dtpIssuedDate;
        private System.Windows.Forms.Label lblTicketType;
        private System.Windows.Forms.ComboBox cmbTicketType;
        private System.Windows.Forms.Label lblPrice;
        private System.Windows.Forms.NumericUpDown numPrice;
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.ComboBox cmbStatus;
        private System.Windows.Forms.Label lblPaymentMethod;
        private System.Windows.Forms.ComboBox cmbPaymentMethod;
        private System.Windows.Forms.Label lblQRCode;
        private System.Windows.Forms.TextBox txtQRCode;
        private System.Windows.Forms.Button btnGenerateQR;
        private System.Windows.Forms.Label lblNotes;
        private System.Windows.Forms.TextBox txtNotes;
        private System.Windows.Forms.Label lblValidFrom;
        private System.Windows.Forms.DateTimePicker dtpValidFrom;
        private System.Windows.Forms.Label lblValidUntil;
        private System.Windows.Forms.DateTimePicker dtpValidUntil;
        private System.Windows.Forms.CheckBox chkIsRefundable;
        private System.Windows.Forms.Label lblRefundAmount;
        private System.Windows.Forms.NumericUpDown numRefundAmount;
        private System.Windows.Forms.Label lblUsedDate;
        private System.Windows.Forms.DateTimePicker dtpUsedDate;
        private System.Windows.Forms.Label lblUsedByDriver;
        private System.Windows.Forms.TextBox txtUsedByDriver;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button btnCancel;
    }
}
