using System.Text.Json.Serialization;

namespace CareSync.DataLayer.Entities;

/// <summary>
/// Represents individual medication items within a prescription in the CareSync system.
/// This entity stores detailed information about each specific medication prescribed,
/// including dosage, frequency, duration, and administration instructions.
/// Each item represents one medication within a larger prescription document.
/// </summary>
public class T_PrescriptionItems : BaseEntity
{
    /// <summary>
    /// Unique identifier for the prescription item record.
    /// Primary key that serves as the main reference for this specific medication within a prescription.
    /// Auto-incremented integer value assigned when a new medication is added to a prescription.
    /// </summary>
    public int PrescriptionItemID { get; set; }

    /// <summary>
    /// Reference to the parent prescription that contains this medication item.
    /// Links to the PrescriptionID in T_Prescriptions table to group medications within a single prescription.
    /// Required field as every medication item must belong to a specific prescription document.
    /// </summary>
    public int PrescriptionID { get; set; }

    /// <summary>
    /// The name of the prescribed medication or therapeutic agent.
    /// Includes brand names, generic names, or specific drug formulations as prescribed by the doctor.
    /// Nullable during prescription creation but essential for pharmacy dispensing and patient safety.
    /// </summary>
    public string? MedicineName { get; set; }

    /// <summary>
    /// The prescribed dosage amount and strength for this medication.
    /// Specifies exact quantities like "500mg", "2 tablets", "5ml" with appropriate strength information.
    /// Nullable but critical for safe medication administration and therapeutic effectiveness.
    /// </summary>
    public string? Dosage { get; set; }

    /// <summary>
    /// The frequency schedule for taking this medication (e.g., "Twice daily", "Every 8 hours", "As needed").
    /// Defines the timing and interval requirements for optimal therapeutic effect and patient compliance.
    /// Nullable but essential for proper medication adherence and treatment success.
    /// </summary>
    public string? Frequency { get; set; }

    /// <summary>
    /// The total duration for which this medication should be taken (e.g., "7 days", "2 weeks", "Until finished").
    /// Important for treatment completion, medication refills, and preventing antibiotic resistance.
    /// Nullable as some medications may be prescribed for ongoing or as-needed use.
    /// </summary>
    public string? Duration { get; set; }

    /// <summary>
    /// Special instructions or notes specific to this medication item.
    /// Includes administration details like "Take with food", "Avoid alcohol", or "Monitor blood pressure".
    /// Nullable but valuable for patient safety, medication effectiveness, and adverse reaction prevention.
    /// </summary>
    public string? Notes { get; set; }

    // Navigation properties
    /// <summary>
    /// Navigation property to the parent prescription containing this medication item.
    /// Provides access to prescription details, prescribing doctor, patient information, and appointment context.
    /// </summary>
    /// 
    [JsonIgnore]
    public virtual T_Prescriptions? Prescription { get; set; }
}
