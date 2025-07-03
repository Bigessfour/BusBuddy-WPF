using Microsoft.Extensions.Logging;
using Syncfusion.Windows.Forms;

namespace Bus_Buddy;

static class Program
{
    /// <summary>
    ///  The main entry point for the application.
    /// </summary>
    [STAThread]
    static void Main()
    {
        try
        {
            // Enable high DPI support
            if (Environment.OSVersion.Version.Major >= 6)
            {
                SetProcessDpiAwarenessContext(DPI_AWARENESS_CONTEXT_PER_MONITOR_AWARE_V2);
            }

            // Initialize application configuration with high DPI settings
            ApplicationConfiguration.Initialize();

            // Set high DPI mode
            Application.SetHighDpiMode(HighDpiMode.PerMonitorV2);

            // Initialize dependency injection container
            ServiceContainer.Initialize();

            var loggerFactory = ServiceContainer.GetService<ILoggerFactory>();
            var logger = loggerFactory.CreateLogger("BusBuddy.Program");
            logger.LogInformation("BusBuddy application starting...");

            // Get the main form from the DI container
            var mainForm = ServiceContainer.GetService<Dashboard>();

            Application.Run(mainForm);

            logger.LogInformation("BusBuddy application shutting down...");
        }
        catch (Exception ex)
        {
            MessageBoxAdv.Show($"An error occurred while starting the application: {ex.Message}",
                "Startup Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        finally
        {
            ServiceContainer.Dispose();
        }
    }

    // Windows API for DPI awareness
    [System.Runtime.InteropServices.DllImport("user32.dll")]
    private static extern bool SetProcessDpiAwarenessContext(IntPtr dpiContext);

    private static readonly IntPtr DPI_AWARENESS_CONTEXT_PER_MONITOR_AWARE_V2 = new IntPtr(-4);
}