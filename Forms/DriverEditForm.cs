using Syncfusion.Windows.Forms;
using Microsoft.Extensions.Logging;
using Bus_Buddy.Services;
using Bus_Buddy.Models;
using System;
using System.Windows.Forms;

namespace Bus_Buddy.Forms;

/// <summary>
/// Driver Edit Form - placeholder for driver editing functionality
/// This will be implemented in a future iteration
/// </summary>
public partial class DriverEditForm : MetroForm
{
    private readonly ILogger<DriverEditForm> _logger;
    private readonly IBusService _busService;

    public Driver? EditedDriver { get; private set; }
    public bool IsDataSaved { get; private set; }

    public DriverEditForm(ILogger<DriverEditForm> logger, IBusService busService, Driver? driver = null)
    {
        _logger = logger;
        _busService = busService;

        _logger.LogInformation("DriverEditForm placeholder initialized");

        // For now, just show a message box
        MessageBox.Show("Driver Edit Form will be implemented in a future update.\n\nThis form will provide:\n• Driver personal information\n• License details\n• Contact information\n• Employment history",
            "Coming Soon", MessageBoxButtons.OK, MessageBoxIcon.Information);

        // Simulate successful operation for testing
        IsDataSaved = false;
        this.DialogResult = DialogResult.Cancel;
    }
}
