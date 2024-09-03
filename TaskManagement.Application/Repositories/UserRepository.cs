using Microsoft.EntityFrameworkCore;
using TaskManagement.Application.Interfaces;
using TaskManagement.Application.Methods;
using TaskManagement.Data.DbContexts;
using TaskManagement.Domain.DTOs;
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

        public async Task<User> GetAsync(LoginDTO loginDto)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x =>
            x.Username == loginDto.UserNameOrEmail ||
            x.Email == loginDto.UserNameOrEmail &&
            x.PasswordHash == PasswordHasher.HashPassword(loginDto.Password));
            return user;
        }

        public async Task<User> AddAsync(RegisterDTO registerDto)
        {
            var user = new User
            {
                Username = registerDto.UserName,
                Email = registerDto.Email,
                PasswordHash = PasswordHasher.HashPassword(registerDto.Password),
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
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
