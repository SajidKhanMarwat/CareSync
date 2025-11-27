using CareSync.ApplicationLayer.Contracts.UserManagementDTOs;
using CareSync.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CareSync.Pages.Admin;

[Authorize(Roles = "Admin")]
public class ViewUserProfileModel : PageModel
{
    private readonly UserManagementApiService _userManagementService;
    private readonly ILogger<ViewUserProfileModel> _logger;

    public ViewUserProfileModel(UserManagementApiService userManagementService, ILogger<ViewUserProfileModel> logger)
    {
        _userManagementService = userManagementService;
        _logger = logger;
    }

    [BindProperty(SupportsGet = true)]
    public string UserId { get; set; } = string.Empty;

    public UserDetail_DTO UserDetail { get; set; } = new();

    public async Task<IActionResult> OnGetAsync(string userId)
    {
        if (string.IsNullOrEmpty(userId))
        {
            TempData["Error"] = "Invalid user ID.";
            return RedirectToPage("/Admin/Users");
        }

        try
        {
            UserId = userId;
            var result = await _userManagementService.GetUserByIdAsync(userId);
            
            if (result.IsSuccess && result.Data != null)
            {
                UserDetail = result.Data;
                return Page();
            }
            else
            {
                TempData["Error"] = result.Error ?? "User not found.";
                return RedirectToPage("/Admin/Users");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading user profile for ID: {UserId}", userId);
            TempData["Error"] = "An error occurred while loading the user profile.";
            return RedirectToPage("/Admin/Users");
        }
    }
}
