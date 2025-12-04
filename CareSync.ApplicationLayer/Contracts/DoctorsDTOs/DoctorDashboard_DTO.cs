namespace CareSync.ApplicationLayer.Contracts.DoctorsDTOs;

public class DoctorDashboard_DTO
{
    public string DoctorName { get; set; } = string.Empty;
    public string Specialization { get; set; } = string.Empty;
    public int TotalRatings { get; set; }

    // Statistics
    public int TotalPatients { get; set; }
    public int TotalSurgeries { get; set; }
    public decimal MonthlyEarnings { get; set; }

    // Detailed insights
    public int TodayAppointmentsCount { get; set; }
    public int TotalPrescriptions { get; set; }
    public int PendingAppointments { get; set; }
    public int LabReports { get; set; }

    // Patient statistics
    public int NewPatients { get; set; }
    public int RegularPatients { get; set; }
    public int FollowUpPatients { get; set; }

    // Collections
    public List<TodayAppointment_DTO> TodayAppointments { get; set; } = new();
    public List<RecentPatient_DTO> RecentPatients { get; set; } = new();

    // Appointments overview timeseries (last 12 months) and labels
    public List<int> AppointmentsOverviewData { get; set; } = new();
    public List<string> AppointmentsOverviewLabels { get; set; } = new();
}

public class TodayAppointment_DTO
{
    public int Id { get; set; }
    public int DoctorID { get; set; }
    public int PatientID { get; set; }
    public string PatientName { get; set; } = string.Empty;
    public int PatientAge { get; set; }
    public DateTime AppointmentTime { get; set; }
    public string Diagnosis { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
}

public class RecentPatient_DTO
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateTime LastAppointment { get; set; }
    public string Notes { get; set; } = string.Empty;
}