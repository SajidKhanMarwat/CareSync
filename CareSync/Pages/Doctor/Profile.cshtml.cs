using Microsoft.AspNetCore.Mvc;
using CareSync.Pages.Shared;

namespace CareSync.Pages.Doctor
{
    public class ProfileModel : BasePageModel
    {
        public IActionResult OnGet()
        {
            // Check if user is authenticated and has Doctor role
            var authResult = RequireRole("Doctor");
            if (authResult != null) return authResult;

            // Doctor profile logic here
            return Page();
        }
    }
}
