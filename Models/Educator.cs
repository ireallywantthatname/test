using System.ComponentModel.DataAnnotations;

namespace Test.Models
{
   public class Educator
    {
        public int EducatorID {get; set; }

        [Required]
        [StringLength(12, MinimumLength = 8)]
        [RegularExpression(@"^[a-zA-Z]+[a-zA-Z0-9_]*[0-9]*$")]
        public string Username {get; set; } = null!;

        [Required]
        [StringLength(24, MinimumLength = 8)]
        [RegularExpression(@"^(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])(?=.*?[#?!@$%^&*-]).{8,}$")]
        public string Password {get; set; } = null!;

        [Required]
        [Compare("Password")]
        public string ConfirmPassword { get; set; } = null!;

        public virtual ICollection<StudentEducator> StudentEducators { get; set; } = new List<StudentEducator>();
    }
}