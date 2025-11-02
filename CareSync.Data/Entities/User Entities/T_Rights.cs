namespace CareSync.DataLayer.Entities;

public class T_Rights : BaseEntity
{
    public int RightID { get; set; }
    public string Name { get; set; }
    public string? Description { get; set; }
    public string? Module { get; set; }

    // Navigation properties
    public virtual ICollection<T_RoleRights> RoleRights { get; set; } = new List<T_RoleRights>();
}
