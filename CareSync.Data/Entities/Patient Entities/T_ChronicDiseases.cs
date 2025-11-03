namespace CareSync.DataLayer.Entities;

/// <summary>
/// Represents chronic diseases and long-term medical conditions for patients in the CareSync system.
/// This entity tracks ongoing health conditions that require continuous monitoring, treatment,
/// and management over extended periods. Critical for comprehensive patient care planning,
/// medication management, and long-term health outcome tracking.
/// </summary>
public class T_ChronicDiseases : BaseEntity
{
    /// <summary>
    /// Unique identifier for the chronic disease record.
    /// Primary key that serves as the main reference for this specific chronic condition.
    /// Auto-incremented integer value assigned when a new chronic disease is diagnosed and recorded.
    /// </summary>
    public int DiseaseID { get; set; }

    /// <summary>
    /// Reference to the patient who has been diagnosed with this chronic disease.
    /// Links to the PatientID in T_PatientDetails table to associate the condition with the correct patient.
    /// Required field as every chronic disease record must belong to a specific patient.
    /// </summary>
    public int PatientID { get; set; }

    /// <summary>
    /// The name of the chronic disease or long-term medical condition (e.g., "Type 2 Diabetes", "Hypertension", "Asthma").
    /// Essential for medical decision-making, treatment planning, and medication management.
    /// Nullable during initial assessment but critical for complete medical record documentation.
    /// </summary>
    public string? DiseaseName { get; set; }

    /// <summary>
    /// The date when the chronic disease was first diagnosed or confirmed.
    /// Important for tracking disease progression, treatment duration, and medical history timeline.
    /// Nullable as some chronic conditions may have uncertain or gradual onset dates.
    /// </summary>
    public DateTime? DiagnosedDate { get; set; }

    /// <summary>
    /// Current status of the chronic disease (e.g., "Controlled", "Uncontrolled", "In Remission", "Progressive").
    /// Critical for treatment adjustment, monitoring frequency, and care intensity determination.
    /// Nullable but important for ongoing care management and treatment effectiveness assessment.
    /// </summary>
    public string? CurrentStatus { get; set; }

    // Navigation properties
    /// <summary>
    /// Navigation property to the patient who has this chronic disease.
    /// Provides access to complete patient profile, other medical conditions, and treatment history.
    /// </summary>
    public virtual T_PatientDetails? Patient { get; set; }
}
