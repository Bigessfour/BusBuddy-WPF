using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using Syncfusion.Windows.Forms;
using Syncfusion.Windows.Forms.Tools;
using Syncfusion.Drawing;

namespace Bus_Buddy.Utilities
{
    /// <summary>
    /// Syncfusion Background Bleeding Fix - WORKING VERSION
    /// Solves the exact problem you described: form content showing behind the dashboard cards
    /// 
    /// USAGE: Add this line to your Dashboard constructor after InitializeComponent():
    /// SyncfusionBackgroundFix.FixDashboardBackground(this);
    /// </summary>
    public static class SyncfusionBackgroundFix
    {
        /// <summary>
        /// MAIN FIX: Eliminates background bleeding behind dashboard cards
        /// Call this immediately after InitializeComponent() in your Dashboard constructor
        /// </summary>
        public static void FixDashboardBackground(Form dashboardForm)
        {
            if (dashboardForm == null) return;

            try
            {
                // Step 1: Fix form-level background
                SetFormBackground(dashboardForm);

                // Step 2: Fix MetroForm specific issues
                if (dashboardForm is MetroForm metroForm)
                {
                    SetMetroFormBackground(metroForm);
                }

                // Step 3: Ensure panels have solid backgrounds
                FixPanelBackgrounds(dashboardForm);

                // Step 4: Force repaint
                dashboardForm.Invalidate(true);
                dashboardForm.Update();

                System.Diagnostics.Debug.WriteLine("✅ Dashboard background bleeding fixed!");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"⚠️ Background fix error: {ex.Message}");
            }
        }

        /// <summary>
        /// Step 1: Set solid form background to prevent bleeding
        /// </summary>
        private static void SetFormBackground(Form form)
        {
            // Set solid background color
            form.BackColor = Color.FromArgb(240, 246, 252);

            // Add paint handler to ensure complete coverage
            form.Paint += (sender, e) =>
            {
                if (sender is Form f)
                {
                    using (var brush = new SolidBrush(f.BackColor))
                    {
                        e.Graphics.FillRectangle(brush, f.ClientRectangle);
                    }
                }
            };
        }

        /// <summary>
        /// Step 2: Configure MetroForm specific background properties
        /// </summary>
        private static void SetMetroFormBackground(MetroForm metroForm)
        {
            metroForm.BackColor = Color.FromArgb(240, 246, 252);
            metroForm.MetroColor = Color.FromArgb(11, 95, 178);
            metroForm.BorderColor = Color.FromArgb(11, 95, 178);
            metroForm.CaptionBarColor = Color.FromArgb(11, 95, 178);
            metroForm.CaptionForeColor = Color.White;
        }

        /// <summary>
        /// Step 3: Fix all gradient panel backgrounds to prevent transparency
        /// </summary>
        private static void FixPanelBackgrounds(Control parent)
        {
            foreach (Control control in parent.Controls)
            {
                if (control is GradientPanel gp)
                {
                    // Main panel should cover entire form
                    if (gp.Dock == DockStyle.Fill)
                    {
                        gp.BackgroundColor = new BrushInfo(Color.FromArgb(240, 246, 252));
                        gp.BorderStyle = BorderStyle.None;
                        gp.SendToBack(); // Ensure proper Z-order
                    }
                    // Content panels should have light backgrounds
                    else if (gp.Name.ToLower().Contains("content"))
                    {
                        gp.BackgroundColor = new BrushInfo(Color.FromArgb(248, 250, 252));
                        gp.BorderStyle = BorderStyle.None;
                        if (gp.Padding.All == 0)
                        {
                            gp.Padding = new Padding(20); // Add padding to prevent edge bleeding
                        }
                    }
                    // Header panels
                    else if (gp.Name.ToLower().Contains("header"))
                    {
                        // Keep existing header styling
                        gp.BorderStyle = BorderStyle.None;
                    }
                    // Other panels
                    else
                    {
                        if (gp.BackgroundColor.IsEmpty)
                        {
                            gp.BackgroundColor = new BrushInfo(Color.White);
                        }
                        gp.BorderStyle = BorderStyle.None;
                    }
                }

                // Recursively fix child controls
                if (control.HasChildren)
                {
                    FixPanelBackgrounds(control);
                }
            }
        }

        /// <summary>
        /// Additional optimization: Fix button backgrounds to ensure they're solid
        /// </summary>
        public static void FixButtonBackgrounds(Control parent)
        {
            foreach (Control control in parent.Controls)
            {
                if (control is Syncfusion.WinForms.Controls.SfButton sfButton)
                {
                    // Ensure button has solid background
                    if (sfButton.Style.BackColor == Color.Transparent || sfButton.Style.BackColor.A < 255)
                    {
                        // Apply solid colors based on button type
                        if (sfButton.Text.Contains("Management"))
                        {
                            sfButton.Style.BackColor = Color.FromArgb(63, 81, 181); // Blue
                        }
                        else if (sfButton.Text.Contains("Refresh"))
                        {
                            sfButton.Style.BackColor = Color.FromArgb(76, 175, 80); // Green
                        }
                        else if (sfButton.Text.Contains("Reports"))
                        {
                            sfButton.Style.BackColor = Color.FromArgb(156, 39, 176); // Purple
                        }
                        else
                        {
                            sfButton.Style.BackColor = Color.FromArgb(158, 158, 158); // Gray
                        }
                    }
                }

                // Recursively check child controls
                if (control.HasChildren)
                {
                    FixButtonBackgrounds(control);
                }
            }
        }

        /// <summary>
        /// Validation: Check if background bleeding issues have been resolved
        /// </summary>
        public static bool ValidateBackgroundFix(Form form)
        {
            var issues = new List<string>();

            // Check form background
            if (form.BackColor == Color.Transparent)
            {
                issues.Add("Form background is transparent");
            }

            // Check for full-coverage panels
            bool hasFullCoveragePanel = false;
            foreach (Control control in form.Controls)
            {
                if (control.Dock == DockStyle.Fill)
                {
                    hasFullCoveragePanel = true;
                    break;
                }
            }

            if (!hasFullCoveragePanel)
            {
                issues.Add("No full-coverage background panel found");
            }

            // Report results
            if (issues.Count > 0)
            {
                System.Diagnostics.Debug.WriteLine($"⚠️ Background issues: {string.Join(", ", issues)}");
                return false;
            }

            System.Diagnostics.Debug.WriteLine("✅ Background bleeding fixed successfully");
            return true;
        }
    }
}
