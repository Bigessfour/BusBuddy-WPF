using System;

namespace BusBuddy.Models
{
    public class Fuel
    {
        public int ID { get; set; }
        public DateTime FuelDate { get; set; }
        public string FuelLocation { get; set; } = string.Empty;
        public int? BusID { get; set; }
        public double? Gallons { get; set; }
        public double? Cost { get; set; }
        public Bus? Bus { get; set; }
    }
}
