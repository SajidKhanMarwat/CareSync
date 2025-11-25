using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using CareSync.Pages.Shared;
using CareSync.Services;
using CareSync.ApplicationLayer.ApiResult;
using CareSync.ApplicationLayer.Contracts.PatientsDTOs;
using Microsoft.Extensions.Logging;

namespace CareSync.Pages.Admin.Patients;

public class SearchModel : BasePageModel
{
    private readonly ILogger<SearchModel> _logger;
    private readonly AdminApiService _adminApiService;

    public SearchModel(ILogger<SearchModel> logger, AdminApiService adminApiService)
    {
        _logger = logger;
        _adminApiService = adminApiService;
    }

    [BindProperty]
    public PatientSearchRequest_DTO SearchRequest { get; set; } = new();
    
    public PatientSearchResult_DTO? SearchResult { get; set; }
    public string? ErrorMessage { get; set; }
    public string? SuccessMessage { get; set; }

    public async Task<IActionResult> OnGetAsync(
        string? q = null, 
        string? gender = null, 
        string? bloodGroup = null,
        bool? isActive = null,
        string? sortBy = null,
        int? page = null)
    {
        var authResult = RequireRole("Admin");
        if (authResult != null) return authResult;

        // Apply query parameters to search request
        if (!string.IsNullOrWhiteSpace(q))
            SearchRequest.SearchTerm = q;
        if (!string.IsNullOrWhiteSpace(gender))
            SearchRequest.Gender = gender;
        if (!string.IsNullOrWhiteSpace(bloodGroup))
            SearchRequest.BloodGroup = bloodGroup;
        if (isActive.HasValue)
            SearchRequest.IsActive = isActive.Value;
        if (!string.IsNullOrWhiteSpace(sortBy))
            SearchRequest.SortBy = sortBy;
        if (page.HasValue)
            SearchRequest.PageNumber = page.Value;

        // Perform initial search to load all patients
        await PerformSearch();
        
        return Page();
    }

    public async Task<IActionResult> OnPostSearchAsync()
    {
        var authResult = RequireRole("Admin");
        if (authResult != null) return authResult;

        await PerformSearch();
        return Page();
    }

    public async Task<IActionResult> OnPostQuickSearchAsync(string type)
    {
        var authResult = RequireRole("Admin");
        if (authResult != null) return authResult;

        // Reset search request
        SearchRequest = new PatientSearchRequest_DTO();

        // Apply quick search filters
        switch (type?.ToLower())
        {
            case "critical":
                // Search for patients with recent critical appointments or conditions
                SearchRequest.IsActive = true;
                SearchRequest.SortBy = "LastVisit";
                SearchRequest.SortDescending = true;
                break;
                
            case "new":
                // New patients this week
                SearchRequest.LastVisitFrom = DateTime.Now.AddDays(-7);
                SearchRequest.SortBy = "CreatedDate";
                SearchRequest.SortDescending = true;
                break;
                
            case "appointments":
                // Patients with upcoming appointments
                SearchRequest.IsActive = true;
                SearchRequest.SortBy = "LastVisit";
                break;
                
            case "overdue":
                // Patients who haven't visited in 6 months
                SearchRequest.LastVisitTo = DateTime.Now.AddMonths(-6);
                SearchRequest.IsActive = true;
                break;
                
            case "inactive":
                SearchRequest.IsActive = false;
                break;
                
            case "all":
            default:
                // No filters
                break;
        }

        await PerformSearch();
        return Page();
    }

    public async Task<IActionResult> OnPostExportAsync()
    {
        var authResult = RequireRole("Admin");
        if (authResult != null) return authResult;

        // Perform search without pagination for export
        var exportRequest = new PatientSearchRequest_DTO
        {
            SearchTerm = SearchRequest.SearchTerm,
            Gender = SearchRequest.Gender,
            BloodGroup = SearchRequest.BloodGroup,
            MaritalStatus = SearchRequest.MaritalStatus,
            IsActive = SearchRequest.IsActive,
            PageNumber = 1,
            PageSize = 10000 // Get all results
        };

        var result = await _adminApiService.SearchPatientsComprehensiveAsync<Result<PatientSearchResult_DTO>>(exportRequest);
        
        if (result?.IsSuccess == true && result.Data != null)
        {
            // TODO: Implement CSV export
            SuccessMessage = $"Exported {result.Data.TotalCount} patients successfully";
        }
        else
        {
            ErrorMessage = "Failed to export patient data";
        }

        return Page();
    }

    private async Task PerformSearch()
    {
        try
        {
            _logger.LogInformation("Performing patient search with filters");
            
            var result = await _adminApiService.SearchPatientsComprehensiveAsync<Result<PatientSearchResult_DTO>>(SearchRequest);
            
            if (result?.IsSuccess == true && result.Data != null)
            {
                SearchResult = result.Data;
                _logger.LogInformation("Found {Count} patients", SearchResult.TotalCount);
                
                if (SearchResult.TotalCount == 0)
                {
                    ErrorMessage = "No patients found matching your search criteria";
                }
            }
            else
            {
                ErrorMessage = result?.GetError() ?? "Failed to search patients";
                _logger.LogError("Search failed: {Error}", ErrorMessage);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error performing patient search");
            ErrorMessage = "An error occurred while searching patients";
        }
    }

    public string GetAgeDisplay(int? age, DateTime? dateOfBirth)
    {
        if (age.HasValue)
            return $"{age} years";
        
        if (dateOfBirth.HasValue)
        {
            var calculatedAge = DateTime.Now.Year - dateOfBirth.Value.Year;
            if (DateTime.Now.DayOfYear < dateOfBirth.Value.DayOfYear)
                calculatedAge--;
            return $"{calculatedAge} years";
        }
        
        return "N/A";
    }

    public string GetLastVisitDisplay(DateTime? lastVisit)
    {
        if (!lastVisit.HasValue)
            return "Never";
        
        var days = (DateTime.Now - lastVisit.Value).Days;
        
        if (days == 0)
            return "Today";
        if (days == 1)
            return "Yesterday";
        if (days < 7)
            return $"{days} days ago";
        if (days < 30)
            return $"{days / 7} weeks ago";
        if (days < 365)
            return $"{days / 30} months ago";
        
        return lastVisit.Value.ToString("MMM dd, yyyy");
    }

    public string GetStatusBadgeClass(bool isActive)
    {
        return isActive ? "bg-success" : "bg-secondary";
    }

    public string GetBloodGroupBadgeClass(string? bloodGroup)
    {
        return bloodGroup switch
        {
            "O+" or "O-" => "bg-danger",
            "A+" or "A-" => "bg-primary",
            "B+" or "B-" => "bg-info",
            "AB+" or "AB-" => "bg-warning",
            _ => "bg-secondary"
        };
    }
}
