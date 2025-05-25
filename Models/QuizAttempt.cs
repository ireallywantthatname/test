using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Test.Models
{
    public class QuizAttempt
    {
        [Key]
        public int AttemptID { get; set; }

        [ForeignKey("Student")]
        public int StudentID { get; set; }

        public virtual Student Student {get; set;} = null!;

        public int QuizID { get; set; }

        [ForeignKey("QuizID")]
        public virtual Quiz Quiz {get; set;} = null!; 

        public float TotalScore {get; set;}

        public bool IsCompleted { get; set; }
    }
}