using System.ComponentModel.DataAnnotations;

namespace Test.Models
{
    public class Question
    {
        public int Id { get; set; }

        [Required]
        [StringLength(384, MinimumLength = 2)]
        public string Text { get; set; } = null!;

        [Required]
        public string Difficulty { get; set; } = null!;

        public int QuizId { get; set; }

        public virtual Quiz Quiz { get; set; } = null!;
    }
}