// Suggestion: Create a Dtos folder, e.g., backend/EduPlatform.Api/Dtos/CourseDtos.cs

// For creating a course
public class CourseForCreationDto
{
    [Required]
    [StringLength(200, MinimumLength = 3)]
    public string Title { get; set; } = string.Empty;

    [StringLength(1000)]
    public string? Description { get; set; }

    // InstructorId will be taken from the authenticated user's claims
    // MediaUrl might be handled separately or set after creation
}

// For updating a course
public class CourseForUpdateDto
{
    [Required]
    [StringLength(200, MinimumLength = 3)]
    public string Title { get; set; } = string.Empty;

    [StringLength(1000)]
    public string? Description { get; set; }
    
    // MediaUrl might be updatable
    [StringLength(500)]
    public string? MediaUrl { get; set; }
}

// For representing a course in API responses
public class CourseDto
{
    public Guid CourseId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public Guid InstructorId { get; set; }
    public string? InstructorName { get; set; } // Good to include
    public string? MediaUrl { get; set; }
    public DateTime CreatedAt { get; set; } // Assuming you add this to your Course model
    public DateTime UpdatedAt { get; set; } // Assuming you add this to your Course model
    // public int EnrolledStudentsCount { get; set; } // Example of additional info
}