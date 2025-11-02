namespace CareSync.DataLayer.Entities;

/// <summary>
/// Represents the many-to-many relationship between roles and rights in the CareSync system.
/// This junction entity manages the assignment of specific permissions to user roles,
/// enabling fine-grained access control and security management for the medical platform.
/// Each record grants a specific right/permission to a particular role.
/// </summary>
public class T_RoleRights : BaseEntity
{
    /// <summary>
    /// Unique identifier for the role-right assignment record.
    /// Primary key that serves as the main reference for this specific permission assignment.
    /// Auto-incremented integer value assigned when a right is granted to a role.
    /// </summary>
    public int RoleRightID { get; set; }

    /// <summary>
    /// Reference to the role that is being granted the permission.
    /// Links to the RoleID in T_Roles table to identify which role receives the right.
    /// Required field as every permission assignment must be associated with a specific role.
    /// </summary>
    public Guid RoleID { get; set; }

    /// <summary>
    /// Reference to the specific right or permission being granted.
    /// Links to the RightID in T_Rights table to identify which permission is being assigned.
    /// Required field as every assignment must specify which right is being granted.
    /// </summary>
    public int RightID { get; set; }

    // Navigation properties
    /// <summary>
    /// Navigation property to the role that is receiving the permission.
    /// Provides access to role details, description, and other assigned rights.
    /// </summary>
    public virtual T_Roles? Role { get; set; }

    /// <summary>
    /// Navigation property to the specific right or permission being granted.
    /// Provides access to permission details, description, and module information.
    /// </summary>
    public virtual T_Rights? Right { get; set; }
}
