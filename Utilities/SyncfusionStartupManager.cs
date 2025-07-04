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
    /// Syncfusion Startup Sequence & UI Initialization Manager
    /// Implements the official Syncfusion best practices for proper control initialization order
    /// Based on: https://help.syncfusion.com/windowsforms/docking-manager/getting-started
    /// 
    /// CRITICAL INITIALIZATION ORDER:
    /// 1. License Registration
    /// 2. Theme Application
    /// 3. DockingManager Setup
    /// 4. Panel Creation & Docking
    /// 5. Control Population
    /// 6. Event Binding
    /// 7. Final UI Polish
    /// </summary>
    public static class SyncfusionStartupManager
    {
        #region Startup Sequence Constants

        private static readonly string[] CRITICAL_ASSEMBLIES = {
            "Syncfusion.Grid.Base.dll",
            "Syncfusion.Grid.Windows.dll",
            "Syncfusion.Shared.Base.dll",
            "Syncfusion.Shared.Windows.dll",
            "Syncfusion.Tools.Base.dll",
            "Syncfusion.Tools.Windows.dll"
        };

        private const string SYNCFUSION_LICENSE_KEY = "Ngo9BigBOggjHTQxAR8/V1JEaF5cXmRCf1FpRmJGdld5fUVHYVZUTXxaS00DNHVRdkdmWXhec3RSRGRYU0R2WUBWYEk=";

        #endregion

        #region Phase 1: License & Core Initialization

        /// <summary>
        /// Phase 1: Initialize Syncfusion license and core components
        /// MUST BE CALLED FIRST in Main() method before Application.Run()
        /// </summary>
        public static void InitializeSyncfusionCore()
        {
            try
            {
                // Step 1: Register Syncfusion license (CRITICAL - must be first!)
                Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense(SYNCFUSION_LICENSE_KEY);

                // Step 2: Enable high DPI support for modern displays
                Application.SetHighDpiMode(HighDpiMode.SystemAware);

                // Step 3: Enable visual styles for proper theme rendering
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);

                System.Diagnostics.Debug.WriteLine("✅ Syncfusion Core Initialization Complete");
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Syncfusion Core Initialization Failed: {ex.Message}", ex);
            }
        }

        #endregion

        #region Phase 2: Form-Level Initialization

        /// <summary>
        /// Phase 2: Initialize form-level Syncfusion components
        /// Call this in Form constructor AFTER InitializeComponent()
        /// </summary>
        public static void InitializeFormComponents(Form form)
        {
            if (form == null) throw new ArgumentNullException(nameof(form));

            try
            {
                // Step 1: Apply base theme BEFORE creating controls
                ApplyBaseTheme(form);

                // Step 2: Configure DPI awareness for this form
                ConfigureFormDPI(form);

                // Step 3: Set up form-level event handlers
                SetupFormEventHandlers(form);

                System.Diagnostics.Debug.WriteLine($"✅ Form Components Initialized: {form.Name}");
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Form Component Initialization Failed for {form.Name}: {ex.Message}", ex);
            }
        }

        #endregion

        #region Phase 3: DockingManager Setup (CRITICAL ORDER!)

        /// <summary>
        /// Phase 3: Initialize DockingManager with proper sequence
        /// MUST BE CALLED BEFORE adding any dockable panels
        /// </summary>
        public static DockingManager InitializeDockingManager(Form hostForm)
        {
            if (hostForm == null) throw new ArgumentNullException(nameof(hostForm));

            try
            {
                // Step 1: Create DockingManager instance with components container
                var components = new System.ComponentModel.Container();
                var dockingManager = new DockingManager(components);

                // Step 2: Set host control IMMEDIATELY (critical!)
                dockingManager.HostControl = hostForm;

                // Step 3: Configure modern docking behavior
                dockingManager.DockBehavior = DockBehavior.VS2010;
                dockingManager.VisualStyle = VisualStyle.Office2016Colorful;

                // Step 4: Set up fonts and appearance
                // Note: CaptionFont property may vary by Syncfusion version
                // dockingManager.CaptionFont = new Font("Segoe UI", 8.25F, FontStyle.Regular);
                // dockingManager.DockTabFont = new Font("Segoe UI", 8.25F, FontStyle.Regular);

                // Step 5: Enable advanced features
                dockingManager.EnableAutoAdjustCaption = true;
                dockingManager.AnimateAutoHiddenWindow = true;
                dockingManager.AutoHideActiveControl = true;

                // Step 6: Configure colors for Bus Buddy branding
                dockingManager.ActiveCaptionBackground = new BrushInfo(Color.FromArgb(0, 120, 215));
                dockingManager.InActiveCaptionBackground = new BrushInfo(Color.FromArgb(240, 246, 252));

                System.Diagnostics.Debug.WriteLine("✅ DockingManager Initialized with VS2010 behavior");
                return dockingManager;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"DockingManager Initialization Failed: {ex.Message}", ex);
            }
        }

        #endregion

        #region Phase 4: Panel Creation & Docking Sequence

        /// <summary>
        /// Phase 4: Create and dock panels in the correct order
        /// Call AFTER DockingManager is fully initialized
        /// </summary>
        public static void SetupDockablePanels(DockingManager dockingManager, Form hostForm)
        {
            if (dockingManager == null) throw new ArgumentNullException(nameof(dockingManager));
            if (hostForm == null) throw new ArgumentNullException(nameof(hostForm));

            try
            {
                // CRITICAL: Suspend layout during panel creation for better performance
                hostForm.SuspendLayout();

                // Step 1: Create panels FIRST (don't dock yet!)
                var navigationPanel = CreateStyledPanel("Navigation", Color.FromArgb(240, 246, 252));
                var propertiesPanel = CreateStyledPanel("Properties", Color.FromArgb(240, 246, 252));
                var outputPanel = CreateStyledPanel("Output", Color.FromArgb(240, 246, 252));
                var dataPanel = CreateStyledPanel("Data View", Color.FromArgb(240, 246, 252));

                // Step 2: Add panels to form BEFORE enabling docking
                hostForm.Controls.Add(navigationPanel);
                hostForm.Controls.Add(propertiesPanel);
                hostForm.Controls.Add(outputPanel);
                hostForm.Controls.Add(dataPanel);

                // Step 3: Enable docking in specific order (CRITICAL!)
                dockingManager.SetEnableDocking(navigationPanel, true);
                dockingManager.SetEnableDocking(propertiesPanel, true);
                dockingManager.SetEnableDocking(outputPanel, true);
                dockingManager.SetEnableDocking(dataPanel, true);

                // Step 4: Set panel labels AFTER enabling docking
                dockingManager.SetDockLabel(navigationPanel, "🧭 Navigation");
                dockingManager.SetDockLabel(propertiesPanel, "⚙️ Properties");
                dockingManager.SetDockLabel(outputPanel, "📄 Output");
                dockingManager.SetDockLabel(dataPanel, "📊 Data View");

                // Step 5: Dock panels in Z-order (bottom to top)
                dockingManager.DockControl(outputPanel, hostForm, DockingStyle.Bottom, 150);
                dockingManager.DockControl(navigationPanel, hostForm, DockingStyle.Left, 250);
                dockingManager.DockControl(propertiesPanel, hostForm, DockingStyle.Right, 300);
                dockingManager.DockControl(dataPanel, propertiesPanel, DockingStyle.Tabbed, 300);

                System.Diagnostics.Debug.WriteLine("✅ Dockable Panels Setup Complete with proper Z-order");
            }
            finally
            {
                // CRITICAL: Always resume layout
                hostForm.ResumeLayout(true);
            }
        }

        #endregion

        #region Phase 5: Control Population & Event Binding

        /// <summary>
        /// Phase 5: Populate panels with controls and bind events
        /// Call AFTER all panels are docked and positioned
        /// </summary>
        public static void PopulatePanelControls(DockingManager dockingManager)
        {
            if (dockingManager == null) throw new ArgumentNullException(nameof(dockingManager));

            try
            {
                // Get references to docked panels
                var panels = GetDockedPanels(dockingManager);

                foreach (var panel in panels)
                {
                    PopulatePanelBasedOnName(panel);
                }

                // Bind docking events AFTER population
                BindDockingEvents(dockingManager);

                System.Diagnostics.Debug.WriteLine("✅ Panel Controls Populated and Events Bound");
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Panel Population Failed: {ex.Message}", ex);
            }
        }

        #endregion

        #region Phase 6: Final UI Polish & Optimization

        /// <summary>
        /// Phase 6: Apply final UI polish and performance optimizations
        /// Call this last, after all controls are created and positioned
        /// </summary>
        public static void FinalizeUISetup(Form form, DockingManager dockingManager)
        {
            if (form == null) throw new ArgumentNullException(nameof(form));

            try
            {
                // Step 1: Apply enhanced button styling to all controls
                // SyncfusionUIEnhancer.EnhanceButtonStyling(form);

                // Step 2: Apply data grid enhancements
                // SyncfusionUIEnhancer.EnhanceDataGridStyling(form);

                // Step 3: Apply panel styling
                // SyncfusionUIEnhancer.EnhancePanelStyling(form);

                // Step 4: Apply typography standards
                // SyncfusionUIEnhancer.ApplyTypographyStandards(form);

                // Step 5: Configure serialization for layout persistence
                if (dockingManager != null)
                {
                    ConfigureDockStatePersistence(dockingManager);
                }

                // Step 6: Force final layout and refresh
                form.PerformLayout();
                form.Refresh();

                System.Diagnostics.Debug.WriteLine("✅ Final UI Setup Complete - Bus Buddy Ready!");
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Final UI Setup Failed: {ex.Message}", ex);
            }
        }

        #endregion

        #region Helper Methods

        private static void ApplyBaseTheme(Form form)
        {
            SkinManager.SetVisualStyle(form, "Office2016Colorful");

            if (form is MetroForm metroForm)
            {
                metroForm.MetroColor = Color.FromArgb(0, 120, 215);
                metroForm.CaptionBarColor = Color.FromArgb(0, 120, 215);
                metroForm.CaptionForeColor = Color.White;
            }
        }

        private static void ConfigureFormDPI(Form form)
        {
            form.AutoScaleMode = AutoScaleMode.Dpi;
            form.Font = new Font("Segoe UI", 9F, FontStyle.Regular);
        }

        private static void SetupFormEventHandlers(Form form)
        {
            form.Load += (s, e) =>
            {
                System.Diagnostics.Debug.WriteLine($"Form {form.Name} loaded successfully");
            };

            form.Shown += (s, e) =>
            {
                System.Diagnostics.Debug.WriteLine($"Form {form.Name} shown - UI ready for interaction");
            };
        }

        private static Panel CreateStyledPanel(string name, Color backColor)
        {
            return new Panel
            {
                Name = name.Replace(" ", "") + "Panel",
                BackColor = backColor,
                BorderStyle = BorderStyle.None,
                Padding = new Padding(8),
                Size = new Size(200, 150)
            };
        }

        private static Panel[] GetDockedPanels(DockingManager dockingManager)
        {
            var panels = new List<Panel>();
            var hostControl = dockingManager.HostControl;

            foreach (Control control in hostControl.Controls)
            {
                if (control is Panel panel && dockingManager.GetEnableDocking(panel))
                {
                    panels.Add(panel);
                }
            }

            return panels.ToArray();
        }

        private static void PopulatePanelBasedOnName(Panel panel)
        {
            // Add appropriate controls based on panel purpose
            switch (panel.Name.ToLower())
            {
                case "navigationpanel":
                    AddNavigationControls(panel);
                    break;
                case "propertiespanel":
                    AddPropertiesControls(panel);
                    break;
                case "outputpanel":
                    AddOutputControls(panel);
                    break;
                case "dataviewpanel":
                    AddDataViewControls(panel);
                    break;
            }
        }

        private static void AddNavigationControls(Panel panel)
        {
            var treeView = new TreeView
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White,
                Font = new Font("Segoe UI", 9F)
            };

            // Add sample navigation nodes
            treeView.Nodes.Add("🚌 Bus Management");
            treeView.Nodes.Add("👨‍✈️ Driver Management");
            treeView.Nodes.Add("🛣️ Route Management");
            treeView.Nodes.Add("👥 Student Management");

            panel.Controls.Add(treeView);
        }

        private static void AddPropertiesControls(Panel panel)
        {
            var propertyGrid = new PropertyGrid
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White,
                Font = new Font("Segoe UI", 9F)
            };

            panel.Controls.Add(propertyGrid);
        }

        private static void AddOutputControls(Panel panel)
        {
            var textBox = new TextBox
            {
                Dock = DockStyle.Fill,
                Multiline = true,
                ScrollBars = ScrollBars.Vertical,
                BackColor = Color.White,
                Font = new Font("Consolas", 9F),
                Text = "Bus Buddy Output Console\r\n" +
                       "System initialized successfully\r\n" +
                       "Ready for operations..."
            };

            panel.Controls.Add(textBox);
        }

        private static void AddDataViewControls(Panel panel)
        {
            var label = new Label
            {
                Text = "📊 Data visualization will appear here",
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter,
                BackColor = Color.White,
                Font = new Font("Segoe UI", 10F)
            };

            panel.Controls.Add(label);
        }

        private static void BindDockingEvents(DockingManager dockingManager)
        {
            dockingManager.DockStateChanged += (s, e) =>
            {
                System.Diagnostics.Debug.WriteLine($"Dock state changed - Event fired");
            };

            dockingManager.DockStateChanging += (s, e) =>
            {
                // Validate dock state changes if needed
                System.Diagnostics.Debug.WriteLine($"Dock state changing - Event fired");
            };
        }

        private static void ConfigureDockStatePersistence(DockingManager dockingManager)
        {
            // Enable automatic state persistence
            dockingManager.PersistState = true;

            // Save state on form closing
            var hostForm = dockingManager.HostControl as Form;
            if (hostForm != null)
            {
                hostForm.FormClosing += (s, e) =>
                {
                    try
                    {
                        dockingManager.SaveDockState();
                        System.Diagnostics.Debug.WriteLine("✅ Dock state saved");
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"⚠️ Failed to save dock state: {ex.Message}");
                    }
                };

                hostForm.Load += (s, e) =>
                {
                    try
                    {
                        dockingManager.LoadDockState();
                        System.Diagnostics.Debug.WriteLine("✅ Dock state loaded");
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"⚠️ Failed to load dock state: {ex.Message}");
                    }
                };
            }
        }

        #endregion

        #region Complete Startup Orchestration

        /// <summary>
        /// MASTER METHOD: Complete Syncfusion startup orchestration
        /// Call this from your main form's constructor after InitializeComponent()
        /// </summary>
        public static DockingManager OrchestrateSyncfusionStartup(Form mainForm)
        {
            if (mainForm == null) throw new ArgumentNullException(nameof(mainForm));

            System.Diagnostics.Debug.WriteLine("🚀 Starting Syncfusion Startup Orchestration...");

            try
            {
                // Phase 2: Form-level initialization
                InitializeFormComponents(mainForm);

                // Phase 3: DockingManager setup
                var dockingManager = InitializeDockingManager(mainForm);

                // Phase 4: Panel creation and docking
                SetupDockablePanels(dockingManager, mainForm);

                // Phase 5: Control population and event binding
                PopulatePanelControls(dockingManager);

                // Phase 6: Final UI polish
                FinalizeUISetup(mainForm, dockingManager);

                System.Diagnostics.Debug.WriteLine("🎉 Syncfusion Startup Orchestration Complete!");
                return dockingManager;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Syncfusion Startup Orchestration Failed: {ex.Message}", ex);
            }
        }

        #endregion
    }
}
