using CareSync.DataLayer.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CareSync.DataLayer.Configurations.Identity;

/// <summary>
/// Entity configuration for T_RoleClaim
/// </summary>
public class RoleClaimConfiguration : IEntityTypeConfiguration<T_RoleClaim>
{
    public void Configure(EntityTypeBuilder<T_RoleClaim> builder)
    {
        // Table name
        builder.ToTable("T_RoleClaims");

        // Primary key
        builder.HasKey(rc => rc.Id);
        builder.Property(rc => rc.Id).HasMaxLength(128);

        // Properties
        builder.Property(rc => rc.RoleId).HasMaxLength(128);
        builder.Property(rc => rc.ClaimType).HasMaxLength(256);
    }
}
