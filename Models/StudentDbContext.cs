using Microsoft.EntityFrameworkCore;

namespace Ganesh1.Models
{
    public class StudentDbContext : DbContext
    {
        public StudentDbContext(DbContextOptions<StudentDbContext> options) : base(options) { }

        public DbSet<Student> Students { get; set; }
        public DbSet<Passout> Passouts { get; set; }
        public DbSet<Teacher> Teachers { get; set; }
        public DbSet<Stock> Stocks { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Passout>(entity =>
            {
                entity.Property(e => e.Percentage)
                      .HasColumnType("decimal(5,2)");
            });

            modelBuilder.Entity<Stock>(entity =>
            {
                entity.Property(e => e.UnitPrice)
                      .HasColumnType("decimal(10,2)");
            });
        }
    }
}
