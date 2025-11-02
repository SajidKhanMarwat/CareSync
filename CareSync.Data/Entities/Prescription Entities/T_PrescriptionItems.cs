namespace CareSync.DataLayer.Entities;

public class T_PrescriptionItems : BaseEntity
{
    public int PrescriptionItemID { get; set; }
    public int PrescriptionID { get; set; }
    public string? MedicineName { get; set; }
    public string? Dosage { get; set; }
    public string? Frequency { get; set; }
    public string? Duration { get; set; }
    public string? Notes { get; set; }

    // Navigation properties
    public virtual T_Prescriptions Prescription { get; set; }
}
