using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CareSync.Pages.Patients;

public class CreateModel : PageModel
{
    public void OnGet()
    {
    }

    public IActionResult OnPost()
    {
        // TODO: Implement patient creation logic
        return Page();
    }
}
