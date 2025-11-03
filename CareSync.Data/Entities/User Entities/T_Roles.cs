using Microsoft.AspNetCore.Identity;

namespace CareSync.DataLayer.Entities;

/// <summary>
/// Represents user roles in the CareSync medical management system.
/// Extends ASP.NET Core Identity's IdentityRole to provide role-based access control
/// for different types of users including doctors, patients, lab technicians, and administrators.
/// Each role defines specific permissions and system access levels within the medical platform.
/// </summary>
public class T_Roles : IdentityRole<Guid>
{
    /// <summary>
    /// Arabic translation of the role name for multilingual support.
    /// Enables the system to display role names in Arabic language for localized user interfaces.
    /// Nullable as not all deployments may require Arabic language support.
    /// </summary>
    public required string RoleName { get; set; }

    /// <summary>
    /// Arabic translation of the role name for multilingual support.
    /// Enables the system to display role names in Arabic language for localized user interfaces.
    /// Nullable as not all deployments may require Arabic language support.
    /// </summary>
    public required string RoleArabicName { get; set; }

    /// <summary>
    /// Detailed description of the role's purpose and responsibilities within the system.
    /// Provides clear explanation of what permissions and access this role grants to users.
    /// Nullable but recommended for administrative clarity and user management.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Indicates whether this record has been soft deleted.
    /// When true, the record is considered deleted but remains in the database for audit purposes.
    /// Default value is false.
    /// </summary>
    public bool IsDeleted { get; set; } = false;

    /// <summary>
    /// The unique identifier of the user who created this record.
    /// References the UserID from T_Users table.
    /// Nullable to handle system-generated records.
    /// </summary>
    public Guid CreatedBy { get; set; }

    /// <summary>
    /// The date and time when this record was created.
    /// Automatically set to current UTC time when the record is first created.
    /// Nullable to handle legacy data migration scenarios.
    /// </summary>
    public DateTime CreatedOn { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// The unique identifier of the user who last updated this record.
    /// References the UserID from T_Users table.
    /// Nullable until the first update operation occurs.
    /// </summary>
    public Guid? UpdatedBy { get; set; }

    /// <summary>
    /// The date and time when this record was last updated.
    /// Set automatically during update operations.
    /// Nullable until the first update operation occurs.
    /// </summary>
    public DateTime? UpdatedOn { get; set; }
}