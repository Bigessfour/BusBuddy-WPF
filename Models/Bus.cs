using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Bus_Buddy.Models;

/// <summary>
/// Represents a school bus vehicle in the fleet
/// Based on Vehicle Table from BusBuddy Tables schema
/// </summary>
[Table("Vehicles")]
public class Bus
{
    [Key]
    public int VehicleId { get; set; }

    [Required]
    [StringLength(20)]
    [Display(Name = "Bus #")]
    public string BusNumber { get; set; } = string.Empty;

    [Required]
    [Range(1900, 2030)]
    public int Year { get; set; }

    [Required]
    [StringLength(50)]
    public string Make { get; set; } = string.Empty; // BlueBird, Thomas

    [Required]
    [StringLength(50)]
    public string Model { get; set; } = string.Empty;

    [Required]
    [Range(1, 90)]
    [Display(Name = "Seating Capacity")]
    public int SeatingCapacity { get; set; }

    [Required]
    [StringLength(17)]
    [Display(Name = "VIN Number")]
    public string VINNumber { get; set; } = string.Empty;

    [Required]
    [StringLength(20)]
    [Display(Name = "License Number")]
    public string LicenseNumber { get; set; } = string.Empty;

    [Display(Name = "Date Last Inspection")]
    public DateTime? DateLastInspection { get; set; }

    [Display(Name = "Current Odometer")]
    public int? CurrentOdometer { get; set; }

    [StringLength(20)]
    [Display(Name = "Current Status")]
    public string Status { get; set; } = "Active"; // Active, Maintenance, Retired

    [Display(Name = "Purchase Date")]
    public DateTime? PurchaseDate { get; set; }

    [Column(TypeName = "decimal(10,2)")]
    [Display(Name = "Purchase Price")]
    public decimal? PurchasePrice { get; set; }

    [StringLength(100)]
    [Display(Name = "Insurance Policy")]
    public string? InsurancePolicyNumber { get; set; }

    [Display(Name = "Insurance Expiry")]
    public DateTime? InsuranceExpiryDate { get; set; }

    // Navigation properties
    public virtual ICollection<Route> Routes { get; set; } = new List<Route>();
    public virtual ICollection<Activity> Activities { get; set; } = new List<Activity>();
    public virtual ICollection<Fuel> FuelRecords { get; set; } = new List<Fuel>();
    public virtual ICollection<Maintenance> MaintenanceRecords { get; set; } = new List<Maintenance>();
}
