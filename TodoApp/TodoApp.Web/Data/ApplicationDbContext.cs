using Microsoft.EntityFrameworkCore;
using TodoApp.Web.Models.Entities;

namespace TodoApp.Web.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) 
        {
        
        }
        public DbSet<Category> Categories{ get; set; }
        public DbSet<TodoApp.Web.Models.Entities.Task> Tasks { get; set; }
    }
}
