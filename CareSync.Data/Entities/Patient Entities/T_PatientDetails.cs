namespace CareSync.DataLayer.Entities;

public class T_PatientDetails : BaseEntity
{
    public int PatientID { get; set; }
    public Guid? UserID { get; set; }
    public string? BloodGroup { get; set; }
    public string? MaritalStatus { get; set; }
    public string? Occupation { get; set; }
    public string? EmergencyContactName { get; set; }
    public string? EmergencyContactNumber { get; set; }
    public string? RelationshipToEmergency { get; set; }

    // Navigation properties
    public virtual T_Users? User { get; set; }
    public virtual ICollection<T_Appointments> Appointments { get; set; } = new List<T_Appointments>();
    public virtual ICollection<T_AdditionalNotes> AdditionalNotes { get; set; } = new List<T_AdditionalNotes>();
    public virtual ICollection<T_ChronicDiseases> ChronicDiseases { get; set; } = new List<T_ChronicDiseases>();
    public virtual ICollection<T_LifestyleInfo> LifestyleInfo { get; set; } = new List<T_LifestyleInfo>();
    public virtual ICollection<T_MedicalFollowUp> MedicalFollowUps { get; set; } = new List<T_MedicalFollowUp>();
    public virtual ICollection<T_MedicalHistory> MedicalHistories { get; set; } = new List<T_MedicalHistory>();
    public virtual ICollection<T_MedicationPlan> MedicationPlans { get; set; } = new List<T_MedicationPlan>();
    public virtual ICollection<T_PatientVitals> PatientVitals { get; set; } = new List<T_PatientVitals>();
    public virtual ICollection<T_Prescriptions> Prescriptions { get; set; } = new List<T_Prescriptions>();
}
