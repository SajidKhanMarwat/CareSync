using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CareSync.Pages.Admin.Users;

public class UserReportsModel : PageModel
{
    public void OnGet()
    {
    }

    public IActionResult OnPost()
    {
        return Page();
    }
}
