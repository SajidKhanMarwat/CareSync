using CareSync.ApplicationLayer.ApiResult;
using CareSync.ApplicationLayer.Common;
using CareSync.ApplicationLayer.Contracts.UsersDTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net;
using System.Text.Json;

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
    public bool ShowOnboardingWizard { get; set; }

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
                try
                {
                    // Read the raw response content for debugging
                    var responseContent = await response.Content.ReadAsStringAsync();
                    _logger.LogInformation("Registration API Response: {Content}", responseContent);

                    // Parse the response directly based on the actual JSON structure
                    Result<GeneralResponse>? result = null;
                    if (!string.IsNullOrEmpty(responseContent))
                    {
                        // First try to parse as a simple response structure
                        var jsonDoc = JsonDocument.Parse(responseContent);
                        var root = jsonDoc.RootElement;

                        if (root.TryGetProperty("isSuccess", out var isSuccessElement) &&
                            root.TryGetProperty("data", out var dataElement))
                        {
                            var isSuccess = isSuccessElement.GetBoolean();
                            var data = new GeneralResponse();

                            if (dataElement.TryGetProperty("success", out var successElement))
                                data.Success = successElement.GetBoolean();

                            if (dataElement.TryGetProperty("message", out var messageElement))
                                data.Message = messageElement.GetString() ?? string.Empty;

                            // Create a result object manually
                            result = new Result<GeneralResponse>();
                            result.IsSuccess = isSuccess;
                            result.Data = data;

                            if (root.TryGetProperty("statusCode", out var statusCodeElement))
                                result.StatusCode = (HttpStatusCode)statusCodeElement.GetInt32();
                        }
                    }

                    if (result?.IsSuccess == true && result.Data?.Success == true)
                    {
                        SuccessMessage = result.Data.Message ?? "Registration successful!";
                        ShowOnboardingWizard = true;
                        return Page();
                    }
                    else
                    {
                        ErrorMessage = result?.Data?.Message ?? result?.GetError() ?? "Registration failed. Please try again.";
                        _logger.LogWarning("Registration failed with result: {@Result}", result);
                    }
                }
                catch (JsonException jsonEx)
                {
                    _logger.LogError(jsonEx, "Failed to deserialize registration response");

                    // Try to read the raw content again for error display
                    var rawContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError("Raw response content: {Content}", rawContent);

                    ErrorMessage = "Registration response format error. Please try again.";
                }
                catch (Exception deserializationEx)
                {
                    _logger.LogError(deserializationEx, "Error processing registration response");
                    ErrorMessage = "Error processing registration response. Please try again.";
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
