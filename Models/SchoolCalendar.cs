using System;

namespace BusBuddy.Models
{
    public class SchoolCalendar
    {
        public int ID { get; set; }
        public DateTime Date { get; set; }
        public string Category { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public bool IsRouteDay { get; set; }
    }
}
