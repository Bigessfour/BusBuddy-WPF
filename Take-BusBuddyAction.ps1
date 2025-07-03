#Requires -Version 7.0

<#
.SYNOPSIS
    Action script for implementing TODOs and incomplete methods found by Scan-BusBuddyTodos.ps1

.DESCRIPTION
    This PowerShell 7.x script helps implement and resolve TODOs and incomplete methods:
    - Generates Syncfusion form templates
    - Creates service implementations
    - Provides implementation suggestions
    - Tracks progress and updates

.PARAMETER ScanResultsFile
    Path to JSON scan results file from Scan-BusBuddyTodos.ps1

.PARAMETER ActionType
    Type of action: GenerateForms, GenerateServices, ShowSuggestions, TrackProgress

.PARAMETER Priority
    Filter by priority: High, Medium, Low, All

.PARAMETER ComponentName
    Specific component name to work on

.EXAMPLE
    .\Take-BusBuddyAction.ps1 -ScanResultsFile ".\BusBuddy_TODO_Scan_20250703_143022.json" -ActionType GenerateForms -Priority High

.EXAMPLE
    .\Take-BusBuddyAction.ps1 -ActionType ShowSuggestions -Priority All

.NOTES
    Author: BusBuddy Development Team
    Version: 1.0
    Date: July 3, 2025
    PowerShell Version: 7.0+
    Requires: Scan-BusBuddyTodos.ps1 results
#>

[CmdletBinding()]
param(
    [Parameter(Mandatory = $false)]
    [string]$ScanResultsFile = "",

    [Parameter(Mandatory = $true)]
    [ValidateSet("GenerateForms", "GenerateServices", "ShowSuggestions", "TrackProgress", "CreateWorkItems")]
    [string]$ActionType,

    [Parameter(Mandatory = $false)]
    [ValidateSet("High", "Medium", "Low", "All")]
    [string]$Priority = "All",

    [Parameter(Mandatory = $false)]
    [string]$ComponentName = "",

    [Parameter(Mandatory = $false)]
    [string]$ProjectPath = (Get-Location).Path
)

# Initialize variables
$script:ScanResults = $null
$script:ActionResults = @{
    ActionsPerformed = @()
    FilesGenerated = @()
    Recommendations = @()
    WorkItems = @()
}

# Color scheme
$Colors = @{
    Header = "Cyan"
    Success = "Green"
    Warning = "Yellow"
    Error = "Red"
    Info = "Blue"
    Action = "Magenta"
}

function Write-ColorOutput {
    param([string]$Message, [string]$Color = "White")
    Write-Host $Message -ForegroundColor $Color
}

function Show-Header {
    $headerText = @"
╔══════════════════════════════════════════════════════════════════════════════╗
║                  BusBuddy TODO Action & Implementation Tool                  ║
║                           PowerShell 7.x Version                            ║
║                              July 3, 2025                                   ║
╚══════════════════════════════════════════════════════════════════════════════╝
"@
    Write-ColorOutput $headerText $Colors.Header
    Write-Host ""
}

function Load-ScanResults {
    param([string]$FilePath)

    if ([string]::IsNullOrEmpty($FilePath)) {
        # Find the most recent scan results file
        $scanFiles = Get-ChildItem -Path $ProjectPath -Filter "BusBuddy_TODO_Scan_*.json" | Sort-Object LastWriteTime -Descending
        if ($scanFiles.Count -eq 0) {
            Write-ColorOutput "⚠️  No scan results found. Please run Scan-BusBuddyTodos.ps1 first." $Colors.Warning
            return $false
        }
        $FilePath = $scanFiles[0].FullName
        Write-ColorOutput "📄 Using most recent scan results: $($scanFiles[0].Name)" $Colors.Info
    }

    if (-not (Test-Path $FilePath)) {
        Write-ColorOutput "❌ Scan results file not found: $FilePath" $Colors.Error
        return $false
    }

    try {
        $script:ScanResults = Get-Content $FilePath -Raw | ConvertFrom-Json
        Write-ColorOutput "✅ Loaded scan results from: $FilePath" $Colors.Success
        Write-ColorOutput "📊 Found $($script:ScanResults.TodoItems.Count) TODOs, $($script:ScanResults.IncompleteItems.Count) incomplete items, $($script:ScanResults.MissingComponents.Count) missing components" $Colors.Info
        return $true
    }
    catch {
        Write-ColorOutput "❌ Error loading scan results: $($_.Exception.Message)" $Colors.Error
        return $false
    }
}

function Generate-SyncfusionForm {
    param(
        [string]$FormName,
        [string]$Purpose,
        [string]$Priority
    )

    $fileName = "$FormName.cs"
    $filePath = Join-Path $ProjectPath "Forms" $fileName

    if (Test-Path $filePath) {
        Write-ColorOutput "⚠️  Form already exists: $fileName" $Colors.Warning
        return
    }

    $formTemplate = @"
using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Syncfusion.Windows.Forms;
using Syncfusion.Windows.Forms.Grid;
using Syncfusion.Windows.Forms.Tools;
using Syncfusion.WinForms.DataGrid;
using Syncfusion.WinForms.DataGrid.Enums;
using Syncfusion.WinForms.Input;
using Bus_Buddy.Data.Interfaces;
using Bus_Buddy.Models;
using Bus_Buddy.Utilities;

namespace Bus_Buddy.Forms
{
    /// <summary>
    /// $Purpose management form using Syncfusion v30.1.37 components
    /// Generated by BusBuddy TODO Action Tool - Priority: $Priority
    /// </summary>
    public partial class $FormName : MetroForm
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<$FormName> _logger;
        private SfDataGrid dataGrid;
        private SfButton addButton, editButton, deleteButton, refreshButton;
        private SfTextBox searchTextBox;
        private AutoLabel titleLabel, searchLabel;
        private GradientPanel mainPanel, buttonPanel;

        public $FormName(IServiceProvider serviceProvider, ILogger<$FormName> logger)
        {
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            InitializeComponent();
            SetupForm();
            SetupControls();
            LoadData();

            _logger.LogInformation("$FormName initialized successfully");
        }

        private void InitializeComponent()
        {
            SuspendLayout();

            // Form properties
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1000, 700);
            MinimumSize = new Size(800, 600);
            Name = "$FormName";
            Text = "$Purpose Management";
            StartPosition = FormStartPosition.CenterParent;

            ResumeLayout(false);
        }

        private void SetupForm()
        {
            // Apply Syncfusion visual enhancements
            VisualEnhancementManager.ApplyEnhancedTheme(this);
            VisualEnhancementManager.EnableHighQualityFontRendering(this);

            // Configure MetroForm properties
            MetroColor = Color.FromArgb(67, 126, 231);
            CaptionBarColor = Color.FromArgb(31, 31, 31);
            CaptionForeColor = Color.White;
            BorderColor = Color.FromArgb(67, 126, 231);

            _logger.LogDebug("Form visual enhancements applied");
        }

        private void SetupControls()
        {
            try
            {
                // Create main panel
                mainPanel = new GradientPanel()
                {
                    Dock = DockStyle.Fill,
                    BackgroundColor = new Syncfusion.Drawing.BrushInfo(Color.FromArgb(250, 250, 250))
                };

                // Create title label
                titleLabel = new AutoLabel()
                {
                    Text = "$Purpose Management",
                    Font = new Font("Segoe UI", 16F, FontStyle.Bold),
                    ForeColor = Color.FromArgb(31, 31, 31),
                    Anchor = AnchorStyles.Top | AnchorStyles.Left,
                    Location = new Point(20, 20),
                    Size = new Size(300, 30)
                };

                // Create search controls
                searchLabel = new AutoLabel()
                {
                    Text = "Search:",
                    Font = new Font("Segoe UI", 10F),
                    ForeColor = Color.FromArgb(31, 31, 31),
                    Location = new Point(20, 70),
                    Size = new Size(60, 25)
                };

                searchTextBox = new SfTextBox()
                {
                    Location = new Point(90, 68),
                    Size = new Size(250, 25),
                    Font = new Font("Segoe UI", 10F),
                    Watermark = "Search $Purpose..."
                };
                searchTextBox.TextChanged += SearchTextBox_TextChanged;

                // Create data grid
                dataGrid = new SfDataGrid()
                {
                    Location = new Point(20, 110),
                    Size = new Size(940, 480),
                    Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right,
                    AllowEditing = false,
                    AllowSorting = true,
                    AllowFiltering = true,
                    ShowGroupDropArea = true,
                    AutoGenerateColumns = false,
                    SelectionMode = GridSelectionMode.Single,
                    GridLinesVisibility = GridLinesVisibility.Both
                };

                // Apply enhanced grid visuals
                VisualEnhancementManager.ApplyEnhancedGridVisuals(dataGrid);

                // Setup columns (customize based on your entity)
                SetupDataGridColumns();

                // Create button panel
                buttonPanel = new GradientPanel()
                {
                    Location = new Point(20, 610),
                    Size = new Size(940, 50),
                    Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right,
                    BackgroundColor = new Syncfusion.Drawing.BrushInfo(Color.FromArgb(245, 245, 245))
                };

                // Create action buttons
                CreateActionButtons();

                // Add controls to form
                mainPanel.Controls.AddRange(new Control[] {
                    titleLabel, searchLabel, searchTextBox, dataGrid, buttonPanel
                });

                Controls.Add(mainPanel);

                _logger.LogDebug("Controls setup completed");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error setting up controls");
                throw;
            }
        }

        private void SetupDataGridColumns()
        {
            // TODO: Customize columns based on your specific entity
            // Example columns - replace with actual entity properties
            dataGrid.Columns.Add(new GridTextColumn()
            {
                MappingName = "Id",
                HeaderText = "ID",
                Width = 80,
                TextAlignment = GoodAlignment.Center
            });

            dataGrid.Columns.Add(new GridTextColumn()
            {
                MappingName = "Name",
                HeaderText = "Name",
                Width = 200
            });

            dataGrid.Columns.Add(new GridTextColumn()
            {
                MappingName = "Description",
                HeaderText = "Description",
                Width = 300
            });

            dataGrid.Columns.Add(new GridDateTimeColumn()
            {
                MappingName = "CreatedDate",
                HeaderText = "Created Date",
                Width = 150,
                Format = "yyyy-MM-dd HH:mm"
            });
        }

        private void CreateActionButtons()
        {
            // Add button
            addButton = new SfButton()
            {
                Text = "Add New",
                Size = new Size(100, 35),
                Location = new Point(10, 8),
                Style = Syncfusion.WinForms.Controls.Styles.Office2016Colorful
            };
            addButton.Click += AddButton_Click;
            VisualEnhancementManager.ApplyEnhancedButtonStyling(addButton);

            // Edit button
            editButton = new SfButton()
            {
                Text = "Edit",
                Size = new Size(80, 35),
                Location = new Point(120, 8),
                Style = Syncfusion.WinForms.Controls.Styles.Office2016Colorful
            };
            editButton.Click += EditButton_Click;
            VisualEnhancementManager.ApplyEnhancedButtonStyling(editButton);

            // Delete button
            deleteButton = new SfButton()
            {
                Text = "Delete",
                Size = new Size(80, 35),
                Location = new Point(210, 8),
                Style = Syncfusion.WinForms.Controls.Styles.Office2016Colorful,
                BackColor = Color.FromArgb(231, 76, 60),
                ForeColor = Color.White
            };
            deleteButton.Click += DeleteButton_Click;
            VisualEnhancementManager.ApplyEnhancedButtonStyling(deleteButton);

            // Refresh button
            refreshButton = new SfButton()
            {
                Text = "Refresh",
                Size = new Size(80, 35),
                Location = new Point(300, 8),
                Style = Syncfusion.WinForms.Controls.Styles.Office2016Colorful
            };
            refreshButton.Click += RefreshButton_Click;
            VisualEnhancementManager.ApplyEnhancedButtonStyling(refreshButton);

            buttonPanel.Controls.AddRange(new Control[] {
                addButton, editButton, deleteButton, refreshButton
            });
        }

        private void LoadData()
        {
            try
            {
                // TODO: Implement actual data loading
                // Replace this with your actual service call
                var sampleData = GenerateSampleData();
                dataGrid.DataSource = sampleData;

                _logger.LogInformation("Data loaded successfully - {Count} items", sampleData.Count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading data");
                MessageBoxAdv.Show(this,
                    "Error loading data. Please check the logs for details.",
                    "$Purpose Management",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private System.Collections.Generic.List<dynamic> GenerateSampleData()
        {
            // TODO: Replace with actual data service implementation
            return new System.Collections.Generic.List<dynamic>
            {
                new { Id = 1, Name = "Sample Item 1", Description = "Description 1", CreatedDate = DateTime.Now.AddDays(-5) },
                new { Id = 2, Name = "Sample Item 2", Description = "Description 2", CreatedDate = DateTime.Now.AddDays(-3) },
                new { Id = 3, Name = "Sample Item 3", Description = "Description 3", CreatedDate = DateTime.Now.AddDays(-1) }
            };
        }

        private void SearchTextBox_TextChanged(object sender, EventArgs e)
        {
            try
            {
                // TODO: Implement search functionality
                var searchText = searchTextBox.Text?.Trim();

                if (string.IsNullOrEmpty(searchText))
                {
                    dataGrid.View.Filter = null;
                }
                else
                {
                    // Example filter - customize based on your data
                    dataGrid.View.Filter = (item) =>
                    {
                        dynamic record = item;
                        return record.Name?.ToString().ToLower().Contains(searchText.ToLower()) == true ||
                               record.Description?.ToString().ToLower().Contains(searchText.ToLower()) == true;
                    };
                }

                dataGrid.View.RefreshFilter();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error applying search filter");
            }
        }

        private void AddButton_Click(object sender, EventArgs e)
        {
            try
            {
                // TODO: Implement add functionality
                // Example: Open edit form for new item
                _logger.LogInformation("Add new $Purpose requested");
                MessageBoxAdv.Show(this,
                    "Add new $Purpose functionality will be implemented here.",
                    "$Purpose Management",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in add operation");
            }
        }

        private void EditButton_Click(object sender, EventArgs e)
        {
            try
            {
                if (dataGrid.SelectedItem == null)
                {
                    MessageBoxAdv.Show(this,
                        "Please select an item to edit.",
                        "$Purpose Management",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                    return;
                }

                // TODO: Implement edit functionality
                _logger.LogInformation("Edit $Purpose requested");
                MessageBoxAdv.Show(this,
                    "Edit $Purpose functionality will be implemented here.",
                    "$Purpose Management",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in edit operation");
            }
        }

        private void DeleteButton_Click(object sender, EventArgs e)
        {
            try
            {
                if (dataGrid.SelectedItem == null)
                {
                    MessageBoxAdv.Show(this,
                        "Please select an item to delete.",
                        "$Purpose Management",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                    return;
                }

                var result = MessageBoxAdv.Show(this,
                    "Are you sure you want to delete the selected $Purpose?",
                    "Confirm Delete",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    // TODO: Implement delete functionality
                    _logger.LogInformation("Delete $Purpose confirmed");
                    MessageBoxAdv.Show(this,
                        "Delete $Purpose functionality will be implemented here.",
                        "$Purpose Management",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in delete operation");
            }
        }

        private void RefreshButton_Click(object sender, EventArgs e)
        {
            try
            {
                LoadData();
                searchTextBox.Text = string.Empty;
                _logger.LogInformation("Data refreshed");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error refreshing data");
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                // Dispose Syncfusion components
                dataGrid?.Dispose();
                addButton?.Dispose();
                editButton?.Dispose();
                deleteButton?.Dispose();
                refreshButton?.Dispose();
                searchTextBox?.Dispose();
                titleLabel?.Dispose();
                searchLabel?.Dispose();
                mainPanel?.Dispose();
                buttonPanel?.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
"@

    try {
        # Ensure Forms directory exists
        $formsDir = Join-Path $ProjectPath "Forms"
        if (-not (Test-Path $formsDir)) {
            New-Item -Path $formsDir -ItemType Directory -Force | Out-Null
        }

        $formTemplate | Out-File $filePath -Encoding UTF8

        $script:ActionResults.FilesGenerated += $filePath
        $script:ActionResults.ActionsPerformed += "Generated Syncfusion form: $FormName"

        Write-ColorOutput "✅ Generated form: $fileName" $Colors.Success
        Write-ColorOutput "📍 Location: $filePath" $Colors.Info

        # Add service registration recommendation
        $script:ActionResults.Recommendations += "Register $FormName in ServiceContainer.cs using: services.AddTransient<$FormName>();"

    }
    catch {
        Write-ColorOutput "❌ Error generating form $FormName`: $($_.Exception.Message)" $Colors.Error
    }
}

function Show-Suggestions {
    Write-ColorOutput "`n💡 IMPLEMENTATION SUGGESTIONS:" $Colors.Action
    Write-ColorOutput "═══════════════════════════════════════════════════════════════" $Colors.Action

    if ($script:ScanResults.TodoItems.Count -gt 0) {
        Write-ColorOutput "`n📝 TODO ITEMS SUGGESTIONS:" $Colors.Info

        $prioritizedTodos = $script:ScanResults.TodoItems | Where-Object {
            $Priority -eq "All" -or $_.Priority -eq $Priority
        } | Sort-Object Priority, Category

        foreach ($todo in $prioritizedTodos) {
            $suggestion = Get-TodoSuggestion -Todo $todo
            Write-Host "  [$($todo.Priority)] $($todo.File):$($todo.LineNumber)" -ForegroundColor Yellow
            Write-Host "     💡 $suggestion" -ForegroundColor Gray
            Write-Host ""
        }
    }

    if ($script:ScanResults.MissingComponents.Count -gt 0) {
        Write-ColorOutput "`n❌ MISSING COMPONENTS SUGGESTIONS:" $Colors.Info

        $prioritizedMissing = $script:ScanResults.MissingComponents | Where-Object {
            $Priority -eq "All" -or $_.Priority -eq $Priority
        } | Sort-Object Priority

        foreach ($missing in $prioritizedMissing) {
            Write-Host "  [$($missing.Priority)] $($missing.ComponentName) ($($missing.Type))" -ForegroundColor Red

            switch ($missing.Type) {
                "MISSING_FORM" {
                    Write-Host "     💡 Run: .\Take-BusBuddyAction.ps1 -ActionType GenerateForms -ComponentName $($missing.ComponentName)" -ForegroundColor Gray
                }
                "MISSING_SERVICE" {
                    Write-Host "     💡 Run: .\Take-BusBuddyAction.ps1 -ActionType GenerateServices -ComponentName $($missing.ComponentName)" -ForegroundColor Gray
                }
            }
            Write-Host "     ⏱️  Estimated Effort: $($missing.EstimatedEffort)" -ForegroundColor Gray
            Write-Host ""
        }
    }

    Write-ColorOutput "`n🎯 RECOMMENDED ACTION PLAN:" $Colors.Success
    Write-Host "1. Generate missing forms: .\Take-BusBuddyAction.ps1 -ActionType GenerateForms -Priority High"
    Write-Host "2. Generate missing services: .\Take-BusBuddyAction.ps1 -ActionType GenerateServices -Priority High"
    Write-Host "3. Review and customize generated templates"
    Write-Host "4. Update ServiceContainer.cs with new registrations"
    Write-Host "5. Test new components and update TODO comments"
}

function Get-TodoSuggestion {
    param($Todo)

    switch -Regex ($Todo.Content) {
        "Update UI with loaded data" {
            return "Implement dashboard data display using HubTile components and real-time updates"
        }
        "Open.*Management form" {
            return "Create the management form and register it in ServiceContainer"
        }
        "Implement.*functionality" {
            return "Create full CRUD operations with Syncfusion DataGrid and edit forms"
        }
        "Add.*validation" {
            return "Implement input validation using Syncfusion validation controls"
        }
        "Connect to database" {
            return "Use Entity Framework with repository pattern and unit of work"
        }
        default {
            return "Review implementation requirements and use Syncfusion v30.1.37 best practices"
        }
    }
}

function Create-WorkItems {
    Write-ColorOutput "`n📋 CREATING WORK ITEMS..." $Colors.Action

    $workItems = @()

    # High priority TODOs
    $highPriorityTodos = $script:ScanResults.TodoItems | Where-Object Priority -eq "High"
    foreach ($todo in $highPriorityTodos) {
        $workItems += [PSCustomObject]@{
            Type = "User Story"
            Title = "Implement TODO: $($todo.Content -replace 'TODO:', '').Trim()"
            Priority = "High"
            EstimatedHours = 4
            File = $todo.File
            LineNumber = $todo.LineNumber
            Category = $todo.Category
            Description = "Implement the TODO item found in $($todo.File) at line $($todo.LineNumber)"
        }
    }

    # Missing components
    foreach ($missing in $script:ScanResults.MissingComponents) {
        $workItems += [PSCustomObject]@{
            Type = "Task"
            Title = "Create $($missing.ComponentName)"
            Priority = $missing.Priority
            EstimatedHours = switch ($missing.EstimatedEffort) {
                "1-2 hours" { 2 }
                "2-4 hours" { 3 }
                "4-8 hours" { 6 }
                default { 4 }
            }
            ComponentType = $missing.Type
            ReferencedIn = $missing.ReferencedIn
            Description = "Create missing component: $($missing.ComponentName) of type $($missing.Type)"
        }
    }

    $script:ActionResults.WorkItems = $workItems

    # Export work items
    $workItemsFile = Join-Path $ProjectPath "BusBuddy_WorkItems_$(Get-Date -Format 'yyyyMMdd_HHmmss').json"
    $workItems | ConvertTo-Json -Depth 5 | Out-File $workItemsFile -Encoding UTF8

    Write-ColorOutput "✅ Created $($workItems.Count) work items" $Colors.Success
    Write-ColorOutput "📄 Exported to: $workItemsFile" $Colors.Info

    # Show summary
    Write-ColorOutput "`n📊 WORK ITEMS SUMMARY:" $Colors.Info
    $workItems | Group-Object Priority | ForEach-Object {
        Write-Host "  $($_.Name) Priority: $($_.Count) items ($($_.Group | Measure-Object EstimatedHours -Sum | Select-Object -ExpandProperty Sum) hours)" -ForegroundColor $Colors.Info
    }
}

function Track-Progress {
    Write-ColorOutput "`n📊 TRACKING IMPLEMENTATION PROGRESS..." $Colors.Action

    if (-not $script:ScanResults) {
        Write-ColorOutput "❌ No scan results loaded. Please load scan results first." $Colors.Error
        return
    }

    # Check what's been implemented since last scan
    $implementedItems = @()
    $remainingItems = @()

    foreach ($todo in $script:ScanResults.TodoItems) {
        $filePath = Join-Path $ProjectPath $todo.File
        if (Test-Path $filePath) {
            $content = Get-Content $filePath -Raw -ErrorAction SilentlyContinue
            $lines = $content -split "`n"

            if ($todo.LineNumber -le $lines.Count) {
                $currentLine = $lines[$todo.LineNumber - 1]
                if ($currentLine -notmatch "TODO:" -and $currentLine -notmatch $todo.Content) {
                    $implementedItems += $todo
                } else {
                    $remainingItems += $todo
                }
            }
        }
    }

    $totalTodos = $script:ScanResults.TodoItems.Count
    $implementedCount = $implementedItems.Count
    $remainingCount = $remainingItems.Count
    $progressPercent = if ($totalTodos -gt 0) { [math]::Round(($implementedCount / $totalTodos) * 100, 1) } else { 0 }

    Write-ColorOutput "`n🎯 PROGRESS SUMMARY:" $Colors.Success
    Write-Host "  Total TODOs: $totalTodos"
    Write-Host "  Implemented: $implementedCount ($progressPercent%)" -ForegroundColor $Colors.Success
    Write-Host "  Remaining: $remainingCount" -ForegroundColor $Colors.Warning

    if ($implementedItems.Count -gt 0) {
        Write-ColorOutput "`n✅ RECENTLY IMPLEMENTED:" $Colors.Success
        foreach ($item in $implementedItems) {
            Write-Host "  ✓ $($item.File):$($item.LineNumber) - $($item.Content)" -ForegroundColor $Colors.Success
        }
    }

    if ($remainingItems.Count -gt 0) {
        Write-ColorOutput "`n⏳ STILL REMAINING:" $Colors.Warning
        foreach ($item in $remainingItems | Sort-Object Priority) {
            Write-Host "  [$($item.Priority)] $($item.File):$($item.LineNumber) - $($item.Content)" -ForegroundColor $Colors.Warning
        }
    }
}

# Main execution
try {
    Show-Header

    Write-ColorOutput "🚀 Starting BusBuddy TODO Action Tool..." $Colors.Info
    Write-ColorOutput "🎬 Action Type: $ActionType" $Colors.Info
    Write-ColorOutput "🎯 Priority Filter: $Priority" $Colors.Info

    # Load scan results for most actions
    if ($ActionType -ne "ShowSuggestions" -or $ScanResultsFile) {
        if (-not (Load-ScanResults -FilePath $ScanResultsFile)) {
            exit 1
        }
    }

    switch ($ActionType) {
        "GenerateForms" {
            Write-ColorOutput "`n🏗️  GENERATING SYNCFUSION FORMS..." $Colors.Action

            if ($ComponentName) {
                # Generate specific form
                $purpose = $ComponentName -replace "Form$", "" -replace "Management$", ""
                Generate-SyncfusionForm -FormName $ComponentName -Purpose $purpose -Priority "Manual"
            } else {
                # Generate all missing forms
                $missingForms = $script:ScanResults.MissingComponents | Where-Object {
                    $_.Type -eq "MISSING_FORM" -and ($Priority -eq "All" -or $_.Priority -eq $Priority)
                }

                foreach ($form in $missingForms) {
                    $purpose = $form.ComponentName -replace "Form$", "" -replace "Management$", ""
                    Generate-SyncfusionForm -FormName $form.ComponentName -Purpose $purpose -Priority $form.Priority
                }
            }
        }

        "GenerateServices" {
            Write-ColorOutput "`n⚙️  GENERATING SERVICES..." $Colors.Action
            Write-ColorOutput "💡 Service generation will be implemented in next version" $Colors.Info
        }

        "ShowSuggestions" {
            Show-Suggestions
        }

        "TrackProgress" {
            Track-Progress
        }

        "CreateWorkItems" {
            Create-WorkItems
        }
    }

    Write-Host "`n"
    Write-ColorOutput "✅ Action completed successfully!" $Colors.Success

    if ($script:ActionResults.FilesGenerated.Count -gt 0) {
        Write-ColorOutput "📁 Files Generated: $($script:ActionResults.FilesGenerated.Count)" $Colors.Info
        $script:ActionResults.FilesGenerated | ForEach-Object {
            Write-Host "   - $_" -ForegroundColor $Colors.Info
        }
    }

    if ($script:ActionResults.Recommendations.Count -gt 0) {
        Write-ColorOutput "`n💡 NEXT STEPS:" $Colors.Action
        $script:ActionResults.Recommendations | ForEach-Object {
            Write-Host "   • $_" -ForegroundColor $Colors.Action
        }
    }
}
catch {
    Write-ColorOutput "❌ Error during action execution: $($_.Exception.Message)" $Colors.Error
    Write-ColorOutput "📍 At: $($_.ScriptStackTrace)" $Colors.Warning
    exit 1
}
