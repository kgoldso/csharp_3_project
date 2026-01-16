using Microsoft.EntityFrameworkCore;
using _3_project.Models;

namespace _3_project.Data
{
    /// <summary>
    /// Entity Framework database context.
    /// </summary>
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Person> People { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Person>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.FirstName).HasMaxLength(100).IsRequired();
                entity.Property(e => e.LastName).HasMaxLength(100).IsRequired();
                entity.Property(e => e.SurName).HasMaxLength(100).IsRequired();
                entity.Property(e => e.City).HasMaxLength(100).IsRequired();
                entity.Property(e => e.Country).HasMaxLength(100).IsRequired();
                
                // Индексы для оптимизации поиска
                entity.HasIndex(e => e.FirstName);
                entity.HasIndex(e => e.LastName);
                entity.HasIndex(e => e.City);
                entity.HasIndex(e => e.Country);
                entity.HasIndex(e => e.Date);
            });
        }
    }
}
