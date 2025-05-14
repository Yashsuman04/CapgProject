using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EduPlatform.Api.Models;

public class Course
{
    [Key]
    public Guid CourseId { get; set; }

    [Required]
    [StringLength(200)]
    public string Title { get; set; } = string.Empty;

    [StringLength(1000)]
    public string? Description { get; set; }

    [Required]
    public Guid InstructorId { get; set; } // Foreign Key to User

    [ForeignKey("InstructorId")]
    public virtual User? Instructor { get; set; } // Navigation property

    [StringLength(500)]
    public string? MediaUrl { get; set; } // Link to Blob Storage

    // Navigation property for assessments in this course
    public virtual ICollection<Assessment>? Assessments { get; set; }

    // In backend/EduPlatform.Api/Models/Course.cs
public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
} 