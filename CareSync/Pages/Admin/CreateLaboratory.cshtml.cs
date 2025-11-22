using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using CareSync.Pages.Shared;
using CareSync.Services;
using CareSync.ApplicationLayer.ApiResult;
using CareSync.ApplicationLayer.Contracts.UsersDTOs;
using CareSync.ApplicationLayer.Common;
using CareSync.Shared.Enums;

namespace CareSync.Pages.Admin;

public class CreateLaboratoryModel : BasePageModel
{
    private readonly ILogger<CreateLaboratoryModel> _logger;
    private readonly AdminApiService _adminApiService;

    public CreateLaboratoryModel(ILogger<CreateLaboratoryModel> logger, AdminApiService adminApiService)
    {
        _logger = logger;
        _adminApiService = adminApiService;
    }

    [BindProperty]
    public UserRegisteration_DTO LabRegistration { get; set; } = new()
    {
        ArabicUserName = "",
        FirstName = "",
        ArabicFirstName = "",
        Email = "",
        Password = "",
        ConfirmPassword = "",
        Gender = Gender_Enum.Male,
        RoleType = CareSync.Shared.Enums.RoleType.Lab
    };

    public string? SuccessMessage { get; set; }
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

            if (LabRegistration.Password != LabRegistration.ConfirmPassword)
            {
                ErrorMessage = "Password and confirm password do not match.";
                return Page();
            }

            _logger.LogInformation("Registering new lab staff: {Email}", LabRegistration.Email);

            // Ensure role is set to Lab
            LabRegistration.RoleType = CareSync.Shared.Enums.RoleType.Lab;

            var result = await _adminApiService.RegisterLabAsync<Result<GeneralResponse>>(LabRegistration);

            if (result?.IsSuccess == true)
            {
                TempData["SuccessMessage"] = $"Lab staff {LabRegistration.FirstName} registered successfully!";
                return RedirectToPage("/Admin/MedicalStaff");
            }
            else
            {
                ErrorMessage = result?.GetError() ?? "Failed to register lab staff.";
                return Page();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error registering lab staff");
            ErrorMessage = "An error occurred while registering the lab staff.";
            return Page();
        }
    }
}
