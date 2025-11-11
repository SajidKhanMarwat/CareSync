using Microsoft.AspNetCore.Mvc;
using CareSync.Pages.Shared;

namespace CareSync.Pages.Admin
{
    public class DashboardModel : BasePageModel
    {
        private readonly ILogger<DashboardModel> _logger;

        public DashboardModel(ILogger<DashboardModel> logger)
        {
            _logger = logger;
        }

        public IActionResult OnGet()
        {
            // Check if user is authenticated and has Admin role
            var authResult = RequireRole("Admin");
            if (authResult != null) return authResult;

            // Admin dashboard logic here
            return Page();
        }
    }
}
