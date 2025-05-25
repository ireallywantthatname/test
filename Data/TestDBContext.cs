using Microsoft.EntityFrameworkCore;

namespace Test.Models
{
    public class TestDbContext(IConfiguration configuration) : DbContext
    {
        protected readonly IConfiguration Configuration = configuration;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite(Configuration.GetConnectionString("TestDB"));
        }
        public virtual DbSet<Quiz> Quizzes { get; set; } = null!;
        public virtual DbSet<Question> Questions { get; set; } = null!;
        public virtual DbSet<Answer> Answers { get; set; } = null!;
        public virtual DbSet<Student> Students { get; set; } = null!;
        public virtual DbSet<Educator> Educators { get; set; } = null!;
        public virtual DbSet<QuestionPart> QuestionParts { get; set; } = null!;
        public virtual DbSet<QuizAttempt> QuizAttempts { get; set; } = null!;
        public virtual DbSet<StudentEducator> StudentEducators { get; set; } = null!;

        // to handle many-to-many relationship
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<StudentEducator>()
                .HasKey(se => new { se.StudentID, se.EducatorID });

            modelBuilder.Entity<StudentEducator>()
                .HasOne(se => se.Student)
                .WithMany(s => s.StudentEducators)
                .HasForeignKey(se => se.StudentID);

            modelBuilder.Entity<StudentEducator>()
                .HasOne(se => se.Educator)
                .WithMany(e => e.StudentEducators)
                .HasForeignKey(se => se.EducatorID);
        }
    }


}
