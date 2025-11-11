using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CareSync.Pages.Auth;

public class LogoutModel : PageModel
{
    public IActionResult OnGet()
    {
        // Clear all session data
        HttpContext.Session.Clear();
        
        // Optionally, you can also clear specific session keys
        // HttpContext.Session.Remove("UserRole");
        // HttpContext.Session.Remove("UserToken");
        // HttpContext.Session.Remove("RefreshToken");
        // HttpContext.Session.Remove("RoleRights");
        
        // The page will handle the redirect via JavaScript
        return Page();
    }
}
