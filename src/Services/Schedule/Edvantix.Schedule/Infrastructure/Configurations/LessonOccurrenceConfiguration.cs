using Edvantix.Chassis.EF.Configurations;
using Edvantix.Schedule.Domain.AggregatesModel.LessonOccurrenceAggregate;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Edvantix.Schedule.Infrastructure.Configurations;

internal sealed class LessonOccurrenceConfiguration : IEntityTypeConfiguration<LessonOccurrence>
{
    public void Configure(EntityTypeBuilder<LessonOccurrence> builder)
    {
        builder.UseDefaultConfiguration();

        builder.Property(o => o.LessonDate).IsRequired();
        builder.Property(o => o.StartMinutes).IsRequired();
        builder.Property(o => o.DurationMinutes).IsRequired();

        builder
            .Property(o => o.Status)
            .IsRequired()
            .HasMaxLength(DataSchemaLength.Small)
            .HasConversion<string>();

        builder.Property(o => o.SkipReason).IsRequired(false).HasMaxLength(DataSchemaLength.Large);
        builder.Property(o => o.LessonRefId).IsRequired(false);

        builder.HasIndex(o => new { o.GroupId, o.LessonDate });
        builder.HasIndex(o => new { o.ScheduleId, o.Status });
    }
}
