using TaskManagement.Domain.DTOs;
using TaskManagement.Domain.Entities;
using TaskManagement.Domain.Enums;
using TaskStatus = TaskManagement.Domain.Enums.TaskStatus;

namespace TaskManagement.Application.Interfaces
{
    public interface IUserTaskService
    {
        Task<UserTask> CreateTaskAsync(UserTaskDTO userTaskDTO);
        Task<IEnumerable<UserTask>> GetTasksAsync(
    Guid userId,
    TaskStatus? status = null,
    DateTime? dueDate = null,
    TaskPriority? priority = null,
    string sortBy = null,
    bool descending = false,
    int pageNumber = 1,
    int pageSize = 10);
        Task<UserTask> GetTaskByIdAsync(Guid userId, Guid taskId);
        Task<UserTask> UpdateTaskAsync(UserTaskDTO userTaskDTO);
        Task<bool> DeleteTaskAsync(Guid userId, Guid taskId);
    }
}
