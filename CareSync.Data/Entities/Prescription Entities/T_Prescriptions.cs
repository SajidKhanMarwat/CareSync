namespace CareSync.DataLayer.Entities;

public class T_Prescriptions : BaseEntity
{
    public int PrescriptionID { get; set; }
    public int AppointmentID { get; set; }
    public int DoctorID { get; set; }
    public int PatientID { get; set; }
    public string? Notes { get; set; }

    // Navigation properties
    public virtual T_Appointments Appointment { get; set; }
    public virtual T_DoctorDetails Doctor { get; set; }
    public virtual T_PatientDetails Patient { get; set; }
    public virtual ICollection<T_PrescriptionItems> PrescriptionItems { get; set; } = new List<T_PrescriptionItems>();
}
