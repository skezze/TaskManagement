using TaskManagement.Domain.Entities;
using TaskManagement.Domain.Enums;
using TaskStatus = TaskManagement.Domain.Enums.TaskStatus;

namespace TaskManagement.Application.Interfaces
{
    public interface IUserTaskRepository
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

        Task<UserTask> GetTaskByIdAsync(UserTask userTask);
        Task<UserTask> UpdateTaskAsync(UserTask userTask);
        Task DeleteTaskAsync(UserTask userTask);
    }
}
