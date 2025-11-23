namespace CareSync.ApplicationLayer.Contracts.AdminDashboardDTOs;

/// <summary>
/// User Distribution by role
/// </summary>
public class UserDistributionStats_DTO
{
    public int TotalPatients { get; set; }
    public decimal PatientsPercentageChange { get; set; }
    public int TotalDoctors { get; set; }
    public decimal DoctorsPercentageChange { get; set; }
    public int TotalAdminStaff { get; set; }
    public decimal AdminPercentageChange { get; set; }
    public int TotalLabStaff { get; set; }
    public decimal LabPercentageChange { get; set; }
    public int TotalUsers { get; set; }
}

/// <summary>
/// Monthly statistics summary
/// </summary>
public class MonthlyStatistics_DTO
{
    public int NewRegistrationsThisMonth { get; set; }
    public decimal RegistrationPercentageChange { get; set; }
    public int TotalAppointmentsThisMonth { get; set; }
    public decimal AppointmentPercentageChange { get; set; }
    public int LabTestsCompletedThisMonth { get; set; }
    public decimal LabTestsPercentageChange { get; set; }
    public decimal RevenueThisMonth { get; set; }
    public decimal RevenuePercentageChange { get; set; }
}

/// <summary>
/// Patient registration trends over last 12 months
/// </summary>
public class PatientRegistrationTrends_DTO
{
    public List<MonthlyRegistration> MonthlyData { get; set; } = new();
    public int TotalNewPatients { get; set; }
    public decimal AveragePerMonth { get; set; }
    public string TrendDirection { get; set; } = "Stable"; // Up, Down, Stable
}

public class MonthlyRegistration
{
    public string MonthName { get; set; } = string.Empty;
    public int Year { get; set; }
    public int Count { get; set; }
    public string FormattedMonth { get; set; } = string.Empty; // "Jan 2024"
}

/// <summary>
/// Appointment status breakdown
/// </summary>
public class AppointmentStatusBreakdown_DTO
{
    public int ScheduledCount { get; set; }
    public int ApprovedCount { get; set; }
    public int CompletedCount { get; set; }
    public int CancelledCount { get; set; }
    public int PendingCount { get; set; }
    public int TotalAppointments { get; set; }
    
    public decimal ScheduledPercentage { get; set; }
    public decimal ApprovedPercentage { get; set; }
    public decimal CompletedPercentage { get; set; }
    public decimal CancelledPercentage { get; set; }
    public decimal PendingPercentage { get; set; }
}

/// <summary>
/// Today's appointments list
/// </summary>
public class TodaysAppointmentsList_DTO
{
    public List<TodayAppointmentItem> Appointments { get; set; } = new();
    public int TotalToday { get; set; }
    public int CompletedToday { get; set; }
    public int PendingToday { get; set; }
}

public class TodayAppointmentItem
{
    public int AppointmentID { get; set; }
    public int PatientID { get; set; }
    public string PatientName { get; set; } = string.Empty;
    public int DoctorID { get; set; }
    public string DoctorName { get; set; } = string.Empty;
    public string? DoctorSpecialization { get; set; }
    public DateTime AppointmentDate { get; set; }
    public DateTime AppointmentTime { get; set; }
    public string Status { get; set; } = string.Empty;
    public string AppointmentType { get; set; } = string.Empty;
    public string? Reason { get; set; }
}

/// <summary>
/// Recent lab results
/// </summary>
public class RecentLabResults_DTO
{
    public List<LabResultItem> Results { get; set; } = new();
    public int TotalPendingResults { get; set; }
    public int TotalCompletedToday { get; set; }
}

public class LabResultItem
{
    public int LabServiceID { get; set; }
    public string PatientName { get; set; } = string.Empty;
    public string TestName { get; set; } = string.Empty;
    public DateTime RequestDate { get; set; }
    public DateTime? CompletionDate { get; set; }
    public string Status { get; set; } = string.Empty;
    public bool IsUrgent { get; set; }
    public string? Notes { get; set; }
}
