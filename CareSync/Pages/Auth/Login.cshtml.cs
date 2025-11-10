using CareSync.Shared.Models;
using CareSync.Shared.ViewModels.Login;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CareSync.Pages;

public class LoginModel : PageModel
{
    private readonly IHttpClientFactory _httpClientFactory;

    public LoginModel(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    public void OnGet()
    {
    }

    [BindProperty]
    public required Login_Request LoginRequest { get; set; }
    public async Task<IActionResult> OnPostAsync(string returnUrl = "")
    {
        if (!ModelState.IsValid)
            return Page();
        else
        {
            LoginResponse loginResponse = new LoginResponse();
            var client = _httpClientFactory.CreateClient("ApiClient");
            var response = await client.PostAsJsonAsync("auth/login", LoginRequest);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<LoginResponse>();
                loginResponse.Message = result?.Message!;

                //// Optionally store token in session/cookie
                //HttpContext.Session.SetString("AuthToken", result.Token);

                // Redirect after login
                return RedirectToPage("/Index");
            }
            else
            {
                loginResponse.Message = "Invalid email or password";
                return Page();
            }
        }
    }
}
