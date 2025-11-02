using Microsoft.AspNetCore.Identity;

namespace CareSync.DataLayer.Entities;

public class T_UserToken : IdentityUserToken<Guid>
{
    // Navigation properties
    public virtual T_Users User { get; set; }
}