namespace CareSync.DataLayer.Entities;

public class T_MedicalFollowUp : BaseEntity
{
    public int FollowUpID { get; set; }
    public int PatientID { get; set; }
    public DateTime? LastVisitDate { get; set; }
    public string? RecentExaminations { get; set; }
    public DateTime? UpcomingReviewDate { get; set; }
    public string? DailyNotes { get; set; }

    // Navigation properties
    public virtual T_PatientDetails Patient { get; set; }
}
