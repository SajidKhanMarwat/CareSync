using CareSync.ApplicationLayer.Contracts.AppointmentsDTOs;
using CareSync.Pages.Shared;
using CareSync.Services;
using Microsoft.AspNetCore.Mvc;

namespace CareSync.Pages.Doctor;

public class AppointmentReportsModel : BasePageModel
{
    private readonly ILogger<AppointmentReportsModel> _logger;
    private readonly DoctorApiService _doctorApiService;

    public AppointmentReportsModel(ILogger<AppointmentReportsModel> logger, DoctorApiService doctorApiService)
    {
        _logger = logger;
        _doctorApiService = doctorApiService;
    }

    public AppointmentDetails_DTO? Appointment { get; set; }
    public string? ErrorMessage { get; set; }

    public async Task<IActionResult> OnGetAsync(int appointmentId)
    {
        var authResult = RequireRole("Doctor");
        if (authResult != null) return authResult;

        if (appointmentId <= 0)
        {
            ErrorMessage = "Invalid appointment id.";
            return Page();
        }

        try
        {
            var result = await _doctorApiService.GetAppointmentByIdAsync(appointmentId);
            if (result == null || !result.IsSuccess || result.Data == null)
            {
                ErrorMessage = result?.GetError() ?? "Unable to load appointment details.";
                return Page();
            }

            Appointment = result.Data;
            return Page();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading appointment reports for {AppointmentId}", appointmentId);
            ErrorMessage = "An error occurred while loading reports.";
            return Page();
        }
    }
}
