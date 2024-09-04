using TaskManagement.Domain.Entities;
using TaskManagement.Domain.Enums;
using TaskStatus = TaskManagement.Domain.Enums.TaskStatus;

namespace TaskManagement.Application.Interfaces
{
    public interface IUserTaskService
    {
        Task<UserTask> CreateTaskAsync(UserTask task);
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
        Task<UserTask> UpdateTaskAsync(UserTask task);
        Task<bool> DeleteTaskAsync(Guid userId, Guid taskId);
    }
}
