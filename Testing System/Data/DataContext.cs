using Microsoft.EntityFrameworkCore;

namespace Testing_System.Data
{
    public class DataContext : DbContext
    {

        public DbSet<Entity.User> Users { get; set; }


        public DataContext(DbContextOptions options) : base(options)
        {

        }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

        }
    }
}
