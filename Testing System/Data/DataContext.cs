using Microsoft.EntityFrameworkCore;
using Testing_System.Data.Entity;

namespace Testing_System.Data
{
    public class DataContext : DbContext
    {

        public DbSet<Entity.Student> Students { get; set; }

        public DbSet<Entity.Teacher> Teachers { get; set; }

        public DbSet<Entity.Test> Tests { get; set; }

        public DbSet<Entity.Question> Questions { get; set; }

        public DbSet<Entity.Answer> Answers { get; set; }

        public DbSet<Entity.History> History { get; set; }


        public DataContext(DbContextOptions options) : base(options)
        {

        }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Question>()
                .Property(d => d.Difficulty)
                .HasConversion<string>()
                .HasColumnType("ENUM('Beginner', 'Easy', 'Medium', 'Hard', 'Advanced', 'Deep')");
        }
    }
}
