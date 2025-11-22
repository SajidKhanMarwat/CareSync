using CareSync.DataLayer.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CareSync.DataLayer.Configurations.Patient;

public class PatientReportsConfiguration : IEntityTypeConfiguration<T_PatientReports>
{
    public void Configure(EntityTypeBuilder<T_PatientReports> builder)
    {
        builder.ToTable("T_PatientReports");
        builder.HasKey(pr => pr.PatientReportID);
        builder.Property(pr => pr.PatientReportID).ValueGeneratedOnAdd();

        // TODO: Add properties to T_PatientReports entity first
        // builder.Property(pr => pr.ReportType).IsRequired().HasMaxLength(100);
        // builder.Property(pr => pr.ReportDate).IsRequired().HasColumnType("date");
        // builder.Property(pr => pr.ReportPath).HasMaxLength(500);
        // builder.Property(pr => pr.Notes).HasMaxLength(1000);

        // builder.Property(pr => pr.CreatedBy).HasMaxLength(128);
        // builder.Property(pr => pr.UpdatedBy).HasMaxLength(128);
        // builder.Property(pr => pr.IsDeleted).HasDefaultValue(false);
        // builder.Property(pr => pr.CreatedOn).HasDefaultValueSql("GETUTCDATE()");

        // builder.HasOne(pr => pr.Patient)
        //     .WithMany(p => p.PatientReports)
        //     .HasForeignKey(pr => pr.PatientID)
        //     .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(pr => pr.PatientID);
        // builder.HasIndex(pr => pr.ReportDate);
    }
}
