namespace CareSync.DataLayer.Entities;

public class T_MedicationPlan : BaseEntity
{
    public int MedicationID { get; set; }
    public int PatientID { get; set; }
    public string? MedicationName { get; set; }
    public string? Dosage { get; set; }
    public string? Frequency { get; set; }
    public string? Duration { get; set; }
    public string? Instructions { get; set; }
    public bool? Status { get; set; }

    // Navigation properties
    public virtual T_PatientDetails Patient { get; set; }
}
