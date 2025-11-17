using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CareSync.Pages.Admin;

public class CreateDoctorModel : PageModel
{
    public void OnGet()
    {
    }

    public IActionResult OnPost()
    {
        // TODO: Implement doctor creation logic
        return Page();
    }
}
