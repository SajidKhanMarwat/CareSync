namespace CareSync.ApplicationLayer.Contracts.AdminDashboardDTOs;

/// <summary>
/// DTO for displaying doctor availability status on admin dashboard
/// </summary>
public class DoctorAvailability_DTO
{
    public int DoctorID { get; set; }
    public string DoctorName { get; set; } = string.Empty;
    public string Specialization { get; set; } = string.Empty;
    public string Status { get; set; } = "Available"; // Available, InSession, OnBreak, Off
    public string AvailableDays { get; set; } = string.Empty;
    public string? StartTime { get; set; }
    public string? EndTime { get; set; }
    public int TodaysAppointmentCount { get; set; }
    public int CompletedAppointmentsToday { get; set; }
    public string? ProfileImage { get; set; }
    public bool IsActive { get; set; }
}

/// <summary>
/// Summary of doctor availability metrics
/// </summary>
public class DoctorAvailabilitySummary_DTO
{
    public int TotalAvailable { get; set; }
    public int InSession { get; set; }
    public int OnBreak { get; set; }
    public int OffToday { get; set; }
    public List<DoctorAvailability_DTO> Doctors { get; set; } = new();
}
