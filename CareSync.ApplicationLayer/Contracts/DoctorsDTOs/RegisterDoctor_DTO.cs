namespace CareSync.ApplicationLayer.Contracts.DoctorsDTOs;

public class RegisterDoctor_DTO
{
    public string? Specialization { get; set; }
    public string? ArabicSpecialization { get; set; }
    public string? ClinicAddress { get; set; }
    public string? ArabicClinicAddress { get; set; }
    public int? ExperienceYears { get; set; }
    public string? LicenseNumber { get; set; }
    public string? QualificationSummary { get; set; }
    public string? HospitalAffiliation { get; set; }
    public required string AvailableDays { get; set; }
    public string? StartTime { get; set; }
    public string? EndTime { get; set; }
    public List<Qualification_DTO>? Qualifications { get; set; }
}
