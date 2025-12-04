using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CareSync.Pages.Admin.Labs;

public class StaffListModel : PageModel
{
    public void OnGet()
    {
    }

    public IActionResult OnPost()
    {
        return Page();
    }
}
