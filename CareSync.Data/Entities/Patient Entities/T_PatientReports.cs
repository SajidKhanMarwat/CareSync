namespace CareSync.DataLayer.Entities;

public class T_PatientReports
{
    public Guid PatientReprtsID { get; set; }
    public Guid? AppointmentID { get; set; }
    public Guid? DocterID { get; set; }
    public Guid? PatientID { get; set; }
    public string? Documnt { get; set; }
    public string? FilePath { get; set; }
    public string? Description { get; set; }

    // Navigation properties - Note: This table doesn't have audit fields in the schema
}
