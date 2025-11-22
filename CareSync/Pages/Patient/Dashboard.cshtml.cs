using Microsoft.AspNetCore.Mvc;
using CareSync.Pages.Shared;
using CareSync.Services;
using CareSync.ApplicationLayer.Contracts.PatientsDTOs;

namespace CareSync.Pages.Patient;

public class DashboardModel : BasePageModel
{
    private readonly ILogger<DashboardModel> _logger;
    private readonly PatientApiService _patientApiService;

    public DashboardModel(
        ILogger<DashboardModel> logger,
        PatientApiService patientApiService)
    {
        _logger = logger;
        _patientApiService = patientApiService;
    }

    // Patient Information Properties
    public string PatientName { get; set; } = string.Empty;
    public string Gender { get; set; } = string.Empty;
    public int Age { get; set; }
    public string BloodType { get; set; } = string.Empty;
    public string PrimaryDoctor { get; set; } = "Not Assigned";
    public string LastVisitDate { get; set; } = "No visits yet";
    public string NextAppointmentDate { get; set; } = "Not scheduled";

    // Dashboard Statistics
    public int UpcomingAppointments { get; set; }
    public int ActivePrescriptions { get; set; }
    public int PendingLabTests { get; set; }
    public int NewReports { get; set; }

    // Health Vitals
    public decimal CurrentBP { get; set; }
    public decimal CurrentSugar { get; set; }
    public int CurrentHeartRate { get; set; }
    public decimal CurrentCholesterol { get; set; }

    // Doctor Visits
    public List<DoctorVisit> DoctorVisits { get; set; } = new();

    // Medical Reports
    public List<MedicalReport> MedicalReports { get; set; } = new();

    public async Task<IActionResult> OnGetAsync()
    {
        // Check if user is authenticated and has Patient role
        var authResult = RequireRole("Patient");
        if (authResult != null) return authResult;

        try
        {
            _logger.LogInformation("Loading patient dashboard data from API");
            
            // Call API to get dashboard data
            var result = await _patientApiService.GetPatientDashboardAsync();

            if (result.IsSuccess && result.Data != null)
            {
                var dashboard = result.Data;

                // Map Profile Data
                PatientName = dashboard.Profile.PatientName;
                Gender = dashboard.Profile.Gender;
                Age = dashboard.Profile.Age;
                BloodType = dashboard.Profile.BloodType;
                PrimaryDoctor = dashboard.Profile.PrimaryDoctor;
                LastVisitDate = dashboard.Profile.LastVisitDate ?? "No visits yet";
                NextAppointmentDate = dashboard.Profile.NextAppointmentDate ?? "Not scheduled";

                // Map Statistics
                UpcomingAppointments = dashboard.Statistics.UpcomingAppointments;
                ActivePrescriptions = dashboard.Statistics.ActivePrescriptions;
                PendingLabTests = dashboard.Statistics.PendingLabTests;
                NewReports = dashboard.Statistics.NewReports;

                // Map Recent Visits
                DoctorVisits = dashboard.RecentVisits.Select(v => new DoctorVisit
                {
                    Id = v.AppointmentId,
                    DoctorName = v.DoctorName,
                    DoctorImage = v.DoctorImage,
                    VisitDate = v.VisitDate,
                    Department = v.Department,
                    Specialization = v.Specialization
                }).ToList();

                // Map Medical Reports
                MedicalReports = dashboard.RecentReports.Select(r => new MedicalReport
                {
                    Id = r.ReportId,
                    ReportTitle = r.ReportTitle,
                    ReportDate = r.ReportDate,
                    ReportType = r.ReportType
                }).ToList();

                // Map Health Vitals if available
                if (dashboard.LatestVitals != null)
                {
                    CurrentBP = dashboard.LatestVitals.BloodPressureSystolic ?? 0;
                    CurrentSugar = dashboard.LatestVitals.BloodSugar ?? 0;
                    CurrentHeartRate = dashboard.LatestVitals.HeartRate ?? 0;
                    CurrentCholesterol = dashboard.LatestVitals.Cholesterol ?? 0;
                }

                _logger.LogInformation("Successfully loaded patient dashboard data");
            }
            else
            {
                _logger.LogWarning($"Failed to load dashboard: {result.GetError()}");
                TempData["ErrorMessage"] = "Unable to load dashboard data. Please try again.";
                
                // Set default values to prevent UI errors
                SetDefaultValues();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading patient dashboard");
            TempData["ErrorMessage"] = "An error occurred while loading your dashboard.";
            
            // Set default values to prevent UI errors
            SetDefaultValues();
        }

        return Page();
    }

    private void SetDefaultValues()
    {
        PatientName = PatientName ?? "Guest";
        Gender = Gender ?? "Not specified";
        BloodType = BloodType ?? "Unknown";
        PrimaryDoctor = PrimaryDoctor ?? "Not Assigned";
        LastVisitDate = LastVisitDate ?? "No visits yet";
        NextAppointmentDate = NextAppointmentDate ?? "Not scheduled";
        DoctorVisits = DoctorVisits ?? new List<DoctorVisit>();
        MedicalReports = MedicalReports ?? new List<MedicalReport>();
    }
}

// Supporting Models
public class DoctorVisit
{
    public int Id { get; set; }
    public string DoctorName { get; set; } = string.Empty;
    public string DoctorImage { get; set; } = string.Empty;
    public string VisitDate { get; set; } = string.Empty;
    public string Department { get; set; } = string.Empty;
    public string Specialization { get; set; } = string.Empty;
}

public class MedicalReport
{
    public int Id { get; set; }
    public string ReportTitle { get; set; } = string.Empty;
    public string ReportDate { get; set; } = string.Empty;
    public string ReportType { get; set; } = string.Empty;
}
