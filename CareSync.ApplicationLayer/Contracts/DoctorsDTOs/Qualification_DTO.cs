namespace CareSync.ApplicationLayer.Contracts.DoctorsDTOs;

public class Qualification_DTO
{
    public required string Degree { get; set; }
    public required string Institution { get; set; }
    public required int YearOfCompletion { get; set; }
    public string? Certificate { get; set; }
}
