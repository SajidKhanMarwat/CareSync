using CareSync.DataLayer.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CareSync.DataLayer;

public class CareSyncDbContext : IdentityDbContext<T_Users, T_Roles, string, IdentityUserClaim<string>, T_UserRole, T_UserLogin, T_RoleClaim, T_UserToken>
{
    public CareSyncDbContext(DbContextOptions<CareSyncDbContext> options) : base(options)
    {        
    }

    DbSet<T_Users> T_Users { get; set; }
    DbSet<T_UserRole> T_UserRole { get; set; }
    DbSet<T_UserLogin> T_UserLogins { get; set; }
    DbSet<T_UserToken> T_UserTokens { get; set; }
    DbSet<T_Roles> T_Roles { get; set; }
    DbSet<T_RoleClaim> T_RoleClaims { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        #region User Entities
        var users = builder.Entity<T_Users>();
        var userRoles = builder.Entity<T_UserRole>();
        var userLogins = builder.Entity<T_UserLogin>();
        var userTokens = builder.Entity<T_UserToken>();
        #endregion

        #region Role Entities
        var roles = builder.Entity<T_Roles>();
        var roleClaims = builder.Entity<T_RoleClaim>();
        #endregion
    }
}