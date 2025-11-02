using Microsoft.AspNetCore.Identity;

namespace CareSync.DataLayer.Entities;

public class T_UserRole : IdentityUserRole<Guid>
{
    // Navigation properties
    public virtual T_Users User { get; set; }
    public virtual T_Roles Role { get; set; }
}