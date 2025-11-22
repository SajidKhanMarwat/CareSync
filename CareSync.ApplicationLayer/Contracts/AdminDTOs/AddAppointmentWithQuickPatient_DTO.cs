using CareSync.Shared.Enums;
using CareSync.Shared.Enums.Appointment;

namespace CareSync.ApplicationLayer.Contracts.AdminDTOs;

/// <summary>
/// Used for creating Appointment with Patient together by admin
/// </summary>
public class AddAppointmentWithQuickPatient_DTO
{
    // Patient/User Information
    public required string FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Username { get; set; }
    public required string Email { get; set; }
    public string? PhoneNumber { get; set; }
    public Gender_Enum Gender { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public string? Password { get; set; }
    public string? Address { get; set; }
    
    // Patient Details
    public string? BloodGroup { get; set; }
    public string? MaritalStatus { get; set; }
    
    // Emergency Contact
    public string? EmergencyContactName { get; set; }
    public string? EmergencyContactPhone { get; set; }
    
    // Appointment Information
    public required int DoctorID { get; set; }
    public required DateTime AppointmentDate { get; set; }
    public required AppointmentType_Enum AppointmentType { get; set; }
    public string? Reason { get; set; }
    public string? Notes { get; set; }
}
