using CareSync.DataLayer.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CareSync.DataLayer.Configurations.Lab;

/// <summary>
/// Entity configuration for T_LabRequests
/// </summary>
public class LabRequestsConfiguration : IEntityTypeConfiguration<T_LabRequests>
{
    public void Configure(EntityTypeBuilder<T_LabRequests> builder)
    {
        builder.ToTable("T_LabRequests");
        builder.HasKey(lr => lr.LabRequestID);
        builder.Property(lr => lr.LabRequestID).ValueGeneratedOnAdd();

        builder.Property(lr => lr.Status).IsRequired().HasMaxLength(50);
        builder.Property(lr => lr.Remarks).HasMaxLength(1000);
        // builder.Property(lr => lr.RequestedDate).IsRequired(); // Property doesn't exist

        builder.Property(lr => lr.CreatedBy).HasMaxLength(128);
        builder.Property(lr => lr.UpdatedBy).HasMaxLength(128);
        builder.Property(lr => lr.IsDeleted).HasDefaultValue(false);
        builder.Property(lr => lr.CreatedOn).HasDefaultValueSql("GETUTCDATE()");

        // Note: Navigation properties commented out in entity, so not configuring relationships here
        builder.HasIndex(lr => lr.AppointmentID);
        builder.HasIndex(lr => lr.LabServiceID);
        builder.HasIndex(lr => lr.Status);
        // builder.HasIndex(lr => lr.RequestedDate); // Property doesn't exist
    }
}
