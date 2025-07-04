using System;

namespace BusBuddy.Models
{
    public class ActivitySchedule
    {
        public int ID { get; set; }
        public DateTime Date { get; set; }
        public string TripType { get; set; } = string.Empty;
        public int? ScheduledBusID { get; set; }
        public string ScheduledDestination { get; set; } = string.Empty;
        public TimeSpan? ScheduledLeaveTime { get; set; }
        public TimeSpan? ScheduledEventTime { get; set; }
        public int? ScheduledRiders { get; set; }
        public int? ScheduledDriverID { get; set; }
        public Bus ScheduledBus { get; set; } = new Bus();
        public Driver ScheduledDriver { get; set; } = new Driver();
    }
}
