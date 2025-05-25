using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Test.Models
{
    public class StudentEducator
    {
        public int StudentID { get; set; }
        public virtual Student Student { get; set; } = null!;

        public int EducatorID { get; set; }
        public virtual Educator Educator { get; set; } = null!;
    }

}