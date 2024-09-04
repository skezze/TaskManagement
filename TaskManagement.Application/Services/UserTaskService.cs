using TaskManagement.Application.Interfaces;
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

        public async Task<UserTask> CreateTaskAsync(UserTask task)
        {
            return await _userTaskRepository.CreateTaskAsync(task);
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
            return await _userTaskRepository.GetTasksAsync(userId, status, dueDate, priority, sortBy, descending, pageNumber, pageSize);
        }

        public async Task<UserTask> GetTaskByIdAsync(Guid userId, Guid taskId)
        {
            return await _userTaskRepository.GetTaskByIdAsync(userId, taskId);
        }

        public async Task<UserTask> UpdateTaskAsync(UserTask task)
        {
            return await _userTaskRepository.UpdateTaskAsync(task);
        }

        public async Task<bool> DeleteTaskAsync(Guid userId, Guid taskId)
        {
            return await _userTaskRepository.DeleteTaskAsync(userId, taskId);
        }
    }
}
