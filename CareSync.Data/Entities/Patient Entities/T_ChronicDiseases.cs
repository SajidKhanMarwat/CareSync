namespace CareSync.DataLayer.Entities;

public class T_ChronicDiseases : BaseEntity
{
    public int DiseaseID { get; set; }
    public int PatientID { get; set; }
    public string? DiseaseName { get; set; }
    public DateTime? DiagnosedDate { get; set; }
    public string? CurrentStatus { get; set; }

    // Navigation properties
    public virtual T_PatientDetails Patient { get; set; }
}
