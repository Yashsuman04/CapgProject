using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EduPlatform.Api.Models;

public class Result
{
    [Key]
    public Guid ResultId { get; set; }

    [Required]
    public Guid AssessmentId { get; set; } // Foreign Key to Assessment

    [ForeignKey("AssessmentId")]
    public virtual Assessment? Assessment { get; set; } // Navigation property

    [Required]
    public Guid UserId { get; set; } // Foreign Key to User

    [ForeignKey("UserId")]
    public virtual User? User { get; set; } // Navigation property

    public int Score { get; set; }

    public DateTime AttemptDate { get; set; }
} 