using Microsoft.Extensions.Logging;
using Syncfusion.Windows.Forms;
using Syncfusion.Licensing;

namespace Bus_Buddy;

static class Program
{
    /// <summary>
    ///  The main entry point for the application.
    /// </summary>
    [STAThread]
    static void Main()
    {
        Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("Ngo9BigBOggjHTQxAR8/V1JEaF5cXmRCf1FpRmJGdld5fUVHYVZUTXxaS00DNHVRdkdmWXhec3RSRGRYU0R2WUBWYEk=");

        // Initialize the service container
        ServiceContainer.Initialize();

        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);

        // Resolve the Dashboard from the service container
        var dashboard = ServiceContainer.GetService<Dashboard>();
        Application.Run(dashboard);
    }
}