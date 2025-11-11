
using System.ComponentModel.DataAnnotations;

namespace CareSync.ApplicationLayer.Contracts.UsersDTOs
{
    using System;
    using System.ComponentModel.DataAnnotations;

    public record UserRegisteration_DTO
    {
        [Required(ErrorMessage = "Arabic name is required")]
        public required string ArabicUserName { get; set; }

        [Required(ErrorMessage = "First name is required")]
        public required string FirstName { get; set; }

        public string? MiddleName { get; set; }

        public string? LastName { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        public required string Email { get; set; }

        [Required(ErrorMessage = "Username is required")]
        public string UserName { get; set; } = string.Empty;

        [Phone(ErrorMessage = "Please enter a valid phone number")]
        public string PhoneNumber { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password is required")]
        [DataType(DataType.Password)]
        [StringLength(20, MinimumLength = 6, ErrorMessage = "Password must be between 6–20 characters.")]
        public required string Password { get; set; }

        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Passwords do not match.")]
        public required string ConfirmPassword { get; set; }

        [Required(ErrorMessage = "Gender is required")]
        public int Gender { get; set; } // 1 = Male, 2 = Female, 3 = Other

        [DataType(DataType.Date)]
        public DateTime? DateOfBirth { get; set; }

        public int? Age { get; set; }

        public string? Address { get; set; }

        public bool TwoFactorEnabled { get; set; } = false;
    }

}
