namespace CareSync.DataLayer.Entities;

/// <summary>
/// Represents individual system rights and permissions in the CareSync medical management system.
/// This entity defines granular permissions that can be assigned to roles to control access
/// to specific features, modules, and operations within the medical platform.
/// Rights provide fine-grained security control for sensitive medical data and system functions.
/// </summary>
public class T_Rights : BaseEntity
{
    /// <summary>
    /// Unique identifier for the system right or permission.
    /// Primary key that serves as the main reference for this specific permission.
    /// Auto-incremented integer value assigned when a new right is defined in the system.
    /// </summary>
    public int RightID { get; set; }

    /// <summary>
    /// The name of the system right or permission.
    /// Should be descriptive and follow a consistent naming convention (e.g., "VIEW_PATIENT_RECORDS", "PRESCRIBE_MEDICATION").
    /// Required field that uniquely identifies the permission within the system.
    /// </summary>
    public required string Name { get; set; }

    /// <summary>
    /// Detailed description of what this right allows users to do in the system.
    /// Provides clear explanation of the permission's scope and limitations for administrative purposes.
    /// Nullable but recommended for security auditing and role management clarity.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// The system module or functional area this right applies to (e.g., "Patients", "Appointments", "Lab Results").
    /// Helps organize permissions by functional areas and simplifies role management.
    /// Nullable but useful for grouping related permissions and system administration.
    /// </summary>
    public string? Module { get; set; }

    // Navigation properties
    /// <summary>
    /// Navigation property to role-right assignments that include this permission.
    /// Shows which roles have been granted this specific right or permission.
    /// </summary>
    public virtual ICollection<T_RoleRights> RoleRights { get; set; } = new List<T_RoleRights>();
}
