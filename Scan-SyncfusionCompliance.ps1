# Syncfusion Compliance Scanner for BusBuddy Project
# This script scans the codebase to identify non-Syncfusion controls and methods

param(
    [string]$ProjectPath = '.',
    [string]$OutputFile = 'syncfusion-compliance-report.html',
    [switch]$Detailed = $false,
    [switch]$ExportJson = $false
)

# Define patterns for non-Syncfusion controls and their Syncfusion replacements
$NonSyncfusionPatterns = @{
    # Windows Forms Controls
    'System\.Windows\.Forms\.Button'           = 'Syncfusion.WinForms.Controls.SfButton'
    'System\.Windows\.Forms\.Label'            = 'Syncfusion.Windows.Forms.Tools.AutoLabel'
    'System\.Windows\.Forms\.TextBox'          = 'Syncfusion.Windows.Forms.Tools.TextBoxExt'
    'System\.Windows\.Forms\.ComboBox'         = 'Syncfusion.Windows.Forms.Tools.ComboBoxAdv'
    'System\.Windows\.Forms\.DataGridView'     = 'Syncfusion.WinForms.DataGrid.SfDataGrid'
    'System\.Windows\.Forms\.Panel'            = 'Syncfusion.Windows.Forms.Tools.GradientPanel'
    'System\.Windows\.Forms\.GroupBox'         = 'Syncfusion.Windows.Forms.Tools.GroupBox'
    'System\.Windows\.Forms\.TabControl'       = 'Syncfusion.Windows.Forms.Tools.TabControlAdv'
    'System\.Windows\.Forms\.DateTimePicker'   = 'Syncfusion.Windows.Forms.Tools.DateTimePickerAdv'
    'System\.Windows\.Forms\.NumericUpDown'    = 'Syncfusion.Windows.Forms.Tools.IntegerTextBox'
    'System\.Windows\.Forms\.CheckBox'         = 'Syncfusion.Windows.Forms.Tools.CheckBoxAdv'
    'System\.Windows\.Forms\.RadioButton'      = 'Syncfusion.Windows.Forms.Tools.RadioButtonAdv'
    'System\.Windows\.Forms\.ListBox'          = 'Syncfusion.Windows.Forms.Tools.ListBoxAdv'
    'System\.Windows\.Forms\.TreeView'         = 'Syncfusion.Windows.Forms.Tools.TreeViewAdv'
    'System\.Windows\.Forms\.ProgressBar'      = 'Syncfusion.Windows.Forms.Tools.ProgressBarAdv'
    'System\.Windows\.Forms\.StatusStrip'      = 'Syncfusion.Windows.Forms.Tools.AutoLabel'
    'System\.Windows\.Forms\.ToolStrip'        = 'Syncfusion.Windows.Forms.Tools.ToolStripEx'
    'System\.Windows\.Forms\.MenuStrip'        = 'Syncfusion.Windows.Forms.Tools.MainFrameBarManager'
    'System\.Windows\.Forms\.ContextMenuStrip' = 'Syncfusion.Windows.Forms.Tools.PopupMenusManager'

    # Non-qualified control declarations (less specific)
    '\bnew Button\('                           = 'new SfButton('
    '\bnew Label\('                            = 'new AutoLabel('
    '\bnew TextBox\('                          = 'new TextBoxExt('
    '\bnew ComboBox\('                         = 'new ComboBoxAdv('
    '\bnew DataGridView\('                     = 'new SfDataGrid('
    '\bnew Panel\('                            = 'new GradientPanel('
    '\bnew GroupBox\('                         = 'new GroupBox(' # Syncfusion version
    '\bnew TabControl\('                       = 'new TabControlAdv('
    '\bnew DateTimePicker\('                   = 'new DateTimePickerAdv('
    '\bnew NumericUpDown\('                    = 'new IntegerTextBox('
    '\bnew CheckBox\('                         = 'new CheckBoxAdv('
    '\bnew RadioButton\('                      = 'new RadioButtonAdv('
    '\bnew ListBox\('                          = 'new ListBoxAdv('
    '\bnew TreeView\('                         = 'new TreeViewAdv('
    '\bnew ProgressBar\('                      = 'new ProgressBarAdv('
    '\bnew StatusStrip\('                      = 'new AutoLabel('
    '\bnew ToolStrip\('                        = 'new ToolStripEx('
    '\bnew MenuStrip\('                        = 'new MainFrameBarManager('
    '\bnew ContextMenuStrip\('                 = 'new PopupMenusManager('
}

# Define method patterns that might indicate non-Syncfusion usage
$MethodPatterns = @{
    'MessageBox\.Show'    = 'Syncfusion.Windows.Forms.MessageBoxAdv.Show'
    'ColorDialog'         = 'Syncfusion.Windows.Forms.Tools.ColorPickerUIAdv'
    'FontDialog'          = 'Syncfusion.Windows.Forms.Tools.FontDialog'
    'OpenFileDialog'      = 'Syncfusion.Windows.Forms.Tools.OpenFileDialog'
    'SaveFileDialog'      = 'Syncfusion.Windows.Forms.Tools.SaveFileDialog'
    'FolderBrowserDialog' = 'Syncfusion.Windows.Forms.Tools.FolderBrowserDialog'
}

# Acceptable System.Windows.Forms enums and properties (these are framework-level and OK to use)
$AcceptableSystemWinForms = @(
    'System\.Windows\.Forms\.DockStyle',
    'System\.Windows\.Forms\.AnchorStyles',
    'System\.Windows\.Forms\.BorderStyle',
    'System\.Windows\.Forms\.AutoScaleMode',
    'System\.Windows\.Forms\.FormStartPosition',
    'System\.Windows\.Forms\.FormWindowState',
    'System\.Windows\.Forms\.FormBorderStyle',
    'System\.Windows\.Forms\.DialogResult',
    'System\.Windows\.Forms\.MessageBoxButtons',
    'System\.Windows\.Forms\.MessageBoxIcon',
    'System\.Windows\.Forms\.KeyEventArgs',
    'System\.Windows\.Forms\.MouseEventArgs',
    'System\.Windows\.Forms\.PaintEventArgs',
    'System\.Windows\.Forms\.Padding',
    'System\.Windows\.Forms\.Orientation',
    'System\.Windows\.Forms\.ScrollBars',
    'System\.Windows\.Forms\.HorizontalAlignment',
    'System\.Windows\.Forms\.VerticalAlignment',
    'System\.Windows\.Forms\.ContentAlignment'
)

# Initialize results
$Results = @{
    FilesScanned          = 0
    TotalIssues           = 0
    NonSyncfusionControls = @()
    NonSyncfusionMethods  = @()
    AcceptableUsages      = @()
    Summary               = @{}
}

function Write-ColorOutput {
    param([string]$Text, [string]$Color = 'White')

    $colorMap = @{
        'Red'     = [ConsoleColor]::Red
        'Green'   = [ConsoleColor]::Green
        'Yellow'  = [ConsoleColor]::Yellow
        'Blue'    = [ConsoleColor]::Blue
        'Cyan'    = [ConsoleColor]::Cyan
        'Magenta' = [ConsoleColor]::Magenta
        'White'   = [ConsoleColor]::White
        'Gray'    = [ConsoleColor]::Gray
    }

    Write-Host $Text -ForegroundColor $colorMap[$Color]
}

function Test-AcceptableUsage {
    param([string]$Line)

    foreach ($acceptable in $AcceptableSystemWinForms) {
        if ($Line -match $acceptable) {
            return $true
        }
    }
    return $false
}

function Scan-File {
    param([string]$FilePath)

    $content = Get-Content $FilePath
    $lineNumber = 0
    $fileIssues = @()

    foreach ($line in $content) {
        $lineNumber++

        # Skip if it's an acceptable usage
        if (Test-AcceptableUsage $line) {
            $Results.AcceptableUsages += @{
                File    = $FilePath
                Line    = $lineNumber
                Content = $line.Trim()
                Type    = 'AcceptableFrameworkUsage'
            }
            continue
        }

        # Check for non-Syncfusion controls
        foreach ($pattern in $NonSyncfusionPatterns.Keys) {
            if ($line -match $pattern) {
                $issue = @{
                    File                 = $FilePath
                    Line                 = $lineNumber
                    Content              = $line.Trim()
                    Pattern              = $pattern
                    SuggestedReplacement = $NonSyncfusionPatterns[$pattern]
                    Type                 = 'Control'
                    Severity             = 'High'
                }
                $fileIssues += $issue
                $Results.NonSyncfusionControls += $issue
            }
        }

        # Check for non-Syncfusion methods
        foreach ($pattern in $MethodPatterns.Keys) {
            if ($line -match $pattern) {
                $issue = @{
                    File                 = $FilePath
                    Line                 = $lineNumber
                    Content              = $line.Trim()
                    Pattern              = $pattern
                    SuggestedReplacement = $MethodPatterns[$pattern]
                    Type                 = 'Method'
                    Severity             = 'Medium'
                }
                $fileIssues += $issue
                $Results.NonSyncfusionMethods += $issue
            }
        }
    }

    return $fileIssues
}

function Generate-HtmlReport {
    param([string]$OutputPath)

    $html = @"
<!DOCTYPE html>
<html>
<head>
    <title>BusBuddy Syncfusion Compliance Report</title>
    <style>
        body { font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif; margin: 20px; background-color: #f5f5f5; }
        .header { background: linear-gradient(135deg, #667eea 0%, #764ba2 100%); color: white; padding: 20px; border-radius: 10px; margin-bottom: 20px; }
        .summary { background: white; padding: 20px; border-radius: 10px; margin-bottom: 20px; box-shadow: 0 2px 4px rgba(0,0,0,0.1); }
        .section { background: white; padding: 20px; border-radius: 10px; margin-bottom: 20px; box-shadow: 0 2px 4px rgba(0,0,0,0.1); }
        .issue-high { border-left: 5px solid #e74c3c; padding: 10px; margin: 10px 0; background: #fdf2f2; }
        .issue-medium { border-left: 5px solid #f39c12; padding: 10px; margin: 10px 0; background: #fef9e7; }
        .issue-low { border-left: 5px solid #27ae60; padding: 10px; margin: 10px 0; background: #eafaf1; }
        .acceptable { border-left: 5px solid #3498db; padding: 10px; margin: 10px 0; background: #ebf3fd; }
        .code { font-family: 'Courier New', monospace; background: #f8f9fa; padding: 5px; border-radius: 3px; }
        .suggestion { color: #27ae60; font-weight: bold; }
        .file-path { color: #666; font-size: 0.9em; }
        .stats { display: flex; justify-content: space-around; text-align: center; }
        .stat-box { background: #f8f9fa; padding: 15px; border-radius: 8px; border: 1px solid #e9ecef; }
        .stat-number { font-size: 2em; font-weight: bold; color: #495057; }
        .stat-label { color: #6c757d; margin-top: 5px; }
        .compliance-score { font-size: 3em; font-weight: bold; }
        .score-excellent { color: #27ae60; }
        .score-good { color: #f39c12; }
        .score-poor { color: #e74c3c; }
        table { width: 100%; border-collapse: collapse; margin-top: 15px; }
        th, td { padding: 10px; text-align: left; border-bottom: 1px solid #ddd; }
        th { background-color: #f2f2f2; font-weight: bold; }
        .timestamp { color: #666; font-size: 0.9em; margin-top: 10px; }
    </style>
</head>
<body>
    <div class="header">
        <h1>🔍 BusBuddy Syncfusion Compliance Report</h1>
        <p>Comprehensive analysis of Syncfusion usage in the BusBuddy transportation management system</p>
        <div class="timestamp">Generated: $(Get-Date -Format 'yyyy-MM-dd HH:mm:ss')</div>
    </div>

    <div class="summary">
        <h2>📊 Compliance Summary</h2>
        <div class="stats">
            <div class="stat-box">
                <div class="stat-number">$($Results.FilesScanned)</div>
                <div class="stat-label">Files Scanned</div>
            </div>
            <div class="stat-box">
                <div class="stat-number">$($Results.TotalIssues)</div>
                <div class="stat-label">Total Issues</div>
            </div>
            <div class="stat-box">
                <div class="stat-number">$($Results.NonSyncfusionControls.Count)</div>
                <div class="stat-label">Control Issues</div>
            </div>
            <div class="stat-box">
                <div class="stat-number">$($Results.NonSyncfusionMethods.Count)</div>
                <div class="stat-label">Method Issues</div>
            </div>
        </div>
"@

    # Calculate compliance score
    $totalChecks = $Results.FilesScanned * 10 # Rough estimate
    $complianceScore = if ($totalChecks -gt 0) {
        [math]::Round((($totalChecks - $Results.TotalIssues) / $totalChecks) * 100, 1)
    } else { 100 }

    $scoreClass = if ($complianceScore -ge 95) { 'score-excellent' }
    elseif ($complianceScore -ge 80) { 'score-good' }
    else { 'score-poor' }

    $html += @"
        <div style="text-align: center; margin: 20px 0;">
            <div class="compliance-score $scoreClass">$complianceScore%</div>
            <div>Syncfusion Compliance Score</div>
        </div>
    </div>
"@

    # Non-Syncfusion Controls Section
    if ($Results.NonSyncfusionControls.Count -gt 0) {
        $html += @'
    <div class="section">
        <h2>🚨 Non-Syncfusion Controls Found</h2>
        <p>These controls should be replaced with their Syncfusion equivalents:</p>
'@
        foreach ($issue in $Results.NonSyncfusionControls) {
            $html += @"
        <div class="issue-high">
            <div class="file-path">📁 $($issue.File):$($issue.Line)</div>
            <div class="code">$([System.Web.HttpUtility]::HtmlEncode($issue.Content))</div>
            <div class="suggestion">💡 Suggested: $($issue.SuggestedReplacement)</div>
        </div>
"@
        }
        $html += '</div>'
    }

    # Non-Syncfusion Methods Section
    if ($Results.NonSyncfusionMethods.Count -gt 0) {
        $html += @'
    <div class="section">
        <h2>⚠️ Non-Syncfusion Methods Found</h2>
        <p>These methods should be replaced with their Syncfusion equivalents:</p>
'@
        foreach ($issue in $Results.NonSyncfusionMethods) {
            $html += @"
        <div class="issue-medium">
            <div class="file-path">📁 $($issue.File):$($issue.Line)</div>
            <div class="code">$([System.Web.HttpUtility]::HtmlEncode($issue.Content))</div>
            <div class="suggestion">💡 Suggested: $($issue.SuggestedReplacement)</div>
        </div>
"@
        }
        $html += '</div>'
    }

    # Acceptable Usages Section (if detailed report requested)
    if ($Detailed -and $Results.AcceptableUsages.Count -gt 0) {
        $html += @'
    <div class="section">
        <h2>✅ Acceptable Framework Usages</h2>
        <p>These System.Windows.Forms usages are acceptable as they are framework enums/properties:</p>
'@
        foreach ($usage in $Results.AcceptableUsages | Select-Object -First 20) {
            $html += @"
        <div class="acceptable">
            <div class="file-path">📁 $($usage.File):$($usage.Line)</div>
            <div class="code">$([System.Web.HttpUtility]::HtmlEncode($usage.Content))</div>
        </div>
"@
        }
        if ($Results.AcceptableUsages.Count -gt 20) {
            $html += "<p><em>... and $($Results.AcceptableUsages.Count - 20) more acceptable usages</em></p>"
        }
        $html += '</div>'
    }

    # Recommendations Section
    $html += @'
    <div class="section">
        <h2>📋 Recommendations</h2>
        <table>
            <tr>
                <th>Priority</th>
                <th>Action</th>
                <th>Description</th>
            </tr>
'@

    if ($Results.NonSyncfusionControls.Count -gt 0) {
        $html += @"
            <tr>
                <td>🔴 High</td>
                <td>Replace Controls</td>
                <td>Replace $($Results.NonSyncfusionControls.Count) non-Syncfusion controls with Syncfusion equivalents</td>
            </tr>
"@
    }

    if ($Results.NonSyncfusionMethods.Count -gt 0) {
        $html += @"
            <tr>
                <td>🟡 Medium</td>
                <td>Replace Methods</td>
                <td>Replace $($Results.NonSyncfusionMethods.Count) non-Syncfusion methods with Syncfusion equivalents</td>
            </tr>
"@
    }

    if ($Results.TotalIssues -eq 0) {
        $html += @'
            <tr>
                <td>🟢 Excellent</td>
                <td>Maintain Compliance</td>
                <td>Your project is fully Syncfusion compliant! Consider adding this script to your CI/CD pipeline.</td>
            </tr>
'@
    }

    $html += @'
        </table>
    </div>

    <div class="section">
        <h2>🛠️ Syncfusion Version 30.1.37 Guidelines</h2>
        <p><strong>Recommended Syncfusion Controls:</strong></p>
        <ul>
            <li><code>SfButton</code> - Enhanced button with styling options</li>
            <li><code>SfDataGrid</code> - Advanced data grid with filtering and sorting</li>
            <li><code>AutoLabel</code> - Smart label with auto-sizing</li>
            <li><code>TextBoxExt</code> - Extended textbox with validation</li>
            <li><code>ComboBoxAdv</code> - Advanced combo box</li>
            <li><code>GradientPanel</code> - Panel with gradient background support</li>
            <li><code>DateTimePickerAdv</code> - Enhanced date picker</li>
        </ul>

        <p><strong>Local Installation Path:</strong></p>
        <code>C:\Program Files (x86)\Syncfusion\Essential Studio\Windows\30.1.37</code>
    </div>

    <div class="section">
        <h2>📞 Support Resources</h2>
        <ul>
            <li><a href="https://help.syncfusion.com/cr/windowsforms/Syncfusion.html">Syncfusion Windows Forms API Reference</a></li>
            <li><a href="https://help.syncfusion.com/windowsforms/overview">Syncfusion Windows Forms Overview</a></li>
            <li><a href="https://github.com/Bigessfour/BusBuddy_Syncfusion">BusBuddy GitHub Repository</a></li>
        </ul>
    </div>

</body>
</html>
'@

    # Ensure we can write HTML entities
    Add-Type -AssemblyName System.Web
    $html | Out-File -FilePath $OutputPath -Encoding UTF8
}

# Main execution
Write-ColorOutput '🔍 BusBuddy Syncfusion Compliance Scanner' 'Cyan'
Write-ColorOutput '=======================================' 'Cyan'

# Find all C# files
$csharpFiles = Get-ChildItem -Path $ProjectPath -Recurse -Filter '*.cs' | Where-Object {
    $_.FullName -notmatch '\\bin\\' -and
    $_.FullName -notmatch '\\obj\\' -and
    $_.FullName -notmatch 'Migrations'
}

Write-ColorOutput "📁 Scanning $($csharpFiles.Count) C# files..." 'Yellow'

foreach ($file in $csharpFiles) {
    $Results.FilesScanned++
    Write-ColorOutput "   Scanning: $($file.Name)" 'Gray'

    $fileIssues = Scan-File $file.FullName
    $Results.TotalIssues += $fileIssues.Count

    if ($fileIssues.Count -gt 0) {
        Write-ColorOutput "   ⚠️  Found $($fileIssues.Count) issues in $($file.Name)" 'Yellow'
    }
}

# Generate summary
Write-ColorOutput "`n📊 Scan Results:" 'Green'
Write-ColorOutput '=================' 'Green'
Write-ColorOutput "Files Scanned: $($Results.FilesScanned)" 'White'
Write-ColorOutput "Total Issues: $($Results.TotalIssues)" 'White'
Write-ColorOutput "Control Issues: $($Results.NonSyncfusionControls.Count)" 'White'
Write-ColorOutput "Method Issues: $($Results.NonSyncfusionMethods.Count)" 'White'
Write-ColorOutput "Acceptable Usages: $($Results.AcceptableUsages.Count)" 'White'

if ($Results.TotalIssues -eq 0) {
    Write-ColorOutput "`n🎉 Excellent! Your project is 100% Syncfusion compliant!" 'Green'
} elseif ($Results.TotalIssues -le 5) {
    Write-ColorOutput "`n✅ Good! Only a few minor issues found." 'Yellow'
} else {
    Write-ColorOutput "`n⚠️  Multiple issues found. See detailed report for recommendations." 'Red'
}

# Generate reports
Write-ColorOutput "`n📄 Generating reports..." 'Blue'

# HTML Report
$htmlPath = Join-Path $ProjectPath $OutputFile
Generate-HtmlReport $htmlPath
Write-ColorOutput "   📋 HTML Report: $htmlPath" 'Green'

# JSON Report (if requested)
if ($ExportJson) {
    $jsonPath = Join-Path $ProjectPath 'syncfusion-compliance-report.json'
    $Results | ConvertTo-Json -Depth 10 | Out-File -FilePath $jsonPath -Encoding UTF8
    Write-ColorOutput "   📋 JSON Report: $jsonPath" 'Green'
}

# Summary for terminal
if ($Results.NonSyncfusionControls.Count -gt 0) {
    Write-ColorOutput "`n🚨 High Priority Issues (Controls):" 'Red'
    foreach ($issue in $Results.NonSyncfusionControls | Select-Object -First 5) {
        Write-ColorOutput "   📁 $($issue.File):$($issue.Line)" 'Gray'
        Write-ColorOutput "   ❌ $($issue.Content.Substring(0, [Math]::Min($issue.Content.Length, 80)))" 'White'
        Write-ColorOutput "   💡 $($issue.SuggestedReplacement)" 'Green'
        Write-ColorOutput '' 'White'
    }
    if ($Results.NonSyncfusionControls.Count -gt 5) {
        Write-ColorOutput "   ... and $($Results.NonSyncfusionControls.Count - 5) more control issues" 'Gray'
    }
}

Write-ColorOutput "`n✨ Scan completed! Open the HTML report for detailed analysis." 'Cyan'
