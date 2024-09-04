using Microsoft.EntityFrameworkCore;
using TaskManagement.Application.Interfaces;
using TaskManagement.Data.DbContexts;
using TaskManagement.Domain.DTOs;
using TaskManagement.Domain.Entities;
using TaskManagement.Domain.Enums;
using TaskStatus = TaskManagement.Domain.Enums.TaskStatus;

namespace TaskManagement.Application.Repositories
{
    public class UserTaskRepository : IUserTaskRepository
    {
        private readonly TaskManagementDbContext _context;

        public UserTaskRepository(TaskManagementDbContext context)
        {
            _context = context;
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
            _context.UserTasks.Add(task);
            await _context.SaveChangesAsync();
            return task;
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

            return await query.ToListAsync();
        }

        public async Task<UserTask> GetTaskByIdAsync(Guid userId, Guid taskId)
        {
            return await _context.UserTasks.FirstOrDefaultAsync(t => t.UserId == userId && t.Id == taskId);
        }

        public async Task<UserTask> UpdateTaskAsync(UserTask task)
        {
            _context.UserTasks.Update(task);
            await _context.SaveChangesAsync();
            return task;
        }

        public async Task<bool> DeleteTaskAsync(Guid userId, Guid taskId)
        {
            var task = await _context.UserTasks.FirstOrDefaultAsync(t => t.UserId == userId && t.Id == taskId);
            if (task == null) return false;

            _context.UserTasks.Remove(task);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
