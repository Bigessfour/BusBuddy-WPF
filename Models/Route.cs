using System;

namespace BusBuddy.Models
{
    public class Route
    {
        public int ID { get; set; }
        public DateTime Date { get; set; }
        public string RouteName { get; set; } = string.Empty;
        public int? AMVehicleID { get; set; }
        public int? AMBeginMiles { get; set; }
        public int? AMEndMiles { get; set; }
        public int? AMRiders { get; set; }
        public int? AMDriverID { get; set; }
        public int? PMVehicleID { get; set; }
        public int? PMBeginMiles { get; set; }
        public int? PMEndMiles { get; set; }
        public int? PMRiders { get; set; }
        public int? PMDriverID { get; set; }
        public Vehicle AMVehicle { get; set; } = new Vehicle();
        public Driver AMDriver { get; set; } = new Driver();
        public Vehicle PMVehicle { get; set; } = new Vehicle();
        public Driver PMDriver { get; set; } = new Driver();
    }
}
