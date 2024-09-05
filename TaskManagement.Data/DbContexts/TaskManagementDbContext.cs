using Microsoft.EntityFrameworkCore;
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
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Username)
                .IsUnique();

            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            modelBuilder.Entity<User>()
                .HasMany(u => u.UserTasks)
                .WithOne(ut => ut.User)
                .HasForeignKey(ut => ut.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
