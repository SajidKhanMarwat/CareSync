namespace CareSync.DataLayer.Entities;

public class T_Lab : BaseEntity
{
    public int LabID { get; set; }
    public Guid? UserID { get; set; }
    public string? LabName { get; set; }
    public string? Location { get; set; }
    public string? ContactNumber { get; set; }
    public string? Email { get; set; }
    public string? LicenseNumber { get; set; }
    public TimeSpan? OpeningTime { get; set; }
    public TimeSpan? ClosingTime { get; set; }

    // Navigation properties
    public virtual T_Users? User { get; set; }
    public virtual ICollection<T_LabServices> LabServices { get; set; } = new List<T_LabServices>();
}
