namespace CareSync.DataLayer.Entities;

public class BaseEntity
{
    public bool IsActive { get; set; }
    public DateTime CreatedOn { get; set; }
    public required string CreatedBy { get; set; }
    public DateTime UpdatedOn { get; set; }
    public string? UpdatedBy { get; set; }
    public bool IsDeleted { get; set; } = false;
}
