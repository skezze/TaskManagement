using Microsoft.EntityFrameworkCore;
using TaskManagement.Application.Repositories;
using TaskManagement.Data.DbContexts;
using TaskManagement.Domain.Entities;
using TaskManagement.Domain.Enums;
using TaskStatus = TaskManagement.Domain.Enums.TaskStatus;

namespace TaskManagement.Tests.Repositories
{
    [TestClass]
    public class UserTaskRepositoryTests
    {
        private TaskManagementDbContext _context;
        private UserTaskRepository _repository;

        [TestInitialize]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<TaskManagementDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            _context = new TaskManagementDbContext(options);
            _repository = new UserTaskRepository(_context);

            _context.UserTasks.RemoveRange(_context.UserTasks);
            _context.SaveChanges();
        }

        [TestMethod]
        public async Task CreateTaskAsync_ShouldAddTaskToDatabase()
        {
            // Arrange
            var userTask = new UserTask
            {
                Title = "Test Task",
                Description = "Test Description",
                UserId = Guid.NewGuid(),
                DueDate = DateTime.UtcNow.AddDays(1),
                Status = TaskStatus.Pending,
                Priority = TaskPriority.Medium,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            // Act
            var result = await _repository.CreateTaskAsync(userTask);
            var tasks = await _context.UserTasks.ToListAsync();

            // Assert
            Assert.AreEqual(1, tasks.Count);
            Assert.AreEqual("Test Task", tasks.First().Title);
        }

        [TestMethod]
        public async Task GetTasksAsync_ShouldReturnFilteredTasks()
        {
            // Arrange
            var userId = Guid.NewGuid();
            _context.UserTasks.Add(new UserTask
            {
                Title = "Task 1",
                UserId = userId,
                Description = "Test Description",
                Status = TaskStatus.Pending,
                Priority = TaskPriority.Medium,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            });
            _context.UserTasks.Add(new UserTask
            {
                Title = "Task 2",
                UserId = userId,
                Description = "Test Description",
                Status = TaskStatus.Completed,
                Priority = TaskPriority.High,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            });
            await _context.SaveChangesAsync();

            // Act
            var tasks = await _repository.GetTasksAsync(userId, status: TaskStatus.Pending);

            // Assert
            Assert.AreEqual(1, tasks.Count());
            Assert.AreEqual("Task 1", tasks.First().Title);
        }

        [TestMethod]
        public async Task UpdateTaskAsync_ShouldModifyExistingTask()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var task = new UserTask
            {
                Title = "Original Title",
                UserId = userId,
                Description = "Test Description",
                Status = TaskStatus.Pending,
                Priority = TaskPriority.Medium,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            _context.UserTasks.Add(task);
            await _context.SaveChangesAsync();

            task.Title = "Updated Title";

            // Act
            var result = await _repository.UpdateTaskAsync(task);
            var updatedTask = await _context.UserTasks.FirstOrDefaultAsync(t => t.Id == task.Id);

            // Assert
            Assert.AreEqual("Updated Title", updatedTask.Title);
        }

        [TestMethod]
        public async Task DeleteTaskAsync_ShouldRemoveTaskFromDatabase()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var task = new UserTask
            {
                Title = "Task to Delete",
                Description = "Test Description",
                UserId = userId,
                Status = TaskStatus.Pending,
                Priority = TaskPriority.Medium,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            _context.UserTasks.Add(task);
            await _context.SaveChangesAsync();

            // Act
            await _repository.DeleteTaskAsync(task);
            var tasks = await _context.UserTasks.ToListAsync();

            // Assert
            Assert.AreEqual(0, tasks.Count);
        }
    }
}
