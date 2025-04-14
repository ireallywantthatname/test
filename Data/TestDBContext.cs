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
        public virtual DbSet<User> Users { get; set; } = null!;
    }
}
