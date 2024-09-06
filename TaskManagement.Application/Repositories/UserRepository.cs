using Microsoft.EntityFrameworkCore;
using Serilog;
using ILogger = Serilog.ILogger;
using TaskManagement.Application.Interfaces;
using TaskManagement.Data.DbContexts;
using TaskManagement.Domain.Entities;

namespace TaskManagement.Application.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly TaskManagementDbContext _context;
        private readonly ILogger _logger;

        public UserRepository(TaskManagementDbContext context)
        {
            _context = context;
            _logger = Log.ForContext<UserRepository>(); // Serilog context for this repository
        }

        /// <summary>
        /// Retrieve a user by their unique identifier.
        /// </summary>
        /// <param name="id">The user's ID</param>
        /// <returns>The user entity, or null if not found</returns>
        public async Task<User> GetByIdAsync(Guid id)
        {
            _logger.Information("Fetching user by ID: {UserId}", id);

            var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == id);

            if (user == null)
            {
                _logger.Warning("User not found with ID: {UserId}", id);
            }
            else
            {
                _logger.Information("User found with ID: {UserId}", id);
            }

            return user;
        }

        /// <summary>
        /// Retrieve a user based on their username or email.
        /// </summary>
        /// <param name="user">User object containing username or email</param>
        /// <returns>The user entity, or null if not found</returns>
        public async Task<User> GetAsync(User user)
        {
            _logger.Information("Fetching user by Username: {Username} or Email: {Email}", user.Username, user.Email);

            // Include UserTasks relationship to fetch associated tasks
            var userInDb = await _context.Users
                .Include(x => x.UserTasks)
                .FirstOrDefaultAsync(x => x.Username == user.Username || x.Email == user.Email);

            if (userInDb == null)
            {
                _logger.Warning("User not found with Username: {Username} or Email: {Email}", user.Username, user.Email);
            }
            else
            {
                _logger.Information("User found with Username: {Username} or Email: {Email}", userInDb.Username, userInDb.Email);
            }

            return userInDb;
        }

        /// <summary>
        /// Add a new user to the database.
        /// </summary>
        /// <param name="user">The user entity to be added</param>
        /// <returns>The added user entity</returns>
        public async Task<User> AddAsync(User user)
        {
            _logger.Information("Adding new user: {Username} with Email: {Email}", user.Username, user.Email);

            // Add the user entity to the DbContext
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            _logger.Information("User successfully added with ID: {UserId}", user.Id);

            return user;
        }

        /// <summary>
        /// Update an existing user in the database.
        /// </summary>
        /// <param name="user">The user entity to be updated</param>
        /// <returns>The updated user entity</returns>
        public async Task<User> UpdateAsync(User user)
        {
            _logger.Information("Updating user: {Username} with ID: {UserId}", user.Username, user.Id);

            // Update the user entity in the DbContext
            _context.Users.Update(user);
            await _context.SaveChangesAsync();

            _logger.Information("User successfully updated with ID: {UserId}", user.Id);

            return user;
        }
    }
}
