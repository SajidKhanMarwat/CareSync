using System;
using System.ComponentModel.DataAnnotations;

public class Register_Request
{
    [Required(ErrorMessage = "Arabic name is required")]
    [Display(Name = "Arabic Name")]
    public required string ArabicUserName { get; set; }

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

    [Required(ErrorMessage = "Gender is required")]
    [Display(Name = "Gender")]
    public int Gender { get; set; } // 1 = Male, 2 = Female, 3 = Other

    [Display(Name = "Date of Birth")]
    [DataType(DataType.Date)]
    public DateTime? DateOfBirth { get; set; }

    [Display(Name = "Age")]
    public int? Age { get; set; }

    [Display(Name = "Address")]
    public string? Address { get; set; }

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
