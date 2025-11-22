using CareSync.DataLayer.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CareSync.DataLayer.Configurations.Patient;

/// <summary>
/// Entity configuration for T_PatientVitals
/// </summary>
public class PatientVitalsConfiguration : IEntityTypeConfiguration<T_PatientVitals>
{
    public void Configure(EntityTypeBuilder<T_PatientVitals> builder)
    {
        builder.ToTable("T_PatientVitals");
        builder.HasKey(pv => pv.VitalID);
        builder.Property(pv => pv.VitalID).ValueGeneratedOnAdd();

        builder.Property(pv => pv.Height).HasColumnType("decimal(5,2)");
        builder.Property(pv => pv.Weight).HasColumnType("decimal(5,2)");
        // builder.Property(pv => pv.BMI).HasColumnType("decimal(5,2)"); // Property doesn't exist
        builder.Property(pv => pv.BloodPressure).HasMaxLength(50);
        builder.Property(pv => pv.PulseRate).HasColumnType("decimal(5,2)");
        // builder.Property(pv => pv.Temperature).HasColumnType("decimal(4,2)"); // Property doesn't exist
        // builder.Property(pv => pv.RespiratoryRate).HasColumnType("decimal(5,2)"); // Property doesn't exist
        // builder.Property(pv => pv.OxygenSaturation).HasColumnType("decimal(5,2)"); // Property doesn't exist
        // builder.Property(pv => pv.Notes).HasMaxLength(500); // Property doesn't exist

        builder.Property(pv => pv.CreatedBy).HasMaxLength(128);
        builder.Property(pv => pv.UpdatedBy).HasMaxLength(128);
        builder.Property(pv => pv.IsDeleted).HasDefaultValue(false);
        builder.Property(pv => pv.CreatedOn).HasDefaultValueSql("GETUTCDATE()");

        builder.HasOne(pv => pv.Patient)
            .WithMany(p => p.PatientVitals)
            .HasForeignKey(pv => pv.PatientID)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(pv => pv.PatientID);
        // builder.HasIndex(pv => pv.RecordedDate); // Property doesn't exist
        builder.HasQueryFilter(e => !e.IsDeleted);
    }
}
