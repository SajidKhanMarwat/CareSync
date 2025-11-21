namespace CareSync.ApplicationLayer.Contracts.LabDTOs;

public class RegisterLabAssistant_DTO
{
    public string? LabName { get; set; }
    public string? ArabicLabName { get; set; }
    public string? LabAddress { get; set; }
    public string? ArabicLabAddress { get; set; }
    public string? Location { get; set; }

    public string? ContactNumber { get; set; }
    public string? Email { get; set; }
    public string? LicenseNumber { get; set; }

    public TimeSpan? OpeningTime { get; set; }
    public TimeSpan? ClosingTime { get; set; }
    //public List<LabService_DTO>? LabServices { get; set; }
}
