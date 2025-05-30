using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace Test.Models
{
    public class Question
    {
        public int QuestionID { get; set; }
    
        public string QuestionBody { get; set; } = null!;
    
        public enum Difficulty {
            easy,
            medium,
            hard
        }
    
        [Required]
        public Difficulty QuestionDifficulty { get; set; }

        [ForeignKey("Quiz")]
        public int QuizID { get; set; }

        public virtual Quiz Quiz { get; set; } = null!;
    }
}