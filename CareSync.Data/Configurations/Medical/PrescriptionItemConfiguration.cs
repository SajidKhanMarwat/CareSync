using CareSync.DataLayer.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CareSync.DataLayer.Configurations.Medical;

/// <summary>
/// Entity configuration for T_PrescriptionItems
/// </summary>
public class PrescriptionItemConfiguration : IEntityTypeConfiguration<T_PrescriptionItems>
{
    public void Configure(EntityTypeBuilder<T_PrescriptionItems> builder)
    {
        // Table name
        builder.ToTable("T_PrescriptionItems");

        // Primary key
        builder.HasKey(pi => pi.PrescriptionItemID);
        builder.Property(pi => pi.PrescriptionItemID).ValueGeneratedOnAdd();

        // Properties
        builder.Property(pi => pi.MedicineName).IsRequired().HasMaxLength(200);
        builder.Property(pi => pi.Dosage).HasMaxLength(100);
        builder.Property(pi => pi.Frequency).HasMaxLength(100);
        builder.Property(pi => pi.Duration).HasMaxLength(50);
        // builder.Property(pi => pi.Instructions).HasMaxLength(500); // Property doesn't exist

        // Audit fields
        builder.Property(pi => pi.CreatedBy).HasMaxLength(128);
        builder.Property(pi => pi.UpdatedBy).HasMaxLength(128);
        builder.Property(pi => pi.IsDeleted).HasDefaultValue(false);
        builder.Property(pi => pi.CreatedOn).HasDefaultValueSql("GETUTCDATE()");

        // Relationships
        builder.HasOne(pi => pi.Prescription)
            .WithMany(p => p.PrescriptionItems)
            .HasForeignKey(pi => pi.PrescriptionID)
            .OnDelete(DeleteBehavior.Cascade);

        // Indexes
        builder.HasIndex(pi => pi.PrescriptionID);

        // Query filter for soft delete
        builder.HasQueryFilter(e => !e.IsDeleted);
    }
}
