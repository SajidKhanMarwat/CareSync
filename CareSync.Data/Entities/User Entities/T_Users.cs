using Microsoft.AspNetCore.Identity;

namespace CareSync.DataLayer.Entities;

public class T_Users : IdentityUser<string>
{
    public required string FirstName { get; set; }
    public string? MiddleName { get; set; }
    public string? LastName { get; set; }
    public string? FullName { get; set; }
    public bool IsActive { get; set; }
    public required string CreatedBy { get; set; }
    public DateTime CreatedOn { get; set; } = DateTime.UtcNow;
    public string? UpdatedBy { get; set; }
    public DateTime? UpdatedOn { get; set; }
}
