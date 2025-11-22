using CareSync.DataLayer.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CareSync.DataLayer.Configurations.Doctor;

/// <summary>
/// Entity configuration for T_Qualifications
/// </summary>
public class QualificationsConfiguration : IEntityTypeConfiguration<T_Qualifications>
{
    public void Configure(EntityTypeBuilder<T_Qualifications> builder)
    {
        builder.ToTable("T_Qualifications");
        builder.HasKey(q => q.QualificationID);
        builder.Property(q => q.QualificationID).ValueGeneratedOnAdd();

        // builder.Property(q => q.DegreeOrCertificate).IsRequired().HasMaxLength(200); // Property doesn't exist
        // builder.Property(q => q.InstitutionName).HasMaxLength(300); // Property doesn't exist
        // builder.Property(q => q.FieldOfStudy).HasMaxLength(200); // Property doesn't exist
        // builder.Property(q => q.YearObtained).IsRequired(); // Property doesn't exist
        // builder.Property(q => q.Country).HasMaxLength(100); // Property doesn't exist
        // builder.Property(q => q.CertificateNumber).HasMaxLength(100); // Property doesn't exist

        builder.Property(q => q.CreatedBy).HasMaxLength(128);
        builder.Property(q => q.UpdatedBy).HasMaxLength(128);
        builder.Property(q => q.IsDeleted).HasDefaultValue(false);
        builder.Property(q => q.CreatedOn).HasDefaultValueSql("GETUTCDATE()");

        // builder.HasOne(q => q.Doctor) // Navigation doesn't exist
        //     .WithMany(d => d.Qualifications)
        //     .HasForeignKey(q => q.DoctorID)
        //     .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(q => q.DoctorID);
        // builder.HasIndex(q => q.YearObtained); // Property doesn't exist
        builder.HasQueryFilter(e => !e.IsDeleted);
    }
}
