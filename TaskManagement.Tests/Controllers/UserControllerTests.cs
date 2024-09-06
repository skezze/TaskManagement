using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using NSubstitute;
using TaskManagement.Api.Controllers;
using TaskManagement.Application.Interfaces;
using TaskManagement.Domain.DTOs;
using TaskManagement.Domain.Models;
using TaskManagement.Domain.Entities;
using TaskManagement.Domain.ViewModels;

namespace TaskManagement.Tests.Controllers
{
    [TestClass]
    public class UserControllerTests
    {
        private IUserService _userServiceMock;
        private IConfiguration _configurationMock;
        private UserController _userController;

        [TestInitialize]
        public void Setup()
        {
            _userServiceMock = Substitute.For<IUserService>();
            _configurationMock = Substitute.For<IConfiguration>();
            _userController = new UserController(_userServiceMock, _configurationMock);
        }

        // Test cases for Register action
        [TestMethod]
        public async Task Register_ValidUser_ReturnsOk()
        {
            // Arrange
            var registerViewModel = new RegisterViewModel
            {
                UserName = "testuser",
                Email = "test@example.com",
                Password = "Password@123"
            };
            var user = new User { Id = Guid.NewGuid(), Username = "testuser", Email = "test@example.com" };

            _userServiceMock.RegisterUserAsync(Arg.Any<RegisterDTO>()).Returns(Task.FromResult(user));

            // Act
            var result = await _userController.Register(registerViewModel) as OkObjectResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(200, result.StatusCode);
            Assert.AreEqual(user, result.Value);
        }

        [TestMethod]
        public async Task Register_UserExists_ReturnsBadRequest()
        {
            // Arrange
            var registerViewModel = new RegisterViewModel
            {
                UserName = "testuser",
                Email = "test@example.com",
                Password = "Password@123"
            };

            _userServiceMock.RegisterUserAsync(Arg.Any<RegisterDTO>()).Returns(Task.FromResult((User)null));

            // Act
            var result = await _userController.Register(registerViewModel) as BadRequestObjectResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(400, result.StatusCode);
            Assert.AreEqual("User already exists", ((ErrorResponse)result.Value).ErrorMessage);
        }

        // Test cases for Login action
        [TestMethod]
        public async Task Login_ValidCredentials_ReturnsOkWithToken()
        {
            // Arrange
            var loginViewModel = new LoginViewModel
            {
                UserNameOrEmail = "testuser",
                Password = "Password@123"
            };
            var user = new User 
            {
                Id = Guid.NewGuid(), 
                Username = "testuser"
            };

            _userServiceMock.LoginUserAsync(Arg.Any<LoginDTO>()).Returns(Task.FromResult(user));

            _configurationMock.GetSection("AppSettings")["Secret"].Returns("8c8324e2-2afc-71a5-649e-9b9tf15af6d3");
            _configurationMock.GetSection("AppSettings")["LocalhostUrl"].Returns("http://localhost");

            // Act
            var result = await _userController.Login(loginViewModel) as OkObjectResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(200, result.StatusCode);
            var token = ((AuthenticatedResponse)result.Value).Token;
            Assert.IsFalse(string.IsNullOrEmpty(token));
        }

        [TestMethod]
        public async Task Login_InvalidCredentials_ReturnsUnauthorized()
        {
            // Arrange
            var loginViewModel = new LoginViewModel
            {
                UserNameOrEmail = "wronguser",
                Password = "wrongpassword"
            };
            _userServiceMock.LoginUserAsync(Arg.Any<LoginDTO>()).Returns(Task.FromResult((User)null));

            // Act
            var result = await _userController.Login(loginViewModel) as UnauthorizedObjectResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(401, result.StatusCode);
            Assert.AreEqual("Invalid client credentials", ((ErrorResponse)result.Value).ErrorMessage);
        }

        // Test cases for IsUserAuthorized action
        [TestMethod]
        public async Task IsUserAuthorized_ValidLogin_ReturnsOk()
        {
            // Arrange
            var loginViewModel = new LoginViewModel
            {
                UserNameOrEmail = "testuser",
                Password = "Password@123"
            };
            var user = new User 
            { 
                Id = Guid.NewGuid(), 
                Username = "testuser" 
            };

            _userServiceMock.LoginUserAsync(Arg.Any<LoginDTO>()).Returns(Task.FromResult(user));

            // Act
            var result = await _userController.IsUserAuthorized(loginViewModel) as OkObjectResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(200, result.StatusCode);
            Assert.AreEqual(user, result.Value);
        }

        [TestMethod]
        public async Task IsUserAuthorized_InvalidLogin_ReturnsBadRequest()
        {
            // Arrange
            var loginViewModel = new LoginViewModel
            {
                UserNameOrEmail = "wronguser",
                Password = "wrongpassword"
            };

            _userServiceMock.LoginUserAsync(Arg.Any<LoginDTO>()).Returns(Task.FromResult((User)null));

            // Act
            var result = await _userController.IsUserAuthorized(loginViewModel) as BadRequestObjectResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(400, result.StatusCode);
            Assert.AreEqual("Invalid client credentials", ((ErrorResponse)result.Value).ErrorMessage);
        }
    }
}
