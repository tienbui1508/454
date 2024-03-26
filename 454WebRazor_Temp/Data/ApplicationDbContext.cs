using _454WebRazor_Temp.Models;
using Microsoft.EntityFrameworkCore;

namespace _454WebRazor_Temp.Models
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) 
        {
            
        }

        public DbSet<Category> Categories { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Category>().HasData(
                new Category { Id = 1, Name = "New", DisplayOrder = 1 },
                new Category { Id = 2, Name = "2D", DisplayOrder = 2 },
                new Category { Id = 3, Name = "3D", DisplayOrder = 3 }
                );
        }
    }
}
