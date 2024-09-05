using Microsoft.EntityFrameworkCore;
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

        public UserService(IUserRepository userRepository, ILogger<UserService> logger)
        {
            _userRepository = userRepository;
        }

        public async Task<User> RegisterUserAsync(RegisterDTO registerDto)
        {
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
                return null;
            }
            var userInDb = await _userRepository.AddAsync(user);
            
            return userInDb;
        }

        public async Task<User> LoginUserAsync(LoginDTO loginDto)
        {
            var user = new User()
            {
                Username = loginDto.UserNameOrEmail,
                Email = loginDto.UserNameOrEmail,
            };            

            var userInDb = await _userRepository.GetAsync(user);

            if (PasswordHasher.VerifyPassword(loginDto.Password, userInDb.PasswordHash)) 
            {
                return userInDb;
            }

            return null;
            
        }

        public async Task<User> GetUserByIdAsync(Guid userId)
        {
            var userInDb = await _userRepository.GetByIdAsync(userId);
            return userInDb;
        }

        public async Task<User> UpdateUserAsync(User user)
        {
            user.UpdatedAt = DateTime.UtcNow;
            var userInDb = await _userRepository.UpdateAsync(user);
            return userInDb;
        }
    }
}
