using CareSync.DataLayer.DataEnums;
using Microsoft.AspNetCore.Identity;

namespace CareSync.DataLayer.Entities;

/// <summary>
/// Represents a user in the CareSync medical management system.
/// Extends ASP.NET Core Identity's IdentityUser with additional medical system-specific fields.
/// This entity stores comprehensive user information including personal details, role assignments,
/// and system access information for doctors, patients, lab technicians, and administrators.
/// </summary>
public class T_Users : IdentityUser<string>
{
    /// <summary>
    /// The role identifier assigned to this user.
    /// References the RoleID from T_Roles table to determine user permissions and access levels.
    /// Required field that defines the user's primary role in the system.
    /// </summary>
    public required string RoleID { get; set; }

    /// <summary>
    /// Optional legacy login identifier for backward compatibility.
    /// Used during system migration or integration with external systems.
    /// Nullable as not all users may have a legacy login ID.
    /// </summary>
    public required int LoginID { get; set; }

    /// <summary>
    /// Arabic version of the username for multilingual support.
    /// Allows the system to display usernames in Arabic language.
    /// Nullable as not all users may require Arabic username display.
    /// </summary>
    public required string ArabicUserName { get; set; }

    /// <summary>
    /// The user's first name or given name.
    /// Used for personalization and formal communication within the system.
    /// Nullable to handle cases where full name information is not available.
    /// </summary>
    public required string FirstName { get; set; }

    /// <summary>
    /// The user's last name or family name.
    /// Combined with FirstName for full name display and formal documentation.
    /// Nullable to handle cases where full name information is not available.
    /// </summary>
    public string? LastName { get; set; }

    /// <summary>
    /// Path or URL to the user's profile image/avatar.
    /// Used for user identification in the UI and personalization.
    /// Nullable as profile images are optional.
    /// </summary>
    public string? ProfileImage { get; set; }

    /// <summary>
    /// The user's gender information.
    /// Used for medical records, statistical analysis, and personalized care.
    /// Nullable to respect privacy preferences and handle incomplete profiles.
    /// </summary>
    public required Gender Gender { get; set; }

    /// <summary>
    /// The user's date of birth.
    /// Critical for age calculations, medical dosage determinations, and age-appropriate care.
    /// Nullable to handle privacy concerns and incomplete registration data.
    /// </summary>
    public DateTime? DateOfBirth { get; set; }

    /// <summary>
    /// Indicates whether the user account is currently active and can access the system.
    /// When false, the user cannot log in or perform any system operations.
    /// Default value is true for new user registrations.
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// String representation of the user's role type for quick reference.
    /// Examples: "Doctor", "Patient", "Lab Technician", "Administrator".
    /// Provides human-readable role information without joining to roles table.
    /// </summary>
    public required RoleType RoleType { get; set; } = RoleType.Patient;

    /// <summary>
    /// Timestamp of the user's last successful login to the system.
    /// Used for security monitoring, session management, and user activity tracking.
    /// Nullable for users who have never logged in.
    /// </summary>
    public DateTime? LastLogin { get; set; }

    /// <summary>
    /// The user's physical address information.
    /// Important for patient records, emergency contacts, and service area determination.
    /// Nullable as address information may not be required for all user types.
    /// </summary>
    public string? Address { get; set; }

    /// <summary>
    /// The user's current age in years.
    /// Calculated field that may be stored for performance optimization.
    /// Used in medical calculations, age-appropriate treatments, and statistical analysis.
    /// Nullable as it can be calculated from DateOfBirth when needed.
    /// </summary>
    public int? Age { get; set; }

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
    public required string CreatedBy { get; set; }

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
    public string? UpdatedBy { get; set; }

    /// <summary>
    /// The date and time when this record was last updated.
    /// Set automatically during update operations.
    /// Nullable until the first update operation occurs.
    /// </summary>
    public DateTime? UpdatedOn { get; set; }

    // Navigation properties
    /// <summary>
    /// Navigation property to the user's assigned role.
    /// Provides access to role-specific permissions and system access levels.
    /// </summary>
    public virtual T_Roles? Role { get; set; }

    ///// <summary>
    ///// Navigation property to doctor-specific details if the user is a doctor.
    ///// Contains medical specialization, qualifications, and practice information.
    ///// Empty collection for non-doctor users.
    ///// </summary>
    //public virtual ICollection<T_DoctorDetails> DoctorDetails { get; set; } = new List<T_DoctorDetails>();

    ///// <summary>
    ///// Navigation property to patient-specific details if the user is a patient.
    ///// Contains medical history, emergency contacts, and patient-specific information.
    ///// Empty collection for non-patient users.
    ///// </summary>
    //public virtual ICollection<T_PatientDetails> PatientDetails { get; set; } = new List<T_PatientDetails>();

    ///// <summary>
    ///// Navigation property to laboratory details if the user represents a lab facility.
    ///// Contains lab-specific information, services offered, and operational details.
    ///// Empty collection for non-lab users.
    ///// </summary>
    //public virtual ICollection<T_Lab> Labs { get; set; } = new List<T_Lab>();
}
