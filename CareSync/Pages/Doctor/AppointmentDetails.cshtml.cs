using CareSync.Pages.Shared;
using CareSync.Services;
using CareSync.Shared.Enums.Appointment;
using Microsoft.AspNetCore.Mvc;

namespace CareSync.Pages.Doctor;

public class AppointmentDetails(ILogger<AppointmentDetails> logger, DoctorApiService doctorApiService) : BasePageModel
{
    private readonly ILogger<AppointmentDetails> _logger = logger;
    private readonly DoctorApiService _doctorApiService = doctorApiService;

    [FromQuery(Name = "id")]
    public int AppointmentId { get; set; }

    // Appointment summary
    public DateTime AppointmentDate { get; set; }
    public string AppointmentTime => AppointmentDate.ToString("hh:mm tt");
    public AppointmentType_Enum AppointmentType { get; set; }
    public AppointmentStatus_Enum Status { get; set; }
    public string Reason { get; set; } = string.Empty;

    // Patient info
    public int PatientId { get; set; }
    public string PatientName { get; set; }
    public int PatientAge { get; set; } = 0;
    public string PatientContact { get; set; } = string.Empty;
    public string PatientEmail { get; set; } = string.Empty;

    // Doctor info (current)
    public string DoctorName { get; set; } = "You";
    public string DoctorSpecialization { get; set; } = string.Empty;

    // Medical notes
    public string Diagnosis { get; set; } = string.Empty;
    public string TreatmentPlan { get; set; } = string.Empty;
    public string DoctorNotes { get; set; } = string.Empty;

    // Vitals (simple)
    public string BloodPressure { get; set; } = string.Empty;
    public int HeartRate { get; set; }
    public decimal Temperature { get; set; }

    // Prescriptions / labs (simple lists)
    public List<string> Prescriptions { get; set; } = new();
    public List<string> LabReports { get; set; } = new();

    public async Task<IActionResult> OnGetAsync()
    {
        // Ensure only doctors can access
        var authResult = RequireRole("Doctor");
        if (authResult != null) return authResult;

        if (AppointmentId <= 0)
        {
            _logger.LogWarning("Doctor attempted to open appointment details with invalid id: {Id}", AppointmentId);
            TempData["ErrorMessage"] = "Invalid appointment ID";
            return RedirectToPage("/Doctor/Appointments");
        }

        _logger.LogInformation("Loading appointment details for AppointmentId {AppointmentId}", AppointmentId);

        try
        {
            var result = await _doctorApiService.GetAppointmentByIdAsync(AppointmentId);
            if (result == null)
            {
                _logger.LogWarning("Appointment details API returned null for {AppointmentId}", AppointmentId);
                TempData["ErrorMessage"] = "Failed to load appointment details";
                return Page();
            }

            if (!result.IsSuccess)
            {
                var msg = result.Error;
                _logger.LogWarning("Appointment details API failed for {AppointmentId}: {Error}", AppointmentId, msg);
                TempData["ErrorMessage"] = msg;
                return Page();
            }

            var dto = result.Data;
            if (dto == null)
            {
                _logger.LogWarning("Appointment details API returned success but data was null for {AppointmentId}", AppointmentId);
                TempData["ErrorMessage"] = "Appointment details not found";
                return Page();
            }

            // Map DTO to page properties
            //AppointmentNumber = dto.AppointmentNumber;
            AppointmentDate = dto.AppointmentDate;
            AppointmentType = dto.AppointmentType;
            Status = dto.Status;
            Reason = dto.Reason ?? string.Empty;

            PatientId = dto.PatientID;
            PatientName = dto.PatientName;
            PatientAge = dto.PatientAge;
            PatientContact = dto.PatientContact ?? string.Empty;
            PatientEmail = dto.PatientEmail ?? string.Empty;

            DoctorName = dto.DoctorName ?? "You";
            DoctorSpecialization = dto.DoctorSpecialization ?? string.Empty;

            Diagnosis = dto.Diagnosis ?? string.Empty;
            TreatmentPlan = dto.TreatmentPlan ?? string.Empty;
            DoctorNotes = dto.DoctorNotes ?? string.Empty;

            BloodPressure = dto.BloodPressure ?? string.Empty;
            HeartRate = dto.HeartRate ?? 0;
            Temperature = dto.Temperature ?? 0m;

            Prescriptions = dto.Prescriptions ?? new List<string>();
            LabReports = dto.LabReports ?? new List<string>();

            return Page();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading appointment details for id {AppointmentId}", AppointmentId);
            TempData["ErrorMessage"] = "Failed to load appointment details";
            return Page();
        }
    }

    /// <summary>
    /// Start consultation handler.
    /// Client posts form-data { appointmentId, __RequestVerificationToken } to ?handler=Start
    /// Calls backend API to persist status change.
    /// </summary>
    public async Task<IActionResult> OnPostStartAsync([FromForm] int appointmentId)
    {
        var authResult = RequireRole("Doctor");
        if (authResult != null) return authResult;

        if (appointmentId <= 0)
            return BadRequest(new { success = false, message = "Invalid appointment id" });

        try
        {
            _logger.LogInformation("Doctor starting consultation for appointment {AppointmentId}", appointmentId);

            // Call API to persist change
            var apiResult = await _doctorApiService.StartAppointmentAsync(appointmentId);
            if (apiResult == null)
            {
                _logger.LogWarning("StartAppointment API returned null for {AppointmentId}", appointmentId);
                return new JsonResult(new { success = false, message = "API error starting appointment" });
            }

            // Handle cases where outer Result indicates failure
            if (!apiResult.IsSuccess)
            {
                // prefer message returned inside Data (GeneralResponse.Message) — Result<T> private fields are not populated by deserializer
                var apiMessage = apiResult.Data?.Message ?? apiResult.GetError() ?? "Failed to start appointment";
                _logger.LogWarning("StartAppointment API failed for {AppointmentId}: {Error}", appointmentId, apiMessage);
                return new JsonResult(new { success = false, message = apiMessage });
            }

            // Also handle the case where API wrapper succeeded but inner payload indicates failure
            if (apiResult.Data != null && !apiResult.Data.Success)
            {
                var apiMessage = apiResult.Data.Message ?? "Failed to start appointment";
                _logger.LogWarning("StartAppointment API returned unsuccessful payload for {AppointmentId}: {Message}", appointmentId, apiMessage);
                return new JsonResult(new { success = false, message = apiMessage });
            }

            // Update local state for UI immediate feedback
            if (AppointmentId == 0) AppointmentId = appointmentId;
            Status = AppointmentStatus_Enum.InProgress;

            return new JsonResult(new { success = true, status = Status, startedAt = DateTime.UtcNow });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error starting consultation for appointment {AppointmentId}", appointmentId);
            return new JsonResult(new { success = false, message = "Failed to start consultation" });
        }
    }

    /// <summary>
    /// End consultation handler.
    /// Client posts form-data { appointmentId, __RequestVerificationToken } to ?handler=End
    /// Calls backend API to persist status change.
    /// </summary>
    public async Task<IActionResult> OnPostEndAsync([FromForm] int appointmentId)
    {
        var authResult = RequireRole("Doctor");
        if (authResult != null) return authResult;

        if (appointmentId <= 0)
            return BadRequest(new { success = false, message = "Invalid appointment id" });

        try
        {
            _logger.LogInformation("Doctor ending consultation for appointment {AppointmentId}", appointmentId);

            // Call API to persist change
            var apiResult = await _doctorApiService.EndAppointmentAsync(appointmentId);
            if (apiResult == null)
            {
                _logger.LogWarning("EndAppointment API returned null for {AppointmentId}", appointmentId);
                return new JsonResult(new { success = false, message = "API error ending appointment" });
            }

            if (!apiResult.IsSuccess)
            {
                var apiMessage = apiResult.Data?.Message ?? apiResult.GetError() ?? "Failed to end appointment";
                _logger.LogWarning("EndAppointment API failed for {AppointmentId}: {Error}", appointmentId, apiMessage);
                return new JsonResult(new { success = false, message = apiMessage });
            }

            if (apiResult.Data != null && !apiResult.Data.Success)
            {
                var apiMessage = apiResult.Data.Message ?? "Failed to end appointment";
                _logger.LogWarning("EndAppointment API returned unsuccessful payload for {AppointmentId}: {Message}", appointmentId, apiMessage);
                return new JsonResult(new { success = false, message = apiMessage });
            }

            // Update local state for UI immediate feedback
            if (AppointmentId == 0) AppointmentId = appointmentId;
            Status = AppointmentStatus_Enum.Completed;

            return new JsonResult(new { success = true, status = Status, endedAt = DateTime.UtcNow });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error ending consultation for appointment {AppointmentId}", appointmentId);
            return new JsonResult(new { success = false, message = "Failed to end consultation" });
        }
    }
}