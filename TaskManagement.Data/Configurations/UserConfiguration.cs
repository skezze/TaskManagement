using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using TaskManagement.Domain.Entities;

namespace TaskManagement.Data.Configurations
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            // Define indexes
            builder.HasIndex(u => u.Username)
                   .IsUnique();

            builder.HasIndex(u => u.Email)
                   .IsUnique();

            // Define relationships
            builder.HasMany(u => u.UserTasks)
                   .WithOne(ut => ut.User)
                   .HasForeignKey(ut => ut.UserId)
                   .OnDelete(DeleteBehavior.Cascade);
        }

    }

}
