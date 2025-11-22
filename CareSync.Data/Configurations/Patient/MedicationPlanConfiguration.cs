using CareSync.DataLayer.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CareSync.DataLayer.Configurations.Patient;

public class MedicationPlanConfiguration : IEntityTypeConfiguration<T_MedicationPlan>
{
    public void Configure(EntityTypeBuilder<T_MedicationPlan> builder)
    {
        builder.ToTable("T_MedicationPlans");
        builder.HasKey(mp => mp.MedicationID);
        builder.Property(mp => mp.MedicationID).ValueGeneratedOnAdd();

        builder.Property(mp => mp.MedicationName).IsRequired().HasMaxLength(200);
        builder.Property(mp => mp.Dosage).HasMaxLength(100);
        builder.Property(mp => mp.Frequency).HasMaxLength(100);
        builder.Property(mp => mp.Duration).HasMaxLength(100);
        builder.Property(mp => mp.Instructions).HasMaxLength(500);
        builder.Property(mp => mp.Status);

        builder.Property(mp => mp.CreatedBy).HasMaxLength(128);
        builder.Property(mp => mp.UpdatedBy).HasMaxLength(128);
        builder.Property(mp => mp.IsDeleted).HasDefaultValue(false);
        builder.Property(mp => mp.CreatedOn).HasDefaultValueSql("GETUTCDATE()");

        builder.HasOne(mp => mp.Patient)
            .WithMany(p => p.MedicationPlans)
            .HasForeignKey(mp => mp.PatientID)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(mp => mp.PatientID);
        builder.HasQueryFilter(e => !e.IsDeleted);
    }
}
