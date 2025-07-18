using System.ComponentModel.DataAnnotations;

namespace BusBuddy.WPF.ViewModels
{
    public class ScheduleDialogViewModel
    {
        [Required]
        public int RouteId { get; set; }
        [Required]
        public int BusId { get; set; }
        [Required]
        public int DriverId { get; set; }
        [Required]
        public DateTime DepartureTime { get; set; }
        [Required]
        public DateTime ArrivalTime { get; set; }
        [Required]
        public DateTime ScheduleDate { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
