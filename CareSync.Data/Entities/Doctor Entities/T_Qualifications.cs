namespace CareSync.DataLayer.Entities;

public class T_Qualifications : BaseEntity
{
    public int QualificationID { get; set; }
    public int DoctorID { get; set; }
    public string? Degree { get; set; }
    public string? Institution { get; set; }
    public int? YearOfCompletion { get; set; }
    public string? Certificate { get; set; }
    public string? CertificatePath { get; set; }

    // Navigation properties
    public virtual T_DoctorDetails Doctor { get; set; }
}
