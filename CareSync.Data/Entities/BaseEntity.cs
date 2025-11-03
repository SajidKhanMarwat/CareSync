namespace CareSync.DataLayer.Entities;

/// <summary>
/// Base entity class that provides common audit fields for all entities in the CareSync system.
/// This class contains standard tracking fields for data lifecycle management including
/// soft delete functionality and audit trail information.
/// </summary>
public class BaseEntity
{
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
    public Guid? CreatedBy { get; set; }

    /// <summary>
    /// The date and time when this record was created.
    /// Automatically set to current UTC time when the record is first created.
    /// Nullable to handle legacy data migration scenarios.
    /// </summary>
    public DateTime? CreatedOn { get; set; } = DateTime.UtcNow;

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
