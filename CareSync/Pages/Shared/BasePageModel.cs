using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.Json;

namespace CareSync.Pages.Shared;

public abstract class BasePageModel : PageModel
{
    /// <summary>
    /// Gets the current user's role from session
    /// </summary>
    public string UserRole => HttpContext.Session.GetString("UserRole") ?? "Patient";
    
    /// <summary>
    /// Gets the current user's token from session
    /// </summary>
    public string UserToken => HttpContext.Session.GetString("UserToken") ?? "";
    
    /// <summary>
    /// Gets the current user's refresh token from session
    /// </summary>
    public string RefreshToken => HttpContext.Session.GetString("RefreshToken") ?? "";
    
    /// <summary>
    /// Gets the current user's role rights from session
    /// </summary>
    public List<string> RoleRights
    {
        get
        {
            var roleRightsJson = HttpContext.Session.GetString("RoleRights");
            if (string.IsNullOrEmpty(roleRightsJson))
                return new List<string>();
                
            try
            {
                return JsonSerializer.Deserialize<List<string>>(roleRightsJson) ?? new List<string>();
            }
            catch
            {
                return new List<string>();
            }
        }
    }
    
    /// <summary>
    /// Checks if the user is authenticated (has a valid session)
    /// </summary>
    public bool IsAuthenticated => !string.IsNullOrEmpty(UserToken);
    
    /// <summary>
    /// Checks if the current user has a specific role
    /// </summary>
    /// <param name="role">The role to check</param>
    /// <returns>True if user has the role</returns>
    public bool HasRole(string role)
    {
        return string.Equals(UserRole, role, StringComparison.OrdinalIgnoreCase);
    }
    
    /// <summary>
    /// Checks if the current user has any of the specified roles
    /// </summary>
    /// <param name="roles">The roles to check</param>
    /// <returns>True if user has any of the roles</returns>
    public bool HasAnyRole(params string[] roles)
    {
        return roles.Any(role => HasRole(role));
    }
    
    /// <summary>
    /// Checks if the current user has a specific right
    /// </summary>
    /// <param name="right">The right to check</param>
    /// <returns>True if user has the right</returns>
    public bool HasRight(string right)
    {
        return RoleRights.Contains(right, StringComparer.OrdinalIgnoreCase);
    }
    
    /// <summary>
    /// Redirects to login if user is not authenticated
    /// </summary>
    /// <returns>RedirectToPageResult or null if authenticated</returns>
    public IActionResult? RequireAuthentication()
    {
        if (!IsAuthenticated)
        {
            return RedirectToPage("/Auth/Login");
        }
        return null;
    }
    
    /// <summary>
    /// Redirects to access denied if user doesn't have required role
    /// </summary>
    /// <param name="requiredRole">The required role</param>
    /// <returns>RedirectToPageResult or null if authorized</returns>
    public IActionResult? RequireRole(string requiredRole)
    {
        var authCheck = RequireAuthentication();
        if (authCheck != null) return authCheck;
        
        if (!HasRole(requiredRole))
        {
            return RedirectToPage("/Auth/AccessDenied");
        }
        return null;
    }
    
    /// <summary>
    /// Redirects to access denied if user doesn't have any of the required roles
    /// </summary>
    /// <param name="requiredRoles">The required roles</param>
    /// <returns>RedirectToPageResult or null if authorized</returns>
    public IActionResult? RequireAnyRole(params string[] requiredRoles)
    {
        var authCheck = RequireAuthentication();
        if (authCheck != null) return authCheck;
        
        if (!HasAnyRole(requiredRoles))
        {
            return RedirectToPage("/Auth/AccessDenied");
        }
        return null;
    }
}
