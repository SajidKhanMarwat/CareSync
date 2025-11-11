using CareSync.ApplicationLayer.ApiResult;
using CareSync.ApplicationLayer.Common;
using CareSync.ApplicationLayer.Contracts.UsersDTOs;
using CareSync.Shared.Models;
using CareSync.Shared.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CareSync.Pages.Auth;

public class RegisterModel : PageModel
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<RegisterModel> _logger;

    public RegisterModel(IHttpClientFactory httpClientFactory, ILogger<RegisterModel> logger)
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger;
    }

    [BindProperty]
    public required Register_Request RegisterRequest { get; set; }

    public string? ErrorMessage { get; set; }
    public string? SuccessMessage { get; set; }

    public void OnGet()
    {
        // Initialize the model if needed
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        try
        {
            // Map Register_Request to UserRegisteration_DTO
            var registrationDto = new UserRegisteration_DTO
            {
                FirstName = RegisterRequest.FirstName,
                MiddleName = RegisterRequest.MiddleName,
                LastName = RegisterRequest.LastName,
                Email = RegisterRequest.Email,
                UserName = RegisterRequest.UserName,
                ArabicUserName = RegisterRequest.ArabicUserName,
                PhoneNumber = RegisterRequest.PhoneNumber ?? string.Empty,
                Password = RegisterRequest.Password,
                ConfirmPassword = RegisterRequest.ConfirmPassword,
                Gender = RegisterRequest.Gender,
                DateOfBirth = RegisterRequest.DateOfBirth,
                Age = RegisterRequest.Age,
                Address = RegisterRequest.Address,
                TwoFactorEnabled = RegisterRequest.TwoFactorEnabled
            };

            var client = _httpClientFactory.CreateClient("ApiClient");
            var response = await client.PostAsJsonAsync("account/register", registrationDto);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<Result<GeneralResponse>>();
                
                if (result?.IsSuccess == true && result.Data?.Success == true)
                {
                    SuccessMessage = result.Data.Message ?? "Registration successful! You can now login.";
                    TempData["SuccessMessage"] = SuccessMessage;
                    return RedirectToPage("/Auth/Login");
                }
                else
                {
                    ErrorMessage = result?.Data?.Message ?? result?.GetError() ?? "Registration failed. Please try again.";
                }
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                _logger.LogError("Registration failed with status {StatusCode}: {Content}", 
                    response.StatusCode, errorContent);
                
                ErrorMessage = "Registration failed. Please check your information and try again.";
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during user registration");
            ErrorMessage = "An error occurred during registration. Please try again later.";
        }

        return Page();
    }
}
