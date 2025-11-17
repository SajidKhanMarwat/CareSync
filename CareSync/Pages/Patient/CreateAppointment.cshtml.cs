using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CareSync.Pages.Patient;

public class CreateAppointmentModel : PageModel
{
    public void OnGet()
    {
    }

    public IActionResult OnPost()
    {
        return Page();
    }
}
