using CareSync.ApplicationLayer.Contracts.UserManagementDTOs;
using CareSync.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace CareSync.Pages.Admin.Users;

[Authorize(Roles = "Admin")]
public class ResetPasswordModel : PageModel
{
    private readonly UserManagementApiService _userManagementService;
    private readonly ILogger<ResetPasswordModel> _logger;

    public ResetPasswordModel(UserManagementApiService userManagementService, ILogger<ResetPasswordModel> logger)
    {
        _userManagementService = userManagementService;
        _logger = logger;
    }

    [BindProperty(SupportsGet = true)]
    public string UserId { get; set; } = string.Empty;

    public UserDetail_DTO UserDetail { get; set; } = new();

    [BindProperty]
    public ResetPasswordInput Input { get; set; } = new();

    public class ResetPasswordInput
    {
        [Required(ErrorMessage = "New password is required")]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "New Password")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{6,}$",
            ErrorMessage = "Password must contain at least one uppercase letter, one lowercase letter, one number, and one special character")]
        public string NewPassword { get; set; } = string.Empty;

        [Required(ErrorMessage = "Confirm password is required")]
        [DataType(DataType.Password)]
        [Display(Name = "Confirm Password")]
        [Compare("NewPassword", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; } = string.Empty;

        [Display(Name = "Require password change on next login")]
        public bool RequirePasswordChange { get; set; } = false;

        [Display(Name = "Send notification to user")]
        public bool SendNotification { get; set; } = true;
    }

    public async Task<IActionResult> OnGetAsync(string userId)
    {
        if (string.IsNullOrEmpty(userId))
        {
            TempData["Error"] = "Invalid user ID.";
            return RedirectToPage("/Admin/Users/UsersList");
        }

        try
        {
            UserId = userId;

            // Load user details
            var result = await _userManagementService.GetUserByIdAsync(userId);
            
            if (result.IsSuccess && result.Data != null)
            {
                UserDetail = result.Data;
                return Page();
            }
            else
            {
                TempData["Error"] = result.Error ?? "User not found.";
                return RedirectToPage("/Admin/Users/UsersList");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading user for password reset: {UserId}", userId);
            TempData["Error"] = "An error occurred while loading the user.";
            return RedirectToPage("/Admin/Users/UsersList");
        }
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            // Reload user details for display
            await LoadUserDetails();
            return Page();
        }

        try
        {
            var dto = new AdminPasswordReset_DTO
            {
                UserId = UserId,
                NewPassword = Input.NewPassword,
                RequirePasswordChange = Input.RequirePasswordChange,
                SendNotification = Input.SendNotification
            };

            var result = await _userManagementService.ResetPasswordAsync(dto);
            
            if (result.IsSuccess)
            {
                TempData["Success"] = "Password has been reset successfully.";
                _logger.LogInformation("Password reset successfully for user: {UserId}", UserId);
                
                // Redirect back to users list with success message
                return RedirectToPage("/Admin/Users/UsersList");
            }
            else
            {
                TempData["Error"] = result.Error ?? "Failed to reset password.";
                await LoadUserDetails();
                return Page();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error resetting password for user: {UserId}", UserId);
            TempData["Error"] = "An error occurred while resetting the password.";
            await LoadUserDetails();
            return Page();
        }
    }

    private async Task LoadUserDetails()
    {
        try
        {
            var result = await _userManagementService.GetUserByIdAsync(UserId);
            if (result.IsSuccess && result.Data != null)
            {
                UserDetail = result.Data;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading user details for: {UserId}", UserId);
        }
    }
}
