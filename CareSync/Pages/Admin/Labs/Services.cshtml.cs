using CareSync.ApplicationLayer.ApiResult;
using CareSync.Pages.Shared;
using CareSync.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace CareSync.Pages.Admin.Labs;

public class ServicesModel : BasePageModel
{
    private readonly ILogger<ServicesModel> _logger;
    private readonly AdminApiService _adminApiService;

    public ServicesModel(ILogger<ServicesModel> logger, AdminApiService adminApiService)
    {
        _logger = logger;
        _adminApiService = adminApiService;
    }

    public List<LabServiceItem>? Services { get; set; }
    public List<LabOption>? AvailableLabs { get; set; }
    
    [BindProperty]
    public LabServiceInput ServiceInput { get; set; } = new();

    public string? SuccessMessage { get; set; }
    public string? ErrorMessage { get; set; }

    // Pagination properties
    public int CurrentPage { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public int TotalPages { get; set; }
    public int TotalCount { get; set; }
    
    // Statistics
    public int TotalServices { get; set; }
    public int ActiveServices { get; set; }
    public int TotalLaboratories { get; set; }
    public decimal AveragePrice { get; set; }
    public Dictionary<string, int> CategoryDistribution { get; set; } = new();
    
    // Filters
    public int? FilterLabId { get; set; }
    public string? FilterCategory { get; set; }
    public string? SearchTerm { get; set; }

    public async Task<IActionResult> OnGetAsync(int? page, int? pageSize, int? labId, string? category, string? search)
    {
        var authResult = RequireRole("Admin");
        if (authResult != null) return authResult;

        if (TempData["SuccessMessage"] != null)
        {
            SuccessMessage = TempData["SuccessMessage"]?.ToString();
        }

        // Validate and set page size (10, 25, 50, 100 only)
        var validPageSizes = new[] { 10, 25, 50, 100 };
        PageSize = pageSize.HasValue && validPageSizes.Contains(pageSize.Value) ? pageSize.Value : 10;

        CurrentPage = page ?? 1;
        FilterLabId = labId;
        FilterCategory = category;
        SearchTerm = search;

        await LoadDataAsync();
        return Page();
    }

    public async Task<IActionResult> OnPostCreateAsync()
    {
        var authResult = RequireRole("Admin");
        if (authResult != null) return authResult;

        try
        {
            if (!ModelState.IsValid)
            {
                await LoadDataAsync();
                ErrorMessage = "Please fill in all required fields correctly.";
                return Page();
            }

            _logger.LogInformation("Creating new lab service: {ServiceName}", ServiceInput.ServiceName);

            var result = await _adminApiService.CreateLabServiceAsync<Result<GeneralResponse>>(ServiceInput);

            if (result?.IsSuccess == true)
            {
                TempData["SuccessMessage"] = $"Service '{ServiceInput.ServiceName}' created successfully!";
                return RedirectToPage();
            }
            else
            {
                await LoadDataAsync();
                ErrorMessage = result?.GetError() ?? "Failed to create service.";
                return Page();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating lab service");
            await LoadDataAsync();
            ErrorMessage = "An error occurred while creating the service.";
            return Page();
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

        return RedirectToPage();
    }

    private async Task LoadDataAsync()
    {
        try
        {
            // Create filter for pagination
            var filter = new LabServicesFilterRequest
            {
                LabId = FilterLabId,
                Category = FilterCategory,
                SearchTerm = SearchTerm,
                Page = CurrentPage,
                PageSize = PageSize
            };

            // Load paged services
            var pagedResult = await _adminApiService.GetLabServicesPagedAsync<Result<LabServicesPagedResponse>>(filter);
            if (pagedResult?.IsSuccess == true && pagedResult.Data != null)
            {
                Services = pagedResult.Data.Items?.Select(s => new LabServiceItem
                {
                    LabServiceId = s.LabServiceId,
                    LabId = s.LabId,
                    ServiceName = s.ServiceName,
                    Description = s.Description,
                    Category = s.Category,
                    SampleType = s.SampleType,
                    Instructions = s.Instructions,
                    Price = s.Price,
                    EstimatedTime = s.EstimatedTime,
                    LabName = s.LabName,
                    IsActive = s.IsActive
                }).ToList() ?? new List<LabServiceItem>();
                
                TotalCount = pagedResult.Data.TotalCount;
                TotalPages = pagedResult.Data.TotalPages;
                TotalServices = pagedResult.Data.TotalServices;
                ActiveServices = pagedResult.Data.ActiveServices;
                TotalLaboratories = pagedResult.Data.TotalLaboratories;
                AveragePrice = pagedResult.Data.AveragePrice;
                CategoryDistribution = pagedResult.Data.CategoryDistribution ?? new();
            }
            else
            {
                Services = new List<LabServiceItem>();
            }

            // Load labs for dropdown
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
            _logger.LogError(ex, "Error loading data");
            Services = new List<LabServiceItem>();
            AvailableLabs = new List<LabOption>();
        }
    }

    public class LabServiceItem
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

    public class LabServiceInput
    {
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

    public class GeneralResponse
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
    }

    public class LabServicesFilterRequest
    {
        public int? LabId { get; set; }
        public string? Category { get; set; }
        public string? SearchTerm { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }

    public class LabServicesPagedResponse
    {
        public List<LabServiceDto>? Items { get; set; }
        public int TotalCount { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
        public int TotalServices { get; set; }
        public int ActiveServices { get; set; }
        public int TotalLaboratories { get; set; }
        public decimal AveragePrice { get; set; }
        public Dictionary<string, int>? CategoryDistribution { get; set; }
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
}
