using CareSync.ApplicationLayer.Contracts.AdminDashboardDTOs;
using CareSync.Pages.Shared;
using CareSync.Services;
using Microsoft.AspNetCore.Mvc;

namespace CareSync.Pages.Doctor;

public class PrescriptionsModel : BasePageModel
{
    private readonly ILogger<PrescriptionsModel> _logger;
    private readonly DoctorApiService _doctorApiService;

    public PrescriptionsModel(ILogger<PrescriptionsModel> logger, DoctorApiService doctorApiService)
    {
        _logger = logger;
        _doctorApiService = doctorApiService;
    }

    public List<PendingPrescriptionItem> PendingAppointments { get; set; } = new();
    public string? ErrorMessage { get; set; }

    public async Task<IActionResult> OnGetAsync()
    {
        var authResult = RequireRole("Doctor");
        if (authResult != null) return authResult;

        try
        {
            var result = await _doctorApiService.GetAppointmentsAsync();
            if (result == null || !result.IsSuccess || result.Data == null)
            {
                ErrorMessage = result?.GetError() ?? "Unable to load appointments.";
                return Page();
            }

            var items = new List<PendingPrescriptionItem>();

            if (result.Data.Appointments != null)
            {
                items.AddRange(result.Data.Appointments
                    .Where(a => a.Status == CareSync.Shared.Enums.Appointment.AppointmentStatus_Enum.PrescriptionPending)
                    .Select(a => new PendingPrescriptionItem
                    {
                        AppointmentId = a.AppointmentID,
                        AppointmentDate = a.AppointmentDate,
                        PatientName = a.PatientName,
                        AppointmentType = a.AppointmentType.ToString(),
                        Reason = a.Reason ?? string.Empty
                    }));
            }

            if (result.Data.PreviousAppointments != null)
            {
                items.AddRange(result.Data.PreviousAppointments
                    .Where(a => a.Status == CareSync.Shared.Enums.Appointment.AppointmentStatus_Enum.PrescriptionPending)
                    .Select(a => new PendingPrescriptionItem
                    {
                        AppointmentId = a.AppointmentID,
                        AppointmentDate = a.AppointmentDate,
                        PatientName = a.PatientName,
                        AppointmentType = a.AppointmentType.ToString(),
                        Reason = a.Reason ?? string.Empty
                    }));
            }

            PendingAppointments = items
                .OrderByDescending(a => a.AppointmentDate)
                .ToList();

            return Page();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading pending prescriptions list");
            ErrorMessage = "An error occurred while loading pending prescriptions.";
            return Page();
        }
    }
}

public class PendingPrescriptionItem
{
    public int AppointmentId { get; set; }
    public DateTime AppointmentDate { get; set; }
    public string PatientName { get; set; } = string.Empty;
    public string AppointmentType { get; set; } = string.Empty;
    public string Reason { get; set; } = string.Empty;
}
