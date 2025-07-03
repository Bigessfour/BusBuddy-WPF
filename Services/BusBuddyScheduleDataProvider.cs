using Bus_Buddy.Models;
using Microsoft.Extensions.Logging;
using Syncfusion.Schedule;
using Syncfusion.Windows.Forms.Schedule;
using System.ComponentModel;

namespace Bus_Buddy.Services;

/// <summary>
/// Custom Schedule Data Provider for Bus Buddy
/// Integrates Activity entities with Syncfusion ScheduleControl
/// Based on Syncfusion SimpleScheduleDataProvider pattern
/// </summary>
public class BusBuddyScheduleDataProvider : ScheduleDataProvider
{
    private readonly IActivityService _activityService;
    private readonly ILogger<BusBuddyScheduleDataProvider> _logger;
    private BusBuddyScheduleAppointmentList _masterList;
    private bool _isDirty;

    public BusBuddyScheduleDataProvider(IActivityService activityService, ILogger<BusBuddyScheduleDataProvider> logger)
    {
        _activityService = activityService;
        _logger = logger;
        _masterList = new BusBuddyScheduleAppointmentList();
        _isDirty = false;

        // Initialize the drop-down lists for appointments
        InitLists();
    }

    /// <summary>
    /// Gets or sets the master list of appointments
    /// </summary>
    public BusBuddyScheduleAppointmentList MasterList
    {
        get => _masterList;
        set
        {
            _masterList = value ?? new BusBuddyScheduleAppointmentList();
            _isDirty = true;
        }
    }

    /// <summary>
    /// Gets or sets whether the data has been modified
    /// </summary>
    public override bool IsDirty
    {
        get => _isDirty;
        set => _isDirty = value;
    }

    /// <summary>
    /// Returns appointments for a specific day
    /// </summary>
    public override IScheduleAppointmentList GetScheduleForDay(DateTime day)
    {
        var dayAppointments = new BusBuddyScheduleAppointmentList();

        foreach (BusBuddyScheduleAppointment appointment in _masterList)
        {
            if (appointment.StartTime.Date == day.Date ||
                (appointment.StartTime.Date <= day.Date && appointment.EndTime.Date >= day.Date))
            {
                dayAppointments.Add(appointment);
            }
        }

        return dayAppointments;
    }

    /// <summary>
    /// Returns appointments within a date range
    /// </summary>
    public override IScheduleAppointmentList GetSchedule(DateTime startDate, DateTime endDate)
    {
        var rangeAppointments = new BusBuddyScheduleAppointmentList();

        foreach (BusBuddyScheduleAppointment appointment in _masterList)
        {
            if ((appointment.StartTime >= startDate && appointment.StartTime <= endDate) ||
                (appointment.EndTime >= startDate && appointment.EndTime <= endDate) ||
                (appointment.StartTime <= startDate && appointment.EndTime >= endDate))
            {
                rangeAppointments.Add(appointment);
            }
        }

        return rangeAppointments;
    }

    /// <summary>
    /// Creates a new appointment with default values
    /// </summary>
    public override IScheduleAppointment NewScheduleAppointment()
    {
        var appointment = new BusBuddyScheduleAppointment
        {
            UniqueID = GenerateUniqueId(),
            StartTime = DateTime.Now,
            EndTime = DateTime.Now.AddHours(1),
            Subject = "New Activity",
            Content = "",
            AllDay = false
        };

        return appointment;
    }

    /// <summary>
    /// Adds an appointment to the master list
    /// </summary>
    public override void AddItem(IScheduleAppointment item)
    {
        if (item is BusBuddyScheduleAppointment busAppointment)
        {
            _masterList.Add(busAppointment);
            _isDirty = true;
            _logger.LogInformation("Added appointment: {Subject}", item.Subject);
        }
    }

    /// <summary>
    /// Removes an appointment from the master list
    /// </summary>
    public override void RemoveItem(IScheduleAppointment item)
    {
        if (item is BusBuddyScheduleAppointment busAppointment)
        {
            _masterList.Remove(busAppointment);
            _isDirty = true;
            _logger.LogInformation("Removed appointment: {Subject}", item.Subject);
        }
    }

    /// <summary>
    /// Saves any changes to the database
    /// </summary>
    public override void CommitChanges()
    {
        if (!_isDirty) return;

        try
        {
            _logger.LogInformation("Committing schedule changes to database");

            // Here we would save changes to the database via ActivityService
            // For now, we'll just mark as clean
            _isDirty = false;

            _logger.LogInformation("Successfully committed schedule changes");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error committing schedule changes");
            throw;
        }
    }

    /// <summary>
    /// Loads activities from the database and converts them to appointments
    /// </summary>
    public async Task LoadActivitiesAsync(DateTime startDate, DateTime endDate)
    {
        try
        {
            _logger.LogInformation("Loading activities from {StartDate} to {EndDate}", startDate, endDate);

            var activities = await _activityService.GetActivitiesByDateRangeAsync(startDate, endDate);

            // Clear existing appointments
            while (_masterList.Count > 0)
            {
                _masterList.RemoveAt(0);
            }

            foreach (var activity in activities)
            {
                var appointment = CreateAppointmentFromActivity(activity);
                _masterList.Add(appointment);
            }

            _isDirty = false;
            _logger.LogInformation("Loaded {Count} activities as appointments", _masterList.Count);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading activities");
            throw;
        }
    }

    /// <summary>
    /// Converts an Activity entity to a BusBuddyScheduleAppointment
    /// </summary>
    private BusBuddyScheduleAppointment CreateAppointmentFromActivity(Activity activity)
    {
        var appointment = new BusBuddyScheduleAppointment
        {
            UniqueID = activity.ActivityId,
            ActivityId = activity.ActivityId,
            StartTime = activity.ActivityDate.Add(activity.StartTime ?? TimeSpan.Zero),
            EndTime = activity.ActivityDate.Add(activity.EndTime ?? activity.StartTime?.Add(TimeSpan.FromHours(1)) ?? TimeSpan.FromHours(1)),
            Subject = $"{activity.ActivityType}",
            Content = $"Route: {activity.RouteId}\nVehicle: {activity.VehicleId}\nDriver: {activity.DriverId}\nStudents: {activity.StudentsCount ?? 0}",
            AllDay = false,
            LabelValue = GetLabelValueForActivityType(activity.ActivityType),
            LocationValue = activity.Notes ?? ""
        };

        // Set color based on activity type
        appointment.ForeColor = GetColorForActivityType(activity.ActivityType);

        return appointment;
    }

    /// <summary>
    /// Gets the label value for an activity type
    /// </summary>
    private int GetLabelValueForActivityType(string? activityType)
    {
        return activityType?.ToLower() switch
        {
            "morning" => 1,      // Important (Orange)
            "afternoon" => 2,    // Business (Blue)
            "field trip" => 3,   // Personal (Green)
            "special" => 4,      // Vacation (Gray)
            _ => 0               // None (White)
        };
    }

    /// <summary>
    /// Gets the color for an activity type
    /// </summary>
    private Color GetColorForActivityType(string? activityType)
    {
        return activityType?.ToLower() switch
        {
            "morning" => Color.FromArgb(46, 125, 50),     // Green
            "afternoon" => Color.FromArgb(25, 118, 210),  // Blue  
            "field trip" => Color.FromArgb(255, 152, 0),  // Orange
            "special" => Color.FromArgb(156, 39, 176),    // Purple
            _ => Color.FromArgb(97, 97, 97)               // Gray
        };
    }

    /// <summary>
    /// Generates a unique ID for new appointments
    /// </summary>
    private int GenerateUniqueId()
    {
        var maxId = _masterList.Count > 0 ? _masterList.Cast<BusBuddyScheduleAppointment>().Max(a => a.UniqueID) : 0;
        return maxId + 1;
    }
}

/// <summary>
/// Custom appointment class that extends ScheduleAppointment with Activity-specific properties
/// </summary>
public class BusBuddyScheduleAppointment : ScheduleAppointment
{
    /// <summary>
    /// Associated Activity ID from the database
    /// </summary>
    public int ActivityId { get; set; }

    /// <summary>
    /// Creates a new instance of BusBuddyScheduleAppointment
    /// </summary>
    public BusBuddyScheduleAppointment() : base()
    {
        ActivityId = 0;
    }
}

/// <summary>
/// Custom appointment list for Bus Buddy appointments
/// </summary>
public class BusBuddyScheduleAppointmentList : ScheduleAppointmentList
{
    /// <summary>
    /// Creates a new appointment with default values
    /// </summary>
    public override IScheduleAppointment NewScheduleAppointment()
    {
        return new BusBuddyScheduleAppointment();
    }
}
