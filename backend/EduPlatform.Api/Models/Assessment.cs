using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EduPlatform.Api.Models;

public class Assessment
{
    [Key]
    public Guid AssessmentId { get; set; }

    [Required]
    public Guid CourseId { get; set; } // Foreign Key to Course

    [ForeignKey("CourseId")]
    public virtual Course? Course { get; set; } // Navigation property

    [Required]
    [StringLength(200)]
    public string Title { get; set; } = string.Empty;

    // For JSON content, EF Core can map this to a string column by default.
    // For Azure SQL, you might use nvarchar(max).
    // If you need to query into the JSON, consider specific Azure SQL JSON features or a NoSQL store for this part.
    [Required]
    public string Questions { get; set; } = string.Empty; // JSON string

    public int MaxScore { get; set; }

    // Navigation property for results of this assessment
    public virtual ICollection<Result>? Results { get; set; }
} 