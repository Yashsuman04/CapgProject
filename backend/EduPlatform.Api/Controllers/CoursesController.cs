// DTOs would ideally be in their own files/folder e.g., EduPlatform.Api.Dtos
// For brevity in this example, they are defined here if not already in AuthController or similar.
// Make sure to resolve using statements if they are in separate files.
// Example DTOs (ensure these match or are defined elsewhere):
public class CourseForCreationDto
{
    [Required]
    [StringLength(200, MinimumLength = 3)]
    public string Title { get; set; } = string.Empty;
    [StringLength(1000)]
    public string? Description { get; set; }
    [StringLength(500)]
    public string? MediaUrl { get; set; }
}

public class CourseForUpdateDto
{
    [Required]
    [StringLength(200, MinimumLength = 3)]
    public string Title { get; set; } = string.Empty;
    [StringLength(1000)]
    public string? Description { get; set; }
    [StringLength(500)]
    public string? MediaUrl { get; set; }
}

public class CourseDto // Response DTO
{
    public Guid CourseId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public Guid InstructorId { get; set; }
    public string? InstructorName { get; set; } // Denormalized for convenience
    public string? MediaUrl { get; set; }
    // public DateTime CreatedAt { get; set; }
    // public DateTime UpdatedAt { get; set; }
}

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using EduPlatform.Api.Data;
using EduPlatform.Api.Models;
using System.ComponentModel.DataAnnotations; // For DTO attributes if defined here

namespace EduPlatform.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CoursesController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public CoursesController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: api/courses
    [HttpGet]
    public async Task<ActionResult<IEnumerable<CourseDto>>> GetCourses()
    {
        var courses = await _context.Courses
            .Include(c => c.Instructor) // Include instructor to get their name
            .Select(c => new CourseDto
            {
                CourseId = c.CourseId,
                Title = c.Title,
                Description = c.Description,
                InstructorId = c.InstructorId,
                InstructorName = c.Instructor != null ? c.Instructor.Name : null,
                MediaUrl = c.MediaUrl
                // CreatedAt = c.CreatedAt, // If you add these fields to Course model
                // UpdatedAt = c.UpdatedAt
            })
            .ToListAsync();
        return Ok(courses);
    }

    // GET: api/courses/{id}
    [HttpGet("{id}")]
    public async Task<ActionResult<CourseDto>> GetCourse(Guid id)
    {
        var course = await _context.Courses
            .Include(c => c.Instructor)
            .Where(c => c.CourseId == id)
            .Select(c => new CourseDto
            {
                CourseId = c.CourseId,
                Title = c.Title,
                Description = c.Description,
                InstructorId = c.InstructorId,
                InstructorName = c.Instructor != null ? c.Instructor.Name : null,
                MediaUrl = c.MediaUrl
            })
            .FirstOrDefaultAsync();

        if (course == null)
        {
            return NotFound();
        }
        return Ok(course);
    }

    // POST: api/courses
    [HttpPost]
    [Authorize(Roles = "Instructor")] // Only instructors can create courses
    public async Task<ActionResult<CourseDto>> CreateCourse(CourseForCreationDto courseDto)
    {
        var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier); // Get user ID from token
        if (!Guid.TryParse(userIdString, out Guid instructorId))
        {
            return Unauthorized("Invalid user identifier.");
        }

        var course = new Course
        {
            CourseId = Guid.NewGuid(),
            Title = courseDto.Title,
            Description = courseDto.Description,
            InstructorId = instructorId, // Set instructor from authenticated user
            MediaUrl = courseDto.MediaUrl,
            // CreatedAt = DateTime.UtcNow, // Set these if added to model
            // UpdatedAt = DateTime.UtcNow
        };

        _context.Courses.Add(course);
        await _context.SaveChangesAsync();

        // It's good practice to load the instructor name for the response DTO
        var instructor = await _context.Users.FindAsync(instructorId);

        var courseResponse = new CourseDto
        {
            CourseId = course.CourseId,
            Title = course.Title,
            Description = course.Description,
            InstructorId = course.InstructorId,
            InstructorName = instructor?.Name,
            MediaUrl = course.MediaUrl,
            // CreatedAt = course.CreatedAt,
            // UpdatedAt = course.UpdatedAt
        };

        return CreatedAtAction(nameof(GetCourse), new { id = course.CourseId }, courseResponse);
    }

    // PUT: api/courses/{id}
    [HttpPut("{id}")]
    [Authorize(Roles = "Instructor")]
    public async Task<IActionResult> UpdateCourse(Guid id, CourseForUpdateDto courseUpdateDto)
    {
        var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!Guid.TryParse(userIdString, out Guid currentUserId))
        {
            return Unauthorized("Invalid user identifier.");
        }

        var course = await _context.Courses.FindAsync(id);

        if (course == null)
        {
            return NotFound("Course not found.");
        }

        // Authorization check: Only the instructor who created the course can update it.
        if (course.InstructorId != currentUserId)
        {
            return Forbid("You are not authorized to update this course.");
        }

        course.Title = courseUpdateDto.Title;
        course.Description = courseUpdateDto.Description;
        course.MediaUrl = courseUpdateDto.MediaUrl;
        // course.UpdatedAt = DateTime.UtcNow; // If you add this field

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!_context.Courses.Any(e => e.CourseId == id))
            {
                return NotFound();
            }
            else
            {
                throw;
            }
        }
        return NoContent(); // Or Ok(updatedCourseDto) if you return the updated entity
    }

    // DELETE: api/courses/{id}
    [HttpDelete("{id}")]
    [Authorize(Roles = "Instructor")]
    public async Task<IActionResult> DeleteCourse(Guid id)
    {
        var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!Guid.TryParse(userIdString, out Guid currentUserId))
        {
            return Unauthorized("Invalid user identifier.");
        }

        var course = await _context.Courses.FindAsync(id);
        if (course == null)
        {
            return NotFound("Course not found.");
        }

        // Authorization check: Only the instructor who created the course can delete it.
        if (course.InstructorId != currentUserId)
        {
            return Forbid("You are not authorized to delete this course.");
        }

        _context.Courses.Remove(course);
        await _context.SaveChangesAsync();

        return NoContent();
    }
} 