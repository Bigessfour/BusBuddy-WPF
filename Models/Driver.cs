namespace BusBuddy.Models
{
    public class Driver
    {
        public int ID { get; set; }
        public string DriverName { get; set; } = string.Empty;
        public string DriverPhone { get; set; } = string.Empty;
        public string DriverEmail { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string State { get; set; } = string.Empty;
        public string Zip { get; set; } = string.Empty;
        public string LicenseType { get; set; } = string.Empty;
        public bool? TrainingComplete { get; set; }
    }
}
