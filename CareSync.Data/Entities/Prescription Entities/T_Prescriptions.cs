namespace CareSync.DataLayer.Entities;

/// <summary>
/// Represents medical prescriptions issued during patient consultations in the CareSync system.
/// This entity serves as the header record for prescription documents, linking medications
/// to specific appointments, doctors, and patients. Each prescription can contain multiple
/// medication items and serves as a legal medical document for pharmacy dispensing.
/// </summary>
public class T_Prescriptions : BaseEntity
{
    /// <summary>
    /// Unique identifier for the prescription record.
    /// Primary key that serves as the main reference for this specific prescription document.
    /// Auto-incremented integer value assigned when a new prescription is created during consultation.
    /// </summary>
    public int PrescriptionID { get; set; }

    /// <summary>
    /// Reference to the appointment during which this prescription was issued.
    /// Links to the AppointmentID in T_Appointments table to associate prescriptions with medical consultations.
    /// Required field as prescriptions are typically issued during specific medical encounters.
    /// </summary>
    public int AppointmentID { get; set; }

    /// <summary>
    /// Reference to the doctor who issued this prescription.
    /// Links to the DoctorID in T_DoctorDetails table to identify the prescribing physician.
    /// Required field for legal and regulatory compliance, ensuring prescription authority.
    /// </summary>
    public int DoctorID { get; set; }

    /// <summary>
    /// Reference to the patient for whom this prescription was written.
    /// Links to the PatientID in T_PatientDetails table to associate medications with the correct patient.
    /// Required field for patient safety, medication tracking, and pharmacy dispensing.
    /// </summary>
    public int PatientID { get; set; }

    /// <summary>
    /// Additional notes or instructions related to the entire prescription.
    /// May include general medication guidance, follow-up instructions, or special considerations.
    /// Nullable as not all prescriptions require additional notes beyond individual medication instructions.
    /// </summary>
    public string? Notes { get; set; }

    // Navigation properties
    /// <summary>
    /// Navigation property to the appointment during which this prescription was issued.
    /// Provides access to consultation details, appointment date, and medical encounter context.
    /// </summary>
    public virtual T_Appointments? Appointment { get; set; }

    /// <summary>
    /// Navigation property to the doctor who issued this prescription.
    /// Provides access to prescribing physician details, qualifications, and contact information.
    /// </summary>
    public virtual T_DoctorDetails? Doctor { get; set; }

    /// <summary>
    /// Navigation property to the patient for whom this prescription was written.
    /// Provides access to patient demographics, medical history, and allergy information.
    /// </summary>
    public virtual T_PatientDetails? Patient { get; set; }

    /// <summary>
    /// Navigation property to all individual medication items included in this prescription.
    /// Contains the detailed list of medications, dosages, frequencies, and specific instructions.
    /// </summary>
    public virtual ICollection<T_PrescriptionItems> PrescriptionItems { get; set; } = new List<T_PrescriptionItems>();
}
