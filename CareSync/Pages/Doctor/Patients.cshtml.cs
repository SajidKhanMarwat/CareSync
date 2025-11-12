using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CareSync.Pages.Doctor
{
    public class PatientsModel : PageModel
    {
        public void OnGet()
        {
            // Initialize page data
        }

        public IActionResult OnPost()
        {
            // Handle form submissions
            return Page();
        }
    }
}
