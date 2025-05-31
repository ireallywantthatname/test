using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;

namespace Test.Models
{
    public class TestDbContext : IdentityDbContext<IdentityUser>
    {
        private readonly IConfiguration? _configuration;

        // Default constructor for EF Core tooling
        public TestDbContext(DbContextOptions<TestDbContext> options)
            : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // Nothing needed here as options are configured in Program.cs
            // and in DesignTimeDbContextFactory for migrations
        }

        public virtual DbSet<Quiz> Quizzes { get; set; } = null!;
        public virtual DbSet<Question> Questions { get; set; } = null!;
        public virtual DbSet<Answer> Answers { get; set; } = null!;
        public virtual DbSet<Student> Students { get; set; } = null!;
        public virtual DbSet<Educator> Educators { get; set; } = null!;
        public virtual DbSet<QuizAttempt> QuizAttempts { get; set; } = null!;
        public virtual DbSet<StudentEducator> StudentEducators { get; set; } = null!;

        // to handle many-to-many relationship
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder); // Important: This adds the Identity model configuration

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
