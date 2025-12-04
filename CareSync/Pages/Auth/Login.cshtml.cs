using CareSync.Result;
using CareSync.Shared.Models;
using CareSync.Shared.ViewModels.Login;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
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
                    HttpContext.Session.SetString("UserEmail", LoginRequest.Email);
                    HttpContext.Session.SetString("UserName", LoginRequest.Email);

                    string? extractedUserId = null;
                    var token = result.Data.Token ?? string.Empty;
                    if (!string.IsNullOrEmpty(token))
                    {
                        var handler = new JwtSecurityTokenHandler();
                        var jwt = handler.ReadJwtToken(token);
                        var idClaim = jwt.Claims.FirstOrDefault(c =>
                            c.Type == ClaimTypes.NameIdentifier ||
                            c.Type == "nameid" ||
                            c.Type == "sub" ||
                            c.Type == "uid" ||
                            c.Type == "id");
                        if (idClaim != null)
                            extractedUserId = idClaim.Value;
                    }

                    if (!string.IsNullOrEmpty(extractedUserId))
                    {
                        HttpContext.Session.SetString("UserId", extractedUserId);
                    }

                    // Store role rights as JSON
                    if (result.Data.RoleRights != null && result.Data.RoleRights.Any())
                    {
                        var roleRightsJson = JsonSerializer.Serialize(result.Data.RoleRights);
                        HttpContext.Session.SetString("RoleRights", roleRightsJson);
                    }

                    // Create claims for authentication cookie
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Email, LoginRequest.Email),
                        new Claim(ClaimTypes.Name, LoginRequest.Email),
                        new Claim(ClaimTypes.Role, result.Data.Role ?? "Patient"),
                        new Claim("Token", result.Data.Token ?? ""),
                        new Claim("RefreshToken", result.Data.RefreshToken ?? "")
                    };

                    // Add role rights as claims if available
                    if (result.Data.RoleRights != null)
                    {
                        foreach (var right in result.Data.RoleRights)
                        {
                            claims.Add(new Claim("RoleRight", right));
                        }
                    }

                    // Create identity and principal
                    var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    var authProperties = new AuthenticationProperties
                    {
                        AllowRefresh = true,
                        IsPersistent = true, // Remember me functionality
                        ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(60)
                    };

                    // Sign in the user
                    await HttpContext.SignInAsync(
                        CookieAuthenticationDefaults.AuthenticationScheme,
                        new ClaimsPrincipal(claimsIdentity),
                        authProperties);

                    // Redirect based on role
                    var redirectUrl = result.Data.Role?.ToLower() switch
                    {
                        "admin" => "/Admin/Dashboard",
                        "doctor" => "/Doctor/Dashboard",
                        "patient" => "/Patient/Dashboard",
                        "lab" => "/Lab/Dashboard",
                        _ => "/auth/login"
                    };

                    // Use explicit redirect to ensure proper navigation
                    return Redirect(redirectUrl);
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
