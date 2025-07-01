# BusBuddy PowerShell Helper Functions
# Source this file with: . .\dotnet-helpers.ps1

function Invoke-DotnetClean {
    <#
    .SYNOPSIS
    Clean the BusBuddy project targeting the .csproj file specifically
    
    .DESCRIPTION
    Runs 'dotnet clean BusBuddy.csproj' to avoid ambiguity between .sln and .csproj files
    #>
    Write-Host "Cleaning BusBuddy.csproj..." -ForegroundColor Green
    dotnet clean BusBuddy.csproj
}

function Invoke-DotnetBuild {
    <#
    .SYNOPSIS
    Build the BusBuddy project targeting the .csproj file specifically
    
    .DESCRIPTION
    Runs 'dotnet build BusBuddy.csproj' to avoid ambiguity between .sln and .csproj files
    #>
    Write-Host "Building BusBuddy.csproj..." -ForegroundColor Green
    dotnet build BusBuddy.csproj
}

function Invoke-DotnetRun {
    <#
    .SYNOPSIS
    Run the BusBuddy project targeting the .csproj file specifically
    
    .DESCRIPTION
    Runs 'dotnet run --project BusBuddy.csproj' to avoid ambiguity between .sln and .csproj files
    #>
    Write-Host "Running BusBuddy.csproj..." -ForegroundColor Green
    dotnet run --project BusBuddy.csproj
}

# Create aliases for common commands and typos
Set-Alias -Name "dotnet-clean" -Value Invoke-DotnetClean
Set-Alias -Name "dotnet-cean" -Value Invoke-DotnetClean
Set-Alias -Name "dotnet-build" -Value Invoke-DotnetBuild
Set-Alias -Name "dotnet-run" -Value Invoke-DotnetRun
Set-Alias -Name "dc" -Value Invoke-DotnetClean
Set-Alias -Name "db" -Value Invoke-DotnetBuild  
Set-Alias -Name "dr" -Value Invoke-DotnetRun

Write-Host "BusBuddy dotnet helpers loaded!" -ForegroundColor Cyan
Write-Host "Available commands:" -ForegroundColor Yellow
Write-Host "  dotnet-clean (or dc) - Clean the project" -ForegroundColor White
Write-Host "  dotnet-build (or db) - Build the project" -ForegroundColor White
Write-Host "  dotnet-run (or dr)   - Run the project" -ForegroundColor White
Write-Host "  dotnet-cean          - Alias for dotnet-clean (typo protection)" -ForegroundColor White

# BusBuddy: Override 'dotnet clean' to always target BusBuddy.csproj
function dotnet {
    param([Parameter(ValueFromRemainingArguments=$true)][String[]]$Args)
    if ($Args.Count -ge 1 -and $Args[0] -eq 'clean' -and ($Args.Count -eq 1 -or $Args[1] -notmatch '\.csproj$')) {
        Write-Host "[BusBuddy] Redirecting: dotnet clean BusBuddy.csproj" -ForegroundColor Yellow
        & dotnet clean BusBuddy.csproj @($Args[1..($Args.Count-1)])
    } else {
        & dotnet @Args
    }
}

# Note: Please manually delete the .sln file from your project directory, as file deletion is not supported directly via code edits.
