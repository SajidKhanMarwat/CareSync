namespace CareSync.ApplicationLayer.Contracts.PatientsDTOs;

/// <summary>
/// DTO for patient search request with multiple filter options
/// </summary>
public class PatientSearchRequest_DTO
{
    // Text search
    public string? SearchTerm { get; set; }
    
    // Demographic filters
    public string? Gender { get; set; }
    public int? MinAge { get; set; }
    public int? MaxAge { get; set; }
    public string? BloodGroup { get; set; }
    public string? MaritalStatus { get; set; }
    
    // Location filter
    public string? City { get; set; }
    
    // Medical filters
    public bool? HasChronicDisease { get; set; }
    public string? AssignedDoctor { get; set; }
    
    // Status filters
    public bool? IsActive { get; set; }
    public DateTime? LastVisitFrom { get; set; }
    public DateTime? LastVisitTo { get; set; }
    
    // Pagination
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 12;
    
    // Sorting
    public string SortBy { get; set; } = "Name"; // Name, Age, LastVisit, CreatedDate
    public bool SortDescending { get; set; } = false;
}

/// <summary>
/// DTO for patient search results with pagination
/// </summary>
public class PatientSearchResult_DTO
{
    public List<PatientSearchCard_DTO> Patients { get; set; } = new();
    public int TotalCount { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
    public bool HasPreviousPage => PageNumber > 1;
    public bool HasNextPage => PageNumber < TotalPages;
}

/// <summary>
/// DTO for patient card display in search results
/// </summary>
public class PatientSearchCard_DTO
{
    public int PatientID { get; set; }
    public string UserID { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string Gender { get; set; } = string.Empty;
    public int? Age { get; set; }
    public string? ProfileImage { get; set; }
    public string? BloodGroup { get; set; }
    public string? Address { get; set; }
    public string? City { get; set; }
    public bool IsActive { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public DateTime CreatedOn { get; set; }
    public DateTime? LastVisit { get; set; }
    public int TotalAppointments { get; set; }
    public int UpcomingAppointments { get; set; }
    public string? AssignedDoctor { get; set; }
    public string? EmergencyContact { get; set; }
    public string? ChronicConditions { get; set; }
    public string PatientCode => $"P{PatientID:D4}";
}
