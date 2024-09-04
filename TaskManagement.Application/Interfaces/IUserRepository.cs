using TaskManagement.Domain.Entities;

namespace TaskManagement.Application.Interfaces
{
    public interface IUserRepository
    {
        Task<User> GetByIdAsync(Guid id);
        Task<User> GetAsync(User user);
        Task<User> AddAsync(User user);
        Task<User> UpdateAsync(User user);
    }
}
