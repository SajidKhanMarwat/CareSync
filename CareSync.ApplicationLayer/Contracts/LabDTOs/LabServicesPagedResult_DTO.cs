using CareSync.ApplicationLayer.Common;

namespace CareSync.ApplicationLayer.Contracts.LabDTOs;

/// <summary>
/// Paginated result for lab services
/// </summary>
public class LabServicesPagedResult_DTO : PagedResult<LabService_DTO>
{
    public int TotalServices { get; set; }
    public int ActiveServices { get; set; }
    public int TotalLaboratories { get; set; }
    public decimal AveragePrice { get; set; }
    
    /// <summary>
    /// Service categories distribution
    /// </summary>
    public Dictionary<string, int> CategoryDistribution { get; set; } = new();
}
