using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Test.Models
{
    public class Quiz
    {
        public int QuizId { get; set; }

        [Required]
        [StringLength(48, MinimumLength = 2)]
        public string QuizName { get; set; } = null!;

        [StringLength(256, MinimumLength = 8)]
        public string QuizDescription { get; set; } = null!;

        [ForeignKey("EducatorID")]
        public virtual Educator Educator {get; set;} = null!;

        public int EducatorID { get; set; }
    }
}