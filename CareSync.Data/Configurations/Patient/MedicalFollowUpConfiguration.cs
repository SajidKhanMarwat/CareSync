using CareSync.DataLayer.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CareSync.DataLayer.Configurations.Patient;

public class MedicalFollowUpConfiguration : IEntityTypeConfiguration<T_MedicalFollowUp>
{
    public void Configure(EntityTypeBuilder<T_MedicalFollowUp> builder)
    {
        builder.ToTable("T_MedicalFollowUps");
        builder.HasKey(mf => mf.FollowUpID);
        builder.Property(mf => mf.FollowUpID).ValueGeneratedOnAdd();

        // builder.Property(mf => mf.FollowUpDate).IsRequired(); // Property doesn't exist
        // builder.Property(mf => mf.Reason).HasMaxLength(500); // Property doesn't exist
        // builder.Property(mf => mf.Notes).HasMaxLength(1000); // Property doesn't exist

        builder.Property(mf => mf.CreatedBy).HasMaxLength(128);
        builder.Property(mf => mf.UpdatedBy).HasMaxLength(128);
        builder.Property(mf => mf.IsDeleted).HasDefaultValue(false);
        builder.Property(mf => mf.CreatedOn).HasDefaultValueSql("GETUTCDATE()");

        builder.HasOne(mf => mf.Patient)
            .WithMany(p => p.MedicalFollowUps)
            .HasForeignKey(mf => mf.PatientID)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(mf => mf.PatientID);
        // builder.HasIndex(mf => mf.FollowUpDate); // Property doesn't exist
        builder.HasQueryFilter(e => !e.IsDeleted);
    }
}
