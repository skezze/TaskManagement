using TaskManagement.Domain.DTOs;
using TaskManagement.Domain.Entities;

namespace TaskManagement.Application.Interfaces
{
    public interface IUserRepository
    {
        Task<User> GetByIdAsync(Guid id);
        Task<User> GetAsync(LoginDTO loginDto);
        Task<User> AddAsync(RegisterDTO registerDto);
        Task<User> UpdateAsync(User user);
    }
}
