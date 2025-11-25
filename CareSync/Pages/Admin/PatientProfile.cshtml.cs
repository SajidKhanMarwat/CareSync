using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using CareSync.Pages.Shared;
using CareSync.Services;
using CareSync.ApplicationLayer.ApiResult;
using CareSync.ApplicationLayer.Contracts.PatientsDTOs;
using CareSync.ApplicationLayer.Contracts.AppointmentsDTOs;
using Microsoft.Extensions.Logging;

namespace CareSync.Pages.Admin;

public class PatientProfileModel : BasePageModel
{
    private readonly ILogger<PatientProfileModel> _logger;
    private readonly AdminApiService _adminApiService;

    public PatientProfileModel(ILogger<PatientProfileModel> logger, AdminApiService adminApiService)
    {
        _logger = logger;
        _adminApiService = adminApiService;
    }

    public PatientList_DTO? Patient { get; set; }
    public List<Appointment_DTO> RecentAppointments { get; set; } = new();
    public string? NextAppointment { get; set; }
    public string? ErrorMessage { get; set; }
    public bool HasError { get; set; }

    public async Task<IActionResult> OnGetAsync(int? id)
    {
        try
        {
            var authResult = RequireRole("Admin");
            if (authResult != null) return authResult;

            if (!id.HasValue)
            {
                ErrorMessage = "Patient ID is required";
                HasError = true;
                return Page();
            }

            _logger.LogInformation("Loading patient profile for ID: {PatientId}", id.Value);

            // Load all patients and find the specific one
            var patientsResult = await _adminApiService.GetAllPatientsAsync<Result<List<PatientList_DTO>>>(null, null);
            
            if (patientsResult?.IsSuccess == true && patientsResult.Data != null)
            {
                Patient = patientsResult.Data.FirstOrDefault(p => p.PatientID == id.Value);
                
                if (Patient == null)
                {
                    ErrorMessage = "Patient not found";
                    HasError = true;
                    _logger.LogWarning("Patient with ID {PatientId} not found", id.Value);
                }
                else
                {
                    _logger.LogInformation("Successfully loaded patient profile for {PatientName}", Patient.FullName);
                    
                    // TODO: Load recent appointments when the endpoint is available
                    // For now, create sample data structure
                    RecentAppointments = new List<Appointment_DTO>();
                    
                    // TODO: Get next appointment date when available
                    NextAppointment = null;
                }
            }
            else
            {
                ErrorMessage = patientsResult?.GetError() ?? "Failed to load patient data";
                HasError = true;
                _logger.LogError("Failed to load patients: {Error}", ErrorMessage);
            }

            return Page();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading patient profile");
            ErrorMessage = "An error occurred while loading the patient profile";
            HasError = true;
            return Page();
        }
    }
}
