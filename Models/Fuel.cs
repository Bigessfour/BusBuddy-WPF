using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Bus_Buddy.Models;

/// <summary>
/// Represents a fuel record for bus fleet management
/// Based on Fuel Table from BusBuddy Tables schema
/// </summary>
[Table("Fuel")]
public class Fuel
{
    [Key]
    public int FuelId { get; set; }

    [Required]
    public int VehicleId { get; set; }

    [Required]
    [Display(Name = "Fuel Date")]
    public DateTime FuelDate { get; set; }

    [Required]
    [Column(TypeName = "decimal(8,3)")]
    [Display(Name = "Gallons")]
    public decimal Gallons { get; set; }

    [Required]
    [Column(TypeName = "decimal(8,3)")]
    [Display(Name = "Price per Gallon")]
    public decimal PricePerGallon { get; set; }

    [Required]
    [Column(TypeName = "decimal(10,2)")]
    [Display(Name = "Total Cost")]
    public decimal TotalCost { get; set; }

    [Display(Name = "Odometer Reading")]
    public int? OdometerReading { get; set; }

    [StringLength(50)]
    [Display(Name = "Fuel Station")]
    public string? FuelStation { get; set; }

    [StringLength(50)]
    [Display(Name = "Location")]
    public string? Location { get; set; }

    [StringLength(500)]
    [Display(Name = "Notes")]
    public string? Notes { get; set; }

    // Calculated properties
    [Display(Name = "Miles per Gallon")]
    public decimal? MilesPerGallon { get; set; }

    // Navigation properties
    [ForeignKey("VehicleId")]
    public virtual Bus Vehicle { get; set; } = null!;
}
