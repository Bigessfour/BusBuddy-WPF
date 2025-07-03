using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Bus_Buddy.Models;

/// <summary>
/// Represents a school bus driver
/// Based on Drivers Table from BusBuddy Tables schema
/// </summary>
[Table("Drivers")]
public class Driver
{
    [Key]
    public int DriverId { get; set; }

    [Required]
    [StringLength(100)]
    [Display(Name = "Driver Name")]
    public string DriverName { get; set; } = string.Empty;

    // Additional properties for better data management
    [StringLength(50)]
    [Display(Name = "First Name")]
    public string FirstName { get; set; } = string.Empty;

    [StringLength(50)]
    [Display(Name = "Last Name")]
    public string LastName { get; set; } = string.Empty;

    [StringLength(20)]
    [Display(Name = "Driver Phone")]
    public string? DriverPhone { get; set; }

    // Alias for compatibility
    [NotMapped]
    public string? Phone => DriverPhone;

    [StringLength(100)]
    [EmailAddress]
    [Display(Name = "Driver Email")]
    public string? DriverEmail { get; set; }

    [StringLength(200)]
    public string? Address { get; set; }

    [StringLength(50)]
    public string? City { get; set; }

    [StringLength(2)]
    public string? State { get; set; }

    [StringLength(10)]
    public string? Zip { get; set; }

    [Required]
    [StringLength(20)]
    [Display(Name = "Drivers Licence Type")]
    public string DriversLicenceType { get; set; } = string.Empty; // CDL or Passenger

    // Additional license-related properties
    [StringLength(20)]
    [Display(Name = "License Number")]
    public string? LicenseNumber { get; set; }

    [StringLength(10)]
    [Display(Name = "License Class")]
    public string? LicenseClass { get; set; }

    [Display(Name = "License Issue Date")]
    public DateTime? LicenseIssueDate { get; set; }

    [Display(Name = "License Expiry Date")]
    public DateTime? LicenseExpiryDate { get; set; }

    [StringLength(100)]
    [Display(Name = "Endorsements")]
    public string? Endorsements { get; set; }

    [Display(Name = "Training Complete?")]
    public bool TrainingComplete { get; set; } = false;

    [Display(Name = "Hire Date")]
    public DateTime? HireDate { get; set; }

    [StringLength(20)]
    [Display(Name = "Status")]
    public string Status { get; set; } = "Active"; // Active, Inactive, Suspended, Terminated

    // Navigation properties
    public virtual ICollection<Route> AMRoutes { get; set; } = new List<Route>();
    public virtual ICollection<Route> PMRoutes { get; set; } = new List<Route>();
    public virtual ICollection<Activity> Activities { get; set; } = new List<Activity>();
}
