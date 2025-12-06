using CareSync.ApplicationLayer.Contracts.AppointmentsDTOs;
using CareSync.Pages.Shared;
using CareSync.Services;
using CareSync.Shared.Enums;
using CareSync.Shared.Enums.Appointment;
using CareSync.Shared.Enums.Patient;
using Microsoft.AspNetCore.Mvc;

namespace CareSync.Pages.Doctor;

public class AppointmentDetails(ILogger<AppointmentDetails> logger, DoctorApiService doctorApiService)
    : BasePageModel
{
    private readonly ILogger<AppointmentDetails> _logger = logger;
    private readonly DoctorApiService _doctorApiService = doctorApiService;

    [FromQuery(Name = "id")]
    public int AppointmentId { get; set; }

    // Appointment
    public DateTime AppointmentDate { get; set; }
    public string AppointmentTime => AppointmentDate.ToString("hh:mm tt");
    public AppointmentType_Enum AppointmentType { get; set; }
    public AppointmentStatus_Enum Status { get; set; }
    public string Reason { get; set; } = string.Empty;
    public string? Notes { get; set; }
    public DateTime CreatedOn { get; set; }

    // Patient
    public int PatientId { get; set; }
    public string PatientName { get; set; } = string.Empty;
    public int PatientAge { get; set; }
    public string? PatientContact { get; set; }
    public string? PatientEmail { get; set; }
    public Gender_Enum Gender { get; set; }
    public string? BloodGroup { get; set; }
    public MaritalStatusEnum MaritalStatus { get; set; }

    // Doctor
    public int DoctorId { get; set; }
    public string DoctorName { get; set; } = string.Empty;
    public string DoctorSpecialization { get; set; } = string.Empty;

    // Medical Notes
    public string Diagnosis { get; set; } = string.Empty;
    public string TreatmentPlan { get; set; } = string.Empty;
    public string DoctorNotes { get; set; } = string.Empty;

    // Vitals
    public string BloodPressure { get; set; } = string.Empty;
    public int HeartRate { get; set; }
    public decimal Temperature { get; set; }

    // Lists
    public List<string> Prescriptions { get; set; } = new();
    public List<string> LabReports { get; set; } = new();

    public async Task<IActionResult> OnGetAsync()
    {
        var authResult = RequireRole("Doctor");
        if (authResult != null) return authResult;

        if (AppointmentId <= 0)
        {
            TempData["ErrorMessage"] = "Invalid appointment ID";
            return RedirectToPage("/Doctor/Appointments");
        }

        try
        {
            _logger.LogInformation("Fetching appointment details for {Id}", AppointmentId);

            var result = await _doctorApiService.GetAppointmentByIdAsync(AppointmentId);

            if (result?.IsSuccess != true || result.Data == null)
            {
                TempData["ErrorMessage"] = result?.Error ?? "Unable to load details";
                return Page();
            }

            MapFromDto(result.Data);
            return Page();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading appointment details for {Id}", AppointmentId);
            TempData["ErrorMessage"] = "Error loading details";
            return Page();
        }
    }

    // ---------------------------
    // START APPOINTMENT
    // ---------------------------
    public async Task<IActionResult> OnPostStartAsync([FromForm] int appointmentId)
    {
        var authResult = RequireRole("Doctor");
        if (authResult != null) return authResult;
        if (appointmentId <= 0) return BadRequest(new { success = false, message = "Invalid appointment ID" });

        try
        {
            _logger.LogInformation("Starting consultation for {Id}", appointmentId);

            var apiResult = await _doctorApiService.StartAppointmentAsync(appointmentId);

            if (apiResult?.IsSuccess != true || apiResult.Data?.Success != true)
                return new JsonResult(new
                {
                    success = false,
                    message = apiResult?.Data?.Message ?? apiResult?.GetError() ?? "Unable to start appointment"
                });

            Status = AppointmentStatus_Enum.InProgress;

            return new JsonResult(new
            {
                success = true,
                status = Status,
                startedAt = DateTime.UtcNow
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error starting consultation for {Id}", appointmentId);
            return new JsonResult(new { success = false, message = "Error starting appointment" });
        }
    }

    // ---------------------------
    // END APPOINTMENT
    // ---------------------------
    public async Task<IActionResult> OnPostEndAsync([FromForm] int appointmentId)
    {
        var authResult = RequireRole("Doctor");
        if (authResult != null) return authResult;
        if (appointmentId <= 0) return BadRequest(new { success = false, message = "Invalid appointment ID" });

        try
        {
            _logger.LogInformation("Ending consultation for {Id}", appointmentId);

            var apiResult = await _doctorApiService.EndAppointmentAsync(appointmentId);

            if (apiResult?.IsSuccess != true || apiResult.Data?.Success != true)
                return new JsonResult(new
                {
                    success = false,
                    message = apiResult?.Data?.Message ?? apiResult?.GetError() ?? "Unable to end appointment"
                });

            Status = AppointmentStatus_Enum.Completed;

            return new JsonResult(new
            {
                success = true,
                status = Status,
                endedAt = DateTime.UtcNow
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error ending appointment for {Id}", appointmentId);
            return new JsonResult(new { success = false, message = "Error ending appointment" });
        }
    }

    private void MapFromDto(AppointmentDetails_DTO dto)
    {
        // Appointment
        AppointmentId = dto.AppointmentID;
        AppointmentDate = dto.AppointmentDate;
        AppointmentType = dto.AppointmentType;
        Status = dto.Status;
        Notes = dto.Notes;
        CreatedOn = dto.CreatedOn;
        Reason = dto.Reason ?? string.Empty;

        // Doctor
        DoctorId = dto.DoctorID;
        DoctorName = dto.DoctorName ?? string.Empty;
        DoctorSpecialization = dto.DoctorSpecialization ?? string.Empty;

        // Patient
        PatientId = dto.PatientID;
        PatientName = dto.PatientName ?? string.Empty;
        PatientAge = dto.PatientAge;
        PatientContact = dto.PatientContact ?? string.Empty;
        PatientEmail = dto.PatientEmail ?? string.Empty;
        Gender = dto.Gender;
        BloodGroup = dto.BloodGroup;
        MaritalStatus = dto.MaritalStatus;

        // Medical
        Diagnosis = dto.Diagnosis ?? string.Empty;
        TreatmentPlan = dto.TreatmentPlan ?? string.Empty;
        DoctorNotes = dto.DoctorNotes ?? string.Empty;

        // Vitals
        BloodPressure = dto.BloodPressure ?? string.Empty;
        HeartRate = dto.HeartRate ?? 0;
        Temperature = dto.Temperature ?? 0m;

        // Lists
        Prescriptions = dto.Prescriptions ?? new();
        LabReports = dto.LabReports ?? new();
    }
}
