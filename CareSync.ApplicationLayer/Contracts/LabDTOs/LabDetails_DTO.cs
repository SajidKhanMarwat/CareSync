namespace CareSync.ApplicationLayer.Contracts.LabDTOs;

/// <summary>
/// DTO for detailed laboratory information
/// </summary>
public class LabDetails_DTO
{
    public int LabId { get; set; }
    public Guid? UserId { get; set; }
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
    public DateTime? CreatedOn { get; set; }
    public string? CreatedBy { get; set; }
    public int ServicesCount { get; set; }
    public int AssistantsCount { get; set; }
    public bool IsActive { get; set; }
}
