using System.ComponentModel.DataAnnotations;

namespace Test.Models
{
    public class Answer
    {
        public int Id { get; set; }

        [Required]
        [StringLength(192, MinimumLength = 2)]
        public string Text { get; set; } = null!;

        [Required]
        public bool IsCorrect { get; set; }

        public int QuestionId { get; set; }

        public virtual Question Question { get; set; } = null!;
    }
}