using CareSync.Shared.Enums.Patient;

namespace CareSync.ApplicationLayer.Contracts.PatientsDTOs;

/// <summary>
/// DTO for patient list display with essential information
/// </summary>
public class PatientList_DTO
{
    public int PatientID { get; set; }
    public string UserID { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string FullName => $"{FirstName} {LastName}";
    public string Email { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string Gender { get; set; } = string.Empty;
    public DateTime? DateOfBirth { get; set; }
    public int? Age { get; set; }
    public string? BloodGroup { get; set; }
    public MaritalStatusEnum MaritalStatus { get; set; }
    public string? Occupation { get; set; }
    public bool IsActive { get; set; }
    public string ProfileImage { get; set; } = string.Empty;
    public DateTime CreatedOn { get; set; }
    
    // Additional patient information
    public string? Address { get; set; }
    public string? EmergencyContactName { get; set; }
    public string? EmergencyContactNumber { get; set; }
    public string? RelationshipToEmergency { get; set; }
    
    // Statistics
    public int TotalAppointments { get; set; }
    public DateTime? LastVisit { get; set; }
}

/// <summary>
/// DTO for patient statistics summary
/// </summary>
public class PatientStats_DTO
{
    public int TotalPatients { get; set; }
    public int ActivePatients { get; set; }
    public int InactivePatients { get; set; }
    public int NewPatientsThisMonth { get; set; }
    public int AppointmentsToday { get; set; }
    public Dictionary<string, int> PatientsByBloodGroup { get; set; } = new();
    public Dictionary<string, int> PatientsByGender { get; set; } = new();
    public decimal AverageAge { get; set; }
}

/// <summary>
/// DTO for patient search results
/// </summary>
public class PatientSearch_DTO
{
    public int PatientID { get; set; }
    public string UserID { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public int? Age { get; set; }
    public string? BloodGroup { get; set; }
    public DateTime? LastVisit { get; set; }
}
