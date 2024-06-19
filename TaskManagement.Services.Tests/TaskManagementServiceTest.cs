using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using TaskManagement.Services.Commands;
using TaskManagement.Services.DataContext;
using TaskManagement.Services.DataContext.Entities;
using TaskManagement.Services.Exceptions;
using TaskManagement.Services.Queries;
using Task = System.Threading.Tasks.Task;

namespace TaskManagement.Services.Tests
{
    [TestClass]
    public class TaskManagementServiceTest : IDisposable
    {
        private readonly TaskManagementHandlerService _taskManagementHandlerService;
        private readonly TaskManagementQyeryService _taskManagementQyeryService;
        private readonly TaskDbContext _taskDbContext;

        public TaskManagementServiceTest()
        {

            _taskDbContext = new TaskDbContext(new DbContextOptionsBuilder<TaskDbContext>().UseInMemoryDatabase("TaskManagement.Test").Options);
            _taskManagementHandlerService = new TaskManagementHandlerService(_taskDbContext);
            _taskManagementQyeryService = new TaskManagementQyeryService(_taskDbContext);

            _taskDbContext.Database.EnsureDeleted();
            _taskDbContext.Database.EnsureCreated();
        }

        [TestMethod]
        public async Task CreateTaskTest()
        {
            var taskName = "taskNameTest";
            var description = "descriptionTest";
            var status = Status.NotStarted;
            var assignedTo = "AssignedTo@mail.test";

            CreateTask createTask = new CreateTask()
            {
                TaskName = taskName,
                Description = description,
                Status = status,
                AssignedTo = assignedTo
            };

            _taskManagementHandlerService.Handle(createTask);

            var tasks = await _taskManagementQyeryService.GetTasksAsync(new TasksQuery());

            tasks.Should().HaveCount(1);

            var task = tasks.First();

            task.TaskName.Should().Be(taskName);
            task.Description.Should().Be(description);
            task.Status.Should().Be(status);
            task.AssignedTo.Should().Be(assignedTo);
            task.UpdatedBy.Should().BeNull();
        }

        [TestMethod]
        public async Task UpdateTaskTest()
        {
            var taskName = "taskNameTest";
            var description = "descriptionTest";
            var status = Status.NotStarted;
            var assignedTo = "AssignedTo@mail.test";

            CreateTask createTask = new CreateTask()
            {
                TaskName = taskName,
                Description = description,
                Status = status,
                AssignedTo = assignedTo
            };

            _taskManagementHandlerService.Handle(createTask);

            var tasks = await _taskManagementQyeryService.GetTasksAsync(new TasksQuery());

            tasks.Should().HaveCount(1);

            var task = tasks.First();

            var statusUpdated = Status.InProgress;
            var updatedBy = "updatedBy@mail.test";

            UpdateTask updateTask = new UpdateTask()
            {
                TaskId = task.TaskId,
                Status = statusUpdated,
                UpdatedBy = updatedBy
            };

            _taskManagementHandlerService.Handle(updateTask);

            var taskUpdated = (await _taskManagementQyeryService.GetTasksAsync(new TasksQuery()
            {
                Ids = new int[]
                {
                    task.TaskId
                }
            })).First();

            taskUpdated.TaskName.Should().Be(taskName);
            taskUpdated.Description.Should().Be(description);
            taskUpdated.Status.Should().Be(statusUpdated);
            taskUpdated.AssignedTo.Should().Be(assignedTo);
            taskUpdated.UpdatedBy.Should().Be(updatedBy);
        }

        [TestMethod, ExpectedException(typeof(NotFoundException))]
        public void UpdateTaskWrongTaskIdShouldBeExceptionTest()
        {
            UpdateTask updateTask = new UpdateTask()
            {
                TaskId = 4, // task with this id does not exist in DB, must be an exception
                Status = Status.InProgress
            };

            _taskManagementHandlerService.Handle(updateTask);
        }

        public void Dispose()
        {
            _taskDbContext?.Dispose();
        }
    }
}
