using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using CareSync.Pages.Shared;
using CareSync.Services;
using CareSync.ApplicationLayer.ApiResult;
using CareSync.ApplicationLayer.Common;
using CareSync.ApplicationLayer.Contracts.PatientsDTOs;

namespace CareSync.Pages.Admin;

public class PatientsModel : BasePageModel
{
    private readonly ILogger<PatientsModel> _logger;
    private readonly AdminApiService _adminApiService;

    public PatientsModel(ILogger<PatientsModel> logger, AdminApiService adminApiService)
    {
        _logger = logger;
        _adminApiService = adminApiService;
    }

    public List<PatientList_DTO> Patients { get; set; } = new();
    public PatientStats_DTO? PatientStats { get; set; }
    public string? ErrorMessage { get; set; }
    public bool HasError { get; set; }

    [BindProperty(SupportsGet = true)]
    public string? BloodGroup { get; set; }

    [BindProperty(SupportsGet = true)]
    public bool? IsActive { get; set; }

    public async Task<IActionResult> OnGetAsync()
    {
        try
        {
            var authResult = RequireRole("Admin");
            if (authResult != null) return authResult;

            _logger.LogInformation("Loading patients list with filters: BloodGroup={BloodGroup}, IsActive={IsActive}", 
                BloodGroup, IsActive);

            // Load patients and stats in parallel
            var patientsTask = _adminApiService.GetAllPatientsAsync<Result<List<PatientList_DTO>>>(BloodGroup, IsActive);
            var statsTask = _adminApiService.GetPatientStatsAsync<Result<PatientStats_DTO>>();

            await Task.WhenAll(patientsTask, statsTask);

            var patientsResult = await patientsTask;
            if (patientsResult?.IsSuccess == true && patientsResult.Data != null)
                Patients = patientsResult.Data;

            var statsResult = await statsTask;
            if (statsResult?.IsSuccess == true && statsResult.Data != null)
                PatientStats = statsResult.Data;

            _logger.LogInformation("Loaded {Count} patients", Patients.Count);
            return Page();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading patients");
            ErrorMessage = "Failed to load patients list. Please try again.";
            HasError = true;
            return Page();
        }
    }

    public async Task<IActionResult> OnPostToggleStatusAsync(string userId, bool isActive)
    {
        try
        {
            var authResult = RequireRole("Admin");
            if (authResult != null) return authResult;

            _logger.LogInformation("Toggling patient status: UserId={UserId}, NewStatus={IsActive}", userId, isActive);

            // Use AdminApiService to toggle status
            var result = await _adminApiService.TogglePatientStatusAsync<Result<GeneralResponse>>(userId, isActive);
            
            if (result?.IsSuccess == true)
            {
                TempData["SuccessMessage"] = "Patient status updated successfully.";
            }
            else
            {
                TempData["ErrorMessage"] = result?.Data?.Message ?? "Failed to update patient status.";
            }

            return RedirectToPage();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error toggling patient status");
            TempData["ErrorMessage"] = "An error occurred while updating patient status.";
            return RedirectToPage();
        }
    }
}
