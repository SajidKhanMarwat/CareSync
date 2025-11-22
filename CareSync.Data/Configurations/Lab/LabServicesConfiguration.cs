using CareSync.DataLayer.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CareSync.DataLayer.Configurations.Lab;

/// <summary>
/// Entity configuration for T_LabServices
/// </summary>
public class LabServicesConfiguration : IEntityTypeConfiguration<T_LabServices>
{
    public void Configure(EntityTypeBuilder<T_LabServices> builder)
    {
        builder.ToTable("T_LabServices");
        builder.HasKey(ls => ls.LabServiceID);
        builder.Property(ls => ls.LabServiceID).ValueGeneratedOnAdd();

        builder.Property(ls => ls.ServiceName).IsRequired().HasMaxLength(200);
        // builder.Property(ls => ls.ArabicServiceName).HasMaxLength(200); // Property doesn't exist
        builder.Property(ls => ls.Description).HasMaxLength(1000);
        builder.Property(ls => ls.Price).HasColumnType("decimal(10,2)");
        // builder.Property(ls => ls.ProcessingTime).HasMaxLength(100); // Property doesn't exist

        builder.Property(ls => ls.CreatedBy).HasMaxLength(128);
        builder.Property(ls => ls.UpdatedBy).HasMaxLength(128);
        builder.Property(ls => ls.IsDeleted).HasDefaultValue(false);
        builder.Property(ls => ls.CreatedOn).HasDefaultValueSql("GETUTCDATE()");

        // builder.HasOne(ls => ls.Lab) // Navigation doesn't exist
        //     .WithMany(l => l.LabServices)
        //     .HasForeignKey(ls => ls.LabID)
        //     .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(ls => ls.LabID);
        builder.HasIndex(ls => ls.ServiceName);
    }
}
