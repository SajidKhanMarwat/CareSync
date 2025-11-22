using CareSync.Shared.Enums;

namespace CareSync.ApplicationLayer.Contracts.AdminDTOs;

/// <summary>
/// DTO for creating a patient account without appointment
/// </summary>
public class CreatePatient_DTO
{
    // User Information
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
}
