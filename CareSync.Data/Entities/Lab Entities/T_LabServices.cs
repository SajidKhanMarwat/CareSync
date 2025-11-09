namespace CareSync.DataLayer.Entities;

public class T_LabServices : BaseEntity
{
    public int ServiceID { get; set; }
    public int LabID { get; set; }
    public string? ServiceName { get; set; }
    public string? Description { get; set; }
    public string? Category { get; set; }
    public string? SampleType { get; set; }
    public decimal? Price { get; set; }
    public string? EstimatedTime { get; set; }

    //// Navigation properties
    //public virtual T_Lab Lab { get; set; }
    //public virtual ICollection<T_LabRequests> LabRequests { get; set; } = new List<T_LabRequests>();
}
