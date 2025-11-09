namespace CareSync.DataLayer.Entities;

/// <summary>
/// Represents comprehensive medical history information for patients in the CareSync system.
/// This entity stores critical historical medical data including past diagnoses, surgeries,
/// allergies, and family medical history. This information is essential for making informed
/// medical decisions, avoiding adverse drug reactions, and understanding genetic predispositions.
/// </summary>
public class T_MedicalHistory : BaseEntity
{
    /// <summary>
    /// Unique identifier for the medical history record.
    /// Primary key that serves as the main reference for this patient's historical medical information.
    /// Auto-incremented integer value assigned when medical history is first documented.
    /// </summary>
    public int MedicalHistoryID { get; set; }

    /// <summary>
    /// Reference to the patient whose medical history is being documented.
    /// Links to the PatientID in T_PatientDetails table to associate history with the correct patient.
    /// Required field as every medical history record must belong to a specific patient.
    /// </summary>
    public int PatientID { get; set; }

    /// <summary>
    /// The primary or most significant current medical diagnosis for the patient.
    /// Represents the main health condition that requires ongoing medical attention or treatment.
    /// Nullable as some patients may not have a primary diagnosis or may be undergoing evaluation.
    /// </summary>
    public string? MainDiagnosis { get; set; }

    /// <summary>
    /// List of chronic diseases and long-term medical conditions affecting the patient.
    /// Includes conditions like diabetes, hypertension, arthritis, or other ongoing health issues.
    /// Nullable but important for comprehensive care planning and medication management.
    /// </summary>
    public string? ChronicDiseases { get; set; }

    /// <summary>
    /// Known allergies and adverse reactions to medications, foods, or environmental factors.
    /// Critical safety information to prevent allergic reactions and guide treatment decisions.
    /// Nullable but extremely important for patient safety and medication prescribing.
    /// </summary>
    public string? Allergies { get; set; }

    /// <summary>
    /// Historical record of past diseases, infections, and medical conditions the patient has experienced.
    /// Provides context for current health status and potential complications or recurrences.
    /// Nullable but valuable for understanding the patient's complete medical journey.
    /// </summary>
    public string? PastDiseases { get; set; }

    /// <summary>
    /// Record of all surgical procedures the patient has undergone.
    /// Includes dates, types of surgery, complications, and outcomes when available.
    /// Nullable but important for understanding anatomical changes and potential surgical risks.
    /// </summary>
    public string? Surgery { get; set; }

    /// <summary>
    /// Family medical history including genetic conditions and hereditary diseases.
    /// Important for assessing genetic risk factors and implementing preventive care measures.
    /// Nullable as family history information may not always be available or complete.
    /// </summary>
    public string? FamilyHistory { get; set; }

    // Navigation properties
    /// <summary>
    /// Navigation property to the patient whose medical history is documented.
    /// Provides access to current patient information, demographics, and other medical records.
    /// </summary>
    public virtual T_PatientDetails? Patient { get; set; }
}
