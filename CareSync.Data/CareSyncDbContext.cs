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

        #region Identity Entity Configuration

        builder.Entity<T_Users>().ToTable("T_Users");
        builder.Entity<T_Roles>().ToTable("T_Roles");
        builder.Entity<T_UserRole>().ToTable("T_UserRoles");
        builder.Entity<T_UserLogin>().ToTable("T_UserLogins");
        builder.Entity<T_UserToken>().ToTable("T_UserTokens");
        builder.Entity<T_RoleClaim>().ToTable("T_RoleClaims");

        builder.Entity<T_Users>().Property(x => x.Id).HasMaxLength(128);
        builder.Entity<T_Roles>().Property(x => x.Id).HasMaxLength(128);
        builder.Entity<T_UserRole>().Property(x => x.UserId).HasMaxLength(128);
        builder.Entity<T_UserRole>().Property(x => x.RoleId).HasMaxLength(128);
        builder.Entity<T_RoleClaim>().Property(x => x.Id).HasMaxLength(128);

        builder.Entity<T_UserRole>(entity =>
        {
            entity.HasKey(ur => new { ur.UserId, ur.RoleId });

            // Navigation to T_Users
            entity.HasOne(ur => ur.User)
                  .WithMany(u => u.UserRole)
                  .HasForeignKey(ur => ur.UserId)
                  .OnDelete(DeleteBehavior.Restrict);

            // Navigation to T_Roles
            entity.HasOne(ur => ur.Role)
                  .WithMany(r => r.UserRoles)
                  .HasForeignKey(ur => ur.RoleId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        #endregion

        #region Medical Entity Configuration

        // Primary Keys
        builder.Entity<T_PatientDetails>().HasKey(p => p.PatientID);
        builder.Entity<T_DoctorDetails>().HasKey(d => d.DoctorID);
        builder.Entity<T_Appointments>().HasKey(a => a.AppointmentID);
        builder.Entity<T_Prescriptions>().HasKey(p => p.PrescriptionID);
        builder.Entity<T_PrescriptionItems>().HasKey(pi => pi.PrescriptionItemID);
        builder.Entity<T_AdditionalNotes>().HasKey(pi => pi.NoteID);
        builder.Entity<T_ChronicDiseases>().HasKey(pi => pi.ChronicDiseaseID);
        builder.Entity<T_Lab>().HasKey(pi => pi.LabID);
        builder.Entity<T_LabReports>().HasKey(pi => pi.LabReportID);
        builder.Entity<T_LabRequests>().HasKey(pi => pi.LabRequestID);
        builder.Entity<T_LabServices>().HasKey(pi => pi.LabServiceID);
        builder.Entity<T_LifestyleInfo>().HasKey(pi => pi.LifestyleID);
        builder.Entity<T_MedicalFollowUp>().HasKey(pi => pi.FollowUpID);
        builder.Entity<T_MedicalHistory>().HasKey(pi => pi.MedicalHistoryID);
        builder.Entity<T_MedicationPlan>().HasKey(pi => pi.MedicationID);
        builder.Entity<T_PatientReports>().HasKey(pi => pi.PatientReportID);
        builder.Entity<T_PatientVitals>().HasKey(pi => pi.VitalID);
        builder.Entity<T_Qualifications>().HasKey(pi => pi.QualificationID);

        builder.Entity<T_Appointments>()
            .HasOne(a => a.Doctor)
            .WithMany(d => d.Appointments)
            .HasForeignKey(a => a.DoctorID);

        builder.Entity<T_Appointments>()
            .HasOne(a => a.Patient)
            .WithMany(p => p.Appointments)
            .HasForeignKey(a => a.PatientID);

        builder.Entity<T_Prescriptions>(entity =>
        {
            entity.HasKey(p => p.PrescriptionID);

            entity.HasOne(p => p.Appointment)
                  .WithMany(a => a.Prescriptions)
                  .HasForeignKey(p => p.AppointmentID)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(p => p.Doctor)
                  .WithMany(d => d.Prescriptions)
                  .HasForeignKey(p => p.DoctorID)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(p => p.Patient)
                  .WithMany(pat => pat.Prescriptions)
                  .HasForeignKey(p => p.PatientID)
                  .OnDelete(DeleteBehavior.Restrict);
        });


        builder.Entity<T_PrescriptionItems>()
            .HasOne(pi => pi.Prescription)
            .WithMany(p => p.PrescriptionItems)
            .HasForeignKey(pi => pi.PrescriptionID);

        #endregion

        #region Global Query Filters for Soft Delete

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

        #endregion
    }
}