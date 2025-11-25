namespace CareSync.ApplicationLayer.Contracts.PatientsDTOs;

/// <summary>
/// Complete patient dashboard data including profile, statistics, visits, and reports
/// </summary>
public class PatientDashboard_DTO
{
    /// <summary>
    /// Patient profile information
    /// </summary>
    public PatientDashboardProfile_DTO Profile { get; set; } = new();

    /// <summary>
    /// Dashboard statistics and counts
    /// </summary>
    public DashboardStats_DTO Statistics { get; set; } = new();

    /// <summary>
    /// Recent doctor visits
    /// </summary>
    public List<RecentDoctorVisit_DTO> RecentVisits { get; set; } = new();

    /// <summary>
    /// Recent medical reports
    /// </summary>
    public List<RecentMedicalReport_DTO> RecentReports { get; set; } = new();

    /// <summary>
    /// Latest health vitals
    /// </summary>
    public HealthVitals_DTO? LatestVitals { get; set; }

    /// <summary>
    /// Health vitals history for tracking cards (last 5 readings)
    /// </summary>
    public HealthVitalsHistory_DTO VitalsHistory { get; set; } = new();
}

/// <summary>
/// Patient profile information for dashboard header
/// </summary>
public class PatientDashboardProfile_DTO
{
    public string PatientName { get; set; } = string.Empty;
    public string Gender { get; set; } = string.Empty;
    public int Age { get; set; }
    public string BloodType { get; set; } = string.Empty;
    public string? ProfileImage { get; set; }
    public string PrimaryDoctor { get; set; } = "Not Assigned";
    public string? LastVisitDate { get; set; }
    public string? NextAppointmentDate { get; set; }
}

/// <summary>
/// Dashboard statistics and counts
/// </summary>
public class DashboardStats_DTO
{
    public int UpcomingAppointments { get; set; }
    public int ActivePrescriptions { get; set; }
    public int PendingLabTests { get; set; }
    public int NewReports { get; set; }
}

/// <summary>
/// Recent doctor visit information
/// </summary>
public class RecentDoctorVisit_DTO
{
    public int AppointmentId { get; set; }
    public string DoctorName { get; set; } = string.Empty;
    public string DoctorImage { get; set; } = "~/theme/images/user.png";
    public string Specialization { get; set; } = string.Empty;
    public string VisitDate { get; set; } = string.Empty;
    public string Department { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
}

/// <summary>
/// Recent medical report information
/// </summary>
public class RecentMedicalReport_DTO
{
    public int ReportId { get; set; }
    public string ReportTitle { get; set; } = string.Empty;
    public string ReportType { get; set; } = string.Empty;
    public string ReportDate { get; set; } = string.Empty;
    public string? FileUrl { get; set; }
}

/// <summary>
/// Latest health vitals information
/// </summary>
public class HealthVitals_DTO
{
    public decimal? BloodPressureSystolic { get; set; }
    public decimal? BloodPressureDiastolic { get; set; }
    public decimal? BloodSugar { get; set; }
    public int? HeartRate { get; set; }
    public decimal? Cholesterol { get; set; }
    public decimal? Weight { get; set; }
    public decimal? Height { get; set; }
    public decimal? Temperature { get; set; }
    public string RecordedDate { get; set; } = string.Empty;
}
