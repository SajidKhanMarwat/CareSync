using CareSync.DataLayer.Entities;
using CareSync.Shared.Enums.Patient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CareSync.DataLayer.Configurations.Medical;

/// <summary>
/// Entity configuration for T_PatientDetails with enum string conversion
/// </summary>
public class PatientDetailsConfiguration : IEntityTypeConfiguration<T_PatientDetails>
{
    public void Configure(EntityTypeBuilder<T_PatientDetails> builder)
    {
        // Table name
        builder.ToTable("T_PatientDetails");

        // Primary key
        builder.HasKey(p => p.PatientID);
        builder.Property(p => p.PatientID).ValueGeneratedOnAdd();

        // Properties
        builder.Property(p => p.UserID).HasMaxLength(128);
        builder.Property(p => p.BloodGroup).HasMaxLength(10);
        builder.Property(p => p.Occupation).HasMaxLength(100);
        builder.Property(p => p.EmergencyContactName).HasMaxLength(100);
        builder.Property(p => p.EmergencyContactNumber).HasMaxLength(20);
        builder.Property(p => p.RelationshipToEmergency).HasMaxLength(50);

        // Enum conversion to string
        builder.Property(p => p.MaritalStatus)
            .HasConversion<string>()
            .HasMaxLength(50);

        // Audit fields
        builder.Property(p => p.CreatedBy).HasMaxLength(128);
        builder.Property(p => p.UpdatedBy).HasMaxLength(128);
        builder.Property(p => p.IsDeleted).HasDefaultValue(false);
        builder.Property(p => p.CreatedOn).HasDefaultValueSql("GETUTCDATE()");

        // Relationships
        builder.HasOne(p => p.User)
            .WithMany()
            .HasForeignKey(p => p.UserID)
            .OnDelete(DeleteBehavior.Restrict);

        // Indexes
        builder.HasIndex(p => p.UserID).IsUnique();

        // Query filter for soft delete
        builder.HasQueryFilter(e => !e.IsDeleted);
    }
}
