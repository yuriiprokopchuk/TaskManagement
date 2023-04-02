using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using TaskManagement.Services.Commands;
using TaskManagement.Services.DataContext;
using TaskManagement.Services.DataContext.Entities;
using TaskManagement.Services.Exceptions;
using TaskManagement.Services.Queries;

namespace TaskManagement.Services.Tests
{
    [TestClass]
    public class TaskManagementServiceTest
    {
        public TaskManagementServiceTest()
        {
            using (var db = CreateNewTaskDbContext())
            {
                db.Database.EnsureDeleted();
                db.Database.EnsureCreated();
            }
        }

        [TestMethod]
        public  void CreateTaskTest()
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

            CreateTaskManagementHandlerService().Handle(createTask);

            var tasks = CreateTaskManagementQueryService()
                .GetTasksAsync(new TasksQuery())
                .GetAwaiter().GetResult();

            tasks.Should().HaveCount(1);

            var task = tasks.First();

            task.TaskName.Should().Be(taskName);
            task.Description.Should().Be(description);
            task.Status.Should().Be(status);
            task.AssignedTo.Should().Be(assignedTo);
        }

        [TestMethod]
        public void UpdateTaskTest()
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

            CreateTaskManagementHandlerService().Handle(createTask);

            var tasks = CreateTaskManagementQueryService()
                .GetTasksAsync(new TasksQuery()).GetAwaiter().GetResult();

            tasks.Should().HaveCount(1);

            var task = tasks.First();

            var statusUpdated = Status.InProgress;
            var assignedToUpdated = "AssignedToUpdated@mail.test";

            UpdateTask updateTask = new UpdateTask()
            {
                TaskId = task.TaskId,
                Status = statusUpdated,
                AssignedTo = assignedToUpdated
            };

            CreateTaskManagementHandlerService().Handle(updateTask);

            var taskUpdated = CreateTaskManagementQueryService().GetTasksAsync(new TasksQuery()
            {
                Ids = new int[]
                {
                    task.TaskId
                }
            }).GetAwaiter().GetResult().First();

            taskUpdated.TaskName.Should().Be(taskName);
            taskUpdated.Description.Should().Be(description);
            taskUpdated.Status.Should().Be(statusUpdated);
            taskUpdated.AssignedTo.Should().Be(assignedToUpdated);
        }

        [TestMethod, ExpectedException(typeof(NotFoundException))]
        public void UpdateTaskWrongTaskIdShouldBeExceptionTest()
        {
            UpdateTask updateTask = new UpdateTask()
            {
                TaskId = 4, // task with this id does not exist in DB
                Status = Status.InProgress
            };

            CreateTaskManagementHandlerService().Handle(updateTask);
        }

        private TaskDbContext CreateNewTaskDbContext()
        {
            return new TaskDbContext(new DbContextOptionsBuilder<TaskDbContext>().UseInMemoryDatabase("TaskManagement.Test").Options);
        }

        private TaskManagementHandlerService CreateTaskManagementHandlerService()
        {
            return new TaskManagementHandlerService(CreateNewTaskDbContext());
        }

        private TaskManagementQyeryService CreateTaskManagementQueryService()
        {
            return new TaskManagementQyeryService(CreateNewTaskDbContext());
        }
    }
}
