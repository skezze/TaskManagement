using Microsoft.Extensions.Logging;
using TaskManagement.Application.Interfaces;
using TaskManagement.Application.Methods;
using TaskManagement.Domain.DTOs;
using TaskManagement.Domain.Entities;

namespace TaskManagement.Application.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly ILogger<UserService> _logger;

        public UserService(IUserRepository userRepository, ILogger<UserService> logger)
        {
            _userRepository = userRepository;
            _logger = logger;
        }

        public async Task<User> RegisterUserAsync(RegisterDTO registerDto)
        {
            var user = await _userRepository.AddAsync(registerDto);
            _logger.LogInformation($"User {user.Username} registered successfully.");

            return user;
        }

        public async Task<User> LoginUserAsync(LoginDTO loginDto)
        {
            var user = await _userRepository.GetAsync(loginDto);
            //_logger.LogInformation($"User {user.Username} login successfully.");

            return user;
        }

        public async Task<User> GetUserByIdAsync(Guid userId)
        {
            return await _userRepository.GetByIdAsync(userId);
        }

        public async Task<User> UpdateUserAsync(User user)
        {
            user.UpdatedAt = DateTime.UtcNow;
            await _userRepository.UpdateAsync(user);
            _logger.LogInformation($"User {user.Id} updated successfully.");
            return user;
        }
    }
}
