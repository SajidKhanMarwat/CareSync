namespace CareSync.DataLayer.Entities;

/// <summary>
/// Represents detailed patient information in the CareSync medical management system.
/// This entity stores patient-specific medical and personal information that extends
/// beyond the basic user profile, including emergency contacts, blood type, and
/// marital status which are crucial for medical care and emergency situations.
/// </summary>
public class T_PatientDetails : BaseEntity
{
    /// <summary>
    /// Unique identifier for the patient record.
    /// Primary key that serves as the main reference for all patient-related medical records.
    /// Auto-incremented integer value assigned when a new patient is registered.
    /// </summary>
    public int PatientID { get; set; }

    /// <summary>
    /// Reference to the associated user account in the T_Users table.
    /// Links patient medical records to the user's login credentials and basic profile.
    /// Nullable to support cases where patient records exist without user accounts (e.g., minors).
    /// </summary>
    public string? UserID { get; set; }

    /// <summary>
    /// The patient's blood group/type (e.g., A+, B-, O+, AB-).
    /// Critical information for blood transfusions, surgeries, and emergency medical care.
    /// Nullable as blood type may not be immediately known for new patients.
    /// </summary>
    public string? BloodGroup { get; set; }

    /// <summary>
    /// The patient's current marital status (e.g., Single, Married, Divorced, Widowed).
    /// Important for medical history, insurance coverage, and emergency contact relationships.
    /// Nullable to respect privacy preferences and handle incomplete patient profiles.
    /// </summary>
    public string? MaritalStatus { get; set; }

    /// <summary>
    /// The patient's current occupation or profession.
    /// Relevant for occupational health risks, insurance coverage, and lifestyle-related medical conditions.
    /// Nullable as occupation information may not be required for all patients.
    /// </summary>
    public string? Occupation { get; set; }

    /// <summary>
    /// Full name of the primary emergency contact person.
    /// Critical for emergency situations when the patient cannot make medical decisions.
    /// Nullable but highly recommended for all patients, especially those with serious conditions.
    /// </summary>
    public string? EmergencyContactName { get; set; }

    /// <summary>
    /// Phone number of the emergency contact person.
    /// Must be a reliable number that can be reached 24/7 for medical emergencies.
    /// Nullable but essential for emergency medical care coordination.
    /// </summary>
    public string? EmergencyContactNumber { get; set; }

    /// <summary>
    /// Relationship between the patient and the emergency contact (e.g., Spouse, Parent, Sibling, Friend).
    /// Helps medical staff understand the contact's authority to make decisions on behalf of the patient.
    /// Nullable but important for legal and ethical medical decision-making processes.
    /// </summary>
    public string? RelationshipToEmergency { get; set; }

    // Navigation properties
    /// <summary>
    /// Navigation property to the associated user account.
    /// Provides access to login credentials, basic profile information, and system access details.
    /// </summary>
    public virtual T_Users? User { get; set; }

    /// <summary>
    /// Navigation property to all appointments scheduled for this patient.
    /// Includes past, current, and future appointments with various healthcare providers.
    /// </summary>
    public virtual ICollection<T_Appointments> Appointments { get; set; } = new List<T_Appointments>();

    /// <summary>
    /// Navigation property to additional medical notes and recommendations for this patient.
    /// Contains nutritional plans, doctor recommendations, and special care instructions.
    /// </summary>
    public virtual ICollection<T_AdditionalNotes> AdditionalNotes { get; set; } = new List<T_AdditionalNotes>();

    /// <summary>
    /// Navigation property to all chronic diseases diagnosed for this patient.
    /// Includes ongoing medical conditions that require long-term management and monitoring.
    /// </summary>
    public virtual ICollection<T_ChronicDiseases> ChronicDiseases { get; set; } = new List<T_ChronicDiseases>();

    /// <summary>
    /// Navigation property to lifestyle information records for this patient.
    /// Contains data about exercise habits, diet, smoking status, and daily activities.
    /// </summary>
    public virtual ICollection<T_LifestyleInfo> LifestyleInfo { get; set; } = new List<T_LifestyleInfo>();

    /// <summary>
    /// Navigation property to medical follow-up records for this patient.
    /// Tracks ongoing care, review dates, and progress monitoring for treatments.
    /// </summary>
    public virtual ICollection<T_MedicalFollowUp> MedicalFollowUps { get; set; } = new List<T_MedicalFollowUp>();

    /// <summary>
    /// Navigation property to comprehensive medical history records for this patient.
    /// Includes past diagnoses, surgeries, allergies, and family medical history.
    /// </summary>
    public virtual ICollection<T_MedicalHistory> MedicalHistories { get; set; } = new List<T_MedicalHistory>();

    /// <summary>
    /// Navigation property to medication plans prescribed for this patient.
    /// Contains current and past medication regimens, dosages, and treatment plans.
    /// </summary>
    public virtual ICollection<T_MedicationPlan> MedicationPlans { get; set; } = new List<T_MedicationPlan>();

    /// <summary>
    /// Navigation property to vital signs records for this patient.
    /// Includes measurements like blood pressure, weight, height, and other health metrics.
    /// </summary>
    public virtual ICollection<T_PatientVitals> PatientVitals { get; set; } = new List<T_PatientVitals>();

    /// <summary>
    /// Navigation property to all prescriptions issued for this patient.
    /// Contains medication prescriptions from various doctors and medical consultations.
    /// </summary>
    public virtual ICollection<T_Prescriptions> Prescriptions { get; set; } = new List<T_Prescriptions>();
}
