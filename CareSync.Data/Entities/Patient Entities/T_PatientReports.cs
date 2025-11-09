namespace CareSync.DataLayer.Entities;

/// <summary>
/// Represents patient reports and medical documents in the CareSync system.
/// This entity manages the storage and organization of medical documents, reports,
/// and files associated with patient care including lab results, imaging reports,
/// consultation summaries, and other medical documentation.
/// Note: This entity does not inherit from BaseEntity as per the original database schema.
/// </summary>
public class T_PatientReports
{
    /// <summary>
    /// Unique identifier for the patient report record.
    /// Primary key using GUID format that serves as the main reference for this specific medical document.
    /// GUID value assigned when a new patient report is uploaded or generated in the system.
    /// </summary>
    public int PatientReportID { get; set; }

    /// <summary>
    /// Reference to the appointment during which this report was generated or discussed.
    /// Links to appointment records to associate reports with specific medical consultations.
    /// Nullable as some reports may be generated outside of specific appointments (e.g., lab results).
    /// </summary>
    public required int AppointmentID { get; set; }

    /// <summary>
    /// Reference to the doctor who generated, reviewed, or is responsible for this report.
    /// Links to doctor records to identify the medical professional associated with the document.
    /// Nullable as some reports may be system-generated or from external sources.
    /// </summary>
    public int? DocterID { get; set; }

    /// <summary>
    /// Reference to the patient for whom this report was created.
    /// Links to patient records to associate medical documents with the correct patient.
    /// Nullable to handle system reports or documents that may not be patient-specific.
    /// </summary>
    public int? PatientID { get; set; }

    /// <summary>
    /// Text content or summary of the medical document.
    /// Contains the actual report content, findings, recommendations, or document summary.
    /// Nullable as some reports may only exist as file attachments without text content.
    /// </summary>
    public string? Documnt { get; set; }

    /// <summary>
    /// File system path or URL to the stored document file.
    /// Points to the physical location of PDF reports, images, or other document formats.
    /// Nullable as some reports may only contain text content without separate file attachments.
    /// </summary>
    public string? FilePath { get; set; }

    /// <summary>
    /// Descriptive summary or title of the report for easy identification.
    /// Provides human-readable description of the report type, purpose, or key findings.
    /// Nullable but helpful for report organization and quick identification in lists.
    /// </summary>
    public string? Description { get; set; }

    // Navigation properties - Note: This table doesn't have audit fields in the schema
    // Limited navigation properties due to GUID foreign keys that may not match entity primary keys
}
