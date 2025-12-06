using CareSync.Pages.Shared;
using CareSync.Services;
using Microsoft.AspNetCore.Mvc;

namespace CareSync.Pages.Doctor;

public class DashboardModel : BasePageModel
{
    private readonly ILogger<DashboardModel> _logger;
    private readonly DoctorApiService _doctorApiService;

    public DashboardModel(ILogger<DashboardModel> logger, DoctorApiService doctorApiService)
    {
        _logger = logger;
        _doctorApiService = doctorApiService;
    }

    // Doctor Information
    public string DoctorName { get; set; } = string.Empty;
    public string Specialization { get; set; } = string.Empty;

    // Statistics
    public int TotalPatients { get; set; }
    public int TotalSurgeries { get; set; }
    public decimal MonthlyEarnings { get; set; }

    // Detailed Insights
    public int TodayAppointmentsCount { get; set; }
    public int TotalPrescriptions { get; set; }
    public int PendingPrescriptionsCount { get; set; }
    public int PendingAppointments { get; set; }
    public int LabReports { get; set; }

    // Patient Statistics
    public int NewPatients { get; set; }
    public int RegularPatients { get; set; }
    public int FollowUpPatients { get; set; }

    // Data Collections
    public List<TodayAppointment> TodayAppointments { get; set; } = new();
    public List<RecentPatient> RecentPatients { get; set; } = new();

    // Appointments overview series for server-side render
    public List<int> AppointmentsOverviewData { get; set; } = new();
    public List<string> AppointmentsOverviewLabels { get; set; } = new();

    public async Task<IActionResult> OnGetAsync()
    {
        // Check if user is authenticated and has Doctor role
        var authResult = RequireRole("Doctor");
        if (authResult != null) return authResult;

        try
        {
            _logger.LogInformation("Loading doctor dashboard from API");
            var result = await _doctorApiService.GetDashboardAsync();

            if (result is null)
            {
                _logger.LogWarning("Doctor dashboard API returned null");
                TempData["Error"] = "Unable to load dashboard data.";
                return Page();
            }

            if (!result.IsSuccess || result.Data is null)
            {
                _logger.LogWarning("Doctor dashboard API failure: {Error}", result.GetError());
                TempData["Error"] = result.GetError() ?? "Unable to load dashboard data.";
                return Page();
            }

            DoctorName = result.Data.DoctorName;
            Specialization = result.Data.Specialization;

            TotalPatients = result.Data.TotalPatients;
            TotalSurgeries = result.Data.TotalSurgeries;
            MonthlyEarnings = result.Data.MonthlyEarnings;

            TodayAppointmentsCount = result.Data.TodayAppointmentsCount;
            TotalPrescriptions = result.Data.TotalPrescriptions;
            PendingPrescriptionsCount = result.Data.PendingPrescriptionsCount;

            PendingAppointments = result.Data.PendingAppointments;
            LabReports = result.Data.LabReports;

            NewPatients = result.Data.NewPatients;
            RegularPatients = result.Data.RegularPatients;
            FollowUpPatients = result.Data.FollowUpPatients;

            TodayAppointments = result.Data.TodayAppointments.Select(a => new TodayAppointment
            {
                Id = a.Id,
                PatientName = a.PatientName,
                PatientAge = a.PatientAge,
                AppointmentTime = a.AppointmentTime,
                Diagnosis = a.Diagnosis,
                Type = a.AppointmentType.ToString()
            }).ToList();

            RecentPatients = result.Data.RecentPatients.Select(p => new RecentPatient
            {
                Id = p.Id,
                Name = p.Name,
                LastAppointment = p.LastAppointment,
                Notes = p.Notes
            }).ToList();

            // Map timeseries for initial chart render
            AppointmentsOverviewData = result.Data.AppointmentsOverviewData ?? new List<int>();
            AppointmentsOverviewLabels = result.Data.AppointmentsOverviewLabels ?? new List<string>();

            return Page();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading doctor dashboard");
            TempData["Error"] = "An error occurred while loading dashboard data.";
            return Page();
        }
    }
}

// Supporting Models
public class TodayAppointment
{
    public int Id { get; set; }
    public string PatientName { get; set; } = string.Empty;
    public int PatientAge { get; set; }
    public DateTime AppointmentTime { get; set; }
    public string Diagnosis { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
}

public class PatientReview
{
    public int PatientId { get; set; }
    public string PatientName { get; set; } = string.Empty;
    public string ReviewText { get; set; } = string.Empty;
    public int Rating { get; set; }
}

public class RecentPatient
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateTime LastAppointment { get; set; }
    public string Notes { get; set; } = string.Empty;
}