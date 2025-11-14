using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CareSync.Pages.Admin
{
    public class AppointmentReportsModel : PageModel
    {
        public void OnGet()
        {
        }

        public IActionResult OnPost()
        {
            return Page();
        }
    }
}
