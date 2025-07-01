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
        // Register Syncfusion Community License (replace with your actual key)
        SyncfusionLicenseProvider.RegisterLicense("YOUR LICENSE KEY");
        base.OnStartup(e);
    }
}

