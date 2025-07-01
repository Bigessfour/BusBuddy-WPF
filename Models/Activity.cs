using System;

namespace BusBuddy.Models
{
    public class Activity
    {
        public int ID { get; set; }
        public DateTime Date { get; set; }
        public string ActivityType { get; set; } = string.Empty;
        public string Destination { get; set; } = string.Empty;
        public TimeSpan? LeaveTime { get; set; }
        public TimeSpan? EventTime { get; set; }
        public string RequestedBy { get; set; } = string.Empty;
        public int? AssignedBusID { get; set; }
        public int? AssignedDriverID { get; set; }
        public Bus AssignedBus { get; set; } = new Bus();
        public Driver AssignedDriver { get; set; } = new Driver();
    }
}
