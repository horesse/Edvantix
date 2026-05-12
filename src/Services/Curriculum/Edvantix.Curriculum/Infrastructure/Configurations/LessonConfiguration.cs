using Edvantix.Curriculum.Domain.AggregatesModel.CourseAggregate;
using Edvantix.Curriculum.Domain.Enums;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Edvantix.Curriculum.Infrastructure.Configurations;

internal sealed class LessonConfiguration : IEntityTypeConfiguration<Lesson>
{
    public void Configure(EntityTypeBuilder<Lesson> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(l => l.Title).IsRequired().HasMaxLength(DataSchemaLength.Large);

        builder
            .Property(l => l.Type)
            .IsRequired()
            .HasMaxLength(DataSchemaLength.Small)
            .HasConversion<string>();

        builder
            .Property(l => l.Status)
            .IsRequired()
            .HasMaxLength(DataSchemaLength.Small)
            .HasConversion<string>();

        // PostgreSQL native text[] array
        builder.Property(l => l.Objectives).HasColumnType("text[]");

        builder.HasIndex(l => new { l.ModuleId, l.Position }).IsUnique();
        builder.HasIndex(l => new { l.ModuleId, l.Status });
    }
}
