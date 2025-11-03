namespace CareSync.DataLayer.Entities;

/// <summary>
/// Represents additional medical notes and care instructions for patients in the CareSync system.
/// This entity stores supplementary medical information including nutritional plans,
/// doctor recommendations, and special care instructions that complement the primary medical records.
/// Essential for comprehensive patient care coordination and treatment plan management.
/// </summary>
public class T_AdditionalNotes : BaseEntity
{
    /// <summary>
    /// Unique identifier for the additional notes record.
    /// Primary key that serves as the main reference for this specific set of supplementary medical information.
    /// Auto-incremented integer value assigned when new additional notes are created for a patient.
    /// </summary>
    public int NoteID { get; set; }

    /// <summary>
    /// Reference to the patient for whom these additional notes are documented.
    /// Links to the PatientID in T_PatientDetails table to associate notes with the correct patient.
    /// Required field as every additional note must belong to a specific patient.
    /// </summary>
    public int PatientID { get; set; }

    /// <summary>
    /// Detailed nutritional plan and dietary recommendations for the patient.
    /// Includes specific dietary restrictions, meal plans, nutritional supplements, and eating guidelines.
    /// Nullable but important for patients with diabetes, heart conditions, or other diet-sensitive conditions.
    /// </summary>
    public string? NutritionalPlan { get; set; }

    /// <summary>
    /// Specific recommendations and advice provided by the attending doctor.
    /// Contains medical guidance, lifestyle modifications, follow-up instructions, and care protocols.
    /// Nullable but valuable for continuity of care and treatment plan adherence.
    /// </summary>
    public string? DoctorRecommendations { get; set; }

    /// <summary>
    /// Special care instructions and important notes for medical staff and caregivers.
    /// Includes handling precautions, emergency procedures, medication administration notes, and care alerts.
    /// Nullable but critical for patients with complex conditions or special care requirements.
    /// </summary>
    public string? SpecialInstructions { get; set; }

    // Navigation properties
    /// <summary>
    /// Navigation property to the patient for whom these additional notes are documented.
    /// Provides access to complete patient profile, medical history, and other care information.
    /// </summary>
    public virtual T_PatientDetails? Patient { get; set; }
}
