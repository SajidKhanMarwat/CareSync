using Microsoft.AspNetCore.Identity;

namespace CareSync.DataLayer.Entities;

public class T_Users : IdentityUser<Guid>, BaseEntity
{
    public Guid RoleID { get; set; }
    public int? LoginID { get; set; }
    public string? ArabicUserName { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? ProfileImage { get; set; }
    public string? Gender { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public bool? IsActive { get; set; } = true;
    public string? RoleType { get; set; }
    public DateTime? LastLogin { get; set; }
    public string? Address { get; set; }
    public int? Age { get; set; }

    // Navigation properties
    public virtual T_Roles Role { get; set; }
    public virtual ICollection<T_DoctorDetails> DoctorDetails { get; set; } = new List<T_DoctorDetails>();
    public virtual ICollection<T_PatientDetails> PatientDetails { get; set; } = new List<T_PatientDetails>();
    public virtual ICollection<T_Lab> Labs { get; set; } = new List<T_Lab>();
}
