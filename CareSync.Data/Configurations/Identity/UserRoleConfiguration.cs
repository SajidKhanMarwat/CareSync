using CareSync.DataLayer.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CareSync.DataLayer.Configurations.Identity;

/// <summary>
/// Entity configuration for T_UserRole (many-to-many relationship)
/// </summary>
public class UserRoleConfiguration : IEntityTypeConfiguration<T_UserRole>
{
    public void Configure(EntityTypeBuilder<T_UserRole> builder)
    {
        // Table name
        builder.ToTable("T_UserRoles");

        // Composite primary key
        builder.HasKey(ur => new { ur.UserId, ur.RoleId });

        // Properties
        builder.Property(ur => ur.UserId).HasMaxLength(128);
        builder.Property(ur => ur.RoleId).HasMaxLength(128);

        // Relationships
        builder.HasOne(ur => ur.User)
            .WithMany(u => u.UserRole)
            .HasForeignKey(ur => ur.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(ur => ur.Role)
            .WithMany(r => r.UserRoles)
            .HasForeignKey(ur => ur.RoleId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
