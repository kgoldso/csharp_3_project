using Microsoft.EntityFrameworkCore;
using _3_project.Models;

namespace _3_project.Data
{
    /// <summary>
    /// Entity Framework database context for the application.
    /// </summary>
    public class AppDbContext : DbContext
    {
        public DbSet<Person> People { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(
                    "Server=localhost;Database=CsvPeopleDb;Trusted_Connection=True;TrustServerCertificate=True;");
            }
        }
    }
}
