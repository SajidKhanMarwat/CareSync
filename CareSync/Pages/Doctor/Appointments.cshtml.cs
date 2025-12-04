using CareSync.ApplicationLayer.ApiResult;
using CareSync.ApplicationLayer.Contracts.AdminDashboardDTOs;
using CareSync.ApplicationLayer.Contracts.DoctorsDTOs;
using CareSync.ApplicationLayer.IServices.EntitiesServices;
using CareSync.Pages.Shared;
using CareSync.Services;
using Microsoft.AspNetCore.Mvc;

namespace CareSync.Pages.Doctor;

public class AppointmentsModel : BasePageModel
{
    private readonly ILogger<AppointmentsModel> _logger;
    private readonly DoctorApiService _doctorApiService;
    private readonly IAppointmentService _appointmentService;

    public AppointmentsModel(
        ILogger<AppointmentsModel> logger,
        AdminApiService adminApiService,
        DoctorApiService doctorApiService,
        IAppointmentService appointmentService)
    {
        _logger = logger;
        _doctorApiService = doctorApiService;
        _appointmentService = appointmentService;
    }

    // Statistics
    public int TodayTotal { get; set; }
    public int TotalCompleted { get; set; }
    public int TotalPending { get; set; }
    public int TotalCancelled { get; set; }

    public string TodayDateReadable => DateTime.Today.ToString("dddd, MMMM dd, yyyy");

    // Lists used by the UI
    public List<TodayAppointmentView> TodaysAppointments { get; set; } = new();
    public List<RecentAppointmentView> RecentAppointments { get; set; } = new();

    public async Task<IActionResult> OnGetAsync()
    {
        // Require Doctor role
        var authResult = RequireRole("Doctor");
        if (authResult != null) return authResult;

        try
        {
            var userId = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userId))
            {
                TempData["ErrorMessage"] = "Unable to identify user.";
                return Page();
            }

            int? doctorId = null;

            // Primary: use DoctorApiService.GetAppointmentsAsync() to fetch appointments/schedule data
            var doctorApptsResult = await _doctorApiService.GetAppointmentsAsync();
            if (doctorApptsResult?.IsSuccess == true && doctorApptsResult.Data != null)
            {
                var apptItems = doctorApptsResult.Data.Appointments ?? new List<TodayAppointmentItem>();

                // If appointments contain a doctor id, use it to resolve doctorId (first match)
                if (apptItems.Any(a => a.DoctorID != 0))
                {
                    doctorId = apptItems.Select(a => a.DoctorID).FirstOrDefault();
                    if (doctorId == 0) doctorId = null;
                }

                // Populate RecentAppointments from api items
                RecentAppointments = apptItems
                    .OrderByDescending(a => a.AppointmentTime)
                    .Take(10)
                    .Select(a => new RecentAppointmentView
                    {
                        AppointmentId = a.AppointmentID,
                        Date = a.AppointmentTime,
                        PatientName = a.PatientName ?? "Unknown",
                        AppointmentType = a.AppointmentType ?? "Consultation",
                        Reason = a.Reason ?? string.Empty,
                        Status = a.Status ?? string.Empty,
                        StatusBadge = GetBadgeForStatus(a.Status)
                    }).ToList();

                // Populate Today's appointments from the appointments API
                var todayAppts = apptItems
                    .Where(a => a.AppointmentTime.Date == DateTime.UtcNow.Date)
                    .OrderBy(a => a.AppointmentTime)
                    .ToList();

                TodaysAppointments = todayAppts.Select((a, idx) => new TodayAppointmentView
                {
                    Id = idx + 1,
                    PatientId = a.PatientID,
                    PatientName = string.IsNullOrWhiteSpace(a.PatientName) ? "Unknown Patient" : a.PatientName,
                    PatientImage = "~/theme/images/patient-placeholder.png",
                    PatientMeta = a.Reason ?? "",
                    AppointmentTime = a.AppointmentTime.ToString("hh:mm tt"),
                    AppointmentDate = a.AppointmentTime.Date,
                    AppointmentType = a.AppointmentType ?? "Consultation",
                    Reason = a.Reason ?? string.Empty,
                    Status = a.Status ?? "Scheduled",
                    StatusBadgeBackground = GetBadgeForStatus(a.Status),
                    StatusBadgeText = GetTextColorForStatus(a.Status),
                    CanStart = (a.AppointmentTime <= DateTime.Now.AddMinutes(15)) && (a.Status?.ToLower() != "completed")
                }).ToList();

                // Stats from API items
                TodayTotal = apptItems.Count(a => a.AppointmentTime.Date == DateTime.UtcNow.Date);
                TotalCompleted = apptItems.Count(a => string.Equals(a.Status, "Completed", StringComparison.OrdinalIgnoreCase));
                TotalPending = apptItems.Count(a => string.Equals(a.Status, "Pending", StringComparison.OrdinalIgnoreCase) || string.Equals(a.Status, "Created", StringComparison.OrdinalIgnoreCase) || string.Equals(a.Status, "Scheduled", StringComparison.OrdinalIgnoreCase));
                TotalCancelled = apptItems.Count(a => string.Equals(a.Status, "Cancelled", StringComparison.OrdinalIgnoreCase));
            }
            else
            {
                // If API failed for appointments, attempt to resolve doctorId via profile and fallback to local appointment service
                try
                {
                    var profileResult = await _doctorApiService.GetDoctorProfileAsync<Result<DoctorProfile_DTO>>(userId);
                    if (profileResult?.IsSuccess == true && profileResult.Data != null)
                    {
                        if (profileResult.Data.DoctorId != 0)
                            doctorId = profileResult.Data.DoctorId;
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogDebug(ex, "Unable to load doctor profile to resolve DoctorId for user {UserId}", userId);
                }

                if (doctorId.HasValue)
                {
                    // fallback to application-layer appointment service if API not available
                    var appts = await _appointmentService.GetAppointmentsForDoctorAsync(doctorId.Value);
                    RecentAppointments = appts
                        .OrderByDescending(a => a.AppointmentDate)
                        .Take(10)
                        .Select(a => new RecentAppointmentView
                        {
                            AppointmentId = a.AppointmentID,
                            Date = a.AppointmentDate,
                            PatientName = a.PatientName ?? "Unknown",
                            AppointmentType = a.AppointmentType.ToString(),
                            Reason = a.Reason ?? string.Empty,
                            Status = a.Status.ToString(),
                            StatusBadge = GetBadgeForStatus(a.Status.ToString())
                        }).ToList();

                    TodayTotal = appts.Count(a => a.AppointmentDate.Date == DateTime.Today);
                    TotalCompleted = appts.Count(a => a.Status.ToString().ToLower().Contains("completed"));
                    TotalPending = appts.Count(a => a.Status.ToString().ToLower().Contains("pending") || a.Status.ToString().ToLower().Contains("scheduled"));
                    TotalCancelled = appts.Count(a => a.Status.ToString().ToLower().Contains("cancelled"));

                    // populate today's list if empty
                    if (!TodaysAppointments.Any())
                    {
                        TodaysAppointments = appts
                            .Where(a => a.AppointmentDate.Date == DateTime.Today)
                            .OrderBy(a => a.AppointmentDate)
                            .Select((a, idx) => new TodayAppointmentView
                            {
                                Id = idx + 1,
                                PatientId = a.PatientID,
                                PatientName = a.PatientName ?? "Unknown Patient",
                                PatientImage = "~/theme/images/patient-placeholder.png",
                                PatientMeta = a.Reason ?? "",
                                AppointmentTime = a.AppointmentDate.ToString("hh:mm tt"),
                                AppointmentDate = a.AppointmentDate.Date,
                                AppointmentType = a.AppointmentType.ToString(),
                                Reason = a.Reason ?? string.Empty,
                                Status = a.Status.ToString(),
                                StatusBadgeBackground = GetBadgeForStatus(a.Status.ToString()),
                                StatusBadgeText = GetTextColorForStatus(a.Status.ToString()),
                                CanStart = (a.AppointmentDate <= DateTime.Now.AddMinutes(15)) && (a.Status.ToString().ToLower() != "completed")
                            }).ToList();
                    }
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading doctor appointments");
            TempData["ErrorMessage"] = "An error occurred while loading appointments.";
        }

        return Page();
    }

    private string GetBadgeForStatus(string? status)
    {
        if (string.IsNullOrEmpty(status)) return "secondary";
        return status.ToLower() switch
        {
            "completed" => "success",
            "confirmed" => "info",
            "scheduled" => "primary",
            "pending" => "warning",
            "cancelled" => "danger",
            _ => "secondary"
        };
    }

    private string GetTextColorForStatus(string? status)
    {
        if (string.IsNullOrEmpty(status)) return "secondary";
        return status.ToLower() switch
        {
            "completed" => "success",
            "confirmed" => "info",
            "scheduled" => "primary",
            "pending" => "warning",
            "cancelled" => "danger",
            _ => "secondary"
        };
    }

    public class TodayAppointmentView
    {
        public int Id { get; set; }
        public int PatientId { get; set; }
        public string PatientName { get; set; } = string.Empty;
        public string PatientImage { get; set; } = "~/theme/images/patient-placeholder.png";
        public string PatientMeta { get; set; } = string.Empty;
        public string AppointmentTime { get; set; } = string.Empty;
        public DateTime AppointmentDate { get; set; }
        public string AppointmentType { get; set; } = string.Empty;
        public string Reason { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string StatusBadgeBackground { get; set; } = "secondary";
        public string StatusBadgeText { get; set; } = "secondary";
        public bool CanStart { get; set; } = false;
    }

    public class RecentAppointmentView
    {
        public int AppointmentId { get; set; }
        public DateTime Date { get; set; }
        public string PatientName { get; set; } = string.Empty;
        public string AppointmentType { get; set; } = string.Empty;
        public string Reason { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string StatusBadge { get; set; } = "secondary";
    }
}