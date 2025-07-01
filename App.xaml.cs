using System.Configuration;
using System.Data;
using System.Windows;
using Syncfusion.Licensing;

namespace BusBuddy;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    protected override void OnStartup(StartupEventArgs e)
    {
        // Register Syncfusion License Key from environment variable
        string? licenseKey = Environment.GetEnvironmentVariable("SYNCFUSION_LICENSE_KEY");
        if (!string.IsNullOrEmpty(licenseKey))
        {
            SyncfusionLicenseProvider.RegisterLicense(licenseKey);
        }
        base.OnStartup(e);
    }
}

