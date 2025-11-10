using System.ComponentModel.DataAnnotations;

namespace CareSync.Shared.ViewModels;

public class Register_Request
{
    [Required(ErrorMessage = "First name is required")]
    [Display(Name = "First Name")]
    public required string FirstName { get; set; }

    [Display(Name = "Middle Name")]
    public string? MiddleName { get; set; }

    [Display(Name = "Last Name")]
    public string? LastName { get; set; }

    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Please enter a valid email address")]
    [Display(Name = "Email Address")]
    public required string Email { get; set; }

    [Required(ErrorMessage = "Username is required")]
    [Display(Name = "Username")]
    public required string UserName { get; set; }

    [Phone(ErrorMessage = "Please enter a valid phone number")]
    [Display(Name = "Phone Number")]
    public string? PhoneNumber { get; set; }

    [Required(ErrorMessage = "Password is required")]
    [DataType(DataType.Password)]
    [MinLength(6, ErrorMessage = "Password must be at least 6 characters long")]
    [Display(Name = "Password")]
    public required string Password { get; set; }

    [Required(ErrorMessage = "Please confirm your password")]
    [DataType(DataType.Password)]
    [Compare("Password", ErrorMessage = "Passwords do not match")]
    [Display(Name = "Confirm Password")]
    public required string ConfirmPassword { get; set; }

    [Display(Name = "Enable Two-Factor Authentication")]
    public bool TwoFactorEnabled { get; set; } = false;
}
