using Microsoft.EntityFrameworkCore;
using TaskManagement.Application.Interfaces;
using TaskManagement.Data.DbContexts;
using TaskManagement.Domain.Entities;

namespace TaskManagement.Application.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly TaskManagementDbContext _context;

        public UserRepository(TaskManagementDbContext context)
        {
            _context = context;
        }

        public async Task<User> GetByIdAsync(Guid id)
        {
            return await _context.Users.FirstOrDefaultAsync(x=>x.Id == id);
        }

        public async Task<User> GetAsync(User user)
        {
            var userInDb = await _context.Users.FirstOrDefaultAsync(x =>
            (x.Username == user.Username ||
            x.Email == user.Email) &&
            x.PasswordHash == user.PasswordHash);

            return userInDb;
        }

        public async Task<User> AddAsync(User user)
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return user;
        }

        public async Task<User> UpdateAsync(User user)
        {
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
            return user;
        }
    }
}
