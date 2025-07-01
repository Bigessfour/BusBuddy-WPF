using System;

namespace BusBuddy.Models
{
    public class Maintenance
    {
        public int ID { get; set; }
        public DateTime Date { get; set; }
        public int? BusID { get; set; }
        public string Description { get; set; } = string.Empty;
        public double? Cost { get; set; }
        public Bus? Bus { get; set; }
    }
}
