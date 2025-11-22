using CareSync.DataLayer.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CareSync.DataLayer.Configurations.Identity;

/// <summary>
/// Entity configuration for T_Roles
/// </summary>
public class RoleConfiguration : IEntityTypeConfiguration<T_Roles>
{
    public void Configure(EntityTypeBuilder<T_Roles> builder)
    {
        // Table name
        builder.ToTable("T_Roles");

        // Primary key
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasMaxLength(128);

        // Properties
        builder.Property(r => r.Name).IsRequired().HasMaxLength(256);
        // builder.Property(r => r.ArabicName).HasMaxLength(256); // Property doesn't exist - add to T_Roles entity
        builder.Property(r => r.NormalizedName).HasMaxLength(256);

        // Default values
        builder.Property(r => r.IsDeleted).HasDefaultValue(false);
        builder.Property(r => r.CreatedOn).HasDefaultValueSql("GETUTCDATE()");

        // Indexes
        builder.HasIndex(r => r.NormalizedName).IsUnique();

        // Query filter for soft delete
        builder.HasQueryFilter(e => !e.IsDeleted);
    }
}
