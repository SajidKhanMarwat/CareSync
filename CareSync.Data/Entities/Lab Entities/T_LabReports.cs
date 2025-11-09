namespace CareSync.DataLayer.Entities;

public class T_LabReports : BaseEntity
{
    public int ReportID { get; set; }
    public int LabRequestID { get; set; }
    public string? PatientID { get; set; } // Note: This is nchar(10) in DB, seems inconsistent
    public string? ReportName { get; set; }
    public string? ExpectedTime { get; set; } // Note: This is nchar(10) in DB
    public string? ResultSummary { get; set; }
    public string? FilePath { get; set; }
    public int? ReviewedByDoctorID { get; set; }
    public DateTime? ReviewedDate { get; set; } = DateTime.UtcNow;

    //// Navigation properties
    //public virtual T_LabRequests LabRequest { get; set; }
}
