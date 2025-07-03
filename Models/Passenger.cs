using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Bus_Buddy.Models;

[Table("Passengers")]
public class Passenger
{
    [Key]
    public int PassengerId { get; set; }

    [Required]
    [StringLength(50)]
    public string FirstName { get; set; } = string.Empty;

    [Required]
    [StringLength(50)]
    public string LastName { get; set; } = string.Empty;

    [NotMapped]
    public string FullName => $"{FirstName} {LastName}";

    [StringLength(100)]
    public string? Email { get; set; }

    [StringLength(20)]
    public string? Phone { get; set; }

    [StringLength(200)]
    public string? Address { get; set; }

    public DateTime? DateOfBirth { get; set; }

    [StringLength(20)]
    public string PassengerType { get; set; } = "Regular"; // Regular, Student, Senior, Disabled

    [StringLength(50)]
    public string? EmergencyContact { get; set; }

    [StringLength(20)]
    public string? EmergencyPhone { get; set; }

    [StringLength(20)]
    public string Status { get; set; } = "Active"; // Active, Inactive, Suspended

    [StringLength(500)]
    public string? Notes { get; set; }

    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

    public DateTime? UpdatedDate { get; set; }

    // Navigation properties
    public virtual ICollection<Schedule> Schedules { get; set; } = new List<Schedule>();
    public virtual ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();
}
