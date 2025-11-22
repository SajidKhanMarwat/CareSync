using CareSync.Shared.Enums.Patient;

namespace CareSync.ApplicationLayer.Contracts.PatientsDTOs;

/// <summary>
/// DTO for patient registration containing medical and emergency contact information
/// </summary>
public record RegisterPatient_DTO
{
    /// <summary>
    /// Reference to the associated user account
    /// </summary>
    public string? UserID { get; set; }

    /// <summary>
    /// Blood group (e.g., A+, B-, O+, AB-)
    /// </summary>
    public string? BloodGroup { get; set; }

    /// <summary>
    /// Marital status of the patient
    /// </summary>
    public MaritalStatusEnum MaritalStatus { get; set; } = MaritalStatusEnum.Single;

    /// <summary>
    /// Patient's occupation or profession
    /// </summary>
    public string? Occupation { get; set; }

    /// <summary>
    /// Emergency contact person's full name
    /// </summary>
    public string? EmergencyContactName { get; set; }

    /// <summary>
    /// Emergency contact phone number
    /// </summary>
    public string? EmergencyContactNumber { get; set; }

    /// <summary>
    /// Relationship to emergency contact (e.g., Spouse, Parent, Sibling)
    /// </summary>
    public string? RelationshipToEmergency { get; set; }

    /// <summary>
    /// User ID of the person creating this record
    /// </summary>
    public required string CreatedBy { get; set; }
}
