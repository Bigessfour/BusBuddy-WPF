# PowerShell Script to Replace StatusStrip with Syncfusion AutoLabel
# This script systematically replaces all non-Syncfusion StatusStrip controls with Syncfusion AutoLabel

param(
    [string]$ProjectPath = 'c:\Users\steve.mckitrick\Desktop\Bus Buddy'
)

Write-Host '🔧 BusBuddy Syncfusion Compliance Fixer' -ForegroundColor Cyan
Write-Host 'Replacing StatusStrip controls with Syncfusion AutoLabel...' -ForegroundColor Yellow

# Files that need StatusStrip replacement
$filesToFix = @(
    'Forms\MaintenanceManagementForm.Designer.cs',
    'Forms\StudentManagementForm.Designer.cs',
    'Forms\TicketManagementForm.Designer.cs'
)

# Files that need code updates (corresponding .cs files)
$codeFilesToFix = @(
    'Forms\MaintenanceManagementForm.cs',
    'Forms\StudentManagementForm.cs',
    'Forms\TicketManagementForm.cs'
)

function Replace-StatusStripInDesigner {
    param([string]$FilePath)

    Write-Host "📝 Processing: $FilePath" -ForegroundColor Green

    if (-not (Test-Path $FilePath)) {
        Write-Host "❌ File not found: $FilePath" -ForegroundColor Red
        return
    }

    $content = Get-Content $FilePath -Raw

    # Replace field declarations
    $content = $content -replace 'private System\.Windows\.Forms\.StatusStrip statusStrip;\s*private System\.Windows\.Forms\.ToolStripStatusLabel toolStripStatusLabel;', 'private AutoLabel statusLabel;'

    # Replace initialization
    $content = $content -replace 'this\.statusStrip = new System\.Windows\.Forms\.StatusStrip\(\);\s*this\.toolStripStatusLabel = new System\.Windows\.Forms\.ToolStripStatusLabel\(\);', 'this.statusLabel = new AutoLabel();'

    # Replace SuspendLayout
    $content = $content -replace 'this\.statusStrip\.SuspendLayout\(\);', ''

    # Replace configuration block
    $statusStripConfigPattern = @'
(?s)// \s*statusStrip.*?this\.toolStripStatusLabel\.Text = "Ready";
'@

    $statusLabelConfig = @'
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
'@

    $content = $content -replace $statusStripConfigPattern, $statusLabelConfig

    # Replace Controls.Add
    $content = $content -replace 'this\.Controls\.Add\(this\.statusStrip\);', 'this.Controls.Add(this.statusLabel);'

    # Replace ResumeLayout
    $content = $content -replace 'this\.statusStrip\.ResumeLayout\(false\);\s*this\.statusStrip\.PerformLayout\(\);', ''

    # Manual replacements for specific patterns that might remain
    $content = $content -replace 'this\.statusStrip\.Items\.AddRange\(new System\.Windows\.Forms\.ToolStripItem\[\] \{\s*this\.toolStripStatusLabel\}\);', ''
    $content = $content -replace 'this\.statusStrip\.(Location|Name|Size|TabIndex|Text) = [^;]+;', ''
    $content = $content -replace 'this\.toolStripStatusLabel\.(Name|Size|Text) = [^;]+;', ''

    Set-Content $FilePath $content -Encoding UTF8
    Write-Host "✅ Updated designer file: $FilePath" -ForegroundColor Green
}

function Replace-StatusStripInCode {
    param([string]$FilePath)

    Write-Host "📝 Processing code file: $FilePath" -ForegroundColor Green

    if (-not (Test-Path $FilePath)) {
        Write-Host "❌ File not found: $FilePath" -ForegroundColor Red
        return
    }

    $content = Get-Content $FilePath -Raw

    # Replace UpdateStatus method references
    $content = $content -replace 'toolStripStatusLabel\.Text', 'statusLabel.Text'

    Set-Content $FilePath $content -Encoding UTF8
    Write-Host "✅ Updated code file: $FilePath" -ForegroundColor Green
}

# Main execution
try {
    Write-Host '🚀 Starting Syncfusion compliance fixes...' -ForegroundColor Cyan

    # Fix designer files
    foreach ($file in $filesToFix) {
        $fullPath = Join-Path $ProjectPath $file
        Replace-StatusStripInDesigner $fullPath
    }

    # Fix code files
    foreach ($file in $codeFilesToFix) {
        $fullPath = Join-Path $ProjectPath $file
        Replace-StatusStripInCode $fullPath
    }

    Write-Host '🎉 Syncfusion compliance fixes completed!' -ForegroundColor Green
    Write-Host '📋 Summary:' -ForegroundColor Cyan
    Write-Host '   • Replaced StatusStrip with AutoLabel in designer files' -ForegroundColor White
    Write-Host '   • Updated code references to use statusLabel' -ForegroundColor White
    Write-Host '   • Maintained existing functionality' -ForegroundColor White
    Write-Host ''
    Write-Host '⚠️  Next Steps:' -ForegroundColor Yellow
    Write-Host '   1. Build the project to verify no compilation errors' -ForegroundColor White
    Write-Host '   2. Test each form to ensure status updates work correctly' -ForegroundColor White
    Write-Host '   3. Verify visual styling matches expectations' -ForegroundColor White

} catch {
    Write-Host "❌ Error during processing: $($_.Exception.Message)" -ForegroundColor Red
}

Write-Host '✨ Run completed. Check the SYNCFUSION_COMPLIANCE_AUDIT_REPORT.md for detailed information.' -ForegroundColor Cyan
