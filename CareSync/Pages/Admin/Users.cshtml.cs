using CareSync.ApplicationLayer.Common;
using CareSync.ApplicationLayer.Contracts.UserManagementDTOs;
using CareSync.Services;
using CareSync.Shared.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.Json;

namespace CareSync.Pages.Admin;

[Authorize(Roles = "Admin")]
public class UsersModel : PageModel
{
    private readonly UserManagementApiService _userManagementService;
    private readonly ILogger<UsersModel> _logger;

    public UsersModel(UserManagementApiService userManagementService, ILogger<UsersModel> logger)
    {
        _userManagementService = userManagementService;
        _logger = logger;
    }

    // Properties for displaying data
    public UserStatistics_DTO UserStatistics { get; set; } = new();
    public PagedResult<UserList_DTO> UsersList { get; set; } = new();
    public List<string> Departments { get; set; } = new();
    public List<string> Roles { get; set; } = new();

    // Filter properties
    [BindProperty(SupportsGet = true)]
    public string? SearchTerm { get; set; }
    
    [BindProperty(SupportsGet = true)]
    public string? RoleFilter { get; set; }
    
    [BindProperty(SupportsGet = true)]
    public string? StatusFilter { get; set; }
    
    [BindProperty(SupportsGet = true)]
    public string? DepartmentFilter { get; set; }
    
    [BindProperty(SupportsGet = true)]
    public string? DateFilter { get; set; }
    
    public int CurrentPage { get; set; } = 1;
    
    [BindProperty(SupportsGet = true)]
    public int PageSize { get; set; } = 10;

    public async Task<IActionResult> OnGetAsync([FromQuery(Name = "Page")] int? page)
    {
        try
        {
            // Log the incoming page parameter for debugging
            _logger.LogInformation($"Received page parameter: {page}, CurrentPage before: {CurrentPage}");
            
            // Get page from query parameter
            if (page.HasValue && page.Value > 0)
            {
                CurrentPage = page.Value;
            }
            
            // Validate and adjust page parameters
            if (CurrentPage < 1) CurrentPage = 1;
            if (PageSize < 1) PageSize = 10;
            if (PageSize > 100) PageSize = 100; // Prevent too large page sizes
            
            _logger.LogInformation($"CurrentPage after validation: {CurrentPage}, PageSize: {PageSize}");

            // Get user statistics for cards
            var statsResult = await _userManagementService.GetUserStatisticsAsync();
            if (statsResult.IsSuccess && statsResult.Data != null)
            {
                UserStatistics = statsResult.Data;
            }

            // Get departments and roles for filters
            var deptResult = await _userManagementService.GetDepartmentsAsync();
            if (deptResult.IsSuccess && deptResult.Data != null)
            {
                Departments = deptResult.Data;
            }

            var rolesResult = await _userManagementService.GetRolesAsync();
            if (rolesResult.IsSuccess && rolesResult.Data != null)
            {
                Roles = rolesResult.Data;
            }

            // Build filter object - ensure no null values
            var filter = new UserFilter_DTO
            {
                SearchTerm = SearchTerm ?? string.Empty,
                Status = StatusFilter ?? string.Empty,
                Department = DepartmentFilter ?? string.Empty,
                DateFilter = DateFilter ?? string.Empty,
                Page = CurrentPage,
                PageSize = PageSize,
                SortBy = "RegisteredDate",
                SortDescending = true
            };

            // Parse role filter
            if (!string.IsNullOrEmpty(RoleFilter))
            {
                if (Enum.TryParse<RoleType>(RoleFilter, out var roleType))
                {
                    filter.RoleType = roleType;
                }
            }

            // Get users list
            _logger.LogInformation($"Calling GetAllUsersAsync with Page: {filter.Page}, PageSize: {filter.PageSize}");
            var usersResult = await _userManagementService.GetAllUsersAsync(filter);
            if (usersResult.IsSuccess && usersResult.Data != null)
            {
                UsersList = usersResult.Data;
                _logger.LogInformation($"Retrieved {UsersList.Items?.Count ?? 0} items for page {UsersList.Page} of {UsersList.TotalPages}");
                
                // Update CurrentPage to match what was actually returned
                CurrentPage = UsersList.Page;
                
                // If current page exceeds total pages, redirect to last page
                if (UsersList.TotalPages > 0 && CurrentPage > UsersList.TotalPages)
                {
                    CurrentPage = UsersList.TotalPages;
                    return RedirectToPage(new { 
                        Page = CurrentPage, 
                        PageSize = PageSize,
                        SearchTerm = SearchTerm,
                        RoleFilter = RoleFilter,
                        StatusFilter = StatusFilter,
                        DepartmentFilter = DepartmentFilter,
                        DateFilter = DateFilter
                    });
                }
            }
            else
            {
                _logger.LogWarning($"Failed to get users list: {usersResult.Error}");
            }

            return Page();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading users page");
            TempData["Error"] = "An error occurred while loading users data.";
            return Page();
        }
    }

    public async Task<IActionResult> OnPostToggleStatusAsync(string userId, bool isActive)
    {
        try
        {
            var result = await _userManagementService.ToggleUserStatusAsync(userId, isActive);
            if (result.IsSuccess)
            {
                TempData["Success"] = result.Data?.Message ?? "User status updated successfully.";
            }
            else
            {
                TempData["Error"] = result.Error ?? "Failed to update user status.";
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error toggling user status");
            TempData["Error"] = "An error occurred while updating user status.";
        }

        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostSuspendAsync(string userId, string reason)
    {
        try
        {
            var result = await _userManagementService.SuspendUserAsync(userId, reason);
            if (result.IsSuccess)
            {
                TempData["Success"] = "User suspended successfully.";
            }
            else
            {
                TempData["Error"] = result.Error ?? "Failed to suspend user.";
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error suspending user");
            TempData["Error"] = "An error occurred while suspending user.";
        }

        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostResetPasswordAsync(string userId, string newPassword)
    {
        try
        {
            var dto = new AdminPasswordReset_DTO
            {
                UserId = userId,
                NewPassword = newPassword,
                RequirePasswordChange = true,
                SendNotification = true
            };

            var result = await _userManagementService.ResetPasswordAsync(dto);
            if (result.IsSuccess)
            {
                TempData["Success"] = "Password reset successfully.";
            }
            else
            {
                TempData["Error"] = result.Error ?? "Failed to reset password.";
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error resetting password");
            TempData["Error"] = "An error occurred while resetting password.";
        }

        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostDeleteAsync(string userId)
    {
        try
        {
            var result = await _userManagementService.DeleteUserAsync(userId);
            if (result.IsSuccess)
            {
                TempData["Success"] = "User deleted successfully.";
            }
            else
            {
                TempData["Error"] = result.Error ?? "Failed to delete user.";
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting user");
            TempData["Error"] = "An error occurred while deleting user.";
        }

        return RedirectToPage();
    }

    // AJAX endpoint for getting user details
    public async Task<IActionResult> OnGetUserDetailsAsync(string userId)
    {
        try
        {
            var result = await _userManagementService.GetUserByIdAsync(userId);
            if (result.IsSuccess && result.Data != null)
            {
                return new JsonResult(new { success = true, data = result.Data });
            }
            
            return new JsonResult(new { success = false, message = result.Error });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting user details");
            return new JsonResult(new { success = false, message = "An error occurred." });
        }
    }
}
