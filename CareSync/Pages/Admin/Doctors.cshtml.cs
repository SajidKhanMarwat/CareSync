using CareSync.ApplicationLayer.ApiResult;
using CareSync.ApplicationLayer.Common;
using CareSync.ApplicationLayer.Contracts.DoctorsDTOs;
using CareSync.Pages.Shared;
using CareSync.Services;
using Microsoft.AspNetCore.Mvc;

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

    // Data properties
    public DoctorInsights_DTO? DoctorInsights { get; set; }
    public List<DoctorGridItem_DTO> DoctorGrid { get; set; } = new();
    public DoctorStatisticsSummary_DTO? Statistics { get; set; }
    public List<SpecializationDistribution_DTO> Specializations { get; set; } = new();
    public List<DoctorPerformance_DTO> TopPerformers { get; set; } = new();
    public DoctorAvailabilityOverview_DTO? Availability { get; set; }

    // Legacy properties (for backward compatibility)
    public List<DoctorList_DTO> Doctors { get; set; } = new();
    public DoctorStats_DTO? DoctorStats { get; set; }

    // UI properties
    public string? ErrorMessage { get; set; }
    public bool HasError { get; set; }
    public int TotalPages { get; set; }
    public int CurrentPage { get; set; } = 1;

    [BindProperty(SupportsGet = true)]
    public string? Specialization { get; set; }

    [BindProperty(SupportsGet = true)]
    public bool? IsActive { get; set; }

    [BindProperty(SupportsGet = true)]
    public new int Page { get; set; } = 1;

    [BindProperty(SupportsGet = true)]
    public int PageSize { get; set; } = 10;

    public async Task<IActionResult> OnGetAsync()
    {
        try
        {
            // Check authentication
            var authResult = RequireRole("Admin");
            if (authResult != null) return authResult;

            _logger.LogInformation("Loading doctors page with filters: Specialization={Specialization}, IsActive={IsActive}, Page={Page}",
                Specialization, IsActive, Page);

            CurrentPage = Page;

            // Load all data in parallel for better performance
            var tasks = new List<Task>();

            // Primary data - Doctor Insights
            var insightsTask = LoadDoctorInsightsAsync();
            tasks.Add(insightsTask);

            // Grid data with pagination
            var gridTask = LoadDoctorGridAsync();
            tasks.Add(gridTask);

            // Legacy support - load old format data
            var legacyTask = LoadLegacyDoctorDataAsync();
            tasks.Add(legacyTask);

            await Task.WhenAll(tasks);

            // Extract data from insights if loaded successfully
            if (DoctorInsights != null)
            {
                Statistics = DoctorInsights.Statistics;
                Specializations = DoctorInsights.Specializations;
                TopPerformers = DoctorInsights.TopPerformers;
                Availability = DoctorInsights.Availability;
            }

            _logger.LogInformation("Successfully loaded doctors page data");
            return Page();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading doctors page");
            ErrorMessage = "Failed to load doctors data. Please try again.";
            HasError = true;
            return Page();
        }
    }

    private async Task LoadDoctorInsightsAsync()
    {
        try
        {
            var result = await _adminApiService.GetDoctorInsightsAsync<Result<DoctorInsights_DTO>>();
            if (result?.IsSuccess == true && result.Data != null)
                DoctorInsights = result.Data;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading doctor insights");
        }
    }

    private async Task LoadDoctorGridAsync()
    {
        try
        {
            var result = await _adminApiService.GetDoctorGridDataAsync<Result<List<DoctorGridItem_DTO>>>(
                Specialization, IsActive, Page, PageSize);

            if (result?.IsSuccess == true && result.Data != null)
            {
                DoctorGrid = result.Data;
                // Calculate total pages (this is approximate, should come from API)
                TotalPages = Math.Max(1, (DoctorGrid.Count == PageSize ? Page + 1 : Page));
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading doctor grid data");
        }
    }

    private async Task LoadLegacyDoctorDataAsync()
    {
        try
        {
            // Load legacy format for backward compatibility
            var doctorsTask = _adminApiService.GetAllDoctorsAsync<Result<List<DoctorList_DTO>>>(Specialization, IsActive);
            var statsTask = _adminApiService.GetDoctorStatsAsync<Result<DoctorStats_DTO>>();

            await Task.WhenAll(doctorsTask, statsTask);

            var doctorsResult = await doctorsTask;
            if (doctorsResult?.IsSuccess == true && doctorsResult.Data != null)
                Doctors = doctorsResult.Data;

            var statsResult = await statsTask;
            if (statsResult?.IsSuccess == true && statsResult.Data != null)
                DoctorStats = statsResult.Data;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading legacy doctor data");
        }
    }

    public async Task<IActionResult> OnPostToggleStatusAsync(string userId, bool isActive)
    {
        try
        {
            var authResult = RequireRole("Admin");
            if (authResult != null) return authResult;

            _logger.LogInformation("Toggling doctor status: UserId={UserId}, NewStatus={IsActive}", userId, isActive);

            // Use AdminApiService to toggle status
            var result = await _adminApiService.ToggleDoctorStatusAsync<Result<GeneralResponse>>(userId, isActive);

            if (result?.IsSuccess == true)
            {
                TempData["SuccessMessage"] = $"Doctor status updated successfully.";
            }
            else
            {
                TempData["ErrorMessage"] = result?.Data?.Message ?? "Failed to update doctor status.";
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
