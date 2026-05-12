using Edvantix.Chassis.EF.Configurations;
using Edvantix.Schedule.Domain.AggregatesModel.GroupScheduleAggregate;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Edvantix.Schedule.Infrastructure.Configurations;

internal sealed class ScheduleExceptionConfiguration : IEntityTypeConfiguration<ScheduleException>
{
    public void Configure(EntityTypeBuilder<ScheduleException> builder)
    {
        builder.UseDefaultConfiguration();

        builder.Property(e => e.ExceptionDate).IsRequired();
        builder.Property(e => e.Reason).IsRequired(false).HasMaxLength(DataSchemaLength.Large);

        builder.HasIndex(e => new { e.ScheduleId, e.ExceptionDate }).IsUnique();
    }
}
