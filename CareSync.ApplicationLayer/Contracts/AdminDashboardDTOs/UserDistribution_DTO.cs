namespace CareSync.ApplicationLayer.Contracts.AdminDashboardDTOs;

/// <summary>
/// User distribution across all roles with month-over-month comparison
/// </summary>
public class UserDistribution_DTO
{
    public RoleDistribution_DTO Patients { get; set; } = new();
    public RoleDistribution_DTO Doctors { get; set; } = new();
    public RoleDistribution_DTO AdminStaff { get; set; } = new();
    public RoleDistribution_DTO Labs { get; set; } = new();
    public int TotalUsers { get; set; }
}

/// <summary>
/// Distribution data for a specific role
/// </summary>
public class RoleDistribution_DTO
{
    public int TotalCount { get; set; }
    public int ActiveCount { get; set; }
    public int InactiveCount { get; set; }
    public int ThisMonthCount { get; set; }
    public int LastMonthCount { get; set; }
    public decimal PercentageChange { get; set; }
}

/// <summary>
/// Patient registration trends for charts
/// </summary>
public class RegistrationTrends_DTO
{
    public int NewRegistrationsThisMonth { get; set; }
    public int TotalAppointments { get; set; }
    public int LabTestsCompleted { get; set; }
    public List<MonthlyData_DTO> Last6Months { get; set; } = new();
}

/// <summary>
/// Monthly data point for trends
/// </summary>
public class MonthlyData_DTO
{
    public string MonthName { get; set; } = string.Empty;
    public int Year { get; set; }
    public int Patients { get; set; }
    public int Doctors { get; set; }
    public int Appointments { get; set; }
}

/// <summary>
/// Appointment status distribution for charts
/// </summary>
public class AppointmentStatusChart_DTO
{
    public int ConfirmedAppointments { get; set; }
    public int PendingAppointments { get; set; }
    public int CompletedAppointments { get; set; }
    public int CancelledAppointments { get; set; }
    public int RejectedAppointments { get; set; }
    public int TotalAppointments { get; set; }
}
