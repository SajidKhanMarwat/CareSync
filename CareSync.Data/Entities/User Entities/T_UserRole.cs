using Microsoft.AspNetCore.Identity;

namespace CareSync.DataLayer.Entities;

/// <summary>
/// Represents the many-to-many relationship between users and roles in the CareSync system.
/// Extends ASP.NET Core Identity's IdentityUserRole to manage role assignments for users.
/// This entity determines which roles (and their associated permissions) are granted to each user,
/// enabling role-based access control throughout the medical management platform.
/// </summary>
public class T_UserRole : IdentityUserRole<Guid>
{
    // Navigation properties
    /// <summary>
    /// Navigation property to the user who is assigned the role.
    /// Provides access to complete user profile, demographics, and other role assignments.
    /// </summary>
    public virtual T_Users? User { get; set; }

    /// <summary>
    /// Navigation property to the role being assigned to the user.
    /// Provides access to role permissions, description, and associated rights.
    /// </summary>
    public virtual T_Roles? Role { get; set; }
}