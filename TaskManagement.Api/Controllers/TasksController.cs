using Microsoft.AspNetCore.Mvc;
using TaskManagement.Bus.Infrastructure;
using TaskManagement.Services;
using TaskManagement.Services.Commands;
using TaskManagement.Services.Queries;


namespace TaskManagement.Api.Controllers
{
    [Route("api/tasks")]
    [ApiController]
    public class TasksController : ControllerBase
    {
        private readonly TaskManagementQyeryService _taskManagementQyeryService;
        private readonly ServiceBusHandler _serviceBusHandler;

        public TasksController(ServiceBusHandler serviceBusHandler, TaskManagementQyeryService taskManagementQyeryService)
        {
            _serviceBusHandler = serviceBusHandler;
            _taskManagementQyeryService = taskManagementQyeryService;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var tasks = await _taskManagementQyeryService.GetTasksAsync(new TasksQuery());

            return Ok(tasks);
        }


        [HttpPost]
        public IActionResult Post(CreateTask createTask)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            _serviceBusHandler.SendMessage(createTask);

            return Ok();
        }


        [HttpPatch]
        public async Task<IActionResult> Patch(UpdateTask updateTask)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var task = (await _taskManagementQyeryService
                .GetTasksAsync(new TasksQuery() { Ids = new int[] { updateTask.TaskId } }))
                .FirstOrDefault();

            if (task != null) return NotFound();

            _serviceBusHandler.SendMessage(updateTask);

            return Ok();
        }
    }
}
