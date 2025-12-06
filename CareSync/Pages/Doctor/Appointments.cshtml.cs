using CareSync.ApplicationLayer.ApiResult;
using CareSync.ApplicationLayer.Contracts.AppointmentsDTOs;
using CareSync.ApplicationLayer.Contracts.DoctorsDTOs;
using CareSync.ApplicationLayer.Contracts.AdminDashboardDTOs;

using CareSync.ApplicationLayer.IServices.EntitiesServices;
using CareSync.Pages.Shared;
using CareSync.Services;
using CareSync.Shared.Enums.Appointment;
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

    // Statistics (based on selected date range)
    public int TodayTotal { get; set; }
    public int TotalCompleted { get; set; }
    public int TotalPending { get; set; }
    public int TotalCancelled { get; set; }

    // Date range filter
    [BindProperty(SupportsGet = true)]
    public DateTime? StartDate { get; set; }

    [BindProperty(SupportsGet = true)]
    public DateTime? EndDate { get; set; }

    public string TodayDateReadable
    {
        get
        {
            var start = (StartDate ?? DateTime.Today).Date;
            var end = (EndDate ?? DateTime.Today).Date;

            if (start == end)
            {
                return start.ToString("dddd, MMMM dd, yyyy");
            }

            return $"{start:MMM dd, yyyy} - {end:MMM dd, yyyy}";
        }
    }

    // Lists used by the UI
    public List<TodayAppointmentView> TodaysAppointments { get; set; } = new();
    public List<RecentAppointmentView> RecentAppointments { get; set; } = new();

    [BindProperty]
    public int AppointmentId { get; set; }

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

            // Determine effective date range
            var rangeStart = (StartDate ?? DateTime.Today).Date;
            var rangeEnd = (EndDate ?? DateTime.Today).Date;

            var doctorApptsResult = await _doctorApiService.GetAppointmentsAsync();

            if (doctorApptsResult?.IsSuccess == true && doctorApptsResult.Data != null)
            {
                var apptItems = doctorApptsResult.Data.Appointments;
                var previousItems = doctorApptsResult.Data.PreviousAppointments;

                // Resolve Doctor ID
                doctorId = apptItems
                    .Where(a => a.DoctorID != 0)
                    .Select(a => (int?)a.DoctorID)
                    .FirstOrDefault();

                // Recent (within selected date range)
                RecentAppointments = BuildRecentAppointments(apptItems, previousItems, rangeStart, rangeEnd);

                // Today’s appointments (still based on current day)
                TodaysAppointments = BuildTodayAppointments(apptItems);

                // Stats (based on selected date range)
                BuildStatsFromAppointments(apptItems, previousItems, rangeStart, rangeEnd);
            }
            else
            {
                doctorId = await ResolveDoctorIdFallback(userId);
                if (!doctorId.HasValue)
                    return Page();

                var appts = await _appointmentService.GetAppointmentsForDoctorAsync(doctorId.Value);

                // Stats (based on selected date range)
                BuildStatsFromFallback(appts, rangeStart, rangeEnd);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading doctor appointments");
            TempData["ErrorMessage"] = "An error occurred while loading appointments.";
        }

        return Page();
    }

    public async Task<IActionResult> OnPostStartAsync(int appointmentId)
    {
        var authResult = RequireRole("Doctor");
        if (authResult != null) return authResult;

        try
        {
            var result = await _doctorApiService.StartAppointmentAsync(appointmentId);
            if (result?.IsSuccess == true && result.Data != null && result.Data.Success)
            {
                TempData["SuccessMessage"] = result.Data.Message;
            }
            else
            {
                TempData["ErrorMessage"] = result?.GetError() ?? "Failed to start consultation.";
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error starting consultation for appointment {AppointmentId}", appointmentId);
            TempData["ErrorMessage"] = "An error occurred while starting the consultation.";
        }

        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostAcceptAsync(int appointmentId)
    {
        var authResult = RequireRole("Doctor");
        if (authResult != null) return authResult;

        try
        {
            var result = await _doctorApiService.AcceptAppointmentAsync(appointmentId);
            if (result?.IsSuccess == true && result.Data != null && result.Data.Success)
            {
                TempData["SuccessMessage"] = result.Data.Message;
            }
            else
            {
                TempData["ErrorMessage"] = result?.GetError() ?? "Failed to accept appointment.";
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error accepting appointment {AppointmentId}", appointmentId);
            TempData["ErrorMessage"] = "An error occurred while accepting the appointment.";
        }

        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostRejectAsync(int appointmentId)
    {
        var authResult = RequireRole("Doctor");
        if (authResult != null) return authResult;

        try
        {
            var result = await _doctorApiService.RejectAppointmentAsync(appointmentId);
            if (result?.IsSuccess == true && result.Data != null && result.Data.Success)
            {
                TempData["SuccessMessage"] = result.Data.Message;
            }
            else
            {
                TempData["ErrorMessage"] = result?.GetError() ?? "Failed to reject appointment.";
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error rejecting appointment {AppointmentId}", appointmentId);
            TempData["ErrorMessage"] = "An error occurred while rejecting the appointment.";
        }

        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostCompleteAsync(int appointmentId)
    {
        var authResult = RequireRole("Doctor");
        if (authResult != null) return authResult;

        try
        {
            var result = await _doctorApiService.CompleteAppointmentAsync(appointmentId);
            if (result?.IsSuccess == true && result.Data != null && result.Data.Success)
            {
                TempData["SuccessMessage"] = result.Data.Message;
            }
            else
            {
                TempData["ErrorMessage"] = result?.GetError() ?? "Failed to complete consultation.";
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error completing consultation for appointment {AppointmentId}", appointmentId);
            TempData["ErrorMessage"] = "An error occurred while completing the consultation.";
        }

        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostFollowUpAsync(int appointmentId)
    {
        var authResult = RequireRole("Doctor");
        if (authResult != null) return authResult;

        try
        {
            var result = await _doctorApiService.MarkFollowUpRequiredAsync(appointmentId);
            if (result?.IsSuccess == true && result.Data != null && result.Data.Success)
            {
                TempData["SuccessMessage"] = result.Data.Message;
            }
            else
            {
                TempData["ErrorMessage"] = result?.GetError() ?? "Failed to mark follow-up required.";
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error marking follow-up required for appointment {AppointmentId}", appointmentId);
            TempData["ErrorMessage"] = "An error occurred while marking follow-up required.";
        }

        return RedirectToPage();
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
            "prescriptionpending" => "warning",
            "accepted" => "info",
            "labrequested" => "warning",
            "labcompleted" => "primary",
            "followuprequired" => "secondary",
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
            "accepted" => "info",
            "labrequested" => "warning",
            "labcompleted" => "primary",
            "cancelled" => "danger",
            _ => "secondary"
        };
    }

    private static bool CanStartAppointment(DateTime time, AppointmentStatus_Enum status)
    {
        return time <= DateTime.Now.AddMinutes(15) &&
               status != AppointmentStatus_Enum.Completed &&
               status != AppointmentStatus_Enum.InProgress &&
               status != AppointmentStatus_Enum.PrescriptionPending &&
               status != AppointmentStatus_Enum.Rejected &&
               status != AppointmentStatus_Enum.Cancelled;
    }

    private List<RecentAppointmentView> BuildRecentAppointments(
        List<TodayAppointmentItem> todayItems,
        List<CareSync.ApplicationLayer.Contracts.AdminDashboardDTOs.PreviousAppointment_DTO> previousItems,
        DateTime rangeStart,
        DateTime rangeEnd)
    {
        var combined = new List<(int AppointmentId, DateTime Date, string PatientName, AppointmentType_Enum Type, string Reason, AppointmentStatus_Enum Status)>();

        combined.AddRange(todayItems
            .Where(a => a.AppointmentDate.Date >= rangeStart && a.AppointmentDate.Date <= rangeEnd)
            .Select(a => (a.AppointmentID, a.AppointmentDate, a.PatientName, a.AppointmentType, a.Reason ?? string.Empty, a.Status)));

        combined.AddRange(previousItems
            .Where(p => p.AppointmentDate.Date >= rangeStart && p.AppointmentDate.Date <= rangeEnd)
            .Select(p => (p.AppointmentID, p.AppointmentDate, p.PatientName, p.AppointmentType, p.Reason ?? string.Empty, p.Status)));

        return combined
            .OrderByDescending(a => a.Date)
            .Take(20)
            .Select(a => new RecentAppointmentView
            {
                AppointmentId = a.AppointmentId,
                Date = a.Date,
                PatientName = a.PatientName,
                AppointmentType = a.Type,
                Reason = a.Reason,
                Status = a.Status,
                StatusBadge = GetBadgeForStatus(a.Status.ToString())
            }).ToList();
    }

    private List<TodayAppointmentView> BuildTodayAppointments(List<TodayAppointmentItem> apptItems)
    {
        return apptItems
            .Where(a => a.AppointmentTime.Date == DateTime.Today &&
                        a.Status != AppointmentStatus_Enum.Completed)
            .OrderBy(a => a.AppointmentTime)
            .Select(a => new TodayAppointmentView
            {
                AppointmentId = a.AppointmentID,
                PatientId = a.PatientID,
                PatientName = a.PatientName,
                PatientImage = "~/theme/images/patient-placeholder.png",
                PatientMeta = a.Reason ?? "",
                AppointmentTime = a.AppointmentTime.ToString("hh:mm tt"),
                AppointmentDate = a.AppointmentTime.Date,
                AppointmentType = a.AppointmentType,
                Reason = a.Reason ?? "",
                Status = a.Status,
                StatusBadgeBackground = GetBadgeForStatus(a.Status.ToString()),
                StatusBadgeText = GetTextColorForStatus(a.Status.ToString()),
                CanStart = CanStartAppointment(a.AppointmentTime, a.Status)
            }).ToList();
    }

    private void BuildStatsFromAppointments(
        List<TodayAppointmentItem> todayItems,
        List<CareSync.ApplicationLayer.Contracts.AdminDashboardDTOs.PreviousAppointment_DTO> previousItems,
        DateTime rangeStart,
        DateTime rangeEnd)
    {
        var rangeItems = new List<(AppointmentStatus_Enum Status, DateTime Date)>();

        rangeItems.AddRange(todayItems
            .Where(a => a.AppointmentDate.Date >= rangeStart && a.AppointmentDate.Date <= rangeEnd)
            .Select(a => (a.Status, a.AppointmentDate)));

        rangeItems.AddRange(previousItems
            .Where(p => p.AppointmentDate.Date >= rangeStart && p.AppointmentDate.Date <= rangeEnd)
            .Select(p => (p.Status, p.AppointmentDate)));

        TodayTotal = rangeItems.Count;
        TotalCompleted = rangeItems.Count(a => a.Status == AppointmentStatus_Enum.Completed);
        TotalPending = rangeItems.Count(a =>
            a.Status == AppointmentStatus_Enum.Pending ||
            a.Status == AppointmentStatus_Enum.Created ||
            a.Status == AppointmentStatus_Enum.Scheduled ||
            a.Status == AppointmentStatus_Enum.PrescriptionPending ||
            a.Status == AppointmentStatus_Enum.Accepted ||
            a.Status == AppointmentStatus_Enum.LabRequested ||
            a.Status == AppointmentStatus_Enum.LabCompleted);
        TotalCancelled = rangeItems.Count(a => a.Status == AppointmentStatus_Enum.Cancelled);
    }

    private void BuildStatsFromFallback(List<Appointment_DTO> items, DateTime rangeStart, DateTime rangeEnd)
    {
        var rangeItems = items
            .Where(a => a.AppointmentDate.Date >= rangeStart && a.AppointmentDate.Date <= rangeEnd)
            .ToList();

        TodayTotal = rangeItems.Count;
        TotalCompleted = rangeItems.Count(a => a.Status == AppointmentStatus_Enum.Completed);
        TotalPending = rangeItems.Count(a =>
            a.Status == AppointmentStatus_Enum.Pending ||
            a.Status == AppointmentStatus_Enum.Created ||
            a.Status == AppointmentStatus_Enum.Scheduled ||
            a.Status == AppointmentStatus_Enum.PrescriptionPending ||
            a.Status == AppointmentStatus_Enum.Accepted ||
            a.Status == AppointmentStatus_Enum.LabRequested ||
            a.Status == AppointmentStatus_Enum.LabCompleted);
        TotalCancelled = rangeItems.Count(a => a.Status == AppointmentStatus_Enum.Cancelled);
    }

    private async Task<int?> ResolveDoctorIdFallback(string userId)
    {
        try
        {
            var profile = await _doctorApiService.GetDoctorProfileAsync<Result<DoctorProfile_DTO>>(userId);
            if (profile?.IsSuccess == true && profile.Data?.DoctorId > 0)
                return profile.Data.DoctorId;
        }
        catch (Exception ex)
        {
            _logger.LogDebug(ex, "Unable to load doctor profile for {UserId}", userId);
        }
        return null;
    }


    public class TodayAppointmentView
    {
        public int AppointmentId { get; set; }
        public int PatientId { get; set; }
        public string PatientName { get; set; } = string.Empty;
        public string PatientImage { get; set; } = "~/theme/images/patient-placeholder.png";
        public string PatientMeta { get; set; } = string.Empty;
        public string AppointmentTime { get; set; } = string.Empty;
        public DateTime AppointmentDate { get; set; }
        public AppointmentType_Enum AppointmentType { get; set; }
        public string Reason { get; set; } = string.Empty;
        public AppointmentStatus_Enum Status { get; set; }
        public string StatusBadgeBackground { get; set; } = "secondary";
        public string StatusBadgeText { get; set; } = "secondary";
        public bool CanStart { get; set; } = false;
    }

    public class RecentAppointmentView
    {
        public int AppointmentId { get; set; }
        public DateTime Date { get; set; }
        public string PatientName { get; set; } = string.Empty;
        public AppointmentType_Enum AppointmentType { get; set; }
        public string Reason { get; set; } = string.Empty;
        public AppointmentStatus_Enum Status { get; set; }
        public string StatusBadge { get; set; } = "secondary";
    }
}