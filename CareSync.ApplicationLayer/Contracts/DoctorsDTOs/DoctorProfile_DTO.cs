namespace CareSync.ApplicationLayer.Contracts.DoctorsDTOs;

public class DoctorProfile_DTO
{
    public string UserId { get; set; } = string.Empty;
    public int DoctorId { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string FullName => $"Dr. {FirstName} {LastName}";
    public string Email { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }
    public string? ProfileImage { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public int Age { get; set; }
    public string Gender { get; set; } = string.Empty;
    public string? Address { get; set; }
    
    // Professional Information
    public string? Specialization { get; set; }
    public string? LicenseNumber { get; set; }
    public int? ExperienceYears { get; set; }
    public string? HospitalAffiliation { get; set; }
    public string? Qualifications { get; set; }
    public string? About { get; set; }
    
    // Schedule Information
    public string? AvailableDays { get; set; }
    public string? StartTime { get; set; }
    public string? EndTime { get; set; }
    public decimal? ConsultationFee { get; set; }
    
    // Statistics
    public int TotalPatients { get; set; }
    public int TotalAppointments { get; set; }
    public int CompletedAppointments { get; set; }
    public int TodayAppointments { get; set; }
    public decimal Rating { get; set; }
    public int ReviewCount { get; set; }
    
    // Status
    public bool IsActive { get; set; }
    public DateTime CreatedOn { get; set; }
    public DateTime? UpdatedOn { get; set; }
}

public class UpdateDoctor_DTO
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public string? Address { get; set; }
    public string? Specialization { get; set; }
    public string? LicenseNumber { get; set; }
    public int? ExperienceYears { get; set; }
    public string? HospitalAffiliation { get; set; }
    public string? Qualifications { get; set; }
    public string? About { get; set; }
    public string? AvailableDays { get; set; }
    public string? StartTime { get; set; }
    public string? EndTime { get; set; }
    public decimal? ConsultationFee { get; set; }
}
