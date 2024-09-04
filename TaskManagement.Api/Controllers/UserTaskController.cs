using TaskManagement.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskManagement.Application.Interfaces;
using TaskStatus = TaskManagement.Domain.Enums.TaskStatus;
using TaskManagement.Domain.DTOs;

namespace TaskManagement.Api.Controllers
{
    
    [ApiController]
    [Authorize]
    public class UserTaskController : ControllerBase
    {
        private readonly IUserTaskService _userTaskService;
        private readonly IUserService _userService;

        public UserTaskController(IUserTaskService userTaskService, IUserService userService)
        {
            _userTaskService = userTaskService;
            _userService = userService;
        }

        [HttpPost]        
        [Route("/tasks")]
        public async Task<IActionResult> CreateTask(UserTaskDTO userTaskDTO)
        {
            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "UserId");
            var userId = Guid.Parse(userIdClaim.Value);
            userTaskDTO.UserId = userId;
            var createdTask = await _userTaskService.CreateTaskAsync(userTaskDTO);
            return CreatedAtAction(nameof(GetTaskById), new { id = createdTask.Id }, createdTask);
        }

        [HttpGet]
        [Route("/tasks")]
        public async Task<IActionResult> GetTasks([FromQuery] TaskStatus? status, [FromQuery] DateTime? dueDate, [FromQuery] TaskPriority? priority, [FromQuery] string sortBy, [FromQuery] bool descending, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "UserId");
            var userId = Guid.Parse(userIdClaim.Value);
            var tasks = await _userTaskService.GetTasksAsync(userId, status, dueDate, priority, sortBy, descending, pageNumber, pageSize);
            return Ok(tasks);
        }

        [HttpGet]
        [Route("/tasks/{id}")]
        public async Task<IActionResult> GetTaskById(Guid id)
        {
            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "UserId");
            var userId = Guid.Parse(userIdClaim.Value);
            var task = await _userTaskService.GetTaskByIdAsync(userId, id);
            if (task == null) return NotFound();

            return Ok(task);
        }

        [HttpPut]
        [Route("/tasks/{id}")]
        public async Task<IActionResult> UpdateTask(UserTaskDTO userTaskDto)
        {
            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "UserId");
            var userId = Guid.Parse(userIdClaim.Value);
            userTaskDto.UserId = userId;
            if (userId != userTaskDto.UserId) return BadRequest();

            var updatedTask = await _userTaskService.UpdateTaskAsync(userTaskDto);
            if (updatedTask == null)
            { 
                return BadRequest(); 
            }

            return Ok(updatedTask);
        }

        [HttpDelete]
        [Route("/tasks/{id}")]
        public async Task<IActionResult> DeleteTask(Guid id)
        {
            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "UserId");
            var userId = Guid.Parse(userIdClaim.Value);
            var success = await _userTaskService.DeleteTaskAsync(userId, id);

            if (!success)
            {
                return NotFound(); 
            }
            return NoContent();
        }
    }
}


