using Microsoft.AspNetCore.Identity;

namespace CareSync.DataLayer.Entities;

public class T_Roles : IdentityRole<Guid>, BaseEntity
{
    public string? ArabicName { get; set; }
    public string? Description { get; set; }

    // Navigation properties
    public virtual ICollection<T_Users> Users { get; set; } = new List<T_Users>();
    public virtual ICollection<T_RoleRights> RoleRights { get; set; } = new List<T_RoleRights>();
}