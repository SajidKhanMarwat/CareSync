using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using CareSync.Result;
using CareSync.Models;

namespace CareSync.Pages.Auth
{
    public class ForgetPasswordModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<ForgetPasswordModel> _logger;

        public ForgetPasswordModel(IHttpClientFactory httpClientFactory, ILogger<ForgetPasswordModel> logger)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
        }

        [BindProperty]
        public InputModel Input { get; set; } = new();

        [TempData]
        public bool IsVerified { get; set; }

        public string? ErrorMessage { get; set; }
        public string? SuccessMessage { get; set; }

        public class InputModel
        {
            [Required(ErrorMessage = "Email or Username is required")]
            [Display(Name = "Email or Username")]
            public string EmailOrUsername { get; set; } = string.Empty;

            [DataType(DataType.Password)]
            [Display(Name = "New Password")]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 8)]
            [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$",
                ErrorMessage = "Password must contain at least one uppercase letter, one lowercase letter, one number and one special character.")]
            public string NewPassword { get; set; } = string.Empty;

            [DataType(DataType.Password)]
            [Display(Name = "Confirm Password")]
            [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; } = string.Empty;
        }

        public IActionResult OnGet()
        {
            // Reset verification state when accessing the page directly
            IsVerified = false;
            return Page();
        }

        public async Task<IActionResult> OnPostVerifyUserAsync()
        {
            // Only validate EmailOrUsername for verification step
            ModelState.Remove("Input.NewPassword");
            ModelState.Remove("Input.ConfirmPassword");

            if (!ModelState.IsValid)
            {
                return Page();
            }

            try
            {
                _logger.LogInformation("Attempting to verify user: {EmailOrUsername}", Input.EmailOrUsername);

                // Call API to verify user exists
                var client = _httpClientFactory.CreateClient("ApiClient");
                
                var verifyRequest = new
                {
                    EmailOrUsername = Input.EmailOrUsername
                };

                var response = await client.PostAsJsonAsync("account/verify-user", verifyRequest);

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var result = JsonSerializer.Deserialize<ApiResponse<VerifyUserResponse>>(content, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                    if (result?.IsSuccess == true && result.Data != null)
                    {
                        _logger.LogInformation("User verified successfully: {EmailOrUsername}", Input.EmailOrUsername);
                        IsVerified = true;
                        TempData["IsVerified"] = true;
                        TempData["EmailOrUsername"] = Input.EmailOrUsername;
                        TempData["UserEmail"] = result.Data.Email; // Store actual email for password reset
                        SuccessMessage = "Account verified. Please enter your new password.";
                    }
                    else
                    {
                        ErrorMessage = "No account found with the provided email or username.";
                        _logger.LogWarning("User verification failed: {EmailOrUsername}", Input.EmailOrUsername);
                    }
                }
                else
                {
                    ErrorMessage = "Unable to verify account. Please check your details and try again.";
                    _logger.LogError("API call failed with status: {StatusCode}", response.StatusCode);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during user verification: {EmailOrUsername}", Input.EmailOrUsername);
                ErrorMessage = "An error occurred while verifying your account. Please try again.";
            }

            return Page();
        }
        
        public class VerifyUserResponse
        {
            public string Email { get; set; } = string.Empty;
            public string Username { get; set; } = string.Empty;
        }

        public async Task<IActionResult> OnPostResetPasswordAsync()
        {
            // Restore verification state from TempData
            if (TempData["IsVerified"] as bool? == true)
            {
                IsVerified = true;
                Input.EmailOrUsername = TempData["EmailOrUsername"]?.ToString() ?? Input.EmailOrUsername;
            }

            var userEmail = TempData["UserEmail"]?.ToString();
            if (!IsVerified || string.IsNullOrEmpty(userEmail))
            {
                ErrorMessage = "Please verify your account first.";
                return Page();
            }

            // Only validate password fields for reset step
            ModelState.Remove("Input.EmailOrUsername");

            if (!ModelState.IsValid)
            {
                return Page();
            }

            try
            {
                _logger.LogInformation("Password reset attempt for user: {Email}", userEmail);

                // Call the password reset API
                var resetRequest = new
                {
                    Email = userEmail,
                    NewPassword = Input.NewPassword,
                    ConfirmPassword = Input.ConfirmPassword
                };

                var client = _httpClientFactory.CreateClient("ApiClient");
                var response = await client.PostAsJsonAsync("account/forget-password", resetRequest);

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var result = ApiResponseMapper.MapGeneralResponse(content);

                    if (result.IsSuccess && result.Data?.Success == true)
                    {
                        _logger.LogInformation("Password reset successful for user: {Email}", userEmail);
                        
                        TempData["SuccessMessage"] = "Password reset successful! Please login with your new password.";
                        return RedirectToPage("/Auth/Login");
                    }
                    else
                    {
                        ErrorMessage = result.Data?.Message ?? "Failed to reset password. Please try again.";
                        _logger.LogWarning("Password reset failed for user: {Email}. Error: {Error}", 
                            userEmail, ErrorMessage);
                    }
                }
                else
                {
                    ErrorMessage = "Failed to connect to the server. Please try again.";
                    _logger.LogError("Password reset API call failed with status: {StatusCode}", response.StatusCode);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during password reset for user: {Email}", userEmail);
                ErrorMessage = "An error occurred while resetting your password. Please try again.";
            }

            return Page();
        }
    }
}
