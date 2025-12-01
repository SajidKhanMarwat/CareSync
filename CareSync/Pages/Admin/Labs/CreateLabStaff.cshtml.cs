using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using CareSync.Pages.Shared;
using CareSync.Services;
using CareSync.ApplicationLayer.ApiResult;
using CareSync.ApplicationLayer.Contracts.UsersDTOs;
using CareSync.ApplicationLayer.Contracts.LabDTOs;
using CareSync.ApplicationLayer.Common;
using CareSync.Shared.Enums;

namespace CareSync.Pages.Admin.Labs;

public class CreateLabStaffModel : BasePageModel
{
    private readonly ILogger<CreateLabStaffModel> _logger;
    private readonly AdminApiService _adminApiService;

    public CreateLabStaffModel(ILogger<CreateLabStaffModel> logger, AdminApiService adminApiService)
    {
        _logger = logger;
        _adminApiService = adminApiService;
    }

    [BindProperty]
    public UserRegisteration_DTO LabStaffRegistration { get; set; } = new()
    {
        ArabicUserName = "",
        FirstName = "",
        ArabicFirstName = "",
        Email = "",
        Password = "",
        ConfirmPassword = "",
        Gender = Gender_Enum.Male,
        RoleType = RoleType.LabAssistant
    };

    [BindProperty]
    public RegisterLab_DTO? LabDetails { get; set; }

    [BindProperty]
    public int? SelectedLabId { get; set; }

    public List<LabOption>? AvailableLabs { get; set; }
    public string? SuccessMessage { get; set; }
    public string? ErrorMessage { get; set; }

    public class LabOption
    {
        public int LabId { get; set; }
        public string LabName { get; set; } = string.Empty;
        public string? Location { get; set; }
    }

    public async Task<IActionResult> OnGetAsync()
    {
        var authResult = RequireRole("Admin");
        if (authResult != null) return authResult;

        // Load available labs for Lab Assistant selection
        await LoadAvailableLabsAsync();

        return Page();
    }

    private async Task LoadAvailableLabsAsync()
    {
        try
        {
            var labsResult = await _adminApiService.GetAllLabsAsync<Result<List<LabListItem>>>();
            if (labsResult?.IsSuccess == true && labsResult.Data != null)
            {
                AvailableLabs = labsResult.Data.Select(lab => new LabOption
                {
                    LabId = lab.LabId,
                    LabName = lab.LabName ?? "Unknown Lab",
                    Location = lab.Location
                }).ToList();
            }
            else
            {
                AvailableLabs = new List<LabOption>();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading available labs");
            AvailableLabs = new List<LabOption>();
        }
    }

    public class LabListItem
    {
        public int LabId { get; set; }
        public string? LabName { get; set; }
        public string? Location { get; set; }
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

            if (LabStaffRegistration.Password != LabStaffRegistration.ConfirmPassword)
            {
                ErrorMessage = "Password and confirm password do not match.";
                return Page();
            }

            // Validate and attach role-specific details
            if (LabStaffRegistration.RoleType == RoleType.Lab)
            {
                // Lab facility owner - requires lab facility information
                if (LabDetails == null || string.IsNullOrWhiteSpace(LabDetails.LabName))
                {
                    ErrorMessage = "Lab name is required for Lab role.";
                    await LoadAvailableLabsAsync();
                    return Page();
                }

                // Attach lab facility details
                LabStaffRegistration.RegisterLab = LabDetails;
                LabStaffRegistration.RegisterLab.CreatedBy = GetCurrentUserId() ?? "system";
            }
            else if (LabStaffRegistration.RoleType == RoleType.LabAssistant)
            {
                // Lab assistant - requires lab selection
                if (!SelectedLabId.HasValue || SelectedLabId.Value <= 0)
                {
                    ErrorMessage = "Please select a laboratory for the Lab Assistant.";
                    await LoadAvailableLabsAsync();
                    return Page();
                }

                // Create lab assignment
                LabStaffRegistration.AssignLabAssistant = new AssignLabAssistant_DTO
                {
                    LabId = SelectedLabId.Value,
                    LabAssistantId = "", // Will be set in UserService
                    CreatedBy = GetCurrentUserId()
                };
            }

            _logger.LogInformation("Registering new lab staff: {Email} with role {Role}", 
                LabStaffRegistration.Email, LabStaffRegistration.RoleType);

            var roleName = LabStaffRegistration.RoleType == RoleType.Lab ? "lab" : "labassistant";
            var result = await _adminApiService.RegisterLabAsync<Result<GeneralResponse>>(LabStaffRegistration);

            if (result?.IsSuccess == true)
            {
                var roleDisplay = LabStaffRegistration.RoleType == RoleType.Lab ? "Lab Facility" : "Lab Assistant";
                var labInfo = LabStaffRegistration.RoleType == RoleType.LabAssistant && SelectedLabId.HasValue 
                    ? $" assigned to lab" 
                    : "";
                TempData["SuccessMessage"] = $"{roleDisplay} {LabStaffRegistration.FirstName} {LabStaffRegistration.LastName}{labInfo} registered successfully!";
                return RedirectToPage("/Admin/Labs/StaffList");
            }
            else
            {
                ErrorMessage = result?.GetError() ?? "Failed to register lab staff.";
                await LoadAvailableLabsAsync();
                return Page();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error registering lab staff");
            ErrorMessage = "An error occurred while registering the lab staff.";
            await LoadAvailableLabsAsync();
            return Page();
        }
    }

    private string? GetCurrentUserId()
    {
        return User?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
    }
}
