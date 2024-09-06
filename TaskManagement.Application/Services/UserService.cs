using Microsoft.Extensions.Logging;
using Serilog;
using ILogger = Serilog.ILogger;
using TaskManagement.Application.Interfaces;
using TaskManagement.Application.Methods;
using TaskManagement.Application.Repositories;
using TaskManagement.Domain.DTOs;
using TaskManagement.Domain.Entities;

namespace TaskManagement.Application.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly ILogger _logger;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
            _logger = Log.ForContext<UserService>();
        }

        /// <summary>
        /// Register a new user in the system.
        /// </summary>
        /// <param name="registerDto">The registration data transfer object containing user details</param>
        /// <returns>The registered user entity, or null if the user already exists</returns>
        public async Task<User> RegisterUserAsync(RegisterDTO registerDto)
        {
            _logger.Warning("Registering new user with Username: {UserName}, Email: {Email}", registerDto.UserName, registerDto.Email);

            var user = new User
            {
                Username = registerDto.UserName,
                Email = registerDto.Email,
                PasswordHash = PasswordHasher.HashPassword(registerDto.Password),
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            if (await _userRepository.GetAsync(user) != null)
            {
                _logger.Warning("User already exists with Username: {UserName} or Email: {Email}", registerDto.UserName, registerDto.Email);
                return null;
            }

            var userInDb = await _userRepository.AddAsync(user);
            _logger.Warning("User registered successfully with ID: {UserId}", userInDb.Id);

            return userInDb;
        }

        /// <summary>
        /// Authenticate a user based on login credentials.
        /// </summary>
        /// <param name="loginDto">The login data transfer object containing username or email and password</param>
        /// <returns>The authenticated user entity, or null if authentication fails</returns>
        public async Task<User> LoginUserAsync(LoginDTO loginDto)
        {
            _logger.Warning("Authenticating user with Username or Email: {UserNameOrEmail}", loginDto.UserNameOrEmail);

            var user = new User
            {
                Username = loginDto.UserNameOrEmail,
                Email = loginDto.UserNameOrEmail,
            };

            var userInDb = await _userRepository.GetAsync(user);

            if (userInDb != null && PasswordHasher.VerifyPassword(loginDto.Password, userInDb.PasswordHash))
            {
                _logger.Warning("User authenticated successfully with Username or Email: {UserNameOrEmail}", loginDto.UserNameOrEmail);
                return userInDb;
            }

            _logger.Warning("Authentication failed for Username or Email: {UserNameOrEmail}", loginDto.UserNameOrEmail);
            return null;
        }

        /// <summary>
        /// Retrieve a user by their unique identifier.
        /// </summary>
        /// <param name="userId">The user's ID</param>
        /// <returns>The user entity, or null if not found</returns>
        public async Task<User> GetUserByIdAsync(Guid userId)
        {
            _logger.Warning("Fetching user with ID: {UserId}", userId);

            var userInDb = await _userRepository.GetByIdAsync(userId);

            if (userInDb == null)
            {
                _logger.Warning("User not found with ID: {UserId}", userId);
            }
            else
            {
                _logger.Warning("User found with ID: {UserId}", userId);
            }

            return userInDb;
        }

        /// <summary>
        /// Update an existing user's information.
        /// </summary>
        /// <param name="user">The user entity with updated details</param>
        /// <returns>The updated user entity</returns>
        public async Task<User> UpdateUserAsync(User user)
        {
            _logger.Warning("Updating user with ID: {UserId}", user.Id);

            user.UpdatedAt = DateTime.UtcNow;
            var userInDb = await _userRepository.UpdateAsync(user);

            _logger.Warning("User updated successfully with ID: {UserId}", user.Id);

            return userInDb;
        }
    }
}
