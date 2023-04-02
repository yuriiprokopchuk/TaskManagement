using Microsoft.EntityFrameworkCore;
using TaskManagement.Services.DataContext;
using TaskManagement.Services.Queries;

namespace TaskManagement.Services
{
    public class TaskManagementQyeryService
    {
        private readonly TaskDbContext _context;

        public TaskManagementQyeryService(TaskDbContext context)
        {
            _context = context;
        }

        public async Task<DataContext.Entities.Task[]> GetTasksAsync(TasksQuery tasksQuery)
        {
            var tasks = _context.Tasks.AsQueryable();

            if (tasksQuery?.Ids?.Length > 0)
                tasks = _context.Tasks.Where(u => tasksQuery.Ids.Contains(u.TaskId));

            return await tasks.ToArrayAsync();
        }
    }
}
