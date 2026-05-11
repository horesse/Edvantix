using Edvantix.Curriculum.Domain.AggregatesModel.CourseAggregate;
using Edvantix.Curriculum.Domain.Enums;
using Edvantix.SharedKernel.Helpers;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Edvantix.Curriculum.Infrastructure.Configurations;

internal sealed class CourseConfiguration : IEntityTypeConfiguration<Course>
{
    public void Configure(EntityTypeBuilder<Course> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(p => p.Id).HasDefaultValueSql(UniqueIdentifierHelper.NewUuidV7);

        builder
            .Property(e => e.Code)
            .IsRequired()
            .HasMaxLength(DataSchemaLength.Short);

        builder
            .Property(e => e.Name)
            .IsRequired()
            .HasMaxLength(DataSchemaLength.Large);

        builder
            .Property(e => e.Level)
            .IsRequired()
            .HasMaxLength(DataSchemaLength.Short);

        builder
            .Property(e => e.Description)
            .HasMaxLength(DataSchemaLength.SuperLarge);

        builder
            .Property(e => e.CoverInitials)
            .HasMaxLength(4);

        builder
            .Property(e => e.Subject)
            .IsRequired()
            .HasMaxLength(DataSchemaLength.Medium)
            .HasConversion<string>();

        builder
            .Property(e => e.Status)
            .IsRequired()
            .HasMaxLength(DataSchemaLength.Medium)
            .HasConversion<string>();

        builder.Property(e => e.CreatedAt).HasDefaultValueSql(DateTimeHelper.SqlUtcNow);
        builder.Property(e => e.UpdatedAt).HasDefaultValueSql(DateTimeHelper.SqlUtcNow);

        // Уникальный код курса в рамках организации
        builder.HasIndex(e => new { e.OrganizationId, e.Code }).IsUnique();
        builder.HasIndex(e => new { e.OrganizationId, e.Subject, e.Status });
    }
}
