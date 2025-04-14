using System.ComponentModel.DataAnnotations;

namespace Test.Models
{
    public class Quiz
    {
        [Required]
        public int Id { get; set; }

        [Required]
        [StringLength(48, MinimumLength = 2)]
        public string Title { get; set; } = null!;

        [StringLength(192, MinimumLength = 2)]
        public string Description { get; set; } = null!;
    }
}