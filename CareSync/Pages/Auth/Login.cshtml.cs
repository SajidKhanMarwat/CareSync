using CareSync.ApplicationLayer.ApiResult;
using CareSync.Result;
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

            if (response.IsSuccessStatusCode)
            {
                var result = ApiResponseMapper.MapLoginResponse(await response.Content.ReadAsStringAsync());

                if (result.IsSuccess)
                    return RedirectToPage("/Dashboard/Index");
                else
                    return RedirectToPage("/auth/login");
            }
            else
            {
                loginResponse.Message = "Invalid email or password";
                return Page();
            }
        }
    }
}
