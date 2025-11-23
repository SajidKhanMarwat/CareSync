using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using CareSync.Result;

namespace CareSync.Pages.Auth
{
    public class ResetPasswordModel : PageModel
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<ResetPasswordModel> _logger;

        public ResetPasswordModel(IHttpClientFactory httpClientFactory, ILogger<ResetPasswordModel> logger)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
        }

        [BindProperty]
        public InputModel Input { get; set; } = new();

        public string? ErrorMessage { get; set; }
        public string? SuccessMessage { get; set; }
        public bool IsFirstTimeReset { get; set; }

        public class InputModel
        {
            [Required(ErrorMessage = "Email or username is required")]
            [Display(Name = "Email / Username")]
            public string Email { get; set; } = string.Empty;

            [Display(Name = "Current Password")]
            [DataType(DataType.Password)]
            public string? CurrentPassword { get; set; }

            [Required(ErrorMessage = "New password is required")]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "New Password")]
            [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{6,}$",
                ErrorMessage = "Password must contain at least one uppercase letter, one lowercase letter, one number and one special character.")]
            public string NewPassword { get; set; } = string.Empty;

            [DataType(DataType.Password)]
            [Display(Name = "Confirm Password")]
            [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; } = string.Empty;
        }

        public IActionResult OnGet(string? email = null, bool firstTime = false)
        {
            // If coming from login with password reset required
            if (!string.IsNullOrEmpty(email))
            {
                Input = new InputModel { Email = email };
                IsFirstTimeReset = firstTime;
            }

            // Check if user is already logged in and doesn't need reset
            if (User.Identity?.IsAuthenticated == true && !IsFirstTimeReset)
            {
                // Redirect based on role
                if (User.IsInRole("Admin"))
                    return RedirectToPage("/Admin/Dashboard");
                else if (User.IsInRole("Doctor"))
                    return RedirectToPage("/Doctor/Dashboard");
                else if (User.IsInRole("Patient"))
                    return RedirectToPage("/Patient/Dashboard");
                else if (User.IsInRole("Lab"))
                    return RedirectToPage("/Lab/Dashboard");
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            try
            {
                _logger.LogInformation("Password reset attempt for user: {Email}", Input.Email);

                // For first-time reset (admin-created users), we don't need current password
                // The user should have already authenticated with temporary password
                if (IsFirstTimeReset || !string.IsNullOrEmpty(Input.CurrentPassword))
                {
                    // Call the password reset API
                    var resetRequest = new
                    {
                        Email = Input.Email,
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
                            _logger.LogInformation("Password reset successful for user: {Email}", Input.Email);
                            
                            // Clear session to force re-login with new password
                            HttpContext.Session.Clear();
                            
                            TempData["SuccessMessage"] = "Password reset successful! Please login with your new password.";
                            return RedirectToPage("/Auth/Login");
                        }
                        else
                        {
                            ErrorMessage = result.Data?.Message ?? "Failed to reset password. Please try again.";
                            _logger.LogWarning("Password reset failed for user: {Email}. Error: {Error}", 
                                Input.Email, ErrorMessage);
                        }
                    }
                    else
                    {
                        ErrorMessage = "Failed to connect to the server. Please try again.";
                        _logger.LogError("Password reset API call failed with status: {StatusCode}", response.StatusCode);
                    }
                }
                else
                {
                    ErrorMessage = "Current password is required for password reset.";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during password reset for user: {Email}", Input.Email);
                ErrorMessage = "An error occurred while resetting your password. Please try again.";
            }

            return Page();
        }
    }
}
