using Microsoft.EntityFrameworkCore;

namespace Test.Data
{
    public class TestDbContext : DbContext
    {
        public TestDbContext()
        {
        }
        public TestDbContext(DbContextOptions<TestDbContext> options) : base(options)
        {
        }
        public virtual DbSet<Paper> Papers { get; set; } = null!;
        public virtual DbSet<Question> Questions { get; set; } = null!;
        public virtual DbSet<Answer> Answers { get; set; } = null!;
        // protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        // {
        //     optionsBuilder.UseSqlite(Configuration.GetConnectionString("EmployeeDB"));
        // }
        protected override void OnConfiguring(DbContextOptionsBuilder options)
    => options.UseSqlite($"Data Source=Data/test.db");
    }
}
