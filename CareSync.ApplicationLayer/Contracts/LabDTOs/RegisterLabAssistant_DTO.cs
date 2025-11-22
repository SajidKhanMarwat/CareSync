using System.ComponentModel.DataAnnotations;

namespace CareSync.ApplicationLayer.Contracts.LabDTOs;

/// <summary>
/// DTO for laboratory/lab assistant registration containing facility and operational information
/// </summary>
public class RegisterLabAssistant_DTO
{
    /// <summary>
    /// Reference to the associated user account
    /// </summary>
    public string? UserID { get; set; }

    /// <summary>
    /// Name of the laboratory
    /// </summary>
    [Required(ErrorMessage = "Lab name is required")]
    public required string LabName { get; set; }

    /// <summary>
    /// Arabic translation of lab name
    /// </summary>
    public string? ArabicLabName { get; set; }

    /// <summary>
    /// Physical address of the laboratory
    /// </summary>
    public string? LabAddress { get; set; }

    /// <summary>
    /// Arabic translation of lab address
    /// </summary>
    public string? ArabicLabAddress { get; set; }

    /// <summary>
    /// Location or area (e.g., city, district)
    /// </summary>
    public string? Location { get; set; }

    /// <summary>
    /// Laboratory contact phone number
    /// </summary>
    [Phone(ErrorMessage = "Please enter a valid phone number")]
    public string? ContactNumber { get; set; }

    /// <summary>
    /// Laboratory email address
    /// </summary>
    [EmailAddress(ErrorMessage = "Please enter a valid email address")]
    public string? Email { get; set; }

    /// <summary>
    /// Laboratory license number
    /// </summary>
    public string? LicenseNumber { get; set; }

    /// <summary>
    /// Lab opening time
    /// </summary>
    public TimeSpan? OpeningTime { get; set; }

    /// <summary>
    /// Lab closing time
    /// </summary>
    public TimeSpan? ClosingTime { get; set; }

    /// <summary>
    /// User ID of the person creating this record
    /// </summary>
    public required string CreatedBy { get; set; }

    //public List<LabService_DTO>? LabServices { get; set; }
}
