using CareSync.DataLayer.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CareSync.DataLayer.Configurations.Lab;

/// <summary>
/// Entity configuration for T_LabReports
/// </summary>
public class LabReportsConfiguration : IEntityTypeConfiguration<T_LabReports>
{
    public void Configure(EntityTypeBuilder<T_LabReports> builder)
    {
        builder.ToTable("T_LabReports");
        builder.HasKey(lr => lr.LabReportID);
        builder.Property(lr => lr.LabReportID).ValueGeneratedOnAdd();

        // builder.Property(lr => lr.ReportDate).IsRequired(); // Property doesn't exist
        // builder.Property(lr => lr.ReportPath).HasMaxLength(500); // Property doesn't exist
        // builder.Property(lr => lr.Findings).HasMaxLength(2000); // Property doesn't exist
        // builder.Property(lr => lr.Recommendations).HasMaxLength(1000); // Property doesn't exist
        // builder.Property(lr => lr.Status).IsRequired().HasMaxLength(50); // Property doesn't exist

        builder.Property(lr => lr.CreatedBy).HasMaxLength(128);
        builder.Property(lr => lr.UpdatedBy).HasMaxLength(128);
        builder.Property(lr => lr.IsDeleted).HasDefaultValue(false);
        builder.Property(lr => lr.CreatedOn).HasDefaultValueSql("GETUTCDATE()");

        // Note: Navigation properties commented out in entity, so not configuring relationships here
        builder.HasIndex(lr => lr.LabRequestID);
        // builder.HasIndex(lr => lr.ReportDate); // Property doesn't exist
        // builder.HasIndex(lr => lr.Status); // Property doesn't exist
    }
}
