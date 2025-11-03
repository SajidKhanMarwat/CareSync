using CareSync.DataLayer.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CareSync.DataLayer;

/// <summary>
/// Database context for the CareSync medical management system.
/// Extends IdentityDbContext to provide ASP.NET Core Identity integration with custom entities.
/// Manages all medical entities including users, patients, doctors, appointments, and prescriptions.
/// </summary>
public class CareSyncDbContext : IdentityDbContext<T_Users, T_Roles, Guid, IdentityUserClaim<Guid>, T_UserRole, T_UserLogin, T_RoleClaim, T_UserToken>
{
    public CareSyncDbContext(DbContextOptions<CareSyncDbContext> options) : base(options)
    {        
    }

    // Identity entities
    public DbSet<T_Users> T_Users { get; set; }
    public DbSet<T_Roles> T_Roles { get; set; }
    //public DbSet<T_UserClaim> T_UserClaims { get; set; }
    public DbSet<T_UserRole> T_UserRoles { get; set; }
    public DbSet<T_UserLogin> T_UserLogins { get; set; }
    public DbSet<T_UserToken> T_UserTokens { get; set; }
    public DbSet<T_RoleClaim> T_RoleClaims { get; set; }
    //public DbSet<T_Rights> T_Rights { get; set; }
    //public DbSet<T_RoleRights> T_RoleRights { get; set; }

    // Medical entities
    public DbSet<T_PatientDetails> T_PatientDetails { get; set; }
    public DbSet<T_DoctorDetails> T_DoctorDetails { get; set; }
    public DbSet<T_Appointments> T_Appointments { get; set; }
    public DbSet<T_Prescriptions> T_Prescriptions { get; set; }
    public DbSet<T_PrescriptionItems> T_PrescriptionItems { get; set; }
    
    // Patient related entities
    public DbSet<T_AdditionalNotes> T_AdditionalNotes { get; set; }
    public DbSet<T_ChronicDiseases> T_ChronicDiseases { get; set; }
    public DbSet<T_LifestyleInfo> T_LifestyleInfo { get; set; }
    public DbSet<T_MedicalFollowUp> T_MedicalFollowUps { get; set; }
    public DbSet<T_MedicalHistory> T_MedicalHistories { get; set; }
    public DbSet<T_MedicationPlan> T_MedicationPlans { get; set; }
    public DbSet<T_PatientVitals> T_PatientVitals { get; set; }
    public DbSet<T_PatientReports> T_PatientReports { get; set; }
    
    // Doctor related entities
    public DbSet<T_Qualifications> T_Qualifications { get; set; }
    
    // Lab entities (to be added when documented)
    public DbSet<T_Lab> T_Labs { get; set; }
    public DbSet<T_LabServices> T_LabServices { get; set; }
    public DbSet<T_LabRequests> T_LabRequests { get; set; }
    public DbSet<T_LabReports> T_LabReports { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        #region Identity Entity Configuration
        // Configure Identity table names to match database schema
        builder.Entity<T_Users>().ToTable("T_Users");
        builder.Entity<T_Roles>().ToTable("T_Roles");
        //builder.Entity<T_UserClaim>().ToTable("T_UserClaims");
        builder.Entity<T_UserRole>().ToTable("T_UserRoles");
        builder.Entity<T_UserLogin>().ToTable("T_UserLogins");
        builder.Entity<T_UserToken>().ToTable("T_UserTokens");
        builder.Entity<T_RoleClaim>().ToTable("T_RoleClaims");
        #endregion

        #region Medical Entity Configuration
        // Configure primary keys and relationships
        builder.Entity<T_PatientDetails>()
            .HasKey(p => p.PatientID);

        builder.Entity<T_DoctorDetails>()
            .HasKey(d => d.DoctorID);

        builder.Entity<T_Appointments>()
            .HasKey(a => a.AppointmentID);

        builder.Entity<T_Prescriptions>()
            .HasKey(p => p.PrescriptionID);

        builder.Entity<T_PrescriptionItems>()
            .HasKey(pi => pi.PrescriptionItemID);

        // Configure relationships
        builder.Entity<T_Users>()
            .HasOne(u => u.Role)
            .WithMany(r => r.Users)
            .HasForeignKey(u => u.RoleID);

        builder.Entity<T_PatientDetails>()
            .HasOne(p => p.User)
            .WithMany(u => u.PatientDetails)
            .HasForeignKey(p => p.UserID);

        builder.Entity<T_DoctorDetails>()
            .HasOne(d => d.User)
            .WithMany(u => u.DoctorDetails)
            .HasForeignKey(d => d.UserID);

        builder.Entity<T_Appointments>()
            .HasOne(a => a.Doctor)
            .WithMany(d => d.Appointments)
            .HasForeignKey(a => a.DoctorID);

        builder.Entity<T_Appointments>()
            .HasOne(a => a.Patient)
            .WithMany(p => p.Appointments)
            .HasForeignKey(a => a.PatientID);

        builder.Entity<T_Prescriptions>()
            .HasOne(p => p.Appointment)
            .WithMany(a => a.Prescriptions)
            .HasForeignKey(p => p.AppointmentID);

        builder.Entity<T_PrescriptionItems>()
            .HasOne(pi => pi.Prescription)
            .WithMany(p => p.PrescriptionItems)
            .HasForeignKey(pi => pi.PrescriptionID);

        //// Configure Rights and RoleRights
        //builder.Entity<T_Rights>()
        //    .HasKey(r => r.RightID);

        //builder.Entity<T_RoleRights>()
        //    .HasKey(rr => rr.RoleRightID);

        //builder.Entity<T_RoleRights>()
        //    .HasOne(rr => rr.Role)
        //    .WithMany(r => r.RoleRights)
        //    .HasForeignKey(rr => rr.RoleID);

        //builder.Entity<T_RoleRights>()
        //    .HasOne(rr => rr.Right)
        //    .WithMany(r => r.RoleRights)
        //    .HasForeignKey(rr => rr.RightID);
        #endregion

        #region Global Query Filters for Soft Delete
        // Apply global query filters for soft delete
        builder.Entity<T_Users>().HasQueryFilter(e => !e.IsDeleted);
        builder.Entity<T_Roles>().HasQueryFilter(e => !e.IsDeleted);
        builder.Entity<T_PatientDetails>().HasQueryFilter(e => !e.IsDeleted);
        builder.Entity<T_DoctorDetails>().HasQueryFilter(e => !e.IsDeleted);
        builder.Entity<T_Appointments>().HasQueryFilter(e => !e.IsDeleted);
        builder.Entity<T_Prescriptions>().HasQueryFilter(e => !e.IsDeleted);
        builder.Entity<T_PrescriptionItems>().HasQueryFilter(e => !e.IsDeleted);
        builder.Entity<T_AdditionalNotes>().HasQueryFilter(e => !e.IsDeleted);
        builder.Entity<T_ChronicDiseases>().HasQueryFilter(e => !e.IsDeleted);
        builder.Entity<T_LifestyleInfo>().HasQueryFilter(e => !e.IsDeleted);
        builder.Entity<T_MedicalFollowUp>().HasQueryFilter(e => !e.IsDeleted);
        builder.Entity<T_MedicalHistory>().HasQueryFilter(e => !e.IsDeleted);
        builder.Entity<T_MedicationPlan>().HasQueryFilter(e => !e.IsDeleted);
        builder.Entity<T_PatientVitals>().HasQueryFilter(e => !e.IsDeleted);
        builder.Entity<T_Qualifications>().HasQueryFilter(e => !e.IsDeleted);
        //builder.Entity<T_Rights>().HasQueryFilter(e => !e.IsDeleted);
        //builder.Entity<T_RoleRights>().HasQueryFilter(e => !e.IsDeleted);
        #endregion
    }
}