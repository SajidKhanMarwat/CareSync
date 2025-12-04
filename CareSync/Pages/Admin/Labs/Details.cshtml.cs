using CareSync.ApplicationLayer.ApiResult;
using CareSync.Pages.Shared;
using CareSync.Services;
using Microsoft.AspNetCore.Mvc;

namespace CareSync.Pages.Admin.Labs;

public class DetailsModel : BasePageModel
{
    private readonly ILogger<DetailsModel> _logger;
    private readonly AdminApiService _adminApiService;

    public DetailsModel(ILogger<DetailsModel> logger, AdminApiService adminApiService)
    {
        _logger = logger;
        _adminApiService = adminApiService;
    }

    public LabDetailsViewModel? Lab { get; set; }
    public string? ErrorMessage { get; set; }

    public async Task<IActionResult> OnGetAsync(int labId)
    {
        var authResult = RequireRole("Admin");
        if (authResult != null) return authResult;

        try
        {
            _logger.LogInformation("Loading details for LabId: {LabId}", labId);
            
            var result = await _adminApiService.GetLabByIdAsync<Result<LabDetailsViewModel>>(labId);
            
            if (result?.IsSuccess == true && result.Data != null)
            {
                Lab = result.Data;
            }
            else
            {
                ErrorMessage = result?.GetError() ?? "Failed to load laboratory details.";
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading lab details for LabId: {LabId}", labId);
            ErrorMessage = "An error occurred while loading laboratory details.";
        }

        return Page();
    }

    public class LabDetailsViewModel
    {
        public int LabId { get; set; }
        public Guid? UserId { get; set; }
        public string? LabName { get; set; }
        public string? ArabicLabName { get; set; }
        public string? LabAddress { get; set; }
        public string? ArabicLabAddress { get; set; }
        public string? Location { get; set; }
        public string? ContactNumber { get; set; }
        public string? Email { get; set; }
        public string? LicenseNumber { get; set; }
        public TimeSpan? OpeningTime { get; set; }
        public TimeSpan? ClosingTime { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? CreatedBy { get; set; }
        public int ServicesCount { get; set; }
        public int AssistantsCount { get; set; }
        public bool IsActive { get; set; }
    }
}
