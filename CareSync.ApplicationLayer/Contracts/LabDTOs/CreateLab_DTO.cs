using System.ComponentModel.DataAnnotations;

namespace CareSync.ApplicationLayer.Contracts.LabDTOs;

/// <summary>
/// DTO for creating a new laboratory facility (admin-initiated)
/// </summary>
public class CreateLab_DTO
{
    /// <summary>
    /// Name of the laboratory
    /// </summary>
    [Required(ErrorMessage = "Lab name is required")]
    [StringLength(200, ErrorMessage = "Lab name cannot exceed 200 characters")]
    public required string LabName { get; set; }

    /// <summary>
    /// Arabic translation of lab name
    /// </summary>
    [StringLength(200, ErrorMessage = "Arabic lab name cannot exceed 200 characters")]
    public string? ArabicLabName { get; set; }

    /// <summary>
    /// Physical address of the laboratory
    /// </summary>
    [StringLength(500, ErrorMessage = "Address cannot exceed 500 characters")]
    public string? LabAddress { get; set; }

    /// <summary>
    /// Arabic translation of lab address
    /// </summary>
    [StringLength(500, ErrorMessage = "Arabic address cannot exceed 500 characters")]
    public string? ArabicLabAddress { get; set; }

    /// <summary>
    /// Location or area (e.g., city, district)
    /// </summary>
    [StringLength(100, ErrorMessage = "Location cannot exceed 100 characters")]
    public string? Location { get; set; }

    /// <summary>
    /// Laboratory contact phone number
    /// </summary>
    [Phone(ErrorMessage = "Please enter a valid phone number")]
    [StringLength(20, ErrorMessage = "Phone number cannot exceed 20 characters")]
    public string? ContactNumber { get; set; }

    /// <summary>
    /// Laboratory email address
    /// </summary>
    [EmailAddress(ErrorMessage = "Please enter a valid email address")]
    [StringLength(100, ErrorMessage = "Email cannot exceed 100 characters")]
    public string? Email { get; set; }

    /// <summary>
    /// Laboratory license number
    /// </summary>
    [StringLength(50, ErrorMessage = "License number cannot exceed 50 characters")]
    public string? LicenseNumber { get; set; }

    /// <summary>
    /// Lab opening time
    /// </summary>
    public TimeSpan? OpeningTime { get; set; }

    /// <summary>
    /// Lab closing time
    /// </summary>
    public TimeSpan? ClosingTime { get; set; }
}
