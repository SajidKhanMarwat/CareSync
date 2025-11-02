namespace CareSync.DataLayer.Entities;

public class T_DoctorDetails : BaseEntity
{
    public int DoctorID { get; set; }
    public Guid UserID { get; set; }
    public string? Specialization { get; set; }
    public int? ExperienceYears { get; set; }
    public string? LicenseNumber { get; set; }
    public string? QualificationSummary { get; set; }
    public string? HospitalAffiliation { get; set; }
    public decimal? ConsultationFee { get; set; }
    public string? AvailableDays { get; set; }
    public TimeSpan? StartTime { get; set; }
    public TimeSpan? EndTime { get; set; }

    // Navigation properties
    public virtual T_Users User { get; set; }
    public virtual ICollection<T_Appointments> Appointments { get; set; } = new List<T_Appointments>();
    public virtual ICollection<T_Prescriptions> Prescriptions { get; set; } = new List<T_Prescriptions>();
    public virtual ICollection<T_Qualifications> Qualifications { get; set; } = new List<T_Qualifications>();
}
