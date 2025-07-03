using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Bus_Buddy.Models;

/// <summary>
/// Represents an activity record for bus operations
/// Based on Activity Table from BusBuddy Tables schema
/// </summary>
[Table("Activity")]
public class Activity
{
    [Key]
    public int ActivityId { get; set; }

    [Required]
    public int VehicleId { get; set; }

    [Required]
    public int RouteId { get; set; }

    [Required]
    public int DriverId { get; set; }

    [Required]
    [Display(Name = "Activity Date")]
    public DateTime ActivityDate { get; set; }

    [Required]
    [StringLength(10)]
    [Display(Name = "Activity Type")]
    public string ActivityType { get; set; } = string.Empty; // Morning, Afternoon, Field Trip

    [Display(Name = "Start Time")]
    public TimeSpan? StartTime { get; set; }

    [Display(Name = "End Time")]
    public TimeSpan? EndTime { get; set; }

    [Display(Name = "Start Odometer")]
    public int? StartOdometer { get; set; }

    [Display(Name = "End Odometer")]
    public int? EndOdometer { get; set; }

    [StringLength(500)]
    [Display(Name = "Notes")]
    public string? Notes { get; set; }

    [Display(Name = "Students Count")]
    public int? StudentsCount { get; set; }

    // Navigation properties
    [ForeignKey("VehicleId")]
    public virtual Bus Vehicle { get; set; } = null!;

    [ForeignKey("RouteId")]
    public virtual Route Route { get; set; } = null!;

    [ForeignKey("DriverId")]
    public virtual Driver Driver { get; set; } = null!;
}
