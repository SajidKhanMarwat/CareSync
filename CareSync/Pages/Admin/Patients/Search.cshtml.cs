using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using CareSync.Pages.Shared;
using CareSync.Services;
using CareSync.ApplicationLayer.ApiResult;
using CareSync.ApplicationLayer.Contracts.PatientsDTOs;

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

    public List<PatientSearch_DTO> SearchResults { get; set; } = new();
    public string? ErrorMessage { get; set; }
    public bool HasError { get; set; }
    public bool HasSearched { get; set; }

    [BindProperty(SupportsGet = true)]
    public string? SearchTerm { get; set; }

    public async Task<IActionResult> OnGetAsync()
    {
        var authResult = RequireRole("Admin");
        if (authResult != null) return authResult;

        if (!string.IsNullOrWhiteSpace(SearchTerm))
        {
            await PerformSearchAsync();
        }

        return Page();
    }

    public async Task<IActionResult> OnPostSearchAsync()
    {
        var authResult = RequireRole("Admin");
        if (authResult != null) return authResult;

        if (string.IsNullOrWhiteSpace(SearchTerm))
        {
            ErrorMessage = "Please enter a search term.";
            HasError = true;
            return Page();
        }

        await PerformSearchAsync();
        return Page();
    }

    private async Task PerformSearchAsync()
    {
        try
        {
            HasSearched = true;
            _logger.LogInformation("Searching patients with term: {SearchTerm}", SearchTerm);

            var result = await _adminApiService.SearchPatientsAsync<Result<List<PatientSearch_DTO>>>(SearchTerm!);

            if (result?.IsSuccess == true && result.Data != null)
            {
                SearchResults = result.Data;
                _logger.LogInformation("Found {Count} patients matching search term", SearchResults.Count);
            }
            else
            {
                ErrorMessage = result?.GetError() ?? "Search failed";
                HasError = true;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching patients");
            ErrorMessage = "An error occurred while searching. Please try again.";
            HasError = true;
        }
    }
}
