using Microsoft.Extensions.Logging;
using Serilog;
using ILogger = Serilog.ILogger;
using TaskManagement.Application.Interfaces;
using TaskManagement.Domain.DTOs;
using TaskManagement.Domain.Entities;
using TaskManagement.Domain.Enums;
using TaskStatus = TaskManagement.Domain.Enums.TaskStatus;

namespace TaskManagement.Application.Services
{
    public class UserTaskService : IUserTaskService
    {
        private readonly IUserTaskRepository _userTaskRepository;
        private readonly ILogger _logger;

        public UserTaskService(IUserTaskRepository userTaskRepository)
        {
            _userTaskRepository = userTaskRepository;
            _logger = _logger = Log.ForContext<UserTaskService>();
        }

        public async Task<UserTask> CreateTaskAsync(UserTaskDTO userTaskDTO)
        {
            _logger.Warning("Creating task with title: {Title} for user ID: {UserId}", userTaskDTO.Title, userTaskDTO.UserId);

            var task = new UserTask
            {
                UserId = userTaskDTO.UserId,
                Description = userTaskDTO.Description,
                DueDate = userTaskDTO.DueDate,
                Title = userTaskDTO.Title,
                Status = userTaskDTO.Status,
                Priority = userTaskDTO.Priority
            };

            var taskInDb = await _userTaskRepository.CreateTaskAsync(task);
            _logger.Warning("Task created successfully with ID: {TaskId}", taskInDb.Id);

            return taskInDb;
        }

        public async Task<IEnumerable<UserTask>> GetTasksAsync(
            Guid userId,
            TaskStatus? status = null,
            DateTime? dueDate = null,
            TaskPriority? priority = null,
            string sortBy = null,
            bool descending = false,
            int pageNumber = 1,
            int pageSize = 10)
        {
            _logger.Warning("Fetching tasks for user ID: {UserId} with filters - Status: {Status}, DueDate: {DueDate}, Priority: {Priority}, SortBy: {SortBy}, Descending: {Descending}",
                userId, status, dueDate, priority, sortBy, descending);

            var tasksPageInDb = await _userTaskRepository.GetTasksAsync(userId, status, dueDate, priority, sortBy, descending, pageNumber, pageSize);
            return tasksPageInDb;
        }

        public async Task<UserTask> GetTaskByIdAsync(Guid userId, Guid taskId)
        {
            _logger.Warning("Fetching task by ID: {TaskId} for user ID: {UserId}", taskId, userId);

            var task = new UserTask
            {
                UserId = userId,
                Id = taskId
            };

            var taskInDb = await _userTaskRepository.GetTaskByIdAsync(task);

            if (taskInDb == null)
            {
                _logger.Warning("Task with ID: {TaskId} not found for user ID: {UserId}", taskId, userId);
            }
            else
            {
                _logger.Warning("Task found with ID: {TaskId} for user ID: {UserId}", taskId, userId);
            }

            return taskInDb;
        }

        public async Task<UserTask> UpdateTaskAsync(UserTaskDTO userTaskDTO, Guid taskId)
        {
            _logger.Warning("Updating task with ID: {TaskId} for user ID: {UserId}", taskId, userTaskDTO.UserId);

            var task = new UserTask
            {
                UserId = userTaskDTO.UserId,
                Id = taskId
            };

            var taskInDb = await _userTaskRepository.GetTaskByIdAsync(task);

            if (taskInDb != null)
            {
                taskInDb.Description = userTaskDTO.Description;
                taskInDb.DueDate = userTaskDTO.DueDate;
                taskInDb.Title = userTaskDTO.Title;
                taskInDb.Status = userTaskDTO.Status;
                taskInDb.Priority = userTaskDTO.Priority;
                taskInDb.UpdatedAt = DateTime.UtcNow;

                var updatedTask = await _userTaskRepository.UpdateTaskAsync(taskInDb);
                _logger.Warning("Task updated successfully with ID: {TaskId}", taskId);
                return updatedTask;
            }

            _logger.Warning("Task with ID: {TaskId} not found for update", taskId);
            return null;
        }

        public async Task<bool> DeleteTaskAsync(Guid userId, Guid taskId)
        {
            _logger.Warning("Deleting task with ID: {TaskId} for user ID: {UserId}", taskId, userId);

            var task = new UserTask
            {
                UserId = userId,
                Id = taskId
            };

            var taskInDb = await _userTaskRepository.GetTaskByIdAsync(task);

            if (taskInDb != null)
            {
                await _userTaskRepository.DeleteTaskAsync(taskInDb);
                _logger.Warning("Task deleted successfully with ID: {TaskId}", taskId);
                return true;
            }

            _logger.Warning("Task with ID: {TaskId} not found for deletion", taskId);
            return false;
        }
    }
}
