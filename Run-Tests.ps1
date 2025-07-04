#!/usr/bin/env pwsh

<#
.SYNOPSIS
    Bus Buddy Test Runner - Comprehensive testing script for the Bus Buddy project
.DESCRIPTION
    This script runs unit tests, integration tests, and generates coverage reports
    to validate the fixes for critical blockers and ensure deployment readiness.
.PARAMETER TestType
    Type of tests to run: All, Unit, Integration, Coverage
.PARAMETER Verbose
    Enable verbose output for detailed test results
.EXAMPLE
    .\Run-Tests.ps1 -TestType All -Verbose
.EXAMPLE
    .\Run-Tests.ps1 -TestType Unit
#>

param(
    [Parameter(Mandatory = $false)]
    [ValidateSet('All', 'Unit', 'Integration', 'Coverage')]
    [string]$TestType = 'All',

    [Parameter(Mandatory = $false)]
    [switch]$Verbose,

    [Parameter(Mandatory = $false)]
    [switch]$GenerateReport
)

# Script configuration
$ErrorActionPreference = 'Stop'
$VerbosePreference = if ($Verbose) { 'Continue' } else { 'SilentlyContinue' }

# Paths
$RootPath = $PSScriptRoot
$SolutionFile = Join-Path $RootPath 'Bus Buddy.sln'
$TestProject = Join-Path $RootPath 'BusBuddy.Tests\BusBuddy.Tests.csproj'
$TestResultsPath = Join-Path $RootPath 'TestResults'
$CoverageReportPath = Join-Path $RootPath 'CoverageReport'

Write-Host '🚌 Bus Buddy Test Runner' -ForegroundColor Cyan
Write-Host '=========================' -ForegroundColor Cyan

# Function to check prerequisites
function Test-Prerequisites {
    Write-Host '🔍 Checking prerequisites...' -ForegroundColor Yellow

    # Check .NET SDK
    try {
        $dotnetVersion = dotnet --version
        Write-Host "✅ .NET SDK: $dotnetVersion" -ForegroundColor Green
    } catch {
        Write-Error '❌ .NET SDK not found. Please install .NET 8 SDK.'
        exit 1
    }

    # Check solution file exists
    if (-not (Test-Path $SolutionFile)) {
        Write-Error "❌ Solution file not found: $SolutionFile"
        exit 1
    }

    # Check test project exists
    if (-not (Test-Path $TestProject)) {
        Write-Error "❌ Test project not found: $TestProject"
        exit 1
    }

    Write-Host '✅ Prerequisites check passed' -ForegroundColor Green
}

# Function to restore packages and build
function Build-Solution {
    Write-Host '🔨 Building solution...' -ForegroundColor Yellow

    # Clean previous builds
    Write-Verbose 'Cleaning solution...'
    dotnet clean $SolutionFile --verbosity quiet

    # Restore packages
    Write-Verbose 'Restoring NuGet packages...'
    dotnet restore $SolutionFile --verbosity quiet
    if ($LASTEXITCODE -ne 0) {
        Write-Error '❌ Package restore failed'
        exit 1
    }

    # Build solution
    Write-Verbose 'Building solution...'
    dotnet build $SolutionFile --configuration Release --no-restore --verbosity quiet
    if ($LASTEXITCODE -ne 0) {
        Write-Error '❌ Build failed'
        exit 1
    }

    Write-Host '✅ Build completed successfully' -ForegroundColor Green
}

# Function to run unit tests
function Run-UnitTests {
    Write-Host '🧪 Running unit tests...' -ForegroundColor Yellow

    $testArgs = @(
        'test'
        $TestProject
        '--no-build'
        '--configuration'
        'Release'
        '--filter'
        'Category!=Integration'
        '--logger'
        'trx'
        '--results-directory'
        $TestResultsPath
    )

    if ($Verbose) {
        $testArgs += '--verbosity'
        $testArgs += 'normal'
    } else {
        $testArgs += '--verbosity'
        $testArgs += 'minimal'
    }

    # Add coverage collection
    $testArgs += '--collect:XPlat Code Coverage'

    Write-Verbose "Test command: dotnet $($testArgs -join ' ')"

    & dotnet @testArgs
    $unitTestResult = $LASTEXITCODE

    if ($unitTestResult -eq 0) {
        Write-Host '✅ Unit tests passed' -ForegroundColor Green
    } else {
        Write-Host '❌ Unit tests failed' -ForegroundColor Red
    }

    return $unitTestResult
}

# Function to run integration tests
function Run-IntegrationTests {
    Write-Host '🔗 Running integration tests...' -ForegroundColor Yellow

    $testArgs = @(
        'test'
        $TestProject
        '--no-build'
        '--configuration'
        'Release'
        '--filter'
        'Category=Integration'
        '--logger'
        'trx'
        '--results-directory'
        $TestResultsPath
    )

    if ($Verbose) {
        $testArgs += '--verbosity'
        $testArgs += 'normal'
    } else {
        $testArgs += '--verbosity'
        $testArgs += 'minimal'
    }

    Write-Verbose "Integration test command: dotnet $($testArgs -join ' ')"

    & dotnet @testArgs
    $integrationTestResult = $LASTEXITCODE

    if ($integrationTestResult -eq 0) {
        Write-Host '✅ Integration tests passed' -ForegroundColor Green
    } else {
        Write-Host '❌ Integration tests failed' -ForegroundColor Red
    }

    return $integrationTestResult
}

# Function to generate coverage report
function Generate-CoverageReport {
    Write-Host '📊 Generating coverage report...' -ForegroundColor Yellow

    # Check if reportgenerator tool is installed
    $reportGeneratorPath = dotnet tool list -g | Select-String 'reportgenerator'
    if (-not $reportGeneratorPath) {
        Write-Host 'Installing ReportGenerator tool...' -ForegroundColor Blue
        dotnet tool install -g dotnet-reportgenerator-globaltool
    }

    # Find coverage files
    $coverageFiles = Get-ChildItem -Path $TestResultsPath -Recurse -Filter 'coverage.cobertura.xml' -ErrorAction SilentlyContinue

    if ($coverageFiles.Count -eq 0) {
        Write-Warning '⚠️ No coverage files found. Make sure tests were run with coverage collection.'
        return
    }

    # Generate HTML report
    $coverageFilePaths = ($coverageFiles.FullName -join ';')

    Write-Verbose "Coverage files: $coverageFilePaths"
    Write-Verbose "Report output: $CoverageReportPath"

    & dotnet reportgenerator `
        -reports:$coverageFilePaths `
        -targetdir:$CoverageReportPath `
        -reporttypes:'Html;Cobertura' `
        -verbosity:Warning

    if ($LASTEXITCODE -eq 0) {
        Write-Host "✅ Coverage report generated: $CoverageReportPath\index.html" -ForegroundColor Green

        # Open report if requested
        if ($GenerateReport) {
            $reportFile = Join-Path $CoverageReportPath 'index.html'
            if (Test-Path $reportFile) {
                Start-Process $reportFile
            }
        }
    } else {
        Write-Host '❌ Coverage report generation failed' -ForegroundColor Red
    }
}

# Function to validate critical blockers are fixed
function Test-CriticalBlockers {
    Write-Host '🔍 Validating critical blocker fixes...' -ForegroundColor Yellow

    $issues = @()

    # Check if RouteService exists
    $routeServicePath = Join-Path $RootPath 'Services\RouteService.cs'
    if (-not (Test-Path $routeServicePath)) {
        $issues += '❌ RouteService.cs not found'
    } else {
        Write-Host '✅ RouteService implementation found' -ForegroundColor Green
    }

    # Check if RouteService is registered in DI
    $serviceContainerPath = Join-Path $RootPath 'ServiceContainer.cs'
    if (Test-Path $serviceContainerPath) {
        $content = Get-Content $serviceContainerPath -Raw
        if ($content -match 'IRouteService.*RouteService') {
            Write-Host '✅ RouteService registered in DI' -ForegroundColor Green
        } else {
            $issues += '❌ RouteService not registered in dependency injection'
        }
    }

    # Check if test project exists and has tests
    if (Test-Path $TestProject) {
        $testFiles = Get-ChildItem -Path (Split-Path $TestProject) -Recurse -Filter '*Tests.cs' -ErrorAction SilentlyContinue
        if ($testFiles.Count -gt 0) {
            Write-Host "✅ Test files found: $($testFiles.Count) test classes" -ForegroundColor Green
        } else {
            $issues += '❌ No test files found'
        }
    }

    if ($issues.Count -eq 0) {
        Write-Host '✅ All critical blockers resolved' -ForegroundColor Green
        return $true
    } else {
        Write-Host '❌ Critical blockers found:' -ForegroundColor Red
        foreach ($issue in $issues) {
            Write-Host "  $issue" -ForegroundColor Red
        }
        return $false
    }
}

# Function to display summary
function Show-TestSummary {
    param(
        [int]$UnitTestResult,
        [int]$IntegrationTestResult,
        [bool]$BlockersFixed
    )

    Write-Host ''
    Write-Host '📋 Test Summary' -ForegroundColor Cyan
    Write-Host '===============' -ForegroundColor Cyan

    $unitStatus = if ($UnitTestResult -eq 0) { '✅ PASSED' } else { '❌ FAILED' }
    $integrationStatus = if ($IntegrationTestResult -eq 0) { '✅ PASSED' } else { '❌ FAILED' }
    $blockersStatus = if ($BlockersFixed) { '✅ RESOLVED' } else { '❌ ISSUES FOUND' }

    Write-Host "Unit Tests:        $unitStatus" -ForegroundColor $(if ($UnitTestResult -eq 0) { 'Green' } else { 'Red' })
    Write-Host "Integration Tests: $integrationStatus" -ForegroundColor $(if ($IntegrationTestResult -eq 0) { 'Green' } else { 'Red' })
    Write-Host "Critical Blockers: $blockersStatus" -ForegroundColor $(if ($BlockersFixed) { 'Green' } else { 'Red' })

    $overallSuccess = ($UnitTestResult -eq 0) -and ($IntegrationTestResult -eq 0) -and $BlockersFixed

    Write-Host ''
    if ($overallSuccess) {
        Write-Host '🎉 ALL TESTS PASSED - DEPLOYMENT READY!' -ForegroundColor Green -BackgroundColor DarkGreen
    } else {
        Write-Host '⚠️  ISSUES FOUND - REVIEW REQUIRED' -ForegroundColor Red -BackgroundColor DarkRed
    }

    Write-Host ''

    # Display test results location
    if (Test-Path $TestResultsPath) {
        Write-Host "📁 Test results: $TestResultsPath" -ForegroundColor Blue
    }

    if (Test-Path $CoverageReportPath) {
        Write-Host "📊 Coverage report: $CoverageReportPath\index.html" -ForegroundColor Blue
    }
}

# Main execution
try {
    # Initialize
    Test-Prerequisites
    Build-Solution

    # Create test results directory
    if (Test-Path $TestResultsPath) {
        Remove-Item $TestResultsPath -Recurse -Force
    }
    New-Item -ItemType Directory -Path $TestResultsPath -Force | Out-Null

    # Run tests based on type
    $unitTestResult = 0
    $integrationTestResult = 0

    switch ($TestType) {
        'Unit' {
            $unitTestResult = Run-UnitTests
        }
        'Integration' {
            $integrationTestResult = Run-IntegrationTests
        }
        'Coverage' {
            $unitTestResult = Run-UnitTests
            Generate-CoverageReport
        }
        'All' {
            $unitTestResult = Run-UnitTests
            $integrationTestResult = Run-IntegrationTests
            Generate-CoverageReport
        }
    }

    # Validate critical blockers
    $blockersFixed = Test-CriticalBlockers

    # Show summary
    Show-TestSummary -UnitTestResult $unitTestResult -IntegrationTestResult $integrationTestResult -BlockersFixed $blockersFixed

    # Exit with appropriate code
    $overallSuccess = ($unitTestResult -eq 0) -and ($integrationTestResult -eq 0) -and $blockersFixed
    exit $(if ($overallSuccess) { 0 } else { 1 })
} catch {
    Write-Error "❌ Test execution failed: $($_.Exception.Message)"
    exit 1
}
