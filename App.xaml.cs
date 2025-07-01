using System;
using System.Windows;
using Syncfusion.Licensing;

namespace BusBuddy;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    public App()
    {
        // Register Syncfusion® license - Official WPF Implementation
        // Based on: https://help.syncfusion.com/common/essential-studio/licensing/how-to-register-in-an-application#wpf
        
        // Official Syncfusion license registration guidance:
        // https://help.syncfusion.com/common/essential-studio/licensing/how-to-register-in-an-application#wpf
        //
        // This code reads the license key from the SYNCFUSION_LICENSE_KEY environment variable (process, user, or machine scope).
        // This is secure, supported, and fully compliant with Syncfusion's requirements.
        //
        // If the key is not found, the app will show trial warnings.
        
        // Try multiple sources for license key (in order of preference)
        string? licenseKey = null;
        
        // 1. Environment variable (most secure for production)
        licenseKey = Environment.GetEnvironmentVariable("SYNCFUSION_LICENSE_KEY");
        
        // 2. User environment variable fallback
        if (string.IsNullOrEmpty(licenseKey))
        {
            licenseKey = Environment.GetEnvironmentVariable("SYNCFUSION_LICENSE_KEY", EnvironmentVariableTarget.User);
        }
        
        // 3. Machine environment variable fallback
        if (string.IsNullOrEmpty(licenseKey))
        {
            licenseKey = Environment.GetEnvironmentVariable("SYNCFUSION_LICENSE_KEY", EnvironmentVariableTarget.Machine);
        }
        
        // Register license if found
        if (!string.IsNullOrWhiteSpace(licenseKey))
        {
            SyncfusionLicenseProvider.RegisterLicense(licenseKey!);
        }
        else
        {
            // License key not found - application will show trial warnings
            System.Diagnostics.Debug.WriteLine("WARNING: No Syncfusion license key found in environment variables.");
            System.Diagnostics.Debug.WriteLine("Set environment variable: SYNCFUSION_LICENSE_KEY=your_license_key");
            System.Diagnostics.Debug.WriteLine("Trial license warnings may appear until license is properly configured.");
        }
    }
}

