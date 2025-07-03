#Requires -Version 7.0

<#
.SYNOPSIS
    Scans BusBuddy project for TODOs, incomplete methods, and development tasks

.DESCRIPTION
    This PowerShell 7.x script comprehensively scans the BusBuddy Syncfusion project for:
    - TODO comments and tasks
    - Incomplete method implementations
    - Placeholder functionality
    - Missing forms and components
    - NotImplementedException instances
    - Empty methods and classes

.PARAMETER ProjectPath
    Path to the BusBuddy project root directory

.PARAMETER OutputFormat
    Output format: Console, JSON, CSV, HTML, or All

.PARAMETER ExportPath
    Path to export results (optional, defaults to project root)

.PARAMETER IncludeTests
    Include test files in scan (default: false)

.EXAMPLE
    .\Scan-BusBuddyTodos.ps1 -ProjectPath "C:\Users\steve.mckitrick\Desktop\Bus Buddy"

.EXAMPLE
    .\Scan-BusBuddyTodos.ps1 -ProjectPath "." -OutputFormat JSON -ExportPath ".\reports"

.NOTES
    Author: BusBuddy Development Team
    Version: 1.0
    Date: July 3, 2025
    PowerShell Version: 7.0+
#>

[CmdletBinding()]
param(
    [Parameter(Mandatory = $false)]
    [string]$ProjectPath = (Get-Location).Path,

    [Parameter(Mandatory = $false)]
    [ValidateSet('Console', 'JSON', 'CSV', 'HTML', 'All')]
    [string]$OutputFormat = 'Console',

    [Parameter(Mandatory = $false)]
    [string]$ExportPath = '',

    [Parameter(Mandatory = $false)]
    [switch]$IncludeTests = $false
)

# Initialize script variables
$script:ScanResults = @{
    ScanDate          = Get-Date
    ProjectPath       = $ProjectPath
    TotalFiles        = 0
    TodoItems         = @()
    IncompleteItems   = @()
    PlaceholderItems  = @()
    MissingComponents = @()
    Summary           = @{}
}

# Color scheme for console output
$Colors = @{
    Header      = 'Cyan'
    Todo        = 'Yellow'
    Incomplete  = 'Red'
    Placeholder = 'Magenta'
    Missing     = 'DarkRed'
    Success     = 'Green'
    Warning     = 'DarkYellow'
    Info        = 'Blue'
}

function Write-ColorOutput {
    param(
        [string]$Message,
        [string]$Color = 'White'
    )
    Write-Host $Message -ForegroundColor $Color
}

function Show-Header {
    $headerText = @'
╔══════════════════════════════════════════════════════════════════════════════╗
║                    BusBuddy TODO & Incomplete Methods Scanner                ║
║                           PowerShell 7.x Version                            ║
║                              July 3, 2025                                   ║
╚══════════════════════════════════════════════════════════════════════════════╝
'@
    Write-ColorOutput $headerText $Colors.Header
    Write-Host ''
}

functionGet-ProjectFiles {
    param([string]$Path)

    Write-ColorOutput '🔍 Scanning project files...' $Colors.Info

    $fileExtensions = @('*.cs', '*.csproj', '*.json', '*.md', '*.txt')
    $excludePaths = @('bin', 'obj', 'packages', '.git', '.vs', 'node_modules')

    if (-not $IncludeTests) {
        $excludePaths += @('test', 'tests', '*test*', '*Test*')
    }

    $allFiles = @()

    foreach ($extension in $fileExtensions) {
        $files = Get-ChildItem -Path $Path -Filter $extension -Recurse -File |
        Where-Object {
            $exclude = $false
            foreach ($excludePath in $excludePaths) {
                if ($_.FullName -like "*$excludePath*") {
                    $exclude = $true
                    break
                }
            }
            -not $exclude
        }
        $allFiles += $files
    }

    $script:ScanResults.TotalFiles = $allFiles.Count
    Write-ColorOutput "📁 Found $($allFiles.Count) files to analyze" $Colors.Success
    return $allFiles
}

function Scan-TodoItems {
    param([System.IO.FileInfo[]]$Files)

    Write-ColorOutput "`n🔍 Scanning for TODO items..." $Colors.Info

    $todoPatterns = @(
        'TODO:',
        'todo:',
        'Todo:',
        'FIXME:',
        'HACK:',
        'NOTE:',
        'BUG:',
        'REVIEW:'
    )

    $todoItems = @()

    foreach ($file in $Files) {
        try {
            $content = Get-Content $file.FullName -ErrorAction SilentlyContinue
            if ($null -eq $content) { continue }

            for ($lineNum = 0; $lineNum -lt $content.Count; $lineNum++) {
                $line = $content[$lineNum]

                foreach ($pattern in $todoPatterns) {
                    if ($line -match $pattern) {
                        $context = ''
                        if ($lineNum -gt 0) {
                            $context = $content[$lineNum - 1].Trim()
                        }

                        $todoItems += [PSCustomObject]@{
                            Type       = 'TODO'
                            Pattern    = $pattern.TrimEnd(':')
                            File       = $file.FullName.Replace($ProjectPath, '').TrimStart('\')
                            LineNumber = $lineNum + 1
                            Content    = $line.Trim()
                            Context    = $context
                            Priority   = Get-TodoPriority -Content $line
                            Category   = Get-TodoCategory -File $file.Name -Content $line
                        }
                    }
                }
            }
        } catch {
            Write-Warning "Error reading file: $($file.FullName) - $($_.Exception.Message)"
        }
    }

    $script:ScanResults.TodoItems = $todoItems
    Write-ColorOutput "✅ Found $($todoItems.Count) TODO items" $Colors.Success
}

function Scan-IncompleteItems {
    param([System.IO.FileInfo[]]$Files)

    Write-ColorOutput "`n🔍 Scanning for incomplete implementations..." $Colors.Info

    $incompletePatterns = @(
        'throw new NotImplementedException',
        'NotImplemented',
        'not implemented',
        'not yet implemented',
        'will be implemented',
        'to be implemented',
        'implementation pending',
        'placeholder',
        'empty method',
        '// TODO:',
        '/* TODO'
    )

    $incompleteItems = @()

    foreach ($file in $Files) {
        if ($file.Extension -ne '.cs') { continue }

        try {
            $content = Get-Content $file.FullName -ErrorAction SilentlyContinue
            if ($null -eq $content) { continue }

            for ($lineNum = 0; $lineNum -lt $content.Count; $lineNum++) {
                $line = $content[$lineNum]

                foreach ($pattern in $incompletePatterns) {
                    if ($line -match [regex]::Escape($pattern)) {
                        $methodName = Get-MethodName -Content $content -LineNumber $lineNum

                        $incompleteItems += [PSCustomObject]@{
                            Type       = 'INCOMPLETE'
                            Pattern    = $pattern
                            File       = $file.FullName.Replace($ProjectPath, '').TrimStart('\')
                            LineNumber = $lineNum + 1
                            MethodName = $methodName
                            Content    = $line.Trim()
                            Severity   = Get-IncompleteSeverity -Pattern $pattern
                        }
                    }
                }

                # Check for empty methods
                if ($line -match '^\s*(public|private|protected|internal).*\{\s*\}\s*$') {
                    $methodName = Get-MethodName -Content $content -LineNumber $lineNum

                    $incompleteItems += [PSCustomObject]@{
                        Type       = 'EMPTY_METHOD'
                        Pattern    = 'Empty Method Body'
                        File       = $file.FullName.Replace($ProjectPath, '').TrimStart('\')
                        LineNumber = $lineNum + 1
                        MethodName = $methodName
                        Content    = $line.Trim()
                        Severity   = 'Medium'
                    }
                }
            }
        } catch {
            Write-Warning "Error analyzing file: $($file.FullName) - $($_.Exception.Message)"
        }
    }

    $script:ScanResults.IncompleteItems = $incompleteItems
    Write-ColorOutput "✅ Found $($incompleteItems.Count) incomplete implementations" $Colors.Success
}

function Scan-PlaceholderItems {
    param([System.IO.FileInfo[]]$Files)

    Write-ColorOutput "`n🔍 Scanning for placeholder functionality..." $Colors.Info

    $placeholderPatterns = @(
        'MessageBoxAdv\.Show.*will be implemented',
        'functionality will be implemented here',
        'placeholder message',
        'sample data',
        'for now.*show.*message',
        'temporary implementation',
        'stub implementation'
    )

    $placeholderItems = @()

    foreach ($file in $Files) {
        if ($file.Extension -ne '.cs') { continue }

        try {
            $content = Get-Content $file.FullName -Raw -ErrorAction SilentlyContinue
            if ($null -eq $content) { continue }

            $lines = $content -split "`n"

            for ($lineNum = 0; $lineNum -lt $lines.Count; $lineNum++) {
                $line = $lines[$lineNum]

                foreach ($pattern in $placeholderPatterns) {
                    if ($line -match $pattern) {
                        $methodName = Get-MethodName -Content $lines -LineNumber $lineNum

                        $placeholderItems += [PSCustomObject]@{
                            Type       = 'PLACEHOLDER'
                            Pattern    = $pattern
                            File       = $file.FullName.Replace($ProjectPath, '').TrimStart('\')
                            LineNumber = $lineNum + 1
                            MethodName = $methodName
                            Content    = $line.Trim()
                            Component  = Get-ComponentName -File $file.Name
                        }
                    }
                }
            }
        } catch {
            Write-Warning "Error scanning placeholders in: $($file.FullName) - $($_.Exception.Message)"
        }
    }

    $script:ScanResults.PlaceholderItems = $placeholderItems
    Write-ColorOutput "✅ Found $($placeholderItems.Count) placeholder implementations" $Colors.Success
}

function Scan-MissingComponents {
    param([System.IO.FileInfo[]]$Files)

    Write-ColorOutput "`n🔍 Scanning for missing forms and components..." $Colors.Info

    $missingComponents = @()

    # Define expected components based on button references
    $expectedForms = @(
        'PassengerManagementForm',
        'ActivityLogForm',
        'ReportsForm',
        'SettingsForm',
        'FuelEditForm',
        'MaintenanceEditForm',
        'RouteEditForm',
        'PassengerEditForm'
    )

    $existingForms = $Files | Where-Object { $_.Name -like '*Form.cs' } | ForEach-Object { $_.BaseName }

    foreach ($expectedForm in $expectedForms) {
        if ($expectedForm -notin $existingForms) {
            # Check if it's referenced in code
            $references = @()
            foreach ($file in $Files) {
                if ($file.Extension -eq '.cs') {
                    $content = Get-Content $file.FullName -Raw -ErrorAction SilentlyContinue
                    if ($content -match $expectedForm) {
                        $references += $file.FullName.Replace($ProjectPath, '').TrimStart('\')
                    }
                }
            }

            if ($references.Count -gt 0) {
                $missingComponents += [PSCustomObject]@{
                    Type            = 'MISSING_FORM'
                    ComponentName   = $expectedForm
                    ReferencedIn    = $references -join ', '
                    Priority        = Get-ComponentPriority -Name $expectedForm
                    EstimatedEffort = Get-EstimatedEffort -ComponentType 'Form'
                }
            }
        }
    }

    # Check for missing services
    $servicePattern = 'ServiceContainer\.GetService<([^>]+)>'
    foreach ($file in $Files) {
        if ($file.Extension -eq '.cs') {
            $content = Get-Content $file.FullName -Raw -ErrorAction SilentlyContinue
            if ($content -match $servicePattern) {
                $matches = [regex]::Matches($content, $servicePattern)
                foreach ($match in $matches) {
                    $serviceName = $match.Groups[1].Value
                    if ($serviceName -like '*Service' -and $serviceName -notin @('IBusService', 'IConfigurationService', 'IActivityService', 'IScheduleService', 'IStudentService')) {
                        $missingComponents += [PSCustomObject]@{
                            Type            = 'MISSING_SERVICE'
                            ComponentName   = $serviceName
                            ReferencedIn    = $file.FullName.Replace($ProjectPath, '').TrimStart('\')
                            Priority        = 'Medium'
                            EstimatedEffort = Get-EstimatedEffort -ComponentType 'Service'
                        }
                    }
                }
            }
        }
    }

    $script:ScanResults.MissingComponents = $missingComponents
    Write-ColorOutput "✅ Found $($missingComponents.Count) missing components" $Colors.Success
}

function Show-ActionPlan {
    Write-ColorOutput "`n📋 DETAILED ACTION PLAN" $Colors.Header
    Write-ColorOutput '═══════════════════════════════════════════════════════════════' $Colors.Header

    $taskNumber = 1

    # Phase 1: High Priority TODOs
    $highPriorityTodos = $script:ScanResults.TodoItems | Where-Object Priority -eq 'High' | Sort-Object File
    if ($highPriorityTodos.Count -gt 0) {
        Write-ColorOutput "`n🚨 PHASE 1: HIGH PRIORITY TODOs (Start Here)" $Colors.Incomplete
        foreach ($todo in $highPriorityTodos) {
            Write-ColorOutput "`n$taskNumber. Address High Priority TODO" $Colors.Warning
            Write-Host "   📁 File: $($todo.File)"
            Write-Host "   📍 Line: $($todo.LineNumber)"
            Write-Host "   📝 Task: $($todo.Content)"
            Write-Host "   🎯 Category: $($todo.Category)"
            Write-Host '   ⚡ Action: Open file and implement the required functionality'
            $taskNumber++
        }
    }

    # Phase 2: Missing High Priority Forms
    $highPriorityMissing = $script:ScanResults.MissingComponents | Where-Object Priority -eq 'High' | Sort-Object ComponentName
    if ($highPriorityMissing.Count -gt 0) {
        Write-ColorOutput "`n🏗️ PHASE 2: MISSING HIGH PRIORITY FORMS" $Colors.Missing
        foreach ($missing in $highPriorityMissing) {
            Write-ColorOutput "`n$taskNumber. Create Missing Form: $($missing.ComponentName)" $Colors.Warning
            Write-Host "   📋 Type: $($missing.Type)"
            Write-Host "   📁 Referenced In: $($missing.ReferencedIn)"
            Write-Host "   ⏱️ Estimated Effort: $($missing.EstimatedEffort)"
            Write-Host '   📝 Manual Steps:'
            Write-Host "      1. Create new Windows Form: Forms/$($missing.ComponentName).cs"
            Write-Host "      2. Add Designer file: Forms/$($missing.ComponentName).Designer.cs"
            Write-Host '      3. Inherit from MetroForm (Syncfusion)'
            Write-Host '      4. Apply VisualEnhancementManager.ApplyEnhancedTheme()'
            Write-Host '      5. Add to ServiceContainer registration'
            Write-Host '      6. Wire up button clicks in Dashboard.cs'
            $taskNumber++
        }
    }

    # Phase 3: Critical Incomplete Methods
    $criticalIncomplete = $script:ScanResults.IncompleteItems | Where-Object Severity -eq 'High' | Sort-Object File
    if ($criticalIncomplete.Count -gt 0) {
        Write-ColorOutput "`n🔧 PHASE 3: CRITICAL INCOMPLETE METHODS" $Colors.Incomplete
        foreach ($incomplete in $criticalIncomplete) {
            Write-ColorOutput "`n$taskNumber. Fix Incomplete Method: $($incomplete.MethodName)" $Colors.Warning
            Write-Host "   📁 File: $($incomplete.File)"
            Write-Host "   📍 Line: $($incomplete.LineNumber)"
            Write-Host "   🚫 Issue: $($incomplete.Pattern)"
            Write-Host '   📝 Manual Steps:'
            Write-Host "      1. Open $($incomplete.File)"
            Write-Host "      2. Navigate to line $($incomplete.LineNumber)"
            Write-Host '      3. Replace NotImplementedException with actual implementation'
            Write-Host '      4. Add appropriate error handling and logging'
            Write-Host '      5. Test the functionality'
            $taskNumber++
        }
    }

    # Phase 4: Placeholder Implementations
    if ($script:ScanResults.PlaceholderItems.Count -gt 0) {
        Write-ColorOutput "`n🎭 PHASE 4: REPLACE PLACEHOLDER IMPLEMENTATIONS" $Colors.Placeholder
        $placeholdersByFile = $script:ScanResults.PlaceholderItems | Group-Object File
        foreach ($fileGroup in $placeholdersByFile) {
            Write-ColorOutput "`n$taskNumber. Fix Placeholders in: $($fileGroup.Name)" $Colors.Warning
            Write-Host "   📊 Count: $($fileGroup.Count) placeholder(s)"
            Write-Host '   📝 Manual Steps:'
            Write-Host "      1. Open $($fileGroup.Name)"
            foreach ($placeholder in $fileGroup.Group) {
                Write-Host "      2. Line $($placeholder.LineNumber): Replace '$($placeholder.MethodName)' placeholder"
            }
            Write-Host '      3. Implement actual business logic instead of MessageBoxAdv.Show'
            Write-Host '      4. Add proper form opening/navigation'
            Write-Host '      5. Add data binding and CRUD operations as needed'
            $taskNumber++
        }
    }

    # Phase 5: Medium Priority Tasks
    $mediumPriorityTodos = $script:ScanResults.TodoItems | Where-Object Priority -eq 'Medium' | Sort-Object Category, File
    if ($mediumPriorityTodos.Count -gt 0) {
        Write-ColorOutput "`n📋 PHASE 5: MEDIUM PRIORITY TODOs" $Colors.Todo
        $todosByCategory = $mediumPriorityTodos | Group-Object Category
        foreach ($categoryGroup in $todosByCategory) {
            Write-ColorOutput "`n$taskNumber. Complete $($categoryGroup.Name) TODOs" $Colors.Warning
            Write-Host "   📊 Count: $($categoryGroup.Count) TODO(s)"
            Write-Host '   📝 Tasks:'
            foreach ($todo in $categoryGroup.Group) {
                Write-Host "      • $($todo.File):$($todo.LineNumber) - $($todo.Content)"
            }
            Write-Host '   📝 Manual Steps:'
            Write-Host '      1. Review each TODO item in the category'
            Write-Host '      2. Implement the required functionality'
            Write-Host '      3. Follow Syncfusion v30.1.37 best practices'
            Write-Host '      4. Test each implementation'
            $taskNumber++
        }
    }

    # Phase 6: Missing Services and Repositories
    $missingServices = $script:ScanResults.MissingComponents | Where-Object Type -eq 'MISSING_SERVICE'
    if ($missingServices.Count -gt 0) {
        Write-ColorOutput "`n🔌 PHASE 6: CREATE MISSING SERVICES" $Colors.Missing
        foreach ($service in $missingServices) {
            Write-ColorOutput "`n$taskNumber. Create Service: $($service.ComponentName)" $Colors.Warning
            Write-Host "   📁 Referenced In: $($service.ReferencedIn)"
            Write-Host "   ⏱️ Estimated Effort: $($service.EstimatedEffort)"
            Write-Host '   📝 Manual Steps:'
            Write-Host "      1. Create interface: Data/Interfaces/I$($service.ComponentName).cs"
            Write-Host "      2. Create implementation: Services/$($service.ComponentName).cs"
            Write-Host '      3. Add dependency injection in ServiceContainer.cs'
            Write-Host '      4. Implement required methods based on usage'
            Write-Host '      5. Add logging and error handling'
            $taskNumber++
        }
    }

    # Summary and Recommendations
    Write-ColorOutput "`n🎯 IMPLEMENTATION STRATEGY" $Colors.Success
    Write-ColorOutput '═══════════════════════════════════════════════════════════════' $Colors.Success
    Write-Host "`n📈 Recommended Implementation Order:"
    Write-Host '   1. Start with Phase 1 (High Priority TODOs) - These are blocking issues'
    Write-Host '   2. Move to Phase 2 (Missing Forms) - Core functionality gaps'
    Write-Host '   3. Address Phase 3 (Critical Methods) - Stability issues'
    Write-Host '   4. Continue with Phase 4 (Placeholders) - User experience improvements'
    Write-Host '   5. Complete Phase 5 (Medium TODOs) - Feature completeness'
    Write-Host '   6. Finish with Phase 6 (Services) - Architecture completion'

    Write-ColorOutput "`n🛠️ Development Guidelines:" $Colors.Info
    Write-Host '   • Always use Syncfusion v30.1.37 components from local installation'
    Write-Host '   • Apply VisualEnhancementManager themes to all new forms'
    Write-Host '   • Follow existing code patterns in PassengerManagementForm.cs'
    Write-Host '   • Register all new forms in ServiceContainer.cs'
    Write-Host '   • Test build after each major change: dotnet build'
    Write-Host '   • Commit frequently with descriptive messages'

    Write-ColorOutput "`n📊 Estimated Total Effort:" $Colors.Warning
    $totalTasks = $script:ScanResults.TodoItems.Count + $script:ScanResults.IncompleteItems.Count +
    $script:ScanResults.PlaceholderItems.Count + $script:ScanResults.MissingComponents.Count
    Write-Host "   Total Tasks: $totalTasks"
    Write-Host "   High Priority: $(($script:ScanResults.TodoItems | Where-Object Priority -eq 'High').Count + ($script:ScanResults.MissingComponents | Where-Object Priority -eq 'High').Count + ($script:ScanResults.IncompleteItems | Where-Object Severity -eq 'High').Count)"
    Write-Host '   Estimated Time: 20-40 hours (depending on complexity)'
    Write-Host '   Recommended Sprint: 2-3 development sessions'
}

function Get-TodoPriority {
    param([string]$Content)

    if ($Content -match '(critical|urgent|important|high)' -or $Content -match 'FIXME|BUG') {
        return 'High'
    } elseif ($Content -match '(medium|normal)' -or $Content -match 'TODO') {
        return 'Medium'
    } else {
        return 'Low'
    }
}

function Get-TodoCategory {
    param([string]$File, [string]$Content)

    if ($File -like '*Form*') {
        return 'UI/Forms'
    } elseif ($File -like '*Service*') {
        return 'Business Logic'
    } elseif ($File -like '*Repository*' -or $File -like '*Data*') {
        return 'Data Access'
    } elseif ($Content -match '(dashboard|ui|interface)') {
        return 'User Interface'
    } else {
        return 'General'
    }
}

function Get-IncompleteSeverity {
    param([string]$Pattern)

    switch -Regex ($Pattern) {
        'NotImplementedException' { return 'High' }
        'not implemented' { return 'High' }
        'will be implemented' { return 'Medium' }
        'placeholder' { return 'Low' }
        default { return 'Medium' }
    }
}

function Get-MethodName {
    param([string[]]$Content, [int]$LineNumber)

    # Look backwards for method signature
    for ($i = $LineNumber; $i -ge 0 -and $i -ge ($LineNumber - 10); $i--) {
        if ($Content[$i] -match '^\s*(public|private|protected|internal).*\s+(\w+)\s*\(') {
            return $matches[2]
        }
    }
    return 'Unknown'
}

function Get-ComponentName {
    param([string]$File)

    return $File.Replace('.cs', '').Replace('.Designer', '')
}

function Get-ComponentPriority {
    param([string]$Name)

    $highPriority = @('FuelEditForm', 'MaintenanceEditForm', 'RouteEditForm')
    $mediumPriority = @('PassengerEditForm', 'ActivityLogForm')

    if ($Name -in $highPriority) { return 'High' }
    elseif ($Name -in $mediumPriority) { return 'Medium' }
    else { return 'Low' }
}

function Get-EstimatedEffort {
    param([string]$ComponentType)

    switch ($ComponentType) {
        'Form' { return '4-8 hours' }
        'Service' { return '2-4 hours' }
        'Repository' { return '1-2 hours' }
        default { return '2-4 hours' }
    }
}

function Generate-Summary {
    $summary = @{
        TotalTodos             = $script:ScanResults.TodoItems.Count
        HighPriorityTodos      = ($script:ScanResults.TodoItems | Where-Object Priority -eq 'High').Count
        TotalIncomplete        = $script:ScanResults.IncompleteItems.Count
        HighSeverityIncomplete = ($script:ScanResults.IncompleteItems | Where-Object Severity -eq 'High').Count
        TotalPlaceholders      = $script:ScanResults.PlaceholderItems.Count
        TotalMissing           = $script:ScanResults.MissingComponents.Count

        TodosByCategory        = $script:ScanResults.TodoItems | Group-Object Category | ForEach-Object { @{ $_.Name = $_.Count } }
        IncompleteByFile       = $script:ScanResults.IncompleteItems | Group-Object File | ForEach-Object { @{ $_.Name = $_.Count } }
        MissingByType          = $script:ScanResults.MissingComponents | Group-Object Type | ForEach-Object { @{ $_.Name = $_.Count } }
    }

    $script:ScanResults.Summary = $summary
}

function Show-ConsoleReport {
    Write-Host "`n"
    Write-ColorOutput '📊 SCAN RESULTS SUMMARY' $Colors.Header
    Write-ColorOutput '═══════════════════════════════════════════════════════════════' $Colors.Header

    $summary = $script:ScanResults.Summary

    Write-ColorOutput "`n🎯 OVERVIEW:" $Colors.Info
    Write-Host "   Files Scanned: $($script:ScanResults.TotalFiles)"
    Write-Host "   TODO Items: $($summary.TotalTodos) (High Priority: $($summary.HighPriorityTodos))" -ForegroundColor $Colors.Todo
    Write-Host "   Incomplete Items: $($summary.TotalIncomplete) (High Severity: $($summary.HighSeverityIncomplete))" -ForegroundColor $Colors.Incomplete
    Write-Host "   Placeholder Items: $($summary.TotalPlaceholders)" -ForegroundColor $Colors.Placeholder
    Write-Host "   Missing Components: $($summary.TotalMissing)" -ForegroundColor $Colors.Missing

    if ($script:ScanResults.TodoItems.Count -gt 0) {
        Write-ColorOutput "`n📝 TODO ITEMS:" $Colors.Todo
        $script:ScanResults.TodoItems | Sort-Object Priority, File | ForEach-Object {
            $priorityColor = switch ($_.Priority) {
                'High' { $Colors.Incomplete }
                'Medium' { $Colors.Warning }
                default { $Colors.Info }
            }
            Write-Host "   [$($_.Priority)] $($_.File):$($_.LineNumber) - $($_.Content)" -ForegroundColor $priorityColor
        }
    }

    if ($script:ScanResults.IncompleteItems.Count -gt 0) {
        Write-ColorOutput "`n🚫 INCOMPLETE IMPLEMENTATIONS:" $Colors.Incomplete
        $script:ScanResults.IncompleteItems | Sort-Object Severity, File | ForEach-Object {
            $severityColor = switch ($_.Severity) {
                'High' { $Colors.Incomplete }
                'Medium' { $Colors.Warning }
                default { $Colors.Info }
            }
            Write-Host "   [$($_.Severity)] $($_.File):$($_.LineNumber) - $($_.MethodName)" -ForegroundColor $severityColor
        }
    }

    if ($script:ScanResults.PlaceholderItems.Count -gt 0) {
        Write-ColorOutput "`n🎭 PLACEHOLDER IMPLEMENTATIONS:" $Colors.Placeholder
        $script:ScanResults.PlaceholderItems | Sort-Object File | ForEach-Object {
            Write-Host "   $($_.File):$($_.LineNumber) - $($_.MethodName)" -ForegroundColor $Colors.Placeholder
        }
    }

    if ($script:ScanResults.MissingComponents.Count -gt 0) {
        Write-ColorOutput "`n❌ MISSING COMPONENTS:" $Colors.Missing
        $script:ScanResults.MissingComponents | Sort-Object Priority, Type | ForEach-Object {
            $priorityColor = switch ($_.Priority) {
                'High' { $Colors.Incomplete }
                'Medium' { $Colors.Warning }
                default { $Colors.Info }
            }
            Write-Host "   [$($_.Priority)] $($_.ComponentName) ($($_.Type)) - Effort: $($_.EstimatedEffort)" -ForegroundColor $priorityColor
        }
    }

    # Generate detailed action plan
    Show-ActionPlan
}

function Export-Results {
    param([string]$Format, [string]$Path)

    if ([string]::IsNullOrEmpty($Path)) {
        $Path = $ProjectPath
    }

    $timestamp = Get-Date -Format 'yyyyMMdd_HHmmss'

    switch ($Format) {
        'JSON' {
            $jsonFile = Join-Path $Path "BusBuddy_TODO_Scan_$timestamp.json"
            $script:ScanResults | ConvertTo-Json -Depth 10 | Out-File $jsonFile -Encoding UTF8
            Write-ColorOutput "📄 JSON report saved: $jsonFile" $Colors.Success
        }

        'CSV' {
            $csvFile = Join-Path $Path "BusBuddy_TODO_Scan_$timestamp.csv"
            $allItems = @()
            $allItems += $script:ScanResults.TodoItems | Select-Object Type, File, LineNumber, Content, Priority, Category
            $allItems += $script:ScanResults.IncompleteItems | Select-Object Type, File, LineNumber, @{N = 'Content'; E = { $_.MethodName } }, @{N = 'Priority'; E = { $_.Severity } }, @{N = 'Category'; E = { 'Incomplete' } }
            $allItems += $script:ScanResults.PlaceholderItems | Select-Object Type, File, LineNumber, @{N = 'Content'; E = { $_.MethodName } }, @{N = 'Priority'; E = { 'Medium' } }, @{N = 'Category'; E = { 'Placeholder' } }

            $allItems | Export-Csv $csvFile -NoTypeInformation -Encoding UTF8
            Write-ColorOutput "📊 CSV report saved: $csvFile" $Colors.Success
        }

        'HTML' {
            $htmlFile = Join-Path $Path "BusBuddy_TODO_Scan_$timestamp.html"
            $html = Generate-HtmlReport
            $html | Out-File $htmlFile -Encoding UTF8
            Write-ColorOutput "🌐 HTML report saved: $htmlFile" $Colors.Success
        }
    }
}

function Generate-HtmlReport {
    $html = @"
<!DOCTYPE html>
<html>
<head>
    <title>BusBuddy TODO & Incomplete Methods Report</title>
    <style>
        body { font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif; margin: 20px; background-color: #f5f5f5; }
        .header { background: linear-gradient(135deg, #667eea 0%, #764ba2 100%); color: white; padding: 20px; border-radius: 10px; text-align: center; }
        .summary { background: white; padding: 20px; margin: 20px 0; border-radius: 10px; box-shadow: 0 2px 10px rgba(0,0,0,0.1); }
        .section { background: white; margin: 20px 0; padding: 20px; border-radius: 10px; box-shadow: 0 2px 10px rgba(0,0,0,0.1); }
        .high { color: #e74c3c; font-weight: bold; }
        .medium { color: #f39c12; font-weight: bold; }
        .low { color: #27ae60; }
        table { width: 100%; border-collapse: collapse; margin-top: 10px; }
        th, td { padding: 12px; text-align: left; border-bottom: 1px solid #ddd; }
        th { background-color: #f8f9fa; font-weight: 600; }
        tr:hover { background-color: #f5f5f5; }
        .metric { display: inline-block; margin: 10px 20px 10px 0; padding: 15px; background: #ecf0f1; border-radius: 8px; min-width: 120px; text-align: center; }
        .metric-value { font-size: 24px; font-weight: bold; color: #2c3e50; }
        .metric-label { font-size: 12px; color: #7f8c8d; text-transform: uppercase; }
    </style>
</head>
<body>
    <div class="header">
        <h1>🚌 BusBuddy TODO & Incomplete Methods Report</h1>
        <p>Scan Date: $($script:ScanResults.ScanDate.ToString('yyyy-MM-dd HH:mm:ss'))</p>
        <p>Project: $($script:ScanResults.ProjectPath)</p>
    </div>

    <div class="summary">
        <h2>📊 Summary Overview</h2>
        <div class="metric">
            <div class="metric-value">$($script:ScanResults.TotalFiles)</div>
            <div class="metric-label">Files Scanned</div>
        </div>
        <div class="metric">
            <div class="metric-value">$($script:ScanResults.TodoItems.Count)</div>
            <div class="metric-label">TODO Items</div>
        </div>
        <div class="metric">
            <div class="metric-value">$($script:ScanResults.IncompleteItems.Count)</div>
            <div class="metric-label">Incomplete Items</div>
        </div>
        <div class="metric">
            <div class="metric-value">$($script:ScanResults.PlaceholderItems.Count)</div>
            <div class="metric-label">Placeholders</div>
        </div>
        <div class="metric">
            <div class="metric-value">$($script:ScanResults.MissingComponents.Count)</div>
            <div class="metric-label">Missing Components</div>
        </div>
    </div>
"@

    if ($script:ScanResults.TodoItems.Count -gt 0) {
        $html += @'
    <div class="section">
        <h2>📝 TODO Items</h2>
        <table>
            <tr><th>Priority</th><th>File</th><th>Line</th><th>Category</th><th>Content</th></tr>
'@
        foreach ($item in ($script:ScanResults.TodoItems | Sort-Object Priority, File)) {
            $priorityClass = $item.Priority.ToLower()
            $html += "<tr><td class='$priorityClass'>$($item.Priority)</td><td>$($item.File)</td><td>$($item.LineNumber)</td><td>$($item.Category)</td><td>$($item.Content -replace '<', '&lt;' -replace '>', '&gt;')</td></tr>`n"
        }
        $html += "</table></div>`n"
    }

    if ($script:ScanResults.IncompleteItems.Count -gt 0) {
        $html += @'
    <div class="section">
        <h2>🚫 Incomplete Implementations</h2>
        <table>
            <tr><th>Severity</th><th>File</th><th>Line</th><th>Method</th><th>Pattern</th></tr>
'@
        foreach ($item in ($script:ScanResults.IncompleteItems | Sort-Object Severity, File)) {
            $severityClass = $item.Severity.ToLower()
            $html += "<tr><td class='$severityClass'>$($item.Severity)</td><td>$($item.File)</td><td>$($item.LineNumber)</td><td>$($item.MethodName)</td><td>$($item.Pattern)</td></tr>`n"
        }
        $html += "</table></div>`n"
    }

    $html += '</body></html>'
    return $html
}

# Main execution
try {
    Show-Header

    Write-ColorOutput '🚀 Starting BusBuddy TODO & Incomplete Methods Scan...' $Colors.Info
    Write-ColorOutput "📂 Project Path: $ProjectPath" $Colors.Info
    Write-ColorOutput "📋 Output Format: $OutputFormat" $Colors.Info

    if (-not (Test-Path $ProjectPath)) {
        throw "Project path does not exist: $ProjectPath"
    }

    # Get all project files
    $projectFiles = Get-ProjectFiles -Path $ProjectPath

    # Perform scans
    Scan-TodoItems -Files $projectFiles
    Scan-IncompleteItems -Files $projectFiles
    Scan-PlaceholderItems -Files $projectFiles
    Scan-MissingComponents -Files $projectFiles

    # Generate summary
    Generate-Summary

    # Output results
    if ($OutputFormat -eq 'Console' -or $OutputFormat -eq 'All') {
        Show-ConsoleReport
    }

    if ($OutputFormat -eq 'JSON' -or $OutputFormat -eq 'All') {
        Export-Results -Format 'JSON' -Path $ExportPath
    }

    if ($OutputFormat -eq 'CSV' -or $OutputFormat -eq 'All') {
        Export-Results -Format 'CSV' -Path $ExportPath
    }

    if ($OutputFormat -eq 'HTML' -or $OutputFormat -eq 'All') {
        Export-Results -Format 'HTML' -Path $ExportPath
    }

    Write-Host "`n"
    Write-ColorOutput '✅ Scan completed successfully!' $Colors.Success
    Write-ColorOutput "🎯 Total actionable items found: $(
        $script:ScanResults.TodoItems.Count +
        $script:ScanResults.IncompleteItems.Count +
        $script:ScanResults.PlaceholderItems.Count +
        $script:ScanResults.MissingComponents.Count
    )" $Colors.Header
} catch {
    Write-ColorOutput "❌ Error during scan: $($_.Exception.Message)" $Colors.Incomplete
    Write-ColorOutput "📍 At: $($_.ScriptStackTrace)" $Colors.Warning
    exit 1
}
