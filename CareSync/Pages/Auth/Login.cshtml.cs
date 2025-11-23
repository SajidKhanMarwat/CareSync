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

                if (result.IsSuccess && result.Data != null)
                {
                    // Check if password reset is required
                    if (result.Data.RequiresPasswordReset)
                    {
                        // Store temporary session data for password reset
                        HttpContext.Session.SetString("ResetEmail", LoginRequest.Email);
                        HttpContext.Session.SetString("IsFirstTimeReset", "true");
                        
                        // Redirect to password reset page
                        return RedirectToPage("/Auth/ResetPassword", new { email = LoginRequest.Email, firstTime = true });
                    }

                    // Store user information in session
                    HttpContext.Session.SetString("UserRole", result.Data.Role ?? "Patient");
                    HttpContext.Session.SetString("UserToken", result.Data.Token ?? "");
                    HttpContext.Session.SetString("RefreshToken", result.Data.RefreshToken ?? "");

                    // Store role rights as JSON
                    if (result.Data.RoleRights != null && result.Data.RoleRights.Any())
                    {
                        var roleRightsJson = JsonSerializer.Serialize(result.Data.RoleRights);
                        HttpContext.Session.SetString("RoleRights", roleRightsJson);
                    }

                    // Redirect based on role
                    return result.Data.Role?.ToLower() switch
                    {
                        "admin" => RedirectToPage("/Admin/Dashboard"),
                        "doctor" => RedirectToPage("/Doctor/Dashboard"),
                        "patient" => RedirectToPage("/Patient/Dashboard"),
                        "lab" => RedirectToPage("/Lab/Dashboard"),
                        _ => RedirectToPage("/auth/login")
                    };
                }
                else
                {
                    ModelState.AddModelError(string.Empty, result.Data?.Message ?? "Login failed");
                    return Page();
                }
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Invalid email or password");
                return Page();
            }
        }
    }
}
