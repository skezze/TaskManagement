using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using System.Security.Claims;
using TaskManagement.Api.Controllers;
using TaskManagement.Application.Interfaces;
using TaskManagement.Domain.DTOs;
using TaskManagement.Domain.Entities;
using TaskManagement.Domain.Enums;
using TaskManagement.Domain.ViewModels;
using TaskStatus = TaskManagement.Domain.Enums.TaskStatus;

namespace TaskManagement.Tests.Controllers
{
    [TestClass]
    public class UserTaskControllerTests
    {
        private IUserTaskService _userTaskServiceMock;
        private IUserService _userServiceMock;
        private UserTaskController _controller;
        private Guid _userId;
        private ClaimsPrincipal _userClaimsPrincipal;

        [TestInitialize]
        public void Setup()
        {
            _userTaskServiceMock = Substitute.For<IUserTaskService>();
            _userServiceMock = Substitute.For<IUserService>();
            _controller = new UserTaskController(_userTaskServiceMock, _userServiceMock);

            _userId = Guid.NewGuid();
            _userClaimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim("UserId", _userId.ToString())
            }));
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = _userClaimsPrincipal }
            };
        }

        [TestMethod]
        public async Task CreateTask_ValidTask_ReturnsCreatedAtAction()
        {
            // Arrange
            var taskDto = new UserTaskDTO
            {
                Title = "Test Task",
                DueDate = DateTime.Now,
                Priority = TaskPriority.Medium,
                Status = TaskStatus.InProgress,
            };
            var taskViewModel = new UserTaskViewModel
            {
                Title = "Test Task",
                DueDate = DateTime.Now,
                Priority = TaskPriority.Medium,
                Status = TaskStatus.InProgress,
            };
            var createdTask = new UserTask { Id = Guid.NewGuid(), Title = "Test Task" };

            _userTaskServiceMock.CreateTaskAsync(taskDto).Returns(createdTask);

            // Act
            var result = await _controller.CreateTask(taskViewModel) as CreatedAtActionResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(201, result.StatusCode);
            Assert.AreEqual("GetTaskById", result.ActionName);
            Assert.AreEqual(createdTask.Id, ((UserTask)result.Value).Id);
        }

        [TestMethod]
        public async Task GetTasks_ReturnsOkWithTasks()
        {
            // Arrange
            var tasks = new List<UserTask>
            {
                new UserTask { Id = Guid.NewGuid(), Title = "Task 1", UserId = _userId },
                new UserTask { Id = Guid.NewGuid(), Title = "Task 2", UserId = _userId }
            };

            _userTaskServiceMock.GetTasksAsync(_userId, null, null, null, null, false, 1, 10)
                .Returns(tasks);

            // Act
            var result = await _controller.GetTasks(null, null, null, null, false, 1, 10) as OkObjectResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(200, result.StatusCode);
            Assert.AreEqual(tasks, result.Value);
        }

        [TestMethod]
        public async Task GetTaskById_ValidTask_ReturnsOk()
        {
            // Arrange
            var taskId = Guid.NewGuid();
            var task = new UserTask { Id = taskId, UserId = _userId };

            _userTaskServiceMock.GetTaskByIdAsync(_userId, taskId).Returns(task);

            // Act
            var result = await _controller.GetTaskById(taskId) as OkObjectResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(200, result.StatusCode);
            Assert.AreEqual(task, result.Value);
        }

        [TestMethod]
        public async Task GetTaskById_TaskNotFound_ReturnsNotFound()
        {
            // Arrange
            var taskId = Guid.NewGuid();
            _userTaskServiceMock.GetTaskByIdAsync(_userId, taskId).Returns((UserTask)null);

            // Act
            var result = await _controller.GetTaskById(taskId) as NotFoundResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(404, result.StatusCode);
        }

        [TestMethod]
        public async Task UpdateTask_ValidTask_ReturnsOk()
        {
            // Arrange
            var taskId = Guid.NewGuid();
            var taskDto = new UserTaskDTO 
            { 
                Title = "Updated Task", 
                UserId = _userId 
            };
            var taskViewModel  = new UserTaskViewModel
            {
                Title = "Updated Task"
            };

            var updatedTask = new UserTask { Id = taskId, Title = "Updated Task" };
            _userTaskServiceMock.UpdateTaskAsync(taskDto, taskId).Returns(updatedTask);

            // Act
            var result = await _controller.UpdateTask(taskViewModel, taskId) as OkObjectResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(200, result.StatusCode);
            Assert.AreEqual(updatedTask, result.Value);
        }

        [TestMethod]
        public async Task UpdateTask_TaskNotFound_ReturnsBadRequest()
        {
            // Arrange
            var taskId = Guid.NewGuid();
            var taskDto = new UserTaskDTO 
            { 
                Title = "Non-existent Task", 
                UserId = _userId 
            };
            var taskViewModel = new UserTaskViewModel
            {
                Title = "Non-existent Task"
            };

            _userTaskServiceMock.UpdateTaskAsync(taskDto, taskId).Returns((UserTask)null);

            // Act
            var result = await _controller.UpdateTask(taskViewModel, taskId) as BadRequestResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(400, result.StatusCode);
        }

        [TestMethod]
        public async Task DeleteTask_ValidTask_ReturnsNoContent()
        {
            // Arrange
            var taskId = Guid.NewGuid();
            _userTaskServiceMock.DeleteTaskAsync(_userId, taskId).Returns(true);

            // Act
            var result = await _controller.DeleteTask(taskId) as NoContentResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(204, result.StatusCode);
        }

        [TestMethod]
        public async Task DeleteTask_TaskNotFound_ReturnsNotFound()
        {
            // Arrange
            var taskId = Guid.NewGuid();
            _userTaskServiceMock.DeleteTaskAsync(_userId, taskId).Returns(false);

            // Act
            var result = await _controller.DeleteTask(taskId) as NotFoundResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(404, result.StatusCode);
        }
    }
}
