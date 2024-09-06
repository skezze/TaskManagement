using Microsoft.EntityFrameworkCore;
using Serilog;
using TaskManagement.Application.Interfaces;
using TaskManagement.Data.DbContexts;
using TaskManagement.Domain.Entities;
using TaskManagement.Domain.Enums;
using TaskStatus = TaskManagement.Domain.Enums.TaskStatus;

namespace TaskManagement.Application.Repositories
{
    public class UserTaskRepository : IUserTaskRepository
    {
        private readonly TaskManagementDbContext _context;
        private readonly ILogger _logger;

        public UserTaskRepository(TaskManagementDbContext context)
        {
            _context = context;
            _logger = Log.ForContext<UserTaskRepository>(); // Serilog context for this repository
        }

        /// <summary>
        /// Create a new task in the database.
        /// </summary>
        /// <param name="task">The task entity to be created</param>
        /// <returns>The created task entity</returns>
        public async Task<UserTask> CreateTaskAsync(UserTask task)
        {
            _logger.Information("Creating new task with Title: {TaskTitle}", task.Title);

            _context.UserTasks.Add(task);
            await _context.SaveChangesAsync();

            _logger.Information("Task created with ID: {TaskId}", task.Id);

            return task;
        }

        /// <summary>
        /// Retrieve tasks for a specific user based on various filters.
        /// </summary>
        /// <param name="userId">The user's ID</param>
        /// <param name="status">Optional status filter</param>
        /// <param name="dueDate">Optional due date filter</param>
        /// <param name="priority">Optional priority filter</param>
        /// <param name="sortBy">Field to sort by</param>
        /// <param name="descending">Sort order</param>
        /// <param name="pageNumber">Page number for pagination</param>
        /// <param name="pageSize">Number of tasks per page</param>
        /// <returns>A list of tasks that match the filters</returns>
        public async Task<IEnumerable<UserTask>> GetTasksAsync(
            Guid userId,
            TaskStatus? status = null,
            DateTime? dueDate = null,
            TaskPriority? priority = null,
            string sortBy = "duedate",
            bool descending = false,
            int pageNumber = 1,
            int pageSize = 10)
        {
            _logger.Information("Fetching tasks for User ID: {UserId} with filters - Status: {Status}, DueDate: {DueDate}, Priority: {Priority}, SortBy: {SortBy}, Descending: {Descending}, PageNumber: {PageNumber}, PageSize: {PageSize}",
                userId, status, dueDate, priority, sortBy, descending, pageNumber, pageSize);

            var query = _context.UserTasks.Where(t => t.UserId == userId);

            if (status.HasValue)
                query = query.Where(t => t.Status == status.Value);

            if (dueDate.HasValue)
                query = query.Where(t => t.DueDate == dueDate.Value);

            if (priority.HasValue)
                query = query.Where(t => t.Priority == priority.Value);

            // Sorting
            if (!string.IsNullOrEmpty(sortBy))
            {
                query = sortBy.ToLower() switch
                {
                    "duedate" => descending ? query.OrderByDescending(t => t.DueDate) : query.OrderBy(t => t.DueDate),
                    "priority" => descending ? query.OrderByDescending(t => t.Priority) : query.OrderBy(t => t.Priority),
                    _ => query
                };
            }

            // Pagination
            query = query.Skip((pageNumber - 1) * pageSize).Take(pageSize);

            var tasks = await query.ToListAsync();

            _logger.Information("Fetched {TaskCount} tasks for User ID: {UserId}", tasks.Count, userId);

            return tasks;
        }

        /// <summary>
        /// Retrieve a specific task by its ID and user ID.
        /// </summary>
        /// <param name="userTask">Task object containing user ID and task ID</param>
        /// <returns>The task entity, or null if not found</returns>
        public async Task<UserTask> GetTaskByIdAsync(UserTask userTask)
        {
            _logger.Information("Fetching task with ID: {TaskId} for User ID: {UserId}", userTask.Id, userTask.UserId);

            var task = await _context.UserTasks
                .FirstOrDefaultAsync(t => t.UserId == userTask.UserId && t.Id == userTask.Id);

            if (task == null)
            {
                _logger.Warning("Task not found with ID: {TaskId} for User ID: {UserId}", userTask.Id, userTask.UserId);
            }
            else
            {
                _logger.Information("Task found with ID: {TaskId} for User ID: {UserId}", task.Id, userTask.UserId);
            }

            return task;
        }

        /// <summary>
        /// Update an existing task in the database.
        /// </summary>
        /// <param name="userTask">The task entity to be updated</param>
        /// <returns>The updated task entity</returns>
        public async Task<UserTask> UpdateTaskAsync(UserTask userTask)
        {
            _logger.Information("Updating task with ID: {TaskId} for User ID: {UserId}", userTask.Id, userTask.UserId);

            _context.UserTasks.Update(userTask);
            await _context.SaveChangesAsync();

            _logger.Information("Task updated with ID: {TaskId}", userTask.Id);

            return userTask;
        }

        /// <summary>
        /// Delete a specific task from the database.
        /// </summary>
        /// <param name="userTask">The task entity to be deleted</param>
        public async Task DeleteTaskAsync(UserTask userTask)
        {
            _logger.Information("Deleting task with ID: {TaskId} for User ID: {UserId}", userTask.Id, userTask.UserId);

            _context.UserTasks.Remove(userTask);
            await _context.SaveChangesAsync();

            _logger.Information("Task deleted with ID: {TaskId}", userTask.Id);
        }
    }
}
