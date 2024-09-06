using TaskManagement.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using ILogger = Serilog.ILogger;
using TaskManagement.Application.Interfaces;
using TaskStatus = TaskManagement.Domain.Enums.TaskStatus;
using TaskManagement.Domain.DTOs;
using TaskManagement.Domain.ViewModels;

namespace TaskManagement.Api.Controllers
{
    [ApiController]
    [Authorize] // All routes require authorization
    public class UserTaskController : ControllerBase
    {
        private readonly IUserTaskService _userTaskService;
        private readonly IUserService _userService;
        private readonly ILogger _logger;

        public UserTaskController(IUserTaskService userTaskService, IUserService userService)
        {
            _userTaskService = userTaskService;
            _userService = userService;
            _logger = Log.ForContext<UserTaskController>(); // Serilog context for this controller
        }

        /// <summary>
        /// Create a new task for the logged-in user.
        /// </summary>
        /// <param name="userTaskViewModel">Task creation data</param>
        /// <returns>201 Created with task info, or 400 BadRequest</returns>
        [HttpPost]
        [Route("/tasks")]
        public async Task<IActionResult> CreateTask(UserTaskViewModel userTaskViewModel)
        {
            _logger.Information("CreateTask endpoint hit for task: {TaskTitle}", userTaskViewModel?.Title);

            // Extract user ID from JWT token claims
            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "UserId");
            var userId = Guid.Parse(userIdClaim.Value);

            // Map ViewModel to DTO for task creation
            var userTaskDTO = new UserTaskDTO
            {
                Title = userTaskViewModel.Title,
                UserId = userId,
                Description = userTaskViewModel.Description,
                DueDate = userTaskViewModel.DueDate,
                Status = userTaskViewModel.Status,
                Priority = userTaskViewModel.Priority
            };

            // Call service to create task
            var createdTask = await _userTaskService.CreateTaskAsync(userTaskDTO);

            if (createdTask != null)
            {
                _logger.Information("Task {TaskTitle} created successfully with ID: {TaskId}", createdTask.Title, createdTask.Id);
                return CreatedAtAction(nameof(GetTaskById), new { id = createdTask.Id }, createdTask);
            }

            _logger.Warning("Failed to create task: {TaskTitle}", userTaskViewModel?.Title);
            return BadRequest();
        }

        /// <summary>
        /// Retrieve tasks for the logged-in user with optional filters.
        /// </summary>
        /// <param name="status">Task status filter</param>
        /// <param name="dueDate">Due date filter</param>
        /// <param name="priority">Task priority filter</param>
        /// <param name="sortBy">Sort by field</param>
        /// <param name="descending">Sort order</param>
        /// <param name="pageNumber">Page number for pagination</param>
        /// <param name="pageSize">Page size for pagination</param>
        /// <returns>200 OK with list of tasks</returns>
        [HttpGet]
        [Route("/tasks")]
        public async Task<IActionResult> GetTasks([FromQuery] TaskStatus? status, [FromQuery] DateTime? dueDate, [FromQuery] TaskPriority? priority, [FromQuery] string? sortBy = "priority", [FromQuery] bool descending = true, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            _logger.Information("GetTasks endpoint hit for user");

            // Extract user ID from JWT token claims
            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "UserId");
            var userId = Guid.Parse(userIdClaim.Value);

            // Call service to retrieve filtered, sorted, and paginated tasks
            var tasks = await _userTaskService.GetTasksAsync(userId, status, dueDate, priority, sortBy, descending, pageNumber, pageSize);

            _logger.Information("Retrieved {TaskCount} tasks for user", tasks?.Count());
            return Ok(tasks);
        }

        /// <summary>
        /// Get task details by task ID.
        /// </summary>
        /// <param name="id">Task ID</param>
        /// <returns>200 OK with task details, or 404 NotFound</returns>
        [HttpGet]
        [Route("/tasks/{id}")]
        public async Task<IActionResult> GetTaskById(Guid id)
        {
            _logger.Information("GetTaskById endpoint hit for task ID: {TaskId}", id);

            // Extract user ID from JWT token claims
            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "UserId");
            var userId = Guid.Parse(userIdClaim.Value);

            // Call service to get task by ID
            var task = await _userTaskService.GetTaskByIdAsync(userId, id);

            if (task == null)
            {
                _logger.Warning("Task not found for ID: {TaskId}", id);
                return NotFound();
            }

            _logger.Information("Task {TaskId} retrieved successfully", task.Id);
            return Ok(task);
        }

        /// <summary>
        /// Update an existing task for the logged-in user.
        /// </summary>
        /// <param name="userTaskViewModel">Updated task data</param>
        /// <param name="id">Task ID to update</param>
        /// <returns>200 OK with updated task, or 400 BadRequest if failed</returns>
        [HttpPut]
        [Route("/tasks/{id}")]
        public async Task<IActionResult> UpdateTask(UserTaskViewModel userTaskViewModel, Guid id)
        {
            _logger.Information("UpdateTask endpoint hit for task ID: {TaskId}", id);

            // Extract user ID from JWT token claims
            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "UserId");
            var userId = Guid.Parse(userIdClaim.Value);

            // Map ViewModel to DTO for task update
            var userTaskDTO = new UserTaskDTO
            {
                Title = userTaskViewModel.Title,
                UserId = userId,
                Description = userTaskViewModel.Description,
                DueDate = userTaskViewModel.DueDate,
                Status = userTaskViewModel.Status,
                Priority = userTaskViewModel.Priority
            };

            // Call service to update task
            var updatedTask = await _userTaskService.UpdateTaskAsync(userTaskDTO, id);

            if (updatedTask == null)
            {
                _logger.Warning("Failed to update task for ID: {TaskId}", id);
                return BadRequest();
            }

            _logger.Information("Task {TaskId} updated successfully", updatedTask.Id);
            return Ok(updatedTask);
        }

        /// <summary>
        /// Delete a task by task ID.
        /// </summary>
        /// <param name="id">Task ID</param>
        /// <returns>204 NoContent if successful, or 404 NotFound</returns>
        [HttpDelete]
        [Route("/tasks/{id}")]
        public async Task<IActionResult> DeleteTask(Guid id)
        {
            _logger.Information("DeleteTask endpoint hit for task ID: {TaskId}", id);

            // Extract user ID from JWT token claims
            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "UserId");
            var userId = Guid.Parse(userIdClaim.Value);

            // Call service to delete task
            var success = await _userTaskService.DeleteTaskAsync(userId, id);

            if (!success)
            {
                _logger.Warning("Failed to delete task for ID: {TaskId}", id);
                return NotFound();
            }

            _logger.Information("Task {TaskId} deleted successfully", id);
            return NoContent();
        }
    }
}
