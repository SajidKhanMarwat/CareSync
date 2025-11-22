using CareSync.DataLayer.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CareSync.DataLayer.Configurations.Patient;

public class LifestyleInfoConfiguration : IEntityTypeConfiguration<T_LifestyleInfo>
{
    public void Configure(EntityTypeBuilder<T_LifestyleInfo> builder)
    {
        builder.ToTable("T_LifestyleInfo");
        builder.HasKey(li => li.LifestyleID);
        builder.Property(li => li.LifestyleID).ValueGeneratedOnAdd();

        builder.Property(li => li.IsSmoking);
        builder.Property(li => li.ExerciseFrequency).HasMaxLength(50);
        builder.Property(li => li.ExerciseType).HasMaxLength(100);
        builder.Property(li => li.DailyActivity).HasMaxLength(200);
        builder.Property(li => li.IsOnDiet);
        builder.Property(li => li.DietType).HasMaxLength(100);
        builder.Property(li => li.SleepHours);
        builder.Property(li => li.Occupation).HasMaxLength(100);

        builder.Property(li => li.CreatedBy).HasMaxLength(128);
        builder.Property(li => li.UpdatedBy).HasMaxLength(128);
        builder.Property(li => li.IsDeleted).HasDefaultValue(false);
        builder.Property(li => li.CreatedOn).HasDefaultValueSql("GETUTCDATE()");

        builder.HasOne(li => li.Patient)
            .WithMany(p => p.LifestyleInfo)
            .HasForeignKey(li => li.PatientID)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(li => li.PatientID);
        builder.HasQueryFilter(e => !e.IsDeleted);
    }
}
