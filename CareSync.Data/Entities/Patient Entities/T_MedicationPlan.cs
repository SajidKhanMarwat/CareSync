namespace CareSync.DataLayer.Entities;

/// <summary>
/// Represents a medication plan or treatment regimen for patients in the CareSync system.
/// This entity manages long-term medication schedules, chronic disease treatments, and
/// ongoing therapeutic plans. It differs from prescriptions by focusing on sustained
/// medication management rather than single consultation prescriptions.
/// </summary>
public class T_MedicationPlan : BaseEntity
{
    /// <summary>
    /// Unique identifier for the medication plan record.
    /// Primary key that serves as the main reference for this specific medication regimen.
    /// Auto-incremented integer value assigned when a new medication plan is created.
    /// </summary>
    public int MedicationID { get; set; }

    /// <summary>
    /// Reference to the patient for whom this medication plan is designed.
    /// Links to the PatientID in T_PatientDetails table to associate the plan with the correct patient.
    /// Required field as every medication plan must belong to a specific patient.
    /// </summary>
    public int PatientID { get; set; }

    /// <summary>
    /// The name of the medication or therapeutic agent in the treatment plan.
    /// Includes brand names, generic names, or therapeutic categories as appropriate.
    /// Nullable to handle cases where medication plans are being developed or modified.
    /// </summary>
    public string? MedicationName { get; set; }

    /// <summary>
    /// The prescribed dosage amount and strength for the medication.
    /// Includes specific quantities like "500mg", "2 tablets", or "5ml" with strength information.
    /// Nullable but critical for safe medication administration and therapeutic effectiveness.
    /// </summary>
    public string? Dosage { get; set; }

    /// <summary>
    /// The frequency schedule for taking the medication (e.g., "Twice daily", "Every 8 hours", "As needed").
    /// Defines the timing and interval requirements for optimal therapeutic effect.
    /// Nullable but essential for proper medication adherence and treatment success.
    /// </summary>
    public string? Frequency { get; set; }

    /// <summary>
    /// The total duration for which the medication should be taken (e.g., "30 days", "6 months", "Ongoing").
    /// Important for treatment planning, medication refills, and monitoring therapy effectiveness.
    /// Nullable as some medications may be prescribed for indefinite periods or until further review.
    /// </summary>
    public string? Duration { get; set; }

    /// <summary>
    /// Special instructions for medication administration, storage, or monitoring.
    /// Includes details like "Take with food", "Monitor blood pressure", or "Avoid alcohol".
    /// Nullable but valuable for patient safety and optimal therapeutic outcomes.
    /// </summary>
    public string? Instructions { get; set; }

    /// <summary>
    /// Current status of the medication plan (true for active, false for discontinued).
    /// Indicates whether the patient should currently be following this medication regimen.
    /// Nullable to handle cases where status is being evaluated or transitioning between states.
    /// </summary>
    public bool? Status { get; set; }

    // Navigation properties
    /// <summary>
    /// Navigation property to the patient following this medication plan.
    /// Provides access to patient demographics, medical history, and other treatment information.
    /// </summary>
    public virtual T_PatientDetails? Patient { get; set; }
}
