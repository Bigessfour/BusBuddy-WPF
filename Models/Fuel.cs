using System;

namespace BusBuddy.Models
{
    public class Fuel
    {
        public int ID { get; set; }
        public DateTime FuelDate { get; set; }
        public string FuelLocation { get; set; } = string.Empty;
        public int? VehicleID { get; set; }
        public int? OdometerReading { get; set; }
        public string FuelType { get; set; } = string.Empty;
        public Vehicle Vehicle { get; set; } = new Vehicle();
    }
}
