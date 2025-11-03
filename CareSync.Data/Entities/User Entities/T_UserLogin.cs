using Microsoft.AspNetCore.Identity;

namespace CareSync.DataLayer.Entities;

/// <summary>
/// Represents external login providers for users in the CareSync medical management system.
/// Extends ASP.NET Core Identity's IdentityUserLogin to manage third-party authentication
/// such as Google, Facebook, Microsoft, or other OAuth providers.
/// Enables users to log in using external credentials while maintaining their medical records.
/// </summary>
public class T_UserLogin : IdentityUserLogin<Guid>
{
    // Navigation properties
    /// <summary>
    /// Navigation property to the user account associated with this external login.
    /// Provides access to the complete user profile and medical information linked to the external authentication.
    /// </summary>
    public virtual T_Users? User { get; set; }
}