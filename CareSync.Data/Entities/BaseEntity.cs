namespace CareSync.DataLayer.Entities;

public class BaseEntity
{
    public bool IsDeleted { get; set; } = false;
    public Guid? CreatedBy { get; set; }
    public DateTime? CreatedOn { get; set; } = DateTime.UtcNow;
    public Guid? UpdatedBy { get; set; }
    public DateTime? UpdatedOn { get; set; }
}
