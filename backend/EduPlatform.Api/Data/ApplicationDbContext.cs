using Microsoft.EntityFrameworkCore;
using EduPlatform.Api.Models;

namespace EduPlatform.Api.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Course> Courses { get; set; }
    public DbSet<Assessment> Assessments { get; set; }
    public DbSet<Result> Results { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure primary keys if not already done by [Key] attribute (optional here as [Key] is used)
        // modelBuilder.Entity<User>().HasKey(u => u.UserId);
        // ... and so on for other entities

        // Configure relationships

        // User (Instructor) to Course (one-to-many)
        // This relationship is already defined by ForeignKey attributes and navigation properties.
        // EF Core can infer it, but you can be explicit:
        // modelBuilder.Entity<Course>()
        //     .HasOne(c => c.Instructor)
        //     .WithMany() // If User model had a TaughtCourses collection: .WithMany(u => u.TaughtCourses)
        //     .HasForeignKey(c => c.InstructorId)
        //     .OnDelete(DeleteBehavior.Restrict); // Or Cascade, SetNull, etc.

        // Course to Assessment (one-to-many)
        modelBuilder.Entity<Assessment>()
            .HasOne(a => a.Course)
            .WithMany(c => c.Assessments)
            .HasForeignKey(a => a.CourseId);
            // .OnDelete(DeleteBehavior.Cascade); // Example: if a course is deleted, delete its assessments

        // Assessment to Result (one-to-many)
        modelBuilder.Entity<Result>()
            .HasOne(r => r.Assessment)
            .WithMany(a => a.Results)
            .HasForeignKey(r => r.AssessmentId);

        // User (Student) to Result (one-to-many)
        modelBuilder.Entity<Result>()
            .HasOne(r => r.User)
            .WithMany() // If User model had a TestResults collection: .WithMany(u => u.TestResults)
            .HasForeignKey(r => r.UserId)
            .OnDelete(DeleteBehavior.Restrict); // Prevent deleting a user if they have results, for example

        // Ensure Email is unique for User model
        modelBuilder.Entity<User>()
            .HasIndex(u => u.Email)
            .IsUnique();
            
        // Seed data (optional)
        // modelBuilder.Entity<User>().HasData(...);
    }
} 