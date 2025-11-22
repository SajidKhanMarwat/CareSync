using CareSync.DataLayer.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CareSync.DataLayer.Configurations.Medical;

/// <summary>
/// Entity configuration for T_Prescriptions
/// </summary>
public class PrescriptionConfiguration : IEntityTypeConfiguration<T_Prescriptions>
{
    public void Configure(EntityTypeBuilder<T_Prescriptions> builder)
    {
        // Table name
        builder.ToTable("T_Prescriptions");

        // Primary key
        builder.HasKey(p => p.PrescriptionID);
        builder.Property(p => p.PrescriptionID).ValueGeneratedOnAdd();

        // Properties
        builder.Property(p => p.Notes).HasMaxLength(1000);

        // Audit fields
        builder.Property(p => p.CreatedBy).HasMaxLength(128);
        builder.Property(p => p.UpdatedBy).HasMaxLength(128);
        builder.Property(p => p.IsDeleted).HasDefaultValue(false);
        builder.Property(p => p.CreatedOn).HasDefaultValueSql("GETUTCDATE()");

        // Relationships
        builder.HasOne(p => p.Appointment)
            .WithMany(a => a.Prescriptions)
            .HasForeignKey(p => p.AppointmentID)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(p => p.Doctor)
            .WithMany(d => d.Prescriptions)
            .HasForeignKey(p => p.DoctorID)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(p => p.Patient)
            .WithMany(pat => pat.Prescriptions)
            .HasForeignKey(p => p.PatientID)
            .OnDelete(DeleteBehavior.Restrict);

        // Indexes
        builder.HasIndex(p => p.AppointmentID);
        builder.HasIndex(p => p.DoctorID);
        builder.HasIndex(p => p.PatientID);

        // Query filter for soft delete
        builder.HasQueryFilter(e => !e.IsDeleted);
    }
}
