using TaskManagement.Domain.Entities;
using TaskManagement.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TaskManagement.Application.Interfaces;
using TaskStatus = TaskManagement.Domain.Enums.TaskStatus;

namespace TaskManagement.Api.Controllers
{
    
    [ApiController]
    [Authorize]
    public class UserTaskController : ControllerBase
    {
        private readonly IUserTaskService _userTaskService;

        public UserTaskController(IUserTaskService userTaskService)
        {
            _userTaskService = userTaskService;
        }

        private Guid GetUserId() => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

        [HttpPost]
        [Route("/tasks")]
        public async Task<IActionResult> CreateTask(UserTask task)
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            task.UserId = userId;

            var createdTask = await _userTaskService.CreateTaskAsync(task);
            return CreatedAtAction(nameof(GetTaskById), new { id = createdTask.Id }, createdTask);
        }

        [HttpGet]
        [Route("/tasks")]
        public async Task<IActionResult> GetTasks([FromQuery] TaskStatus? status, [FromQuery] DateTime? dueDate, [FromQuery] TaskPriority? priority, [FromQuery] string sortBy, [FromQuery] bool descending, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            var userId = GetUserId();
            var tasks = await _userTaskService.GetTasksAsync(userId, status, dueDate, priority, sortBy, descending, pageNumber, pageSize);
            return Ok(tasks);
        }

        [HttpGet]
        [Route("/tasks/{id}")]
        public async Task<IActionResult> GetTaskById(Guid id)
        {
            var userId = GetUserId();
            var task = await _userTaskService.GetTaskByIdAsync(userId, id);
            if (task == null) return NotFound();

            return Ok(task);
        }

        [HttpPut]
        [Route("/tasks/{id}")]
        public async Task<IActionResult> UpdateTask(Guid id, UserTask task)
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            if (id != task.Id || userId != task.UserId) return BadRequest();

            var updatedTask = await _userTaskService.UpdateTaskAsync(task);
            return Ok(updatedTask);
        }

        [HttpDelete]
        [Route("/tasks/{id}")]
        public async Task<IActionResult> DeleteTask(Guid id)
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var success = await _userTaskService.DeleteTaskAsync(userId, id);

            if (!success) return NotFound();
            return NoContent();
        }
    }
}


