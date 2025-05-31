using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Test.Models
{
    public class Answer
    {
        public int AnswerID { get; set; }

        public int AttemptID { get; set; }

        [ForeignKey("AttemptID")]
        public virtual QuizAttempt QuizAttempt { get; set; } = null!;

        public int QuestionID { get; set; }

        [ForeignKey("QuestionID")]
        public virtual Question Question { get; set; } = null!;

        [Required]
        [StringLength(192, MinimumLength = 2)]
        public string Body { get; set; } = null!;

        public float Score { get; set; }

        [Required]
        public bool IsCorrect { get; set; }
    }
}