using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Bus_Buddy.Models;

/// <summary>
/// Represents a student in the school bus system
/// Based on Students Table from BusBuddy Tables schema
/// </summary>
[Table("Students")]
public class Student
{
    [Key]
    public int StudentId { get; set; }

    [Required]
    [StringLength(100)]
    [Display(Name = "Student Name")]
    public string StudentName { get; set; } = string.Empty;

    [StringLength(20)]
    [Display(Name = "Student Number")]
    public string? StudentNumber { get; set; }

    [StringLength(20)]
    [Display(Name = "Grade")]
    public string? Grade { get; set; }

    [StringLength(100)]
    [Display(Name = "School")]
    public string? School { get; set; }

    [StringLength(200)]
    [Display(Name = "Home Address")]
    public string? HomeAddress { get; set; }

    [StringLength(50)]
    [Display(Name = "City")]
    public string? City { get; set; }

    [StringLength(2)]
    [Display(Name = "State")]
    public string? State { get; set; }

    [StringLength(10)]
    [Display(Name = "Zip Code")]
    public string? Zip { get; set; }

    [StringLength(20)]
    [Display(Name = "Home Phone")]
    public string? HomePhone { get; set; }

    [StringLength(100)]
    [Display(Name = "Parent/Guardian")]
    public string? ParentGuardian { get; set; }

    [StringLength(20)]
    [Display(Name = "Emergency Phone")]
    public string? EmergencyPhone { get; set; }

    [StringLength(1000)]
    [Display(Name = "Medical Notes")]
    public string? MedicalNotes { get; set; }

    [StringLength(1000)]
    [Display(Name = "Transportation Notes")]
    public string? TransportationNotes { get; set; }

    [Display(Name = "Active")]
    public bool Active { get; set; } = true;

    [Display(Name = "Enrollment Date")]
    public DateTime? EnrollmentDate { get; set; }

    // Bus assignment information
    [StringLength(50)]
    [Display(Name = "AM Route")]
    public string? AMRoute { get; set; }

    [StringLength(50)]
    [Display(Name = "PM Route")]
    public string? PMRoute { get; set; }

    [StringLength(50)]
    [Display(Name = "Bus Stop")]
    public string? BusStop { get; set; }
}
