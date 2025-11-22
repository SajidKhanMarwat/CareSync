using CareSync.DataLayer.Entities;
using CareSync.Shared.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CareSync.DataLayer.Configurations.Identity;

/// <summary>
/// Entity configuration for T_Users with enum string conversion and validation rules
/// </summary>
public class UserConfiguration : IEntityTypeConfiguration<T_Users>
{
    public void Configure(EntityTypeBuilder<T_Users> builder)
    {
        // Table name
        builder.ToTable("T_Users");

        // Primary key
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasMaxLength(128);

        // Required fields
        builder.Property(u => u.UserName).IsRequired().HasMaxLength(256);
        builder.Property(u => u.Email).IsRequired().HasMaxLength(256);
        builder.Property(u => u.FirstName).IsRequired().HasMaxLength(100);

        // Optional fields with max length
        builder.Property(u => u.LastName).HasMaxLength(100);
        builder.Property(u => u.ArabicUserName).HasMaxLength(256);
        builder.Property(u => u.ArabicFirstName).HasMaxLength(100);
        builder.Property(u => u.ArabicLastName).HasMaxLength(100);
        builder.Property(u => u.PhoneNumber).HasMaxLength(20);
        builder.Property(u => u.Address).HasMaxLength(500);
        builder.Property(u => u.ProfileImage).HasMaxLength(500);
        builder.Property(u => u.RoleID).HasMaxLength(128);

        // Enum conversions to string
        builder.Property(u => u.Gender)
            .HasConversion<string>()
            .HasMaxLength(50);

        builder.Property(u => u.RoleType)
            .HasConversion<string>()
            .HasMaxLength(50);

        // Default values
        builder.Property(u => u.IsActive).HasDefaultValue(true);
        builder.Property(u => u.IsDeleted).HasDefaultValue(false);
        builder.Property(u => u.CreatedOn).HasDefaultValueSql("GETUTCDATE()");

        // Indexes
        builder.HasIndex(u => u.Email).IsUnique();
        builder.HasIndex(u => u.UserName).IsUnique();
        builder.HasIndex(u => u.RoleID);

        // Relationships
        builder.HasOne(u => u.Role)
            .WithMany()
            .HasForeignKey(u => u.RoleID)
            .OnDelete(DeleteBehavior.Restrict);

        // Query filter for soft delete
        builder.HasQueryFilter(e => !e.IsDeleted);
    }
}
