using System.ComponentModel.DataAnnotations;

namespace EduPlatform.Api.Models;

public class User
{
    [Key] // Specifies this property as the primary key
    public Guid UserId { get; set; }

    [Required]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    [StringLength(256)]
    public string Email { get; set; } = string.Empty;

    [Required]
    [StringLength(20)] // "Student" or "Instructor"
    public string Role { get; set; } = string.Empty;

    [Required]
    public string PasswordHash { get; set; } = string.Empty;

    // Navigation properties (if any, e.g., courses for an instructor)
    // For example, if an instructor can have multiple courses:
    // public virtual ICollection<Course>? TaughtCourses { get; set; }

    // If a student can have multiple results:
    // public virtual ICollection<Result>? TestResults { get; set; }
} 