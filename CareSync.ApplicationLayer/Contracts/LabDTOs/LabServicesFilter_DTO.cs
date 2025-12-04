using System.ComponentModel.DataAnnotations;

namespace CareSync.ApplicationLayer.Contracts.LabDTOs;

/// <summary>
/// Filter options for lab services
/// </summary>
public class LabServicesFilter_DTO
{
    /// <summary>
    /// Minimum allowed page size
    /// </summary>
    public const int MinPageSize = 1;
    
    /// <summary>
    /// Maximum allowed page size to prevent performance issues
    /// </summary>
    public const int MaxPageSize = 100;
    
    /// <summary>
    /// Default page size
    /// </summary>
    public const int DefaultPageSize = 10;
    
    /// <summary>
    /// Filter by laboratory ID
    /// </summary>
    public int? LabId { get; set; }
    
    /// <summary>
    /// Filter by category
    /// </summary>
    [StringLength(100)]
    public string? Category { get; set; }
    
    /// <summary>
    /// Filter by active status
    /// </summary>
    public bool? IsActive { get; set; }
    
    /// <summary>
    /// Search term for service name or description
    /// </summary>
    [StringLength(200)]
    public string? SearchTerm { get; set; }
    
    /// <summary>
    /// Page number (1-based)
    /// </summary>
    [Range(1, int.MaxValue, ErrorMessage = "Page number must be at least 1")]
    public int Page { get; set; } = 1;
    
    private int _pageSize = DefaultPageSize;
    
    /// <summary>
    /// Number of items per page (1-100)
    /// </summary>
    [Range(MinPageSize, MaxPageSize, ErrorMessage = "Page size must be between 1 and 100")]
    public int PageSize
    {
        get => _pageSize;
        set => _pageSize = value < MinPageSize ? DefaultPageSize : (value > MaxPageSize ? MaxPageSize : value);
    }
    
    /// <summary>
    /// Sort by field (ServiceName, Price, Category, etc.)
    /// </summary>
    [StringLength(50)]
    public string? SortBy { get; set; }
    
    /// <summary>
    /// Sort direction (asc or desc)
    /// </summary>
    public bool SortDescending { get; set; }
}
