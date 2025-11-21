using System.ComponentModel.DataAnnotations;

namespace CareSync.ApplicationLayer.Contracts.UsersDTOs;

public record UserUpdate_DTO
{
    public required string UserId { get; set; }

    [Required(ErrorMessage = "First name is required")]
    public required string FirstName { get; set; }

    public string? MiddleName { get; set; }

    public string? LastName { get; set; }

    [Phone(ErrorMessage = "Please enter a valid phone number")]
    public string PhoneNumber { get; set; } = string.Empty;

    [Required(ErrorMessage = "Password is required")]
    [DataType(DataType.Password)]
    [StringLength(20, MinimumLength = 6, ErrorMessage = "Password must be between 6–20 characters.")]
    public required string Password { get; set; }

    [DataType(DataType.Password)]
    [Compare("Password", ErrorMessage = "Passwords do not match.")]
    public required string ConfirmPassword { get; set; }
    public string? Address { get; set; }
}