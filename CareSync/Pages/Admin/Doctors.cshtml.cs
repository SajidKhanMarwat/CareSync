using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using CareSync.Pages.Shared;
using CareSync.Services;
using CareSync.ApplicationLayer.ApiResult;
using CareSync.ApplicationLayer.Contracts.DoctorsDTOs;

namespace CareSync.Pages.Admin;

public class DoctorsModel : BasePageModel
{
    private readonly ILogger<DoctorsModel> _logger;
    private readonly AdminApiService _adminApiService;

    public DoctorsModel(ILogger<DoctorsModel> logger, AdminApiService adminApiService)
    {
        _logger = logger;
        _adminApiService = adminApiService;
    }

    public List<DoctorList_DTO> Doctors { get; set; } = new();
    public DoctorStats_DTO? DoctorStats { get; set; }
    public string? ErrorMessage { get; set; }
    public bool HasError { get; set; }

    [BindProperty(SupportsGet = true)]
    public string? Specialization { get; set; }

    [BindProperty(SupportsGet = true)]
    public bool? IsActive { get; set; }

    public async Task<IActionResult> OnGetAsync()
    {
        try
        {
            // Check authentication
            var authResult = RequireRole("Admin");
            if (authResult != null) return authResult;

            _logger.LogInformation("Loading doctors list with filters: Specialization={Specialization}, IsActive={IsActive}", 
                Specialization, IsActive);

            // Load doctors and stats in parallel
            var doctorsTask = _adminApiService.GetAllDoctorsAsync<Result<List<DoctorList_DTO>>>(Specialization, IsActive);
            var statsTask = _adminApiService.GetDoctorStatsAsync<Result<DoctorStats_DTO>>();

            await Task.WhenAll(doctorsTask, statsTask);

            var doctorsResult = await doctorsTask;
            if (doctorsResult?.IsSuccess == true && doctorsResult.Data != null)
                Doctors = doctorsResult.Data;

            var statsResult = await statsTask;
            if (statsResult?.IsSuccess == true && statsResult.Data != null)
                DoctorStats = statsResult.Data;

            _logger.LogInformation("Loaded {Count} doctors", Doctors.Count);
            return Page();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading doctors");
            ErrorMessage = "Failed to load doctors list. Please try again.";
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

            _logger.LogInformation("Toggling doctor status: UserId={UserId}, NewStatus={IsActive}", userId, isActive);

            // Call API to toggle status
            var client = new HttpClient { BaseAddress = new Uri("http://localhost:5157/api/") };
            var token = HttpContext.Session.GetString("UserToken");
            if (!string.IsNullOrEmpty(token))
                client.DefaultRequestHeaders.Authorization = 
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var response = await client.PatchAsync($"Admin/doctors/{userId}/toggle-status?isActive={isActive}", null);
            
            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = $"Doctor status updated successfully.";
            }
            else
            {
                TempData["ErrorMessage"] = "Failed to update doctor status.";
            }

            return RedirectToPage();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error toggling doctor status");
            TempData["ErrorMessage"] = "An error occurred while updating doctor status.";
            return RedirectToPage();
        }
    }
}
