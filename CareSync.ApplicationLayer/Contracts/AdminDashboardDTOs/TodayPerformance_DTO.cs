namespace CareSync.ApplicationLayer.Contracts.AdminDashboardDTOs;

/// <summary>
/// DTO for today's performance metrics on admin dashboard
/// </summary>
public class TodayPerformanceMetrics_DTO
{
    // Patient Check-ins
    public int TotalScheduledToday { get; set; }
    public int CheckedInToday { get; set; }
    public decimal CheckInPercentage { get; set; }

    // Appointments Completed
    public int CompletedAppointments { get; set; }
    public int TotalAppointmentsToday { get; set; }
    public decimal CompletionPercentage { get; set; }

    // Lab Reports
    public int LabReportsReady { get; set; }
    public int TotalLabReportsRequested { get; set; }
    public decimal LabReportsPercentage { get; set; }

    // Revenue
    public decimal RevenueToday { get; set; }
    public decimal AverageDailyRevenue { get; set; }
    public decimal RevenuePercentageChange { get; set; }
    public bool IsRevenueAboveAverage { get; set; }

    // Additional Metrics
    public int ActiveDoctorsToday { get; set; }
    public int TotalDoctors { get; set; }
    public int NewPatientsToday { get; set; }
    public int PendingAppointments { get; set; }
    public int CancelledAppointmentsToday { get; set; }
}
