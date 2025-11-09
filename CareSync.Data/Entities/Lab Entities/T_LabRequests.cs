namespace CareSync.DataLayer.Entities;

public class T_LabRequests : BaseEntity
{
    public int RequestID { get; set; }
    public int AppointmentID { get; set; }
    public int LabServiceID { get; set; }
    public int? RequestedByDoctorID { get; set; }
    public int? RequestedByPatientID { get; set; }
    public string? Status { get; set; }
    public string? Remarks { get; set; }

    //// Navigation properties
    //public virtual T_Appointments Appointment { get; set; }
    //public virtual T_LabServices LabService { get; set; }
    //public virtual ICollection<T_LabReports> LabReports { get; set; } = new List<T_LabReports>();
}
