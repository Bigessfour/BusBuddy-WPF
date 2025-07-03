using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Bus_Buddy.Models;

/// <summary>
/// Represents a maintenance record for bus fleet management
/// Based on Maintenance Table from BusBuddy Tables schema
/// </summary>
[Table("Maintenance")]
public class Maintenance
{
    [Key]
    public int MaintenanceId { get; set; }

    [Required]
    public int VehicleId { get; set; }

    [Required]
    [Display(Name = "Maintenance Date")]
    public DateTime MaintenanceDate { get; set; }

    [Required]
    [StringLength(50)]
    [Display(Name = "Maintenance Type")]
    public string MaintenanceType { get; set; } = string.Empty; // Oil Change, Inspection, Repair

    [Required]
    [StringLength(500)]
    [Display(Name = "Description")]
    public string Description { get; set; } = string.Empty;

    [Column(TypeName = "decimal(10,2)")]
    [Display(Name = "Cost")]
    public decimal? Cost { get; set; }

    [Display(Name = "Odometer Reading")]
    public int? OdometerReading { get; set; }

    [StringLength(100)]
    [Display(Name = "Performed By")]
    public string? PerformedBy { get; set; }

    [StringLength(100)]
    [Display(Name = "Shop/Vendor")]
    public string? ShopVendor { get; set; }

    [Display(Name = "Next Service Due")]
    public DateTime? NextServiceDue { get; set; }

    [Display(Name = "Next Service Odometer")]
    public int? NextServiceOdometer { get; set; }

    [StringLength(20)]
    [Display(Name = "Status")]
    public string Status { get; set; } = "Completed"; // Scheduled, In Progress, Completed

    [StringLength(1000)]
    [Display(Name = "Notes")]
    public string? Notes { get; set; }

    // Navigation properties
    [ForeignKey("VehicleId")]
    public virtual Bus Vehicle { get; set; } = null!;
}
