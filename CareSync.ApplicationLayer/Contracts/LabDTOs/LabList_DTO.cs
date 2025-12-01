namespace CareSync.ApplicationLayer.Contracts.LabDTOs;

/// <summary>
/// DTO for listing laboratories
/// </summary>
public class LabListDTO
{
    public int LabId { get; set; }
    public string? LabName { get; set; }
    public string? ArabicLabName { get; set; }
    public string? Location { get; set; }
    public string? ContactNumber { get; set; }
    public string? Email { get; set; }
    public bool IsActive { get; set; }
}
