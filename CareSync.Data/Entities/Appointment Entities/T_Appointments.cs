namespace CareSync.DataLayer.Entities;

public class T_Appointments : BaseEntity
{
    public int AppointmentID { get; set; }
    public int DoctorID { get; set; }
    public int PatientID { get; set; }
    public DateTime AppointmentDate { get; set; }
    public string? AppointmentType { get; set; }
    public string? Status { get; set; }
    public string? Reason { get; set; }
    public string? PaymentStatus { get; set; }
    public string? Notes { get; set; }

    // Navigation properties
    public virtual T_DoctorDetails Doctor { get; set; }
    public virtual T_PatientDetails Patient { get; set; }
    public virtual ICollection<T_LabRequests> LabRequests { get; set; } = new List<T_LabRequests>();
    public virtual ICollection<T_Prescriptions> Prescriptions { get; set; } = new List<T_Prescriptions>();
}
