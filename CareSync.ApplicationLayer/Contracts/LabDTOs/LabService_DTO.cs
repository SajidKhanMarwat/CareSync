using System.ComponentModel.DataAnnotations;

namespace CareSync.ApplicationLayer.Contracts.LabDTOs;

/// <summary>
/// DTO for laboratory service
/// </summary>
public class LabService_DTO
{
    public int LabServiceId { get; set; }

    [Required]
    public int LabId { get; set; }

    [Required(ErrorMessage = "Service name is required")]
    [StringLength(200, ErrorMessage = "Service name cannot exceed 200 characters")]
    public required string ServiceName { get; set; }

    [StringLength(1000, ErrorMessage = "Description cannot exceed 1000 characters")]
    public string? Description { get; set; }

    [StringLength(100, ErrorMessage = "Category cannot exceed 100 characters")]
    public string? Category { get; set; }

    [StringLength(100, ErrorMessage = "Sample type cannot exceed 100 characters")]
    public string? SampleType { get; set; }

    [StringLength(500, ErrorMessage = "Instructions cannot exceed 500 characters")]
    public string? Instructions { get; set; }

    [Range(0, double.MaxValue, ErrorMessage = "Price must be a positive value")]
    public decimal? Price { get; set; }

    [StringLength(50, ErrorMessage = "Estimated time cannot exceed 50 characters")]
    public string? EstimatedTime { get; set; }

    public string? LabName { get; set; }
    public bool IsActive { get; set; }
}
