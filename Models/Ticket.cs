using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Bus_Buddy.Models;

[Table("Tickets")]
public class Ticket
{
    [Key]
    public int TicketId { get; set; }

    [Required]
    [StringLength(20)]
    public string TicketNumber { get; set; } = string.Empty;

    [Required]
    public int PassengerId { get; set; }

    [Required]
    public int ScheduleId { get; set; }

    [Required]
    [StringLength(20)]
    public string TicketType { get; set; } = "Single"; // Single, Return, Monthly, Weekly

    [Required]
    [Column(TypeName = "decimal(8,2)")]
    public decimal Price { get; set; }

    [Required]
    public DateTime PurchaseDate { get; set; } = DateTime.UtcNow;

    [Required]
    public DateTime ValidFrom { get; set; }

    [Required]
    public DateTime ValidUntil { get; set; }

    [StringLength(20)]
    public string Status { get; set; } = "Valid"; // Valid, Used, Expired, Cancelled, Refunded

    [StringLength(50)]
    public string? SeatNumber { get; set; }

    [Column(TypeName = "decimal(8,2)")]
    public decimal? DiscountAmount { get; set; }

    [StringLength(50)]
    public string? DiscountReason { get; set; }

    [StringLength(500)]
    public string? Notes { get; set; }

    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

    public DateTime? UpdatedDate { get; set; }

    // Navigation properties
    [ForeignKey("PassengerId")]
    public virtual Passenger Passenger { get; set; } = null!;

    [ForeignKey("ScheduleId")]
    public virtual Schedule Schedule { get; set; } = null!;
}
