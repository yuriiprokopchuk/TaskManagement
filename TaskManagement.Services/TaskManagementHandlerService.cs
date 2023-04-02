using TaskManagement.Bus.Infrastructure;
using TaskManagement.Services.Commands;
using TaskManagement.Services.DataContext;
using TaskManagement.Services.Exceptions;

namespace TaskManagement.Services
{
    public class TaskManagementHandlerService :
        IHandleCommand<CreateTask>,
        IHandleCommand<UpdateTask>
    {
        private readonly TaskDbContext _context;

        public TaskManagementHandlerService(TaskDbContext context)
        {
            _context = context;
        }

        public void Handle(CreateTask command)
        {
            DataContext.Entities.Task task = new DataContext.Entities.Task()
            {
                TaskName = command.TaskName,
                Description = command.Description,
                Status = command.Status,
                AssignedTo = command.AssignedTo,
            };

            _context.Tasks.Add(task);

            _context.SaveChanges();
        }

        public void Handle(UpdateTask command)
        {
            var task = _context.Tasks.FirstOrDefault(u => u.TaskId == command.TaskId);

            if (task == null) throw new NotFoundException("Task not found");

            task.Status = command.Status;
            task.AssignedTo = command.AssignedTo;

            _context.SaveChanges();
        }
    }
}
