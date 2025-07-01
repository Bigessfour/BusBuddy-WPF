using System;

namespace BusBuddy.Models
{
    public class Maintenance
    {
        public int ID { get; set; }
        public DateTime Date { get; set; }
        public int? VehicleID { get; set; }
        public int? OdometerReading { get; set; }
        public string MaintenanceType { get; set; } = string.Empty;
        public string Vendor { get; set; } = string.Empty;
        public decimal? RepairCost { get; set; }
        public Vehicle Vehicle { get; set; } = new Vehicle();
    }
}
