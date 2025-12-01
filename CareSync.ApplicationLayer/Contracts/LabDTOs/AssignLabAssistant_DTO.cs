using System.ComponentModel.DataAnnotations;

namespace CareSync.ApplicationLayer.Contracts.LabDTOs;

/// <summary>
/// DTO for assigning a Lab Assistant to a Laboratory
/// </summary>
public class AssignLabAssistant_DTO
{
    /// <summary>
    /// User ID of the Lab Assistant
    /// </summary>
    [Required(ErrorMessage = "Lab Assistant ID is required")]
    public required string LabAssistantId { get; set; }

    /// <summary>
    /// ID of the Laboratory the assistant will work at
    /// </summary>
    [Required(ErrorMessage = "Lab ID is required")]
    public required int LabId { get; set; }

    /// <summary>
    /// User ID of the person creating this assignment
    /// </summary>
    public string? CreatedBy { get; set; }
}
