using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Test.Models
{
    public class QuizFeedback
    {
        [Key]
        public int FeedbackId { get; set; }
        
        [ForeignKey("QuizAttempt")]
        public int AttemptID { get; set; }
        public virtual QuizAttempt QuizAttempt { get; set; } = null!;
        
        public string QuizName { get; set; } = null!;
        
        public string QuizDescription { get; set; } = null!;
        
        public float Score { get; set; }
        
        public float TotalPossibleScore { get; set; }
        
        [Required]
        public string FeedbackText { get; set; } = null!;
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}