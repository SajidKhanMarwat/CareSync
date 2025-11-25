using CareSync.Shared.Enums.Appointment;

namespace CareSync.ApplicationLayer.Contracts.AppointmentsDTOs;

public class Appointment_DTO
{
    public int AppointmentID { get; set; }
    public int DoctorID { get; set; }
    public int PatientID { get; set; }
    public DateTime AppointmentDate { get; set; }
    public AppointmentType_Enum AppointmentType { get; set; }
    public AppointmentStatus_Enum Status { get; set; }
    public string Reason { get; set; } = string.Empty;
    public string? Notes { get; set; }
    public bool IsDeleted { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
    public DateTime CreatedOn { get; set; }
    public string? UpdatedBy { get; set; }
    public DateTime? UpdatedOn { get; set; }
    
    // Additional properties for display
    public string? DoctorName { get; set; }
    public string? PatientName { get; set; }
}
