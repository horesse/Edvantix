using Edvantix.Chassis.EF.Configurations;
using Edvantix.Schedule.Domain.AggregatesModel.GroupScheduleAggregate;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Edvantix.Schedule.Infrastructure.Configurations;

internal sealed class ScheduleSlotConfiguration : IEntityTypeConfiguration<ScheduleSlot>
{
    public void Configure(EntityTypeBuilder<ScheduleSlot> builder)
    {
        builder.UseDefaultConfiguration();

        builder.Property(s => s.Weekday).IsRequired();
        builder.Property(s => s.StartMinutes).IsRequired();

        builder
            .HasIndex(s => new { s.ScheduleId, s.Weekday, s.StartMinutes })
            .IsUnique();
    }
}
