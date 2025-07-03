using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Bus_Buddy.Models;

[Table("RouteStops")]
public class RouteStop
{
    [Key]
    public int RouteStopId { get; set; }

    [Required]
    public int RouteId { get; set; }

    [Required]
    [StringLength(100)]
    public string StopName { get; set; } = string.Empty;

    [StringLength(200)]
    public string? StopAddress { get; set; }

    [Column(TypeName = "decimal(10,8)")]
    public decimal? Latitude { get; set; }

    [Column(TypeName = "decimal(11,8)")]
    public decimal? Longitude { get; set; }

    [Required]
    public int StopOrder { get; set; } // Order of stop in the route

    [Required]
    public TimeOnly ScheduledArrival { get; set; }

    [Required]
    public TimeOnly ScheduledDeparture { get; set; }

    [Range(0, 60)]
    public int StopDuration { get; set; } // in minutes

    [StringLength(20)]
    public string Status { get; set; } = "Active"; // Active, Inactive, Temporary

    [StringLength(500)]
    public string? Notes { get; set; }

    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

    public DateTime? UpdatedDate { get; set; }

    // Navigation properties
    [ForeignKey("RouteId")]
    public virtual Route Route { get; set; } = null!;
}
