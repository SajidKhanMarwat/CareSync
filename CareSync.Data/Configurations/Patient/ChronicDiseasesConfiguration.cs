using CareSync.DataLayer.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CareSync.DataLayer.Configurations.Patient;

public class ChronicDiseasesConfiguration : IEntityTypeConfiguration<T_ChronicDiseases>
{
    public void Configure(EntityTypeBuilder<T_ChronicDiseases> builder)
    {
        builder.ToTable("T_ChronicDiseases");
        builder.HasKey(cd => cd.ChronicDiseaseID);
        builder.Property(cd => cd.ChronicDiseaseID).ValueGeneratedOnAdd();

        builder.Property(cd => cd.DiseaseName).IsRequired().HasMaxLength(200);
        // builder.Property(cd => cd.DiagnosisDate).IsRequired(); // Property doesn't exist
        // builder.Property(cd => cd.Treatment).HasMaxLength(1000); // Property doesn't exist
        // builder.Property(cd => cd.Notes).HasMaxLength(500); // Property doesn't exist

        builder.Property(cd => cd.CreatedBy).HasMaxLength(128);
        builder.Property(cd => cd.UpdatedBy).HasMaxLength(128);
        builder.Property(cd => cd.IsDeleted).HasDefaultValue(false);
        builder.Property(cd => cd.CreatedOn).HasDefaultValueSql("GETUTCDATE()");

        builder.HasOne(cd => cd.Patient)
            .WithMany(p => p.ChronicDiseases)
            .HasForeignKey(cd => cd.PatientID)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(cd => cd.PatientID);
        builder.HasQueryFilter(e => !e.IsDeleted);
    }
}
