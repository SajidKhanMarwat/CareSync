using Microsoft.AspNetCore.Identity;

namespace CareSync.DataLayer.Entities;

/// <summary>
/// Represents custom claims associated with roles in the CareSync medical management system.
/// Extends ASP.NET Core Identity's IdentityRoleClaim to store additional role-specific
/// attributes and permissions that apply to all users within that role.
/// Role claims provide a way to assign common attributes or permissions to entire user groups.
/// </summary>
public class T_RoleClaim : IdentityRoleClaim<string>
{
}