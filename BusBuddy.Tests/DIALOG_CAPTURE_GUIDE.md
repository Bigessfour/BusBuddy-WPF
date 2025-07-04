# Dialog Event Capture System - Usage Guide

## Overview

The Dialog Event Capture system monitors and logs all dialog boxes that appear during test execution, including:
- `MessageBox` dialogs
- `MessageBoxAdv` (Syncfusion) dialogs  
- Custom form dialogs
- Error dialogs
- Confirmation dialogs
- Information dialogs

## Quick Start

### 1. Basic Usage in Tests

```csharp
[Test]
public void MyTest_WithDialogs()
{
    // Start capturing dialogs
    StartDialogCapture();
    
    // Run your test code that might trigger dialogs
    SomeMethodThatTriggersDialogs();
    
    // Get the dialog report
    var report = StopDialogCaptureAndGetReport();
    TestContext.WriteLine(report);
    
    // Or access individual dialogs
    var dialogs = GetCapturedDialogs();
    dialogs.Should().NotBeEmpty("Expected dialogs to appear");
}
```

### 2. Automatic Integration (TestBase)

All tests inheriting from `TestBase` automatically have dialog capture available:

```csharp
public class MyTests : TestBase
{
    [Test]
    public void TestMethod()
    {
        StartDialogCapture(); // Start when needed
        
        // Your test code here
        
        LogCapturedDialogs(); // Automatically logs all captured dialogs
    }
}
```

## Features

### Dialog Information Captured

For each dialog, the system captures:
- **Timestamp**: Exact time dialog appeared
- **Dialog Type**: MessageBox, Error, Warning, Confirmation, etc.
- **Title**: Dialog window title
- **Message**: Dialog content (where accessible)
- **Buttons**: Available buttons (OK, Cancel, Yes/No, etc.)
- **Icon**: Dialog icon type (Error, Warning, Information, Question)
- **Window Handle**: Native window handle for advanced operations
- **Duration**: How long dialog was visible
- **Error Context**: Additional context for error dialogs

### Detailed Reporting

The system provides comprehensive reports:

```
=== DIALOG CAPTURE REPORT ===
Total Dialogs Captured: 3
Capture Period: 14:23:15 - 14:23:18 (3.2s)

Dialog #1:
  Time: 14:23:15.234
  Type: ErrorDialog
  Title: Validation Error
  Message: Please enter a valid student name
  Buttons: OK
  Icon: Error
  Error Context: Form validation failed
  Duration: 1250ms

Dialog #2:
  Time: 14:23:16.567
  Type: ConfirmationDialog
  Title: Confirm Delete
  Message: Are you sure you want to delete this record?
  Buttons: Yes/No
  Icon: Question
  Duration: 2100ms
```

## Advanced Usage

### 1. Analyzing Specific Dialog Types

```csharp
[Test]
public void ValidateErrorDialogs()
{
    StartDialogCapture();
    
    // Trigger some operations
    PerformInvalidOperation();
    
    var dialogs = GetCapturedDialogs();
    var errorDialogs = dialogs.Where(d => d.DialogType == "ErrorDialog").ToList();
    
    errorDialogs.Should().HaveCount(1, "Expected exactly one error dialog");
    errorDialogs[0].Title.Should().Contain("Error");
}
```

### 2. Performance Analysis

```csharp
[Test]
public void MeasureDialogPerformance()
{
    StartDialogCapture();
    
    PerformOperation();
    
    var dialogs = GetCapturedDialogs();
    foreach (var dialog in dialogs)
    {
        TestContext.WriteLine($"Dialog {dialog.Title} was visible for {dialog.Duration}ms");
        
        if (dialog.Duration > 5000)
        {
            Assert.Fail($"Dialog {dialog.Title} was visible too long: {dialog.Duration}ms");
        }
    }
}
```

### 3. Dialog Content Validation

```csharp
[Test]
public void ValidateDialogContent()
{
    StartDialogCapture();
    
    TriggerSaveOperation();
    
    var dialogs = GetCapturedDialogs();
    var successDialog = dialogs.FirstOrDefault(d => d.DialogType == "InformationDialog");
    
    successDialog.Should().NotBeNull("Expected success confirmation dialog");
    successDialog.Message.Should().Contain("saved successfully");
}
```

## Integration with Existing Tests

### Updating SyncfusionUITests

```csharp
[TestFixture]
[Apartment(ApartmentState.STA)]
public class SyncfusionUITests : TestBase
{
    [SetUp]
    public void Setup()
    {
        StartDialogCapture();
    }
    
    [TearDown]
    public void TearDown()
    {
        var report = StopDialogCaptureAndGetReport();
        TestContext.WriteLine(report);
        
        // Assert no unexpected dialogs appeared
        var unexpectedDialogs = GetCapturedDialogs()
            .Where(d => d.DialogType == "ErrorDialog")
            .ToList();
            
        if (unexpectedDialogs.Any())
        {
            Assert.Fail($"Unexpected error dialogs appeared: {string.Join(", ", unexpectedDialogs.Select(d => d.Title))}");
        }
    }
}
```

## Troubleshooting

### Common Issues

1. **No Dialogs Captured**
   - Ensure `StartDialogCapture()` is called before dialog-triggering operations
   - Check that dialogs are actually appearing (sometimes they're hidden behind other windows)
   - Verify the test is running in STA apartment state for WinForms dialogs

2. **Dialogs Captured Multiple Times**
   - The system has built-in deduplication, but very fast successive dialogs might be captured multiple times
   - This is usually not a problem for analysis

3. **Performance Impact**
   - Dialog capture has minimal performance impact (checks every 100ms)
   - Can be disabled by simply not calling `StartDialogCapture()`

### Best Practices

1. **Always Start Capture Early**
   ```csharp
   [SetUp]
   public void Setup()
   {
       StartDialogCapture(); // Do this first
       // Other setup code
   }
   ```

2. **Log Results for Debugging**
   ```csharp
   [TearDown]
   public void TearDown()
   {
       var report = StopDialogCaptureAndGetReport();
       TestContext.WriteLine(report); // Always log for debugging
   }
   ```

3. **Use for Regression Testing**
   ```csharp
   [Test]
   public void EnsureNoUnexpectedDialogs()
   {
       StartDialogCapture();
       
       PerformNormalOperation();
       
       var dialogs = GetCapturedDialogs();
       dialogs.Should().BeEmpty("Normal operations should not trigger dialogs");
   }
   ```

## Configuration

The dialog capture system can be configured through test settings:

```json
{
  "TestSettings": {
    "DialogCapture": {
      "MonitoringInterval": 100,
      "EnableAdvancedLogging": true,
      "CaptureDialogContent": true
    }
  }
}
```

## Benefits

1. **Automated Dialog Detection**: No manual intervention needed
2. **Comprehensive Logging**: Full details about every dialog
3. **Performance Monitoring**: Track how long dialogs are visible
4. **Regression Testing**: Detect unexpected dialogs
5. **Debugging Aid**: Understand exactly what dialogs appeared during test failures
6. **User Experience Validation**: Ensure appropriate dialogs appear for user actions

## Example Test Output

When dialogs are captured, you'll see detailed logs like:

```
=== DIALOG CAPTURE SUMMARY ===
Total dialogs captured: 13
DIALOG DETECTED: ErrorDialog - Validation Error - Please correct the following errors
DIALOG ERROR CONTEXT: Form validation failed for student management
DIALOG DETECTED: ConfirmationDialog - Confirm Delete - Are you sure you want to delete?
DIALOG DETECTED: InformationDialog - Success - Student deleted successfully
```

This system gives you complete visibility into dialog interactions during testing, helping you understand exactly what's happening when those "13 dialog boxes" appear during your test runs.
