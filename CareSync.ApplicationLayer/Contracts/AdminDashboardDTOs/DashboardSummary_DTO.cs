namespace CareSync.ApplicationLayer.Contracts.AdminDashboardDTOs;

/// <summary>
/// Complete dashboard summary including all cards, charts, and statistics
/// </summary>
public class DashboardSummary_DTO
{
    public StatsCard_DTO AppointmentsCard { get; set; } = new();
    public StatsCard_DTO DoctorsCard { get; set; } = new();
    public StatsCard_DTO PatientsCard { get; set; } = new();
    public StatsCard_DTO LabsCard { get; set; } = new();
    public List<UrgentItem_DTO> UrgentItems { get; set; } = new();
    public TodayPerformance_DTO TodayPerformance { get; set; } = new();
    public List<TodayAppointment_DTO> TodaysAppointments { get; set; } = new();
}

/// <summary>
/// Statistical card data with percentage change
/// </summary>
public class StatsCard_DTO
{
    public int TotalCount { get; set; }
    public int ThisMonthCount { get; set; }
    public int LastMonthCount { get; set; }
    public decimal PercentageChange { get; set; }
    public bool IsIncrease => PercentageChange >= 0;
}

/// <summary>
/// Urgent items requiring admin attention
/// </summary>
public class UrgentItem_DTO
{
    public string Type { get; set; } = string.Empty; // "Appointment", "LabResult", "PatientAdmission"
    public string Message { get; set; } = string.Empty;
    public string Priority { get; set; } = "High"; // High, Medium, Low
    public DateTime CreatedDate { get; set; }
    public string Url { get; set; } = string.Empty;
}

/// <summary>
/// Today's performance metrics
/// </summary>
public class TodayPerformance_DTO
{
    public int AppointmentsCompleted { get; set; }
    public int AppointmentsPending { get; set; }
    public int LabReportsReady { get; set; }
    public int LabReportsPending { get; set; }
    public int NewPatientRegistrations { get; set; }
}

/// <summary>
/// Today's appointment details
/// </summary>
public class TodayAppointment_DTO
{
    public int AppointmentID { get; set; }
    public string DoctorName { get; set; } = string.Empty;
    public string PatientName { get; set; } = string.Empty;
    public DateTime AppointmentTime { get; set; }
    public string AppointmentType { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string Specialization { get; set; } = string.Empty;
}
