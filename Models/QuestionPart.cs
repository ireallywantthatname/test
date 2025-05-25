using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Test.Models
{
    public class QuestionPart
    {
        public int QuestionPartID { get; set; }

        [ForeignKey("Question")]
        public int QuestionID { get; set; }

        public virtual Question Question { get; set; } = null!;

        [Required]
        [StringLength(192, MinimumLength = 2)]
        public string Body { get; set; } = null!;
    }
}