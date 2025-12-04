using CareSync.ApplicationLayer.ApiResult;
using CareSync.Pages.Shared;
using CareSync.Services;
using Microsoft.AspNetCore.Mvc;

namespace CareSync.Pages.Admin.Labs;

public class ServiceDetailsModel : BasePageModel
{
    private readonly ILogger<ServiceDetailsModel> _logger;
    private readonly AdminApiService _adminApiService;

    public ServiceDetailsModel(ILogger<ServiceDetailsModel> logger, AdminApiService adminApiService)
    {
        _logger = logger;
        _adminApiService = adminApiService;
    }

    public LabServiceDetail? Service { get; set; }

    public async Task<IActionResult> OnGetAsync(int serviceId)
    {
        var authResult = RequireRole("Admin");
        if (authResult != null) return authResult;

        try
        {
            _logger.LogInformation("Loading service details for ServiceId: {ServiceId}", serviceId);

            // Get all services and find the specific one
            var servicesResult = await _adminApiService.GetAllLabServicesAsync<Result<List<LabServiceDetail>>>();
            
            if (servicesResult?.IsSuccess == true && servicesResult.Data != null)
            {
                Service = servicesResult.Data.FirstOrDefault(s => s.LabServiceId == serviceId);
                
                if (Service == null)
                {
                    TempData["ErrorMessage"] = "Service not found.";
                    return RedirectToPage("/Admin/Labs/Services");
                }
            }
            else
            {
                TempData["ErrorMessage"] = "Unable to load service details.";
                return RedirectToPage("/Admin/Labs/Services");
            }

            return Page();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading service details");
            TempData["ErrorMessage"] = "An error occurred while loading service details.";
            return RedirectToPage("/Admin/Labs/Services");
        }
    }

    public async Task<IActionResult> OnPostDeleteAsync(int serviceId)
    {
        var authResult = RequireRole("Admin");
        if (authResult != null) return authResult;

        try
        {
            var result = await _adminApiService.DeleteLabServiceAsync<Result<GeneralResponse>>(serviceId);
            
            if (result?.IsSuccess == true)
            {
                TempData["SuccessMessage"] = "Service deleted successfully.";
            }
            else
            {
                TempData["ErrorMessage"] = result?.GetError() ?? "Failed to delete service.";
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting service");
            TempData["ErrorMessage"] = "An error occurred while deleting the service.";
        }

        return RedirectToPage("/Admin/Labs/Services");
    }

    public class LabServiceDetail
    {
        public int LabServiceId { get; set; }
        public int LabId { get; set; }
        public string ServiceName { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? Category { get; set; }
        public string? SampleType { get; set; }
        public string? Instructions { get; set; }
        public decimal? Price { get; set; }
        public string? EstimatedTime { get; set; }
        public string? LabName { get; set; }
        public bool IsActive { get; set; }
    }

    public class GeneralResponse
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
    }
}
