namespace CareSync.ApplicationLayer.Contracts.DoctorsDTOs;

/// <summary>
/// DTO for doctor list display with essential information
/// </summary>
public class DoctorList_DTO
{
    public int DoctorID { get; set; }
    public string UserID { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string FullName => $"{FirstName} {LastName}";
    public string Email { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string Specialization { get; set; } = string.Empty;
    public string? ArabicSpecialization { get; set; }
    public string LicenseNumber { get; set; } = string.Empty;
    public int? ExperienceYears { get; set; }
    public string? HospitalAffiliation { get; set; }
    public string AvailableDays { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public string ProfileImage { get; set; } = string.Empty;
    public DateTime CreatedOn { get; set; }
    
    // Statistics
    public int TotalAppointments { get; set; }
    public int TodaysAppointments { get; set; }
}

/// <summary>
/// DTO for doctor statistics summary
/// </summary>
public class DoctorStats_DTO
{
    public int TotalDoctors { get; set; }
    public int ActiveDoctors { get; set; }
    public int InactiveDoctors { get; set; }
    public int AppointmentsToday { get; set; }
    public int NewDoctorsThisMonth { get; set; }
    public decimal AverageExperience { get; set; }
    public Dictionary<string, int> DoctorsBySpecialization { get; set; } = new();
}
