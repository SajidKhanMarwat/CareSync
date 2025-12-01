using CareSync.ApplicationLayer.ApiResult;
using CareSync.Pages.Shared;
using CareSync.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CareSync.Pages.Admin.Labs;

public class ManageLabsModel : BasePageModel
{
    private readonly ILogger<ManageLabsModel> _logger;
    private readonly AdminApiService _adminApiService;

    public ManageLabsModel(ILogger<ManageLabsModel> logger, AdminApiService adminApiService)
    {
        _logger = logger;
        _adminApiService = adminApiService;
    }

    public List<LabListItem>? Labs { get; set; }
    public string? SuccessMessage { get; set; }
    public string? ErrorMessage { get; set; }

    public async Task<IActionResult> OnGetAsync()
    {
        var authResult = RequireRole("Admin");
        if (authResult != null) return authResult;

        // Check for success message from TempData
        if (TempData["SuccessMessage"] != null)
        {
            SuccessMessage = TempData["SuccessMessage"]?.ToString();
        }

        await LoadLabsAsync();
        return Page();
    }

    public async Task<IActionResult> OnPostDeleteAsync(int labId)
    {
        var authResult = RequireRole("Admin");
        if (authResult != null) return authResult;

        try
        {
            var result = await _adminApiService.DeleteLabAsync<Result<GeneralResponse>>(labId);
            
            if (result?.IsSuccess == true)
            {
                TempData["SuccessMessage"] = "Laboratory deleted successfully.";
            }
            else
            {
                TempData["ErrorMessage"] = result?.GetError() ?? "Failed to delete laboratory.";
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting lab");
            TempData["ErrorMessage"] = "An error occurred while deleting the laboratory.";
        }

        return RedirectToPage();
    }

    private async Task LoadLabsAsync()
    {
        try
        {
            var result = await _adminApiService.GetAllLabsAsync<Result<List<LabListItem>>>();
            if (result?.IsSuccess == true && result.Data != null)
            {
                Labs = result.Data;
            }
            else
            {
                Labs = new List<LabListItem>();
                ErrorMessage = "Failed to load laboratories.";
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading labs");
            Labs = new List<LabListItem>();
            ErrorMessage = "An error occurred while loading laboratories.";
        }
    }

    public class LabListItem
    {
        public int LabId { get; set; }
        public string? LabName { get; set; }
        public string? ArabicLabName { get; set; }
        public string? Location { get; set; }
        public string? ContactNumber { get; set; }
        public string? Email { get; set; }
        public bool IsActive { get; set; }
    }

    public class GeneralResponse
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
    }
}
