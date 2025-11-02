namespace CareSync.DataLayer.Entities;

public class T_RoleRights : BaseEntity
{
    public int RoleRightID { get; set; }
    public Guid RoleID { get; set; }
    public int RightID { get; set; }

    // Navigation properties
    public virtual T_Roles Role { get; set; }
    public virtual T_Rights Right { get; set; }
}
