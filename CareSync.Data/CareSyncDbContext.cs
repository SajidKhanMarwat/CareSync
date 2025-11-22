using CareSync.DataLayer.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace CareSync.DataLayer;

public class CareSyncDbContext : IdentityDbContext<T_Users, T_Roles, string, IdentityUserClaim<string>, T_UserRole, T_UserLogin, T_RoleClaim, T_UserToken>
{
    public CareSyncDbContext(DbContextOptions<CareSyncDbContext> options) : base(options)
    {
    }

    #region DbSets

    // Identity entities
    public DbSet<T_Users> T_Users { get; set; }
    public DbSet<T_Roles> T_Roles { get; set; }
    public DbSet<T_UserRole> T_UserRoles { get; set; }
    public DbSet<T_UserLogin> T_UserLogins { get; set; }
    public DbSet<T_UserToken> T_UserTokens { get; set; }
    public DbSet<T_RoleClaim> T_RoleClaims { get; set; }

    // Medical entities
    public DbSet<T_PatientDetails> T_PatientDetails { get; set; }
    public DbSet<T_DoctorDetails> T_DoctorDetails { get; set; }
    public DbSet<T_Appointments> T_Appointments { get; set; }
    public DbSet<T_Prescriptions> T_Prescriptions { get; set; }
    public DbSet<T_PrescriptionItems> T_PrescriptionItems { get; set; }

    // Patient-related entities
    public DbSet<T_AdditionalNotes> T_AdditionalNotes { get; set; }
    public DbSet<T_ChronicDiseases> T_ChronicDiseases { get; set; }
    public DbSet<T_LifestyleInfo> T_LifestyleInfo { get; set; }
    public DbSet<T_MedicalFollowUp> T_MedicalFollowUps { get; set; }
    public DbSet<T_MedicalHistory> T_MedicalHistories { get; set; }
    public DbSet<T_MedicationPlan> T_MedicationPlans { get; set; }
    public DbSet<T_PatientVitals> T_PatientVitals { get; set; }
    public DbSet<T_PatientReports> T_PatientReports { get; set; }

    // Doctor-related entities
    public DbSet<T_Qualifications> T_Qualifications { get; set; }

    // Lab entities
    public DbSet<T_Lab> T_Labs { get; set; }
    public DbSet<T_LabServices> T_LabServices { get; set; }
    public DbSet<T_LabRequests> T_LabRequests { get; set; }
    public DbSet<T_LabReports> T_LabReports { get; set; }

    #endregion

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}