using CareSync.DataLayer.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CareSync.DataLayer.Configurations.Patient;

public class AdditionalNotesConfiguration : IEntityTypeConfiguration<T_AdditionalNotes>
{
    public void Configure(EntityTypeBuilder<T_AdditionalNotes> builder)
    {
        builder.ToTable("T_AdditionalNotes");
        builder.HasKey(an => an.NoteID);
        builder.Property(an => an.NoteID).ValueGeneratedOnAdd();

        // builder.Property(an => an.NoteText).HasMaxLength(2000); // Property doesn't exist - check entity
        builder.Property(an => an.CreatedBy).HasMaxLength(128);
        builder.Property(an => an.UpdatedBy).HasMaxLength(128);
        builder.Property(an => an.IsDeleted).HasDefaultValue(false);
        builder.Property(an => an.CreatedOn).HasDefaultValueSql("GETUTCDATE()");

        builder.HasOne(an => an.Patient)
            .WithMany(p => p.AdditionalNotes)
            .HasForeignKey(an => an.PatientID)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(an => an.PatientID);
        builder.HasQueryFilter(e => !e.IsDeleted);
    }
}
