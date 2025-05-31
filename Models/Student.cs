using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Test.Models
{
    public class Student
    {
        public int StudentId { get; set; }

        [Required(ErrorMessage = "Username is required")]
        [StringLength(20, MinimumLength = 3, ErrorMessage = "Username must be between 3 and 20 characters")]
        [RegularExpression(@"^[a-zA-Z]+[a-zA-Z0-9_]*[0-9]*$", 
            ErrorMessage = "Username must start with a letter and can only contain letters, numbers, and underscores")]
        public string Username { get; set; } = null!;

        [Required(ErrorMessage = "Password is required")]
        [RegularExpression(@"^(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])(?=.*?[#?!@$%^&*-]).{8,}$", 
            ErrorMessage = "Password must be at least 8 characters and include uppercase, lowercase, number, and special character")]
        public string Password { get; set; } = null!;
    
        public virtual ICollection<StudentEducator> StudentEducators { get; set; } = new List<StudentEducator>();
    }
}