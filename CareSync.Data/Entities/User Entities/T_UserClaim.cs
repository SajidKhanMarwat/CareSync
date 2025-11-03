using Microsoft.AspNetCore.Identity;

namespace CareSync.DataLayer.Entities;

/// <summary>
/// Represents custom claims associated with users in the CareSync medical management system.
/// Extends ASP.NET Core Identity's IdentityUserClaim to store additional user-specific
/// attributes and permissions that go beyond standard role-based access control.
/// Claims provide flexible, user-specific metadata for personalization and fine-grained permissions.
/// </summary>
public class T_UserClaim : IdentityUserClaim<Guid>
{
    // Navigation properties
    /// <summary>
    /// Navigation property to the user who owns this claim.
    /// Provides access to the complete user profile, roles, and other associated information.
    /// </summary>
    public virtual T_Users? User { get; set; }
}
