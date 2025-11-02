namespace CareSync.DataLayer.Entities;

/// <summary>
/// Represents medical follow-up and ongoing care tracking for patients in the CareSync system.
/// This entity manages continuity of care by tracking visit history, examination results,
/// scheduled reviews, and daily care notes. Essential for chronic disease management,
/// treatment monitoring, and ensuring consistent medical care over time.
/// </summary>
public class T_MedicalFollowUp : BaseEntity
{
    /// <summary>
    /// Unique identifier for the medical follow-up record.
    /// Primary key that serves as the main reference for this specific follow-up care sequence.
    /// Auto-incremented integer value assigned when a new follow-up plan is established.
    /// </summary>
    public int FollowUpID { get; set; }

    /// <summary>
    /// Reference to the patient receiving follow-up medical care.
    /// Links to the PatientID in T_PatientDetails table to associate follow-up care with the correct patient.
    /// Required field as every follow-up record must belong to a specific patient.
    /// </summary>
    public int PatientID { get; set; }

    /// <summary>
    /// Date of the patient's most recent medical visit or consultation.
    /// Important for tracking care continuity, appointment intervals, and treatment timeline.
    /// Nullable as some follow-up plans may be established before the first visit occurs.
    /// </summary>
    public DateTime? LastVisitDate { get; set; }

    /// <summary>
    /// Summary of recent medical examinations, tests, and clinical assessments performed.
    /// Contains key findings, test results, and clinical observations from recent medical encounters.
    /// Nullable but valuable for tracking patient progress and treatment effectiveness.
    /// </summary>
    public string? RecentExaminations { get; set; }

    /// <summary>
    /// Scheduled date for the next medical review or follow-up appointment.
    /// Critical for care continuity, treatment monitoring, and ensuring timely medical intervention.
    /// Nullable as review schedules may be flexible or determined based on patient response.
    /// </summary>
    public DateTime? UpcomingReviewDate { get; set; }

    /// <summary>
    /// Daily care notes and observations about the patient's condition and progress.
    /// Includes symptom tracking, medication adherence, side effects, and general health status.
    /// Nullable but important for detailed care monitoring and treatment adjustment decisions.
    /// </summary>
    public string? DailyNotes { get; set; }

    // Navigation properties
    /// <summary>
    /// Navigation property to the patient receiving follow-up care.
    /// Provides access to complete patient profile, medical history, and current treatment plans.
    /// </summary>
    public virtual T_PatientDetails? Patient { get; set; }
}
