using CareSync.ApplicationLayer.ApiResult;
using CareSync.ApplicationLayer.Contracts.DoctorsDTOs;
using CareSync.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CareSync.Pages.Admin.Doctors;

[Authorize(Roles = "Admin")]
public class DoctorProfileModel : PageModel
{
    private readonly AdminApiService _adminApiService;
    private readonly ILogger<DoctorProfileModel> _logger;

    public DoctorProfileModel(AdminApiService adminApiService, ILogger<DoctorProfileModel> logger)
    {
        _adminApiService = adminApiService;
        _logger = logger;
    }

    [BindProperty(SupportsGet = true)]
    public string Id { get; set; } = string.Empty;

    public DoctorProfile_DTO? Doctor { get; set; }

    public async Task<IActionResult> OnGetAsync()
    {
        if (string.IsNullOrEmpty(Id))
        {
            TempData["ErrorMessage"] = "Doctor ID is required.";
            return RedirectToPage("/Admin/Doctors/DoctorsList");
        }

        try
        {
            var result = await _adminApiService.GetDoctorProfileAsync<Result<DoctorProfile_DTO>>(Id);
            if (result?.IsSuccess == true && result.Data != null)
            {
                Doctor = result.Data;
            }
            else
            {
                TempData["ErrorMessage"] = result?.GetError() ?? "Failed to load doctor profile.";
                return RedirectToPage("/Admin/Doctors/DoctorsList");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading doctor profile for ID: {Id}", Id);
            TempData["ErrorMessage"] = "An error occurred while loading the doctor profile.";
            return RedirectToPage("/Admin/Doctors/DoctorsList");
        }

        return Page();
    }
}
