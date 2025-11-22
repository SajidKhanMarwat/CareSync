using CareSync.DataLayer.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CareSync.DataLayer.Configurations.Identity;

/// <summary>
/// Entity configuration for T_UserLogin
/// </summary>
public class UserLoginConfiguration : IEntityTypeConfiguration<T_UserLogin>
{
    public void Configure(EntityTypeBuilder<T_UserLogin> builder)
    {
        // Table name
        builder.ToTable("T_UserLogins");

        // Composite primary key
        builder.HasKey(ul => new { ul.LoginProvider, ul.ProviderKey });

        // Properties
        builder.Property(ul => ul.LoginProvider).HasMaxLength(128);
        builder.Property(ul => ul.ProviderKey).HasMaxLength(128);
        builder.Property(ul => ul.UserId).HasMaxLength(128);
    }
}
