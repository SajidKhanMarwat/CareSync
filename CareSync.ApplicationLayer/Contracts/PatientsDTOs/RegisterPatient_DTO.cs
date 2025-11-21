namespace CareSync.ApplicationLayer.Contracts.PatientsDTOs;

public record RegisterPatient_DTO
{
    public string? BloodGroup { get; set; }
    public string? MaritalStatus { get; set; }
    public string? Occupation { get; set; }
    public string? EmergencyContactName { get; set; }
    public string? EmergencyContactNumber { get; set; }
    public string? RelationshipToEmergency { get; set; }
    public string? CreatedBy { get; set; }
}
