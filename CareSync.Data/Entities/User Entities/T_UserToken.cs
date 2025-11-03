using Microsoft.AspNetCore.Identity;

namespace CareSync.DataLayer.Entities;

/// <summary>
/// Represents authentication tokens for users in the CareSync medical management system.
/// Extends ASP.NET Core Identity's IdentityUserToken to manage security tokens such as
/// password reset tokens, email confirmation tokens, and two-factor authentication tokens.
/// Critical for secure user authentication and account verification processes.
/// </summary>
public class T_UserToken : IdentityUserToken<Guid>
{
    // Navigation properties
    /// <summary>
    /// Navigation property to the user who owns this authentication token.
    /// Provides access to user account details and security settings for token validation.
    /// </summary>
    public virtual T_Users? User { get; set; }
}