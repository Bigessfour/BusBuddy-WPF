using System;

namespace BusBuddy.Models
{
    public class Route
    {
        public int ID { get; set; }
        public DateTime Date { get; set; }
        public string RouteName { get; set; } = string.Empty;
        public int? AMBusID { get; set; }
        public int? AMBeginMiles { get; set; }
        public int? AMEndMiles { get; set; }
        public int? AMRiders { get; set; }
        public int? AMDriverID { get; set; }
        public int? PMBusID { get; set; }
        public int? PMBeginMiles { get; set; }
        public int? PMEndMiles { get; set; }
        public int? PMRiders { get; set; }
        public int? PMDriverID { get; set; }
        // Navigation properties (nullable, EF Core will set these)
        public Bus? AMBus { get; set; }
        public Driver? AMDriver { get; set; }
        public Bus? PMBus { get; set; }
        public Driver? PMDriver { get; set; }
    }
}
