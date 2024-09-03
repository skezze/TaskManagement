using TaskManagement.Domain.DTOs;
using TaskManagement.Domain.Entities;

namespace TaskManagement.Application.Interfaces
{
    public interface IUserService
    {
        Task<User> RegisterUserAsync(RegisterDTO registerDto);
        Task<User> LoginUserAsync(LoginDTO loginDto);
        Task<User> GetUserByIdAsync(Guid userId);
        Task<User> UpdateUserAsync(User user);
    }
}
