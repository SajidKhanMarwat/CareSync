namespace CareSync.DataLayer.Entities;

public class T_AdditionalNotes : BaseEntity
{
    public int NoteID { get; set; }
    public int PatientID { get; set; }
    public string? NutritionalPlan { get; set; }
    public string? DoctorRecommendations { get; set; }
    public string? SpecialInstructions { get; set; }

    // Navigation properties
    public virtual T_PatientDetails Patient { get; set; }
}
