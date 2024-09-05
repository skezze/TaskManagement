using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TaskManagement.Application.Interfaces;
using TaskManagement.Application.Services;
using TaskManagement.Domain.DTOs;
using TaskManagement.Domain.Entities;
using TaskManagement.Domain.Enums;
using TaskStatus = TaskManagement.Domain.Enums.TaskStatus;

namespace TaskManagement.Tests.Services
{
    [TestClass]
    public class UserTaskServiceTests
    {
        private IUserTaskRepository _userTaskRepository;
        private UserTaskService _userTaskService;

        [TestInitialize]
        public void Setup()
        {
            _userTaskRepository = Substitute.For<IUserTaskRepository>();
            _userTaskService = new UserTaskService(_userTaskRepository);
        }

        [TestMethod]
        public async Task CreateTaskAsync_ShouldReturnCreatedTask()
        {
            // Arrange
            var userTaskDTO = new UserTaskDTO
            {
                UserId = Guid.NewGuid(),
                Description = "Test Task",
                DueDate = DateTime.UtcNow.AddDays(1),
                Title = "Test",
                Status = TaskStatus.Pending,
                Priority = TaskPriority.Medium
            };

            var expectedTask = new UserTask
            {
                UserId = userTaskDTO.UserId,
                Description = userTaskDTO.Description,
                DueDate = userTaskDTO.DueDate,
                Title = userTaskDTO.Title,
                Status = userTaskDTO.Status,
                Priority = userTaskDTO.Priority
            };

            _userTaskRepository.CreateTaskAsync(Arg.Any<UserTask>()).Returns(expectedTask);

            // Act
            var result = await _userTaskService.CreateTaskAsync(userTaskDTO);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("Test Task", result.Description);
            Assert.AreEqual(TaskStatus.Pending, result.Status);
        }

        [TestMethod]
        public async Task GetTasksAsync_ShouldReturnTasks()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var expectedTasks = new List<UserTask>
            {
                new UserTask { UserId = userId, Description = "Task 1", Status = TaskStatus.Completed },
                new UserTask { UserId = userId, Description = "Task 2", Status = TaskStatus.Pending }
            };

            _userTaskRepository.GetTasksAsync(userId, null, null, null, null, false, 1, 10).Returns(expectedTasks);

            // Act
            var result = await _userTaskService.GetTasksAsync(userId);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Count());
            Assert.AreEqual("Task 1", result.First().Description);
        }

        [TestMethod]
        public async Task GetTaskByIdAsync_ShouldReturnTask_WhenTaskExists()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var taskId = Guid.NewGuid();
            var expectedTask = new UserTask { UserId = userId, Id = taskId, Description = "Test Task" };

            _userTaskRepository.GetTaskByIdAsync(Arg.Any<UserTask>()).Returns(expectedTask);

            // Act
            var result = await _userTaskService.GetTaskByIdAsync(userId, taskId);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("Test Task", result.Description);
        }

        [TestMethod]
        public async Task GetTaskByIdAsync_ShouldReturnNull_WhenTaskDoesNotExist()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var taskId = Guid.NewGuid();

            _userTaskRepository.GetTaskByIdAsync(Arg.Any<UserTask>()).Returns(Task.FromResult<UserTask>(null));

            // Act
            var result = await _userTaskService.GetTaskByIdAsync(userId, taskId);

            // Assert
            Assert.IsNull(result);
        }

        [TestMethod]
        public async Task UpdateTaskAsync_ShouldReturnUpdatedTask_WhenTaskExists()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var taskId = Guid.NewGuid();
            var userTaskDTO = new UserTaskDTO
            {
                UserId = userId,
                Description = "Updated Task",
                DueDate = DateTime.UtcNow.AddDays(2),
                Title = "Updated",
                Status = TaskStatus.Completed,
                Priority = TaskPriority.High
            };

            var existingTask = new UserTask
            {
                UserId = userId,
                Id = taskId,
                Description = "Old Task",
                Title = "Old",
                Status = TaskStatus.Pending,
                Priority = TaskPriority.Low
            };

            _userTaskRepository.GetTaskByIdAsync(Arg.Any<UserTask>()).Returns(existingTask);
            _userTaskRepository.UpdateTaskAsync(Arg.Any<UserTask>()).Returns(existingTask);

            // Act
            var result = await _userTaskService.UpdateTaskAsync(userTaskDTO, taskId);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("Updated Task", result.Description);
            Assert.AreEqual(TaskStatus.Completed, result.Status);
        }

        [TestMethod]
        public async Task UpdateTaskAsync_ShouldReturnNull_WhenTaskDoesNotExist()
        {
            // Arrange
            var userTaskDTO = new UserTaskDTO
            {
                UserId = Guid.NewGuid(),
                Description = "Task",
                DueDate = DateTime.UtcNow.AddDays(2),
                Title = "Task",
                Status = TaskStatus.Completed,
                Priority = TaskPriority.High
            };
            var taskId = Guid.NewGuid();

            _userTaskRepository.GetTaskByIdAsync(Arg.Any<UserTask>()).Returns(Task.FromResult<UserTask>(null));

            // Act
            var result = await _userTaskService.UpdateTaskAsync(userTaskDTO, taskId);

            // Assert
            Assert.IsNull(result);
        }

        [TestMethod]
        public async Task DeleteTaskAsync_ShouldReturnTrue_WhenTaskExists()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var taskId = Guid.NewGuid();
            var existingTask = new UserTask { UserId = userId, Id = taskId };

            _userTaskRepository.GetTaskByIdAsync(Arg.Any<UserTask>()).Returns(existingTask);

            // Act
            var result = await _userTaskService.DeleteTaskAsync(userId, taskId);

            // Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public async Task DeleteTaskAsync_ShouldReturnFalse_WhenTaskDoesNotExist()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var taskId = Guid.NewGuid();

            _userTaskRepository.GetTaskByIdAsync(Arg.Any<UserTask>()).Returns(Task.FromResult<UserTask>(null));

            // Act
            var result = await _userTaskService.DeleteTaskAsync(userId, taskId);

            // Assert
            Assert.IsFalse(result);
        }
    }
}
