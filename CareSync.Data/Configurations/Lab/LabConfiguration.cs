using CareSync.DataLayer.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CareSync.DataLayer.Configurations.Lab;

/// <summary>
/// Entity configuration for T_Lab
/// </summary>
public class LabConfiguration : IEntityTypeConfiguration<T_Lab>
{
    public void Configure(EntityTypeBuilder<T_Lab> builder)
    {
        builder.ToTable("T_Labs");
        builder.HasKey(l => l.LabID);
        builder.Property(l => l.LabID).ValueGeneratedOnAdd();

        builder.Property(l => l.LabName).IsRequired().HasMaxLength(200);
        builder.Property(l => l.ArabicLabName).HasMaxLength(200);
        builder.Property(l => l.LabAddress).HasMaxLength(500);
        builder.Property(l => l.ArabicLabAddress).HasMaxLength(500);
        builder.Property(l => l.Location).HasMaxLength(200);
        builder.Property(l => l.ContactNumber).HasMaxLength(20);
        builder.Property(l => l.Email).HasMaxLength(256);
        builder.Property(l => l.LicenseNumber).HasMaxLength(100);

        builder.Property(l => l.CreatedBy).HasMaxLength(128);
        builder.Property(l => l.UpdatedBy).HasMaxLength(128);
        builder.Property(l => l.IsDeleted).HasDefaultValue(false);
        builder.Property(l => l.CreatedOn).HasDefaultValueSql("GETUTCDATE()");

        builder.HasIndex(l => l.UserID);
        builder.HasIndex(l => l.Email);
        builder.HasIndex(l => l.LicenseNumber);
    }
}
