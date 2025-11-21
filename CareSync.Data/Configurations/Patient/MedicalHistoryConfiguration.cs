using CareSync.DataLayer.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CareSync.DataLayer.Configurations.Patient;

/// <summary>
/// Entity configuration for T_MedicalHistory
/// </summary>
public class MedicalHistoryConfiguration : IEntityTypeConfiguration<T_MedicalHistory>
{
    public void Configure(EntityTypeBuilder<T_MedicalHistory> builder)
    {
        builder.ToTable("T_MedicalHistories");
        builder.HasKey(mh => mh.MedicalHistoryID);
        builder.Property(mh => mh.MedicalHistoryID).ValueGeneratedOnAdd();

        builder.Property(mh => mh.MainDiagnosis).HasMaxLength(500);
        builder.Property(mh => mh.ChronicDiseases).HasMaxLength(1000);
        builder.Property(mh => mh.Allergies).HasMaxLength(500);
        builder.Property(mh => mh.PastDiseases).HasMaxLength(1000);
        builder.Property(mh => mh.Surgery).HasMaxLength(1000);
        builder.Property(mh => mh.FamilyHistory).HasMaxLength(1000);

        builder.Property(mh => mh.CreatedBy).HasMaxLength(128);
        builder.Property(mh => mh.UpdatedBy).HasMaxLength(128);
        builder.Property(mh => mh.IsDeleted).HasDefaultValue(false);
        builder.Property(mh => mh.CreatedOn).HasDefaultValueSql("GETUTCDATE()");

        builder.HasOne(mh => mh.Patient)
            .WithMany(p => p.MedicalHistories)
            .HasForeignKey(mh => mh.PatientID)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(mh => mh.PatientID);
        builder.HasQueryFilter(e => !e.IsDeleted);
    }
}
