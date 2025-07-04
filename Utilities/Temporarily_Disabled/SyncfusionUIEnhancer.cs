using System;
using System.Drawing;
using System.Windows.Forms;
using Syncfusion.Windows.Forms;
using Syncfusion.Windows.Forms.Tools;
using Syncfusion.WinForms.Controls;
using Syncfusion.Drawing;

namespace Bus_Buddy.Utilities
{
    /// <summary>
    /// Enhanced Syncfusion UI configuration for Bus Buddy v30.1.37
    /// Implements latest Syncfusion best practices for professional UI appearance
    /// 
    /// ENHANCEMENTS:
    /// - Office2016Colorful theme with custom color schemes
    /// - Proper DPI awareness and scaling
    /// - Enhanced docking manager configuration
    /// - Consistent spacing and padding standards
    /// - Performance-optimized rendering
    /// </summary>
    public static class SyncfusionUIEnhancer
    {
        #region Theme Constants

        public static readonly Color PrimaryBlue = Color.FromArgb(0, 120, 215);
        public static readonly Color SecondaryBlue = Color.FromArgb(240, 246, 252);
        public static readonly Color AccentOrange = Color.FromArgb(255, 140, 0);
        public static readonly Color TextDark = Color.FromArgb(32, 31, 30);
        public static readonly Color TextLight = Color.FromArgb(96, 94, 92);
        public static readonly Color BackgroundLight = Color.FromArgb(250, 249, 248);
        public static readonly Color BorderGray = Color.FromArgb(225, 223, 221);

        // Consistent spacing standards
        public const int StandardPadding = 8;
        public const int LargePadding = 16;
        public const int ButtonHeight = 32;
        public const int ButtonMinWidth = 100;
        public const int ControlSpacing = 12;

        #endregion

        #region Enhanced Theme Application

        /// <summary>
        /// Applies the latest Office2016Colorful theme with Bus Buddy branding
        /// </summary>
        public static void ApplyOffice2016Theme(Form form)
        {
            if (form == null) return;

            try
            {
                // Apply Office2016Colorful theme using SkinManager
                SkinManager.SetVisualStyle(form, "Office2016Colorful");
                
                // Configure MetroForm if applicable
                if (form is MetroForm metroForm)
                {
                    ApplyMetroFormEnhancements(metroForm);
                }

                // Apply to all child controls recursively
                ApplyThemeToControls(form);
            }
            catch (Exception ex)
            {
                // Fallback to default styling if theme application fails
                System.Diagnostics.Debug.WriteLine($"Theme application failed: {ex.Message}");
            }
        }

        /// <summary>
        /// Enhanced MetroForm configuration with modern styling
        /// </summary>
        private static void ApplyMetroFormEnhancements(MetroForm metroForm)
        {
            metroForm.MetroColor = PrimaryBlue;
            metroForm.CaptionBarColor = PrimaryBlue;
            metroForm.CaptionForeColor = Color.White;
            metroForm.CaptionFont = new Font("Segoe UI", 9F, FontStyle.Regular);
            
            // Enhanced border and shadow
            metroForm.BorderColor = BorderGray;
            metroForm.BorderThickness = 1;
            metroForm.DropShadow = true;
            
            // Modern window behavior - use standard properties
            metroForm.MaximizeBox = true;
            metroForm.MinimizeBox = true;
        }

        #endregion

        #region DPI and Scaling Enhancements

        /// <summary>
        /// Configures proper DPI awareness for high-resolution displays
        /// </summary>
        public static void ConfigureDPIAwareness(Form form)
        {
            if (form == null) return;

            // Enable high DPI support
            form.AutoScaleMode = AutoScaleMode.Dpi;
            form.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
            
            // Calculate DPI scaling factor
            using (var g = form.CreateGraphics())
            {
                var dpiX = g.DpiX;
                var scaleFactor = dpiX / 96.0f; // 96 DPI is standard
                
                if (scaleFactor > 1.0f)
                {
                    // Adjust minimum sizes for high DPI
                    var minSize = form.MinimumSize;
                    form.MinimumSize = new Size(
                        (int)(minSize.Width * scaleFactor),
                        (int)(minSize.Height * scaleFactor)
                    );
                }
            }
        }

        #endregion

        #region Enhanced Button Styling

        /// <summary>
        /// Applies consistent button styling throughout the application
        /// </summary>
        public static void EnhanceButtonStyling(Control container)
        {
            foreach (Control control in container.Controls)
            {
                if (control is SfButton sfButton)
                {
                    ApplySfButtonEnhancements(sfButton);
                }
                else if (control is Button regularButton)
                {
                    ApplyRegularButtonEnhancements(regularButton);
                }
                
                // Recursively apply to child containers
                if (control.HasChildren)
                {
                    EnhanceButtonStyling(control);
                }
            }
        }

        private static void ApplySfButtonEnhancements(SfButton button)
        {
            // Consistent sizing
            button.MinimumSize = new Size(ButtonMinWidth, ButtonHeight);
            button.Padding = new Padding(StandardPadding, 6, StandardPadding, 6);
            
            // Modern styling
            button.Style.BackColor = PrimaryBlue;
            button.Style.ForeColor = Color.White;
            button.Style.HoverBackColor = Color.FromArgb(16, 110, 190);
            button.Style.PressedBackColor = Color.FromArgb(0, 90, 158);
            button.Style.BorderColor = PrimaryBlue;
            
            // Typography
            button.Font = new Font("Segoe UI", 9F, FontStyle.Regular);
            button.Style.TextAlign = ContentAlignment.MiddleCenter;
        }

        private static void ApplyRegularButtonEnhancements(Button button)
        {
            button.MinimumSize = new Size(ButtonMinWidth, ButtonHeight);
            button.Padding = new Padding(StandardPadding, 0, StandardPadding, 0);
            button.BackColor = PrimaryBlue;
            button.ForeColor = Color.White;
            button.FlatStyle = FlatStyle.Flat;
            button.FlatAppearance.BorderSize = 0;
            button.Font = new Font("Segoe UI", 9F, FontStyle.Regular);
            button.UseVisualStyleBackColor = false;
        }

        #endregion

        #region Enhanced Docking Manager Configuration

        /// <summary>
        /// Configures DockingManager with modern appearance and behavior
        /// </summary>
        public static void EnhanceDockingManager(DockingManager dockingManager)
        {
            if (dockingManager == null) return;

            // Modern docking behavior
            dockingManager.DockBehavior = DockBehavior.VS2010;
            dockingManager.VisualStyle = VisualStyle.Office2016Colorful;
            
            // Enhanced appearance
            dockingManager.CaptionFont = new Font("Segoe UI", 8.25F, FontStyle.Regular);
            dockingManager.DockTabFont = new Font("Segoe UI", 8.25F, FontStyle.Regular);
            
            // Performance optimizations
            dockingManager.EnableAutoAdjustCaption = true;
            dockingManager.AnimateAutoHiddenWindow = true;
            dockingManager.AutoHideActiveControl = true;
            
            // Modern colors
            dockingManager.ActiveCaptionBackground = new BrushInfo(PrimaryBlue);
            dockingManager.ActiveCaptionFont = new Font("Segoe UI", 8.25F, FontStyle.Regular);
            dockingManager.InActiveCaptionBackground = new BrushInfo(BackgroundLight);
        }

        #endregion

        #region Data Grid Enhancements

        /// <summary>
        /// Applies enhanced styling to SfDataGrid controls
        /// </summary>
        public static void EnhanceDataGridStyling(Control container)
        {
            foreach (Control control in container.Controls)
            {
                if (control is Syncfusion.WinForms.DataGrid.SfDataGrid dataGrid)
                {
                    ApplyDataGridEnhancements(dataGrid);
                }
                
                if (control.HasChildren)
                {
                    EnhanceDataGridStyling(control);
                }
            }
        }

        private static void ApplyDataGridEnhancements(Syncfusion.WinForms.DataGrid.SfDataGrid dataGrid)
        {
            // Modern grid appearance
            dataGrid.Style.BorderColor = BorderGray;
            dataGrid.Style.BorderStyle = BorderStyle.FixedSingle;
            
            // Header styling
            dataGrid.Style.HeaderStyle.BackColor = SecondaryBlue;
            dataGrid.Style.HeaderStyle.ForeColor = TextDark;
            dataGrid.Style.HeaderStyle.Font = new Font("Segoe UI", 9F, FontStyle.Medium);
            dataGrid.Style.HeaderStyle.BorderColor = BorderGray;
            
            // Row styling
            dataGrid.Style.RowStyle.BackColor = Color.White;
            dataGrid.Style.RowStyle.ForeColor = TextDark;
            dataGrid.Style.RowStyle.Font = new Font("Segoe UI", 9F, FontStyle.Regular);
            dataGrid.Style.AlternatingRowStyle.BackColor = BackgroundLight;
            
            // Selection styling
            dataGrid.Style.SelectionStyle.BackColor = Color.FromArgb(51, 153, 255);
            dataGrid.Style.SelectionStyle.ForeColor = Color.White;
            
            // Performance optimizations
            dataGrid.AllowResizingColumns = true;
            dataGrid.AllowSorting = true;
            dataGrid.ShowRowHeader = false;
        }

        #endregion

        #region Panel and Container Enhancements

        /// <summary>
        /// Enhances GradientPanel and other container controls
        /// </summary>
        public static void EnhancePanelStyling(Control container)
        {
            foreach (Control control in container.Controls)
            {
                if (control is GradientPanel gradientPanel)
                {
                    ApplyGradientPanelEnhancements(gradientPanel);
                }
                else if (control is Panel regularPanel)
                {
                    ApplyRegularPanelEnhancements(regularPanel);
                }
                
                if (control.HasChildren)
                {
                    EnhancePanelStyling(control);
                }
            }
        }

        private static void ApplyGradientPanelEnhancements(GradientPanel panel)
        {
            // Subtle gradient for modern appearance
            panel.BackgroundColor = new BrushInfo(GradientStyle.Vertical, 
                BackgroundLight, 
                Color.FromArgb(245, 244, 243));
            
            panel.BorderStyle = BorderStyle.None;
            panel.Padding = new Padding(StandardPadding);
        }

        private static void ApplyRegularPanelEnhancements(Panel panel)
        {
            panel.BackColor = BackgroundLight;
            panel.Padding = new Padding(StandardPadding);
        }

        #endregion

        #region Recursive Theme Application

        private static void ApplyThemeToControls(Control container)
        {
            foreach (Control control in container.Controls)
            {
                // Apply specific enhancements based on control type
                switch (control)
                {
                    case DockingManager dockingManager:
                        EnhanceDockingManager(dockingManager);
                        break;
                }
                
                // Recursively apply to children
                if (control.HasChildren)
                {
                    ApplyThemeToControls(control);
                }
            }
        }

        #endregion

        #region Font Management

        /// <summary>
        /// Applies consistent typography throughout the application
        /// </summary>
        public static void ApplyTypographyStandards(Control container)
        {
            var baseFont = new Font("Segoe UI", 9F, FontStyle.Regular);
            var headerFont = new Font("Segoe UI", 12F, FontStyle.Medium);
            var captionFont = new Font("Segoe UI", 11F, FontStyle.Regular);

            ApplyFontRecursively(container, baseFont, headerFont, captionFont);
        }

        private static void ApplyFontRecursively(Control control, Font baseFont, Font headerFont, Font captionFont)
        {
            // Apply appropriate font based on control type and purpose
            if (control.Name.ToLower().Contains("header") || control.Name.ToLower().Contains("title"))
            {
                control.Font = headerFont;
            }
            else if (control.Name.ToLower().Contains("caption") || control.Name.ToLower().Contains("label"))
            {
                control.Font = captionFont;
            }
            else
            {
                control.Font = baseFont;
            }

            // Recursively apply to children
            foreach (Control child in control.Controls)
            {
                ApplyFontRecursively(child, baseFont, headerFont, captionFont);
            }
        }

        #endregion
    }
}
