using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Bus_Buddy.Models;

[Table("Schedules")]
public class Schedule
{
    [Key]
    public int ScheduleId { get; set; }

    [Required]
    public int BusId { get; set; }

    [Required]
    public int RouteId { get; set; }

    [Required]
    public int DriverId { get; set; }

    [Required]
    public DateTime DepartureTime { get; set; }

    [Required]
    public DateTime ArrivalTime { get; set; }

    [Required]
    public DateTime ScheduleDate { get; set; }

    [StringLength(20)]
    public string Status { get; set; } = "Scheduled"; // Scheduled, InProgress, Completed, Cancelled, Delayed

    [StringLength(500)]
    public string? Notes { get; set; }

    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

    public DateTime? UpdatedDate { get; set; }

    // Navigation properties
    [ForeignKey("BusId")]
    public virtual Bus Bus { get; set; } = null!;

    [ForeignKey("RouteId")]
    public virtual Route Route { get; set; } = null!;

    [ForeignKey("DriverId")]
    public virtual Driver Driver { get; set; } = null!;
}
