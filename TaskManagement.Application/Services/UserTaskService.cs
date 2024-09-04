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

        public UserTaskService(IUserTaskRepository userTaskRepository)
        {
            _userTaskRepository = userTaskRepository;
        }

        public async Task<UserTask> CreateTaskAsync(UserTaskDTO userTaskDTO)
        {
            var task = new UserTask()
            {
                UserId = userTaskDTO.UserId,
                Description = userTaskDTO.Description,
                DueDate = userTaskDTO.DueDate,
                Title = userTaskDTO.Title,
                Status = userTaskDTO.Status,
                Priority = userTaskDTO.Priority
            };

            var taskInDb = await _userTaskRepository.CreateTaskAsync(task);

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
            var tasksPageInDb = await _userTaskRepository.GetTasksAsync(userId, status, dueDate, priority, sortBy, descending, pageNumber, pageSize);
            return tasksPageInDb;
        }

        public async Task<UserTask> GetTaskByIdAsync(Guid userId, Guid taskId)
        {
            var task = new UserTask()
            {
                UserId= userId,
                Id= taskId
            };
            var taskInDb = await _userTaskRepository.GetTaskByIdAsync(task);
            return taskInDb;
        }

        public async Task<UserTask> UpdateTaskAsync(UserTaskDTO userTaskDTO)
        {
            var task = new UserTask()
            {
                UserId = userTaskDTO.UserId,
                Id = userTaskDTO.Id
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

                return await _userTaskRepository.UpdateTaskAsync(taskInDb);
            }
            return null;            
        }

        public async Task<bool> DeleteTaskAsync(Guid userId, Guid taskId)
        {
            var task = new UserTask()
            {
                UserId = userId,
                Id = taskId
            };
            var taskInDb = await _userTaskRepository.GetTaskByIdAsync(task);
            if (taskInDb != null)
            {
                await _userTaskRepository.DeleteTaskAsync(taskInDb);
                return true;  
            };
            return false;
        }
    }
}
