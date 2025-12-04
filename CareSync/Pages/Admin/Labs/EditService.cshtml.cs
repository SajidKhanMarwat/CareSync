using CareSync.ApplicationLayer.ApiResult;
using CareSync.Pages.Shared;
using CareSync.Services;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace CareSync.Pages.Admin.Labs;

public class EditServiceModel : BasePageModel
{
    private readonly ILogger<EditServiceModel> _logger;
    private readonly AdminApiService _adminApiService;

    public EditServiceModel(ILogger<EditServiceModel> logger, AdminApiService adminApiService)
    {
        _logger = logger;
        _adminApiService = adminApiService;
    }

    [BindProperty]
    public LabServiceInput ServiceInput { get; set; } = new();
    
    public List<LabOption>? AvailableLabs { get; set; }
    public string? ErrorMessage { get; set; }

    public async Task<IActionResult> OnGetAsync(int serviceId)
    {
        var authResult = RequireRole("Admin");
        if (authResult != null) return authResult;

        try
        {
            _logger.LogInformation("Loading service for edit: ServiceId {ServiceId}", serviceId);

            // Load labs for dropdown
            await LoadLabsAsync();

            // Get all services and find the specific one
            var servicesResult = await _adminApiService.GetAllLabServicesAsync<Result<List<LabServiceDto>>>();
            
            if (servicesResult?.IsSuccess == true && servicesResult.Data != null)
            {
                var service = servicesResult.Data.FirstOrDefault(s => s.LabServiceId == serviceId);
                
                if (service == null)
                {
                    TempData["ErrorMessage"] = "Service not found.";
                    return RedirectToPage("/Admin/Labs/Services");
                }

                // Map to input model
                ServiceInput = new LabServiceInput
                {
                    LabServiceId = service.LabServiceId,
                    LabId = service.LabId,
                    ServiceName = service.ServiceName,
                    Description = service.Description,
                    Category = service.Category,
                    SampleType = service.SampleType,
                    Instructions = service.Instructions,
                    Price = service.Price,
                    EstimatedTime = service.EstimatedTime
                };
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
            _logger.LogError(ex, "Error loading service for edit");
            TempData["ErrorMessage"] = "An error occurred while loading service details.";
            return RedirectToPage("/Admin/Labs/Services");
        }
    }

    public async Task<IActionResult> OnPostAsync()
    {
        var authResult = RequireRole("Admin");
        if (authResult != null) return authResult;

        try
        {
            if (!ModelState.IsValid)
            {
                await LoadLabsAsync();
                ErrorMessage = "Please fill in all required fields correctly.";
                return Page();
            }

            _logger.LogInformation("Updating lab service: {ServiceName} (ID: {ServiceId})", 
                ServiceInput.ServiceName, ServiceInput.LabServiceId);

            var result = await _adminApiService.UpdateLabServiceAsync<Result<GeneralResponse>>(
                ServiceInput.LabServiceId, ServiceInput);

            if (result?.IsSuccess == true)
            {
                TempData["SuccessMessage"] = $"Service '{ServiceInput.ServiceName}' updated successfully!";
                return RedirectToPage("/Admin/Labs/Services");
            }
            else
            {
                await LoadLabsAsync();
                ErrorMessage = result?.GetError() ?? "Failed to update service.";
                return Page();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating lab service");
            await LoadLabsAsync();
            ErrorMessage = "An error occurred while updating the service.";
            return Page();
        }
    }

    private async Task LoadLabsAsync()
    {
        try
        {
            var labsResult = await _adminApiService.GetAllLabsAsync<Result<List<LabOption>>>();
            if (labsResult?.IsSuccess == true && labsResult.Data != null)
            {
                AvailableLabs = labsResult.Data;
            }
            else
            {
                AvailableLabs = new List<LabOption>();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading labs");
            AvailableLabs = new List<LabOption>();
        }
    }

    public class LabServiceInput
    {
        public int LabServiceId { get; set; }

        [Required(ErrorMessage = "Laboratory is required")]
        public int LabId { get; set; }

        [Required(ErrorMessage = "Service name is required")]
        [StringLength(200, ErrorMessage = "Service name cannot exceed 200 characters")]
        public string ServiceName { get; set; } = string.Empty;

        [StringLength(1000, ErrorMessage = "Description cannot exceed 1000 characters")]
        public string? Description { get; set; }

        [StringLength(100, ErrorMessage = "Category cannot exceed 100 characters")]
        public string? Category { get; set; }

        [StringLength(100, ErrorMessage = "Sample type cannot exceed 100 characters")]
        public string? SampleType { get; set; }

        [StringLength(500, ErrorMessage = "Instructions cannot exceed 500 characters")]
        public string? Instructions { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Price must be a positive value")]
        public decimal? Price { get; set; }

        [StringLength(50, ErrorMessage = "Estimated time cannot exceed 50 characters")]
        public string? EstimatedTime { get; set; }
    }

    public class LabServiceDto
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

    public class LabOption
    {
        public int LabId { get; set; }
        public string? LabName { get; set; }
        public string? Location { get; set; }
    }

    public class GeneralResponse
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
    }
}
