using NSubstitute;
using TaskManagement.Application.Interfaces;
using TaskManagement.Application.Methods;
using TaskManagement.Application.Services;
using TaskManagement.Domain.DTOs;
using TaskManagement.Domain.Entities;

namespace TaskManagement.Tests.Services
{
    [TestClass]
    public class UserServiceTests
    {
        private IUserRepository _userRepository;
        private UserService _userService;

        [TestInitialize]
        public void Setup()
        {
            _userRepository = Substitute.For<IUserRepository>();
            _userService = new UserService(_userRepository, null); // ILogger can be null for testing purposes
        }

        [TestMethod]
        public async Task RegisterUserAsync_ShouldReturnUser_WhenUserDoesNotExist()
        {
            // Arrange
            var registerDto = new RegisterDTO
            {
                UserName = "newuser",
                Email = "newuser@example.com",
                Password = "password123"
            };

            _userRepository.GetAsync(Arg.Any<User>()).Returns(Task.FromResult<User>(null));
            _userRepository.AddAsync(Arg.Any<User>()).Returns(Task.FromResult(new User
            {
                Username = "newuser",
                Email = "newuser@example.com"
            }));

            // Act
            var result = await _userService.RegisterUserAsync(registerDto);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("newuser", result.Username);
            Assert.AreEqual("newuser@example.com", result.Email);
        }

        [TestMethod]
        public async Task RegisterUserAsync_ShouldReturnNull_WhenUserAlreadyExists()
        {
            // Arrange
            var registerDto = new RegisterDTO
            {
                UserName = "existinguser",
                Email = "existinguser@example.com",
                Password = "password123"
            };

            _userRepository.GetAsync(Arg.Any<User>()).Returns(Task.FromResult(new User
            {
                Username = "existinguser",
                Email = "existinguser@example.com"
            }));

            // Act
            var result = await _userService.RegisterUserAsync(registerDto);

            // Assert
            Assert.IsNull(result);
        }

        [TestMethod]
        public async Task LoginUserAsync_ShouldReturnUser_WhenCredentialsAreValid()
        {
            // Arrange
            var loginDto = new LoginDTO
            {
                UserNameOrEmail = "existinguser",
                Password = "password123"
            };

            var existingUser = new User
            {
                Username = "existinguser",
                Email = "existinguser@example.com",
                PasswordHash = PasswordHasher.HashPassword("password123")
            };

            _userRepository.GetAsync(Arg.Any<User>()).Returns(Task.FromResult(existingUser));

            // Act
            var result = await _userService.LoginUserAsync(loginDto);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("existinguser", result.Username);
        }

        [TestMethod]
        public async Task LoginUserAsync_ShouldReturnNull_WhenCredentialsAreInvalid()
        {
            // Arrange
            var loginDto = new LoginDTO
            {
                UserNameOrEmail = "existinguser",
                Password = "wrongpassword"
            };

            var existingUser = new User
            {
                Username = "existinguser",
                Email = "existinguser@example.com",
                PasswordHash = PasswordHasher.HashPassword("password123")
            };

            _userRepository.GetAsync(Arg.Any<User>()).Returns(Task.FromResult(existingUser));

            // Act
            var result = await _userService.LoginUserAsync(loginDto);

            // Assert
            Assert.IsNull(result);
        }

        [TestMethod]
        public async Task GetUserByIdAsync_ShouldReturnUser_WhenUserExists()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var user = new User
            {
                Id = userId,
                Username = "existinguser",
                Email = "existinguser@example.com"
            };

            _userRepository.GetByIdAsync(userId).Returns(Task.FromResult(user));

            // Act
            var result = await _userService.GetUserByIdAsync(userId);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(userId, result.Id);
            Assert.AreEqual("existinguser", result.Username);
        }

        [TestMethod]
        public async Task UpdateUserAsync_ShouldUpdateUserDetails()
        {
            // Arrange
            var user = new User
            {
                Id = Guid.NewGuid(),
                Username = "existinguser",
                Email = "existinguser@example.com",
                PasswordHash = "hash"
            };

            _userRepository.UpdateAsync(Arg.Any<User>()).Returns(Task.FromResult(user));

            // Act
            var result = await _userService.UpdateUserAsync(user);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(user.Username, result.Username);
        }
    }
}
