using CareSync.Shared.Enums.Appointment;

namespace CareSync.ApplicationLayer.Contracts.AdminDTOs;

/// <summary>
/// Used to add only appointment
/// </summary>
public class AddAppointment_DTO
{
    public required int DoctorID { get; set; }
    public required int PatientID { get; set; }
    public DateTime AppointmentDate { get; set; }
    public required AppointmentType_Enum AppointmentType { get; set; }
    public AppointmentStatus_Enum Status { get; set; }
    public required string Reason { get; set; }
    public string? Notes { get; set; }
}
