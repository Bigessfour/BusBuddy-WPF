using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Extensions.Logging;

namespace BusBuddy.Tests.Infrastructure
{
    /// <summary>
    /// Captures and logs dialog box events during testing
    /// Monitors MessageBox, MessageBoxAdv, and custom dialog interactions
    /// Provides detailed logging of dialog content, user actions, and timing
    /// </summary>
    public class DialogEventCapture : IDisposable
    {
        private readonly ILogger<DialogEventCapture> _logger;
        private readonly List<DialogEvent> _capturedDialogs = new();
        private readonly System.Threading.Timer _monitorTimer;
        private readonly object _lockObject = new();
        private bool _isCapturing = false;
        private bool _disposed = false;

        // Win32 API for dialog detection
        [DllImport("user32.dll")]
        private static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("user32.dll")]
        private static extern int GetWindowText(IntPtr hWnd, StringBuilder text, int count);

        [DllImport("user32.dll")]
        private static extern bool EnumWindows(EnumWindowsProc enumProc, IntPtr lParam);

        [DllImport("user32.dll")]
        private static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);

        [DllImport("user32.dll")]
        private static extern bool IsWindowVisible(IntPtr hWnd);

        [DllImport("user32.dll")]
        private static extern IntPtr GetWindow(IntPtr hWnd, uint uCmd);

        [DllImport("user32.dll")]
        private static extern int GetClassName(IntPtr hWnd, StringBuilder lpClassName, int nMaxCount);

        [DllImport("user32.dll")]
        private static extern bool EnumChildWindows(IntPtr hWndParent, EnumWindowsProc lpEnumFunc, IntPtr lParam);

        private delegate bool EnumWindowsProc(IntPtr hWnd, IntPtr lParam);

        public DialogEventCapture(ILogger<DialogEventCapture> logger)
        {
            _logger = logger;
            _monitorTimer = new System.Threading.Timer(MonitorDialogs, null, Timeout.Infinite, Timeout.Infinite);
        }

        /// <summary>
        /// Starts capturing dialog events
        /// </summary>
        public void StartCapture()
        {
            lock (_lockObject)
            {
                if (_isCapturing) return;

                _isCapturing = true;
                _capturedDialogs.Clear();
                _monitorTimer.Change(100, 100); // Check every 100ms
                _logger.LogInformation("Dialog capture started");
            }
        }

        /// <summary>
        /// Stops capturing dialog events
        /// </summary>
        public void StopCapture()
        {
            lock (_lockObject)
            {
                if (!_isCapturing) return;

                _isCapturing = false;
                _monitorTimer.Change(Timeout.Infinite, Timeout.Infinite);
                _logger.LogInformation("Dialog capture stopped. Captured {DialogCount} dialogs", _capturedDialogs.Count);
            }
        }

        /// <summary>
        /// Gets all captured dialog events
        /// </summary>
        public IReadOnlyList<DialogEvent> GetCapturedDialogs()
        {
            lock (_lockObject)
            {
                return _capturedDialogs.AsReadOnly();
            }
        }

        /// <summary>
        /// Gets a summary report of all captured dialogs
        /// </summary>
        public string GetDialogSummaryReport()
        {
            lock (_lockObject)
            {
                var report = new StringBuilder();
                report.AppendLine($"=== DIALOG CAPTURE REPORT ===");
                report.AppendLine($"Total Dialogs Captured: {_capturedDialogs.Count}");
                report.AppendLine($"Capture Period: {GetCapturePeriod()}");
                report.AppendLine();

                for (int i = 0; i < _capturedDialogs.Count; i++)
                {
                    var dialog = _capturedDialogs[i];
                    report.AppendLine($"Dialog #{i + 1}:");
                    report.AppendLine($"  Time: {dialog.Timestamp:HH:mm:ss.fff}");
                    report.AppendLine($"  Type: {dialog.DialogType}");
                    report.AppendLine($"  Title: {dialog.Title}");
                    report.AppendLine($"  Message: {dialog.Message}");
                    report.AppendLine($"  Buttons: {dialog.Buttons}");
                    report.AppendLine($"  Icon: {dialog.Icon}");
                    report.AppendLine($"  Result: {dialog.Result}");
                    report.AppendLine($"  Error Context: {dialog.ErrorContext}");
                    report.AppendLine($"  Duration: {dialog.Duration}ms");
                    report.AppendLine();
                }

                return report.ToString();
            }
        }

        /// <summary>
        /// Logs detailed information about captured dialogs
        /// </summary>
        public void LogDialogDetails()
        {
            lock (_lockObject)
            {
                _logger.LogInformation("=== DIALOG CAPTURE SUMMARY ===");
                _logger.LogInformation("Total dialogs captured: {DialogCount}", _capturedDialogs.Count);

                foreach (var dialog in _capturedDialogs)
                {
                    _logger.LogWarning("DIALOG DETECTED: {DialogType} - {Title} - {Message}",
                        dialog.DialogType, dialog.Title, dialog.Message);

                    if (!string.IsNullOrEmpty(dialog.ErrorContext))
                    {
                        _logger.LogError("DIALOG ERROR CONTEXT: {ErrorContext}", dialog.ErrorContext);
                    }
                }
            }
        }

        private void MonitorDialogs(object? state)
        {
            if (!_isCapturing) return;

            try
            {
                EnumWindows(CheckForDialogs, IntPtr.Zero);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error monitoring dialogs");
            }
        }

        private bool CheckForDialogs(IntPtr hWnd, IntPtr lParam)
        {
            try
            {
                if (!IsWindowVisible(hWnd)) return true;

                // Get window class name
                var className = new StringBuilder(256);
                GetClassName(hWnd, className, className.Capacity);
                var classNameStr = className.ToString();

                // Get window text (title)
                var windowText = new StringBuilder(256);
                GetWindowText(hWnd, windowText, windowText.Capacity);
                var windowTitle = windowText.ToString();

                // Log all visible windows for debugging
                if (!string.IsNullOrEmpty(windowTitle))
                {
                    _logger.LogDebug($"Found window: Class='{classNameStr}', Title='{windowTitle}'");
                }

                // Check if it's a dialog window
                if (IsDialogWindow(classNameStr) || (!string.IsNullOrEmpty(windowTitle) && IsDialogTitle(windowTitle)))
                {
                    // Get window process ID
                    GetWindowThreadProcessId(hWnd, out uint processId);
                    var currentProcessId = (uint)Process.GetCurrentProcess().Id;

                    // Only capture dialogs from our process
                    if (processId == currentProcessId)
                    {
                        _logger.LogInformation($"Capturing dialog: Class='{classNameStr}', Title='{windowTitle}'");
                        CaptureDialog(hWnd, classNameStr);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking window for dialog");
            }

            return true; // Continue enumeration
        }

        private static bool IsDialogTitle(string title)
        {
            return title.Contains("Error", StringComparison.OrdinalIgnoreCase) ||
                   title.Contains("Warning", StringComparison.OrdinalIgnoreCase) ||
                   title.Contains("Alert", StringComparison.OrdinalIgnoreCase) ||
                   title.Contains("Confirm", StringComparison.OrdinalIgnoreCase) ||
                   title.Contains("Message", StringComparison.OrdinalIgnoreCase) ||
                   title.Contains("ServiceContainer", StringComparison.OrdinalIgnoreCase);
        }

        private static bool IsDialogWindow(string className)
        {
            // Check for all types of dialog windows
            return className.Contains("MessageBox") ||
                   className.Contains("Dialog") ||
                   className.Contains("#32770") || // Standard Windows dialog class
                   className.Contains("Syncfusion") ||
                   className.Contains("MetroForm") ||
                   className.Contains("TForm") || // Delphi/C++ Builder forms
                   className.Contains("WindowsForms10") || // Windows Forms dialogs
                   className.Contains("HwndWrapper") || // WPF dialogs
                   className.Equals("#32770", StringComparison.OrdinalIgnoreCase) || // Standard dialog
                   className.Contains("Error", StringComparison.OrdinalIgnoreCase) ||
                   className.Contains("Warning", StringComparison.OrdinalIgnoreCase) ||
                   className.Contains("Alert", StringComparison.OrdinalIgnoreCase) ||
                   IsModalWindow(className);
        }

        private static bool IsModalWindow(string className)
        {
            // Additional check for modal windows that might be dialogs
            return !string.IsNullOrEmpty(className) &&
                   (className.StartsWith("#") ||
                    className.Contains("Modal") ||
                    className.Contains("Popup"));
        }

        private void CaptureDialog(IntPtr hWnd, string className)
        {
            try
            {
                var title = new StringBuilder(256);
                GetWindowText(hWnd, title, title.Capacity);
                var titleStr = title.ToString();

                // Create dialog event
                var dialogEvent = new DialogEvent
                {
                    Timestamp = DateTime.Now,
                    DialogType = DetermineDialogType(className, titleStr),
                    Title = titleStr,
                    Message = ExtractDialogMessage(hWnd),
                    Buttons = ExtractDialogButtons(hWnd),
                    Icon = ExtractDialogIcon(titleStr),
                    WindowHandle = hWnd,
                    ClassName = className,
                    ErrorContext = ExtractErrorContext(titleStr)
                };

                lock (_lockObject)
                {
                    // Avoid duplicate captures
                    if (!_capturedDialogs.Exists(d =>
                        d.WindowHandle == hWnd &&
                        Math.Abs((d.Timestamp - dialogEvent.Timestamp).TotalMilliseconds) < 500))
                    {
                        _capturedDialogs.Add(dialogEvent);
                        _logger.LogWarning("DIALOG CAPTURED: {DialogType} - {Title}",
                            dialogEvent.DialogType, dialogEvent.Title);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error capturing dialog details");
            }
        }

        private static string DetermineDialogType(string className, string title)
        {
            if (className.Contains("MessageBox")) return "MessageBox";
            if (className.Contains("Syncfusion")) return "SyncfusionDialog";
            if (title.Contains("Error")) return "ErrorDialog";
            if (title.Contains("Warning")) return "WarningDialog";
            if (title.Contains("Confirm")) return "ConfirmationDialog";
            if (title.Contains("Information") || title.Contains("Info")) return "InformationDialog";
            return "UnknownDialog";
        }

        private static string ExtractDialogMessage(IntPtr hWnd)
        {
            var messages = new List<string>();

            // Enumerate child windows to find static text controls
            EnumChildWindows(hWnd, (childHWnd, lParam) =>
            {
                var className = new StringBuilder(256);
                GetClassName(childHWnd, className, className.Capacity);
                var classNameStr = className.ToString();

                // Look for static controls that typically contain message text
                if (classNameStr.Contains("Static") || classNameStr.Contains("Label") ||
                    classNameStr.Equals("STATIC", StringComparison.OrdinalIgnoreCase))
                {
                    var text = new StringBuilder(1024);
                    GetWindowText(childHWnd, text, text.Capacity);
                    var textStr = text.ToString().Trim();

                    if (!string.IsNullOrEmpty(textStr) && textStr.Length > 5)
                    {
                        messages.Add(textStr);
                    }
                }

                return true; // Continue enumeration
            }, IntPtr.Zero);

            if (messages.Any())
            {
                return string.Join(" | ", messages);
            }

            // Fallback: try to get the window text itself
            var windowText = new StringBuilder(1024);
            GetWindowText(hWnd, windowText, windowText.Capacity);
            var fallbackText = windowText.ToString();

            return !string.IsNullOrEmpty(fallbackText) ? fallbackText : "[No message text found]";
        }

        private static string ExtractDialogButtons(IntPtr hWnd)
        {
            // This would enumerate child button controls to determine available buttons
            return "OK/Cancel"; // Simplified
        }

        private static string ExtractDialogIcon(string title)
        {
            if (title.Contains("Error")) return "Error";
            if (title.Contains("Warning")) return "Warning";
            if (title.Contains("Information")) return "Information";
            if (title.Contains("Question")) return "Question";
            return "None";
        }

        private static string ExtractErrorContext(string title)
        {
            if (title.Contains("Error") || title.Contains("Exception"))
            {
                return $"Error dialog detected with title: {title}";
            }
            return string.Empty;
        }

        private string GetCapturePeriod()
        {
            if (_capturedDialogs.Count == 0) return "No dialogs captured";

            var first = _capturedDialogs[0].Timestamp;
            var last = _capturedDialogs[_capturedDialogs.Count - 1].Timestamp;
            var duration = last - first;

            return $"{first:HH:mm:ss} - {last:HH:mm:ss} ({duration.TotalSeconds:F1}s)";
        }

        public void Dispose()
        {
            if (_disposed) return;

            StopCapture();
            _monitorTimer?.Dispose();
            _disposed = true;
        }
    }

    /// <summary>
    /// Represents a captured dialog event
    /// </summary>
    public class DialogEvent
    {
        public DateTime Timestamp { get; set; }
        public string DialogType { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public string Buttons { get; set; } = string.Empty;
        public string Icon { get; set; } = string.Empty;
        public string Result { get; set; } = string.Empty;
        public string ErrorContext { get; set; } = string.Empty;
        public int Duration { get; set; }
        public IntPtr WindowHandle { get; set; }
        public string ClassName { get; set; } = string.Empty;
    }
}
