using CareSync.Shared.Enums;

namespace CareSync.ApplicationLayer.Contracts.UsersDTOs;

/// <summary>
/// DTO for user profile information (avoiding direct entity exposure)
/// </summary>
public class UserProfile_DTO
{
    public string Id { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string? LastName { get; set; }
    public string? ArabicUserName { get; set; }
    public string? ArabicFirstName { get; set; }
    public string? ArabicLastName { get; set; }
    public string FullName => $"{FirstName} {LastName}".Trim();
    public string? ProfileImage { get; set; }
    public Gender_Enum Gender { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public int? Age { get; set; }
    public bool IsActive { get; set; }
    public RoleType RoleType { get; set; }
    public string RoleName { get; set; } = string.Empty;
    public string? Address { get; set; }
    public DateTime? LastLogin { get; set; }
    public DateTime CreatedOn { get; set; }
    public bool TwoFactorEnabled { get; set; }
}

/// <summary>
/// DTO for admin user with additional administrative information
/// </summary>
public class AdminUser_DTO : UserProfile_DTO
{
    public int LoginID { get; set; }
    public string? RoleID { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime? UpdatedOn { get; set; }
    public string? UpdatedBy { get; set; }
}
