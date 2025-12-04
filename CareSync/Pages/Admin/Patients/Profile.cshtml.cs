using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using CareSync.Pages.Shared;
using CareSync.Services;
using CareSync.ApplicationLayer.ApiResult;
using CareSync.ApplicationLayer.Contracts.PatientsDTOs;
using CareSync.ApplicationLayer.Contracts.AppointmentsDTOs;
using Microsoft.Extensions.Logging;

namespace CareSync.Pages.Admin.Patients;

public class PatientProfileModel : BasePageModel
{
    private readonly ILogger<PatientProfileModel> _logger;
    private readonly AdminApiService _adminApiService;

    public PatientProfileModel(ILogger<PatientProfileModel> logger, AdminApiService adminApiService)
    {
        _logger = logger;
        _adminApiService = adminApiService;
    }

    public PatientProfile_DTO? Patient { get; set; }
    public string ErrorMessage { get; set; } = string.Empty;
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

            // Load comprehensive patient profile
            var profileResult = await _adminApiService.GetPatientProfileAsync<Result<PatientProfile_DTO>>(id.Value);
            
            if (profileResult?.IsSuccess == true && profileResult.Data != null)
            {
                Patient = profileResult.Data;
                _logger.LogInformation("Successfully loaded comprehensive patient profile for {PatientName}", Patient.FullName);
            }
            else
            {
                ErrorMessage = profileResult?.GetError() ?? "Failed to load patient profile";
                HasError = true;
                _logger.LogError("Failed to load patient profile: {Error}", ErrorMessage);
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
