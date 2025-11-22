using CareSync.DataLayer.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CareSync.DataLayer.Configurations.Medical;

/// <summary>
/// Entity configuration for T_DoctorDetails
/// </summary>
public class DoctorDetailsConfiguration : IEntityTypeConfiguration<T_DoctorDetails>
{
    public void Configure(EntityTypeBuilder<T_DoctorDetails> builder)
    {
        // Table name
        builder.ToTable("T_DoctorDetails");

        // Primary key
        builder.HasKey(d => d.DoctorID);
        builder.Property(d => d.DoctorID).ValueGeneratedOnAdd();

        // Properties
        builder.Property(d => d.UserID).HasMaxLength(128);
        builder.Property(d => d.Specialization).HasMaxLength(100);
        builder.Property(d => d.ArabicSpecialization).HasMaxLength(100);
        builder.Property(d => d.ClinicAddress).HasMaxLength(500);
        builder.Property(d => d.ArabicClinicAddress).HasMaxLength(500);
        builder.Property(d => d.LicenseNumber).IsRequired().HasMaxLength(50);
        builder.Property(d => d.QualificationSummary).HasMaxLength(1000);
        builder.Property(d => d.HospitalAffiliation).HasMaxLength(200);
        builder.Property(d => d.AvailableDays).HasMaxLength(100);
        builder.Property(d => d.StartTime).HasMaxLength(10);
        builder.Property(d => d.EndTime).HasMaxLength(10);

        // Audit fields
        builder.Property(d => d.CreatedBy).HasMaxLength(128);
        builder.Property(d => d.UpdatedBy).HasMaxLength(128);
        builder.Property(d => d.IsDeleted).HasDefaultValue(false);
        builder.Property(d => d.CreatedOn).HasDefaultValueSql("GETUTCDATE()");

        // Relationships
        builder.HasOne(d => d.User)
            .WithMany()
            .HasForeignKey(d => d.UserID)
            .OnDelete(DeleteBehavior.Restrict);

        // Indexes
        builder.HasIndex(d => d.UserID).IsUnique();
        builder.HasIndex(d => d.LicenseNumber).IsUnique();
        builder.HasIndex(d => d.Specialization);

        // Query filter for soft delete
        builder.HasQueryFilter(e => !e.IsDeleted);
    }
}
