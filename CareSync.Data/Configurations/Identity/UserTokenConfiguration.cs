using CareSync.DataLayer.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CareSync.DataLayer.Configurations.Identity;

/// <summary>
/// Entity configuration for T_UserToken
/// </summary>
public class UserTokenConfiguration : IEntityTypeConfiguration<T_UserToken>
{
    public void Configure(EntityTypeBuilder<T_UserToken> builder)
    {
        // Table name
        builder.ToTable("T_UserTokens");

        // Composite primary key
        builder.HasKey(ut => new { ut.UserId, ut.LoginProvider, ut.Name });

        // Properties
        builder.Property(ut => ut.UserId).HasMaxLength(128);
        builder.Property(ut => ut.LoginProvider).HasMaxLength(128);
        builder.Property(ut => ut.Name).HasMaxLength(128);
    }
}
