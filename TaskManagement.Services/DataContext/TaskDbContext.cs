using Microsoft.EntityFrameworkCore;

namespace TaskManagement.Services.DataContext
{
    public class TaskDbContext : DbContext
    {
        public TaskDbContext() { }

        public TaskDbContext(DbContextOptions options) : base(options) { }

        public DbSet<Entities.Task> Tasks { get; set; }
    }
}
