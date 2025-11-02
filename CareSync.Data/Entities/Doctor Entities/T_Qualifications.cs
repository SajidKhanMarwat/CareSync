namespace CareSync.DataLayer.Entities;

/// <summary>
/// Represents detailed educational qualifications and certifications for doctors in the CareSync system.
/// This entity stores comprehensive academic and professional credentials including degrees,
/// certifications, institutions, and supporting documentation for medical practice verification.
/// Critical for regulatory compliance, professional credibility, and patient trust.
/// </summary>
public class T_Qualifications : BaseEntity
{
    /// <summary>
    /// Unique identifier for the qualification record.
    /// Primary key that serves as the main reference for this specific educational credential.
    /// Auto-incremented integer value assigned when a new qualification is added to a doctor's profile.
    /// </summary>
    public int QualificationID { get; set; }

    /// <summary>
    /// Reference to the doctor who holds this qualification.
    /// Links to the DoctorID in T_DoctorDetails table to associate credentials with the correct doctor.
    /// Required field as every qualification must belong to a specific doctor.
    /// </summary>
    public int DoctorID { get; set; }

    /// <summary>
    /// The name of the degree or certification obtained (e.g., "MBBS", "MD Cardiology", "PhD Medicine").
    /// Essential for understanding the doctor's educational background and areas of expertise.
    /// Nullable to handle cases where degree information is being verified or updated.
    /// </summary>
    public string? Degree { get; set; }

    /// <summary>
    /// The name of the educational institution or certifying body that awarded the qualification.
    /// Important for credential verification, institutional reputation assessment, and professional validation.
    /// Nullable during initial data entry but critical for complete professional profile.
    /// </summary>
    public string? Institution { get; set; }

    /// <summary>
    /// The year when the degree or certification was completed or awarded.
    /// Used for chronological ordering of qualifications and experience timeline calculation.
    /// Nullable as some historical qualifications may not have precise completion dates.
    /// </summary>
    public int? YearOfCompletion { get; set; }

    /// <summary>
    /// Text content or description of the certificate or qualification details.
    /// May contain additional information about the qualification, honors, or special recognitions.
    /// Nullable as not all qualifications require detailed textual descriptions.
    /// </summary>
    public string? Certificate { get; set; }

    /// <summary>
    /// File path or URL to the digital copy of the certificate or qualification document.
    /// Essential for document verification, regulatory compliance, and credential authentication.
    /// Nullable during initial entry but important for complete verification process.
    /// </summary>
    public string? CertificatePath { get; set; }

    // Navigation properties
    /// <summary>
    /// Navigation property to the doctor who holds this qualification.
    /// Provides access to complete doctor profile, specialization, and other professional details.
    /// </summary>
    public virtual T_DoctorDetails? Doctor { get; set; }
}
