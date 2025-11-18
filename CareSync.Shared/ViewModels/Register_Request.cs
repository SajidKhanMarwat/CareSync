using CareSync.Shared.Enums;
using System.ComponentModel.DataAnnotations;

public class Register_Request : IValidatableObject
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
    public Gender_Enum Gender { get; set; }

    [Display(Name = "Date of Birth")]
    [DataType(DataType.Date)]
    public DateTime? DateOfBirth { get; set; }

    [Display(Name = "Age")]
    [Range(0, 150, ErrorMessage = "Age must be between 0 and 150 years")]
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

    /// <summary>
    /// Calculate age based on date of birth
    /// </summary>
    public void CalculateAge()
    {
        if (DateOfBirth.HasValue)
        {
            var today = DateTime.Today;
            var birthDate = DateOfBirth.Value.Date;

            if (birthDate <= today)
            {
                var age = today.Year - birthDate.Year;

                // Check if birthday hasn't occurred this year yet
                if (birthDate.Date > today.AddYears(-age))
                {
                    age--;
                }

                Age = age;
            }
            else
            {
                Age = null; // Future date
            }
        }
        else
        {
            Age = null;
        }
    }

    /// <summary>
    /// Server-side validation for age calculation and business rules
    /// </summary>
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        var results = new List<ValidationResult>();

        // Validate Date of Birth
        if (DateOfBirth.HasValue)
        {
            var today = DateTime.Today;
            var birthDate = DateOfBirth.Value.Date;

            // Check if birth date is in the future
            if (birthDate > today)
            {
                results.Add(new ValidationResult(
                    "Birth date cannot be in the future.",
                    new[] { nameof(DateOfBirth) }));
            }
            // Check if birth date is too far in the past (more than 150 years)
            else if (birthDate < today.AddYears(-150))
            {
                results.Add(new ValidationResult(
                    "Birth date cannot be more than 150 years ago.",
                    new[] { nameof(DateOfBirth) }));
            }
            else
            {
                // Calculate and validate age
                CalculateAge();

                if (Age.HasValue)
                {
                    // Business rule: Minimum age for registration (e.g., 13 years)
                    //if (Age < 13)
                    //{
                    //    results.Add(new ValidationResult(
                    //        "You must be at least 13 years old to register.",
                    //        new[] { nameof(DateOfBirth), nameof(Age) }));
                    //}
                }
            }
        }

        // Validate Age if provided directly (should match calculated age)
        if (DateOfBirth.HasValue && Age.HasValue)
        {
            var calculatedAge = CalculateAgeFromDate(DateOfBirth.Value);
            if (Math.Abs(Age.Value - calculatedAge) > 1) // Allow 1 year tolerance
            {
                results.Add(new ValidationResult(
                    "Age does not match the provided date of birth.",
                    new[] { nameof(Age) }));
            }
        }

        return results;
    }

    /// <summary>
    /// Helper method to calculate age from a specific date
    /// </summary>
    private static int CalculateAgeFromDate(DateTime birthDate)
    {
        var today = DateTime.Today;
        var age = today.Year - birthDate.Year;

        if (birthDate.Date > today.AddYears(-age))
        {
            age--;
        }

        return age;
    }
}
