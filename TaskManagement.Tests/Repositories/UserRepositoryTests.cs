using Microsoft.EntityFrameworkCore;
using TaskManagement.Application.Methods;
using TaskManagement.Application.Repositories;
using TaskManagement.Data.DbContexts;
using TaskManagement.Domain.Entities;

namespace TaskManagement.Tests.Repositories
{
    [TestClass]
    public class UserRepositoryTests
    {
        private TaskManagementDbContext _context;
        private UserRepository _repository;

        [TestInitialize]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<TaskManagementDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            _context = new TaskManagementDbContext(options);
            _repository = new UserRepository(_context);
        }

        [TestCleanup]
        public void Cleanup()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        [TestMethod]
        public async Task AddAsync_ShouldAddUserToDatabase()
        {
            // Arrange
            var user = new User
            {
                Id = Guid.NewGuid(),
                Username = "testuser",
                Email = "test@example.com",
                PasswordHash = PasswordHasher.HashPassword("password123")
            };

            // Act
            var result = await _repository.AddAsync(user);
            var users = await _context.Users.ToListAsync();

            // Assert
            Assert.AreEqual(1, users.Count);
            Assert.AreEqual("testuser", users[0].Username);
            Assert.AreEqual("test@example.com", users[0].Email);
        }

        [TestMethod]
        public async Task GetByIdAsync_ShouldReturnUserIfExists()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var user = new User
            {
                Id = userId,
                Username = "testuser",
                Email = "test@example.com",
                PasswordHash = PasswordHasher.HashPassword("password123")
            };
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetByIdAsync(userId);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(userId, result.Id);
            Assert.AreEqual("testuser", result.Username);
        }

        [TestMethod]
        public async Task GetAsync_ShouldReturnUserByUsernameOrEmail()
        {
            // Arrange
            var user = new User
            {
                Id = Guid.NewGuid(),
                Username = "testuser",
                Email = "test@example.com",
                PasswordHash = PasswordHasher.HashPassword("password123")
            };
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            // Act - search by username
            var resultByUsername = await _repository.GetAsync(new User { Username = "testuser" });

            // Act - search by email
            var resultByEmail = await _repository.GetAsync(new User { Email = "test@example.com" });

            // Assert
            Assert.IsNotNull(resultByUsername);
            Assert.AreEqual("testuser", resultByUsername.Username);

            Assert.IsNotNull(resultByEmail);
            Assert.AreEqual("test@example.com", resultByEmail.Email);
        }

        [TestMethod]
        public async Task UpdateAsync_ShouldUpdateUserInDatabase()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var user = new User
            {
                Id = userId,
                Username = "testuser",
                Email = "test@example.com",
                PasswordHash = PasswordHasher.HashPassword("password123")
            };
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            // Update user details
            user.Username = "updateduser";
            user.Email = "updated@example.com";

            // Act
            var updatedUser = await _repository.UpdateAsync(user);
            var result = await _repository.GetByIdAsync(userId);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("updateduser", result.Username);
            Assert.AreEqual("updated@example.com", result.Email);
        }

        [TestMethod]
        public async Task GetByIdAsync_ShouldReturnNullIfUserDoesNotExist()
        {
            // Act
            var result = await _repository.GetByIdAsync(Guid.NewGuid());

            // Assert
            Assert.IsNull(result);
        }
    }
}
