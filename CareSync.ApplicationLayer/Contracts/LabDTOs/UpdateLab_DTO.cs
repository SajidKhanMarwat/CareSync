using System.ComponentModel.DataAnnotations;

namespace CareSync.ApplicationLayer.Contracts.LabDTOs;

/// <summary>
/// DTO for updating laboratory information
/// </summary>
public class UpdateLab_DTO
{
    [Required]
    public int LabId { get; set; }

    [Required(ErrorMessage = "Lab name is required")]
    [StringLength(200, ErrorMessage = "Lab name cannot exceed 200 characters")]
    public required string LabName { get; set; }

    [StringLength(200, ErrorMessage = "Arabic lab name cannot exceed 200 characters")]
    public string? ArabicLabName { get; set; }

    [StringLength(500, ErrorMessage = "Address cannot exceed 500 characters")]
    public string? LabAddress { get; set; }

    [StringLength(500, ErrorMessage = "Arabic address cannot exceed 500 characters")]
    public string? ArabicLabAddress { get; set; }

    [StringLength(100, ErrorMessage = "Location cannot exceed 100 characters")]
    public string? Location { get; set; }

    [Phone(ErrorMessage = "Please enter a valid phone number")]
    [StringLength(20, ErrorMessage = "Phone number cannot exceed 20 characters")]
    public string? ContactNumber { get; set; }

    [EmailAddress(ErrorMessage = "Please enter a valid email address")]
    [StringLength(100, ErrorMessage = "Email cannot exceed 100 characters")]
    public string? Email { get; set; }

    [StringLength(50, ErrorMessage = "License number cannot exceed 50 characters")]
    public string? LicenseNumber { get; set; }

    public TimeSpan? OpeningTime { get; set; }
    public TimeSpan? ClosingTime { get; set; }
}
