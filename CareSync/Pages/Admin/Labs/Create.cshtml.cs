using CareSync.ApplicationLayer.ApiResult;
using CareSync.Pages.Shared;
using CareSync.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace CareSync.Pages.Admin.Labs;

public class CreateModel : BasePageModel
{
    private readonly ILogger<CreateModel> _logger;
    private readonly AdminApiService _adminApiService;

    public CreateModel(ILogger<CreateModel> logger, AdminApiService adminApiService)
    {
        _logger = logger;
        _adminApiService = adminApiService;
    }

    [BindProperty]
    public CreateLabInput Lab { get; set; } = new();

    public string? ErrorMessage { get; set; }

    public IActionResult OnGet()
    {
        var authResult = RequireRole("Admin");
        if (authResult != null) return authResult;

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

            _logger.LogInformation("Creating new laboratory: {LabName}", Lab.LabName);

            var result = await _adminApiService.CreateLabAsync<Result<GeneralResponse>>(Lab);

            if (result?.IsSuccess == true)
            {
                TempData["SuccessMessage"] = $"Laboratory '{Lab.LabName}' created successfully!";
                return RedirectToPage("/Admin/Labs/ManageLabs");
            }
            else
            {
                ErrorMessage = result?.GetError() ?? "Failed to create laboratory.";
                return Page();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating laboratory");
            ErrorMessage = "An error occurred while creating the laboratory.";
            return Page();
        }
    }

    public class CreateLabInput
    {
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

    public class GeneralResponse
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
    }
}
