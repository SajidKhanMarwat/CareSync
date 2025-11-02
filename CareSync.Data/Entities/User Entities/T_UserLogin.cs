using Microsoft.AspNetCore.Identity;

namespace CareSync.DataLayer.Entities;

public class T_UserLogin : IdentityUserLogin<Guid>
{
    // Navigation properties
    public virtual T_Users User { get; set; }
}