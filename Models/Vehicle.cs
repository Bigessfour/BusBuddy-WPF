using System;

// This file is deprecated and should be removed. All vehicle-related logic is now handled by Bus.cs and the Bus entity.

namespace BusBuddy.Models
{
    public class Vehicle
    {
        public int ID { get; set; }
        public string BusNumber { get; set; } = string.Empty;
        public int? Year { get; set; }
        public string Make { get; set; } = string.Empty;
        public string Model { get; set; } = string.Empty;
        public int? SeatingCapacity { get; set; }
        public string VIN { get; set; } = string.Empty;
        public string LicenseNumber { get; set; } = string.Empty;
        public DateTime? LastInspectionDate { get; set; }
    }
}
