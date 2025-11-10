using CareSync.ApplicationLayer.ApiResult;
using CareSync.Shared.Models;
using CareSync.Shared.ViewModels.Login;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.Json;

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
            var response = await client.PostAsJsonAsync("account/login", LoginRequest);
            var resultsString = await response.Content.ReadAsStringAsync();
            if (response.IsSuccessStatusCode)
            {
                using var jsonDoc = await JsonDocument.ParseAsync(await response.Content.ReadAsStreamAsync());

                // Access the "data" property
                var dataElement = jsonDoc.RootElement.GetProperty("data");

                // Deserialize only the "data" object to your model
                var result = JsonSerializer.Deserialize<LoginResponse>(dataElement.GetRawText());


                //// Optionally store token in session/cookie
                //HttpContext.Session.SetString("AuthToken", result.Token);

                // Redirect after login
                return RedirectToPage("/Dashboard/Index");
            }
            else
            {
                loginResponse.Message = "Invalid email or password";
                return Page();
            }
        }
    }
}
