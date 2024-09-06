using Microsoft.EntityFrameworkCore;
using TaskManagement.Data.Configurations;
using TaskManagement.Domain.Entities;
using UserTask = TaskManagement.Domain.Entities.UserTask;

namespace TaskManagement.Data.DbContexts
{
    public class TaskManagementDbContext:DbContext
    {
        public TaskManagementDbContext(DbContextOptions<TaskManagementDbContext> options):base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<UserTask> UserTasks { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new UserConfiguration());
        }
    }
}
