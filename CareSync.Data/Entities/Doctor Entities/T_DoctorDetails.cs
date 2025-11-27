using CareSync.Shared.Enums;

namespace CareSync.DataLayer.Entities;

/// <summary>
/// Represents detailed professional information for doctors in the CareSync medical management system.
/// This entity stores comprehensive doctor profiles including medical specializations, qualifications,
/// practice details, availability schedules, and consultation fees.
/// Essential for patient appointment booking, doctor verification, and medical service management.
/// </summary>
public class T_DoctorDetails : BaseEntity
{
    /// <summary>
    /// Unique identifier for the doctor's professional profile.
    /// Primary key that serves as the main reference for all doctor-related medical activities.
    /// Auto-incremented integer value assigned when a doctor registers in the system.
    /// </summary>
    public int DoctorID { get; set; }

    /// <summary>
    /// Reference to the associated user account in the T_Users table.
    /// Links the doctor's professional profile to their login credentials and basic user information.
    /// Required field as every doctor must have a corresponding user account for system access.
    /// </summary>
    public required string UserID { get; set; }

    /// <summary>
    /// The doctor's medical specialization or field of expertise (e.g., Cardiology, Pediatrics, Orthopedics).
    /// Critical for patient matching, appointment categorization, and medical service classification.
    /// Nullable to handle cases where specialization is being updated or verified.
    /// </summary>
    public string? Specialization { get; set; }
    public string? ArabicSpecialization { get; set; }
    public string? ClinicAddress { get; set; }
    public string? ArabicClinicAddress { get; set; }

    /// <summary>
    /// Number of years of medical practice experience.
    /// Important for patient confidence, appointment prioritization, and medical expertise assessment.
    /// Nullable as experience information may not be immediately available during registration.
    /// </summary>
    public int? ExperienceYears { get; set; }

    /// <summary>
    /// The doctor's medical license number for professional verification.
    /// Critical for regulatory compliance, credential verification, and legal medical practice authorization.
    /// Nullable during initial registration but required for full practice authorization.
    /// </summary>
    public string? LicenseNumber { get; set; }

    /// <summary>
    /// Summary of the doctor's educational qualifications and certifications.
    /// Provides overview of medical degrees, specialization certificates, and professional achievements.
    /// Nullable but important for patient information and professional credibility.
    /// </summary>
    public string? QualificationSummary { get; set; }

    /// <summary>
    /// Name of the hospital, clinic, or medical institution the doctor is affiliated with.
    /// Important for patient referrals, insurance processing, and professional networking.
    /// Nullable as some doctors may have independent practices or multiple affiliations.
    /// </summary>
    public string? HospitalAffiliation { get; set; }

    /// <summary>
    /// Days of the week when the doctor is available for appointments (e.g., "Monday, Wednesday, Friday").
    /// Critical for appointment scheduling and patient booking system functionality.
    /// Nullable during initial setup but important for operational scheduling.
    /// </summary>
    public string AvailableDays { get; set; } = null!;

    /// <summary>
    /// Daily start time for the doctor's availability window.
    /// Defines when the doctor begins accepting appointments each available day.
    /// Nullable but essential for accurate appointment scheduling and time slot management.
    /// </summary>
    public string? StartTime { get; set; }

    /// <summary>
    /// Daily end time for the doctor's availability window.
    /// Defines when the doctor stops accepting appointments each available day.
    /// Nullable but essential for accurate appointment scheduling and time slot management.
    /// </summary>
    public string? EndTime { get; set; }

    // Navigation properties
    /// <summary>
    /// Navigation property to the associated user account.
    /// Provides access to login credentials, basic profile information, and system access details.
    /// </summary>
    public virtual T_Users? User { get; set; }

    /// <summary>
    /// Navigation property to all appointments scheduled with this doctor.
    /// Contains past, current, and future patient consultations and medical encounters.
    /// </summary>
    public virtual ICollection<T_Appointments> Appointments { get; set; } = new List<T_Appointments>();

    /// <summary>
    /// Navigation property to all prescriptions issued by this doctor.
    /// Contains medication prescriptions and treatment plans provided during consultations.
    /// </summary>
    public virtual ICollection<T_Prescriptions> Prescriptions { get; set; } = new List<T_Prescriptions>();

    /// <summary>
    /// Navigation property to detailed qualifications and certifications for this doctor.
    /// Contains comprehensive educational background, certifications, and professional achievements.
    /// </summary>
    public virtual ICollection<T_Qualifications> Qualifications { get; set; } = new List<T_Qualifications>();
}


