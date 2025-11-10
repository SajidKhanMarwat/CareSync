using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization;

namespace CareSync.Pages.Patient
{
   
    public class DashboardModel : PageModel
    {
        private readonly ILogger<DashboardModel> _logger;

        public DashboardModel(ILogger<DashboardModel> logger)
        {
            _logger = logger;
        }

        public void OnGet()
        {
            // Patient dashboard logic here
        }
    }
}
