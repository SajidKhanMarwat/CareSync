using CareSync.ApplicationLayer.ApiResult;
using CareSync.Pages.Shared;
using CareSync.Services;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace CareSync.Pages.Admin.Labs;

public class EditModel : BasePageModel
{
    private readonly ILogger<EditModel> _logger;
    private readonly AdminApiService _adminApiService;

    public EditModel(ILogger<EditModel> logger, AdminApiService adminApiService)
    {
        _logger = logger;
        _adminApiService = adminApiService;
    }

    [BindProperty]
    public UpdateLabInput Lab { get; set; } = new();

    public string? ErrorMessage { get; set; }

    public async Task<IActionResult> OnGetAsync(int labId)
    {
        var authResult = RequireRole("Admin");
        if (authResult != null) return authResult;

        try
        {
            _logger.LogInformation("Loading lab for editing: LabId {LabId}", labId);
            
            var result = await _adminApiService.GetLabByIdAsync<Result<LabDetailsDto>>(labId);
            
            if (result?.IsSuccess == true && result.Data != null)
            {
                Lab = new UpdateLabInput
                {
                    LabId = result.Data.LabId,
                    LabName = result.Data.LabName ?? string.Empty,
                    ArabicLabName = result.Data.ArabicLabName,
                    LabAddress = result.Data.LabAddress,
                    ArabicLabAddress = result.Data.ArabicLabAddress,
                    Location = result.Data.Location,
                    ContactNumber = result.Data.ContactNumber,
                    Email = result.Data.Email,
                    LicenseNumber = result.Data.LicenseNumber,
                    OpeningTime = result.Data.OpeningTime,
                    ClosingTime = result.Data.ClosingTime
                };
            }
            else
            {
                ErrorMessage = result?.GetError() ?? "Failed to load laboratory.";
                return RedirectToPage("/Admin/Labs/ManageLabs");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading lab for editing: LabId {LabId}", labId);
            ErrorMessage = "An error occurred while loading laboratory.";
            return RedirectToPage("/Admin/Labs/ManageLabs");
        }

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        try
        {
            var authResult = RequireRole("Admin");
            if (authResult != null) return authResult;

            if (!ModelState.IsValid)
            {
                ErrorMessage = "Please fill in all required fields correctly.";
                return Page();
            }

            _logger.LogInformation("Updating laboratory: LabId {LabId}", Lab.LabId);

            var result = await _adminApiService.UpdateLabAsync<Result<GeneralResponse>>(Lab.LabId, Lab);

            if (result?.IsSuccess == true)
            {
                TempData["SuccessMessage"] = $"Laboratory '{Lab.LabName}' updated successfully!";
                return RedirectToPage("/Admin/Labs/Details", new { labId = Lab.LabId });
            }
            else
            {
                ErrorMessage = result?.GetError() ?? "Failed to update laboratory.";
                return Page();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating laboratory");
            ErrorMessage = "An error occurred while updating the laboratory.";
            return Page();
        }
    }

    public class UpdateLabInput
    {
        public int LabId { get; set; }

        [Required(ErrorMessage = "Lab name is required")]
        [StringLength(200, ErrorMessage = "Lab name cannot exceed 200 characters")]
        [Display(Name = "Laboratory Name")]
        public string LabName { get; set; } = string.Empty;

        [StringLength(200, ErrorMessage = "Arabic lab name cannot exceed 200 characters")]
        [Display(Name = "Arabic Lab Name")]
        public string? ArabicLabName { get; set; }

        [StringLength(500, ErrorMessage = "Address cannot exceed 500 characters")]
        [Display(Name = "Lab Address")]
        public string? LabAddress { get; set; }

        [StringLength(500, ErrorMessage = "Arabic address cannot exceed 500 characters")]
        [Display(Name = "Arabic Lab Address")]
        public string? ArabicLabAddress { get; set; }

        [StringLength(100, ErrorMessage = "Location cannot exceed 100 characters")]
        [Display(Name = "Location / City")]
        public string? Location { get; set; }

        [Phone(ErrorMessage = "Please enter a valid phone number")]
        [StringLength(20, ErrorMessage = "Phone number cannot exceed 20 characters")]
        [Display(Name = "Contact Number")]
        public string? ContactNumber { get; set; }

        [EmailAddress(ErrorMessage = "Please enter a valid email address")]
        [StringLength(100, ErrorMessage = "Email cannot exceed 100 characters")]
        [Display(Name = "Email Address")]
        public string? Email { get; set; }

        [StringLength(50, ErrorMessage = "License number cannot exceed 50 characters")]
        [Display(Name = "License Number")]
        public string? LicenseNumber { get; set; }

        [Display(Name = "Opening Time")]
        public TimeSpan? OpeningTime { get; set; }

        [Display(Name = "Closing Time")]
        public TimeSpan? ClosingTime { get; set; }
    }

    public class LabDetailsDto
    {
        public int LabId { get; set; }
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
    }

    public class GeneralResponse
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
    }
}
