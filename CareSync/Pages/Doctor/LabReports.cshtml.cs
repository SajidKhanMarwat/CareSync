using CareSync.ApplicationLayer.Contracts.DoctorsDTOs;
using CareSync.Pages.Shared;
using CareSync.Services;
using Microsoft.AspNetCore.Mvc;

namespace CareSync.Pages.Doctor;

public class LabReportsModel : BasePageModel
{
    private readonly ILogger<LabReportsModel> _logger;
    private readonly DoctorApiService _doctorApiService;

    public LabReportsModel(ILogger<LabReportsModel> logger, DoctorApiService doctorApiService)
    {
        _logger = logger;
        _doctorApiService = doctorApiService;
    }

    public List<DoctorLabReport_DTO> Reports { get; set; } = new();
    public string? ErrorMessage { get; set; }

    public async Task<IActionResult> OnGetAsync()
    {
        var authResult = RequireRole("Doctor");
        if (authResult != null) return authResult;

        try
        {
            var result = await _doctorApiService.GetDoctorLabReportsAsync();
            if (result == null || !result.IsSuccess || result.Data == null)
            {
                ErrorMessage = result?.GetError() ?? "Unable to load lab reports.";
                return Page();
            }

            Reports = result.Data
                .OrderByDescending(r => r.ReviewedDate ?? DateTime.MinValue)
                .ToList();

            return Page();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading doctor lab reports page");
            ErrorMessage = "An error occurred while loading lab reports.";
            return Page();
        }
    }
}
