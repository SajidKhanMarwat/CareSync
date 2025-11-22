using CareSync.DataLayer.Entities;
using CareSync.Shared.Enums.Appointment;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CareSync.DataLayer.Configurations.Medical;

/// <summary>
/// Entity configuration for T_Appointments with enum string conversion
/// </summary>
public class AppointmentConfiguration : IEntityTypeConfiguration<T_Appointments>
{
    public void Configure(EntityTypeBuilder<T_Appointments> builder)
    {
        // Table name
        builder.ToTable("T_Appointments");

        // Primary key
        builder.HasKey(a => a.AppointmentID);
        builder.Property(a => a.AppointmentID).ValueGeneratedOnAdd();

        // Properties
        builder.Property(a => a.Reason).HasMaxLength(500);
        builder.Property(a => a.Notes).HasMaxLength(1000);

        // Enum conversions to string
        builder.Property(a => a.Status)
            .HasConversion<string>()
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(a => a.AppointmentType)
            .HasConversion<string>()
            .HasMaxLength(50)
            .IsRequired();

        // Audit fields
        builder.Property(a => a.CreatedBy).HasMaxLength(128);
        builder.Property(a => a.UpdatedBy).HasMaxLength(128);
        builder.Property(a => a.IsDeleted).HasDefaultValue(false);
        builder.Property(a => a.CreatedOn).HasDefaultValueSql("GETUTCDATE()");

        // Relationships
        builder.HasOne(a => a.Doctor)
            .WithMany(d => d.Appointments)
            .HasForeignKey(a => a.DoctorID)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(a => a.Patient)
            .WithMany(p => p.Appointments)
            .HasForeignKey(a => a.PatientID)
            .OnDelete(DeleteBehavior.Restrict);

        // Indexes
        builder.HasIndex(a => a.DoctorID);
        builder.HasIndex(a => a.PatientID);
        builder.HasIndex(a => a.AppointmentDate);
        builder.HasIndex(a => a.Status);

        // Query filter for soft delete
        builder.HasQueryFilter(e => !e.IsDeleted);
    }
}
