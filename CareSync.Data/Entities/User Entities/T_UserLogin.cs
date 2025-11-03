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
}