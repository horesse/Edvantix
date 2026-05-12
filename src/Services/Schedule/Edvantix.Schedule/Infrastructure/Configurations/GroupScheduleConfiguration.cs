using Edvantix.Chassis.EF.Configurations;
using Edvantix.Schedule.Domain.AggregatesModel.GroupScheduleAggregate;
using Edvantix.Schedule.Domain.Enums;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Edvantix.Schedule.Infrastructure.Configurations;

internal sealed class GroupScheduleConfiguration : IEntityTypeConfiguration<GroupSchedule>
{
    public void Configure(EntityTypeBuilder<GroupSchedule> builder)
    {
        builder.UseDefaultConfiguration();

        builder
            .Property(s => s.Recurrence)
            .IsRequired()
            .HasMaxLength(DataSchemaLength.Small)
            .HasConversion<string>();

        builder.Property(s => s.BiweeklyParity).IsRequired(false);

        builder.Property(s => s.LessonDurationMinutes).IsRequired();

        builder
            .Property(s => s.EndMode)
            .IsRequired()
            .HasMaxLength(DataSchemaLength.Small)
            .HasConversion<string>();

        builder.Property(s => s.EndDate).IsRequired(false);
        builder.Property(s => s.LessonCount).IsRequired(false);
        builder.Property(s => s.SkipHolidays).IsRequired();
        builder.Property(s => s.NotifyStudents).IsRequired();
        builder.Property(s => s.CreatedAt).IsRequired();
        builder.Property(s => s.UpdatedAt).IsRequired();

        // Уникальность: одна группа — одно расписание
        builder.HasIndex(s => s.GroupId).IsUnique();
        builder.HasIndex(s => s.OrganizationId);

        builder
            .HasMany(s => s.Slots)
            .WithOne()
            .HasForeignKey(sl => sl.ScheduleId)
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .HasMany(s => s.Exceptions)
            .WithOne()
            .HasForeignKey(e => e.ScheduleId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
