
using System.ComponentModel.DataAnnotations;

namespace CareSync.ApplicationLayer.Contracts.UsersDTOs
{
    public record UserRegisteration_DTO
    {
        public required string FirstName { get; set; }
        public string? MiddleName { get; set; }
        public string? LastName { get; set; }
        public required string Email { get; set; }

        [DataType(DataType.Password)]
        public required string Password { get; set; }
        
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Passwords do not match.")]
        public required string ConfirmPassword { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public bool TwoFactorEnabled { get; set; } = false;
    }
}
