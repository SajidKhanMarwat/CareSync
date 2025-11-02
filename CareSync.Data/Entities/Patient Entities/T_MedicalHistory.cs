namespace CareSync.DataLayer.Entities;

public class T_MedicalHistory : BaseEntity
{
    public int HistoryID { get; set; }
    public int PatientID { get; set; }
    public string? MainDiagnosis { get; set; }
    public string? ChronicDiseases { get; set; }
    public string? Allergies { get; set; }
    public string? PastDiseases { get; set; }
    public string? Surgery { get; set; }
    public string? FamilyHistory { get; set; }

    // Navigation properties
    public virtual T_PatientDetails Patient { get; set; }
}
