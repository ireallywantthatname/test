using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;
using Test.Models;

namespace Test.Data
{
    public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<TestDbContext>
    {
        public TestDbContext CreateDbContext(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false)
                .Build();

            var builder = new DbContextOptionsBuilder<TestDbContext>();
            var connectionString = configuration.GetConnectionString("TestDB") ?? "Data Source=Data/test.db";
            
            builder.UseSqlite(connectionString);
            
            return new TestDbContext(builder.Options);
        }
    }
}