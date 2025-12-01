using CareSync.ApplicationLayer.Contracts.DoctorsDTOs;
using CareSync.ApplicationLayer.Contracts.LabDTOs;
using CareSync.ApplicationLayer.Contracts.PatientsDTOs;
using CareSync.Shared.Enums;
using System.ComponentModel.DataAnnotations;

namespace CareSync.ApplicationLayer.Contracts.UsersDTOs;

public record UserRegisteration_DTO
{
    [Required(ErrorMessage = "Arabic name is required")]
    public required string ArabicUserName { get; set; }

    [Required(ErrorMessage = "First name is required")]
    public required string FirstName { get; set; }
    public string? MiddleName { get; set; }
    public string? LastName { get; set; }

    [Required(ErrorMessage = "Arabic first name is required")]
    public required string ArabicFirstName { get; set; }
    public string? ArabicLastName { get; set; }

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
    public Gender_Enum Gender { get; set; }

    [DataType(DataType.Date)]
    public DateTime? DateOfBirth { get; set; }
    public int? Age { get; set; }
    public string? Address { get; set; }
    public string? ProfileImage { get; set; }
    public bool IsActive { get; set; } = true;
    public RoleType RoleType { get; set; } = RoleType.Patient;
    public DateTime? LastLogin { get; set; }
    public bool TwoFactorEnabled { get; set; } = false;
    public bool RequiresPasswordReset { get; set; } = false;  // Flag to indicate if password reset is required
    public RegisterPatient_DTO? RegisterPatient { get; set; }
    public RegisterDoctor_DTO? RegisterDoctor { get; set; }
    
    /// <summary>
    /// Lab facility details (for RoleType.Lab - facility owner)
    /// </summary>
    public RegisterLab_DTO? RegisterLab { get; set; }
    
    /// <summary>
    /// Lab assignment details (for RoleType.LabAssistant - technician/staff)
    /// </summary>
    public AssignLabAssistant_DTO? AssignLabAssistant { get; set; }
    
    /// <summary>
    /// [Deprecated] Use RegisterLab for Lab role instead
    /// </summary>
    [Obsolete("Use RegisterLab for Lab role and AssignLabAssistant for LabAssistant role")]
    public RegisterLabAssistant_DTO? RegisterLabAssistant { get; set; }
}