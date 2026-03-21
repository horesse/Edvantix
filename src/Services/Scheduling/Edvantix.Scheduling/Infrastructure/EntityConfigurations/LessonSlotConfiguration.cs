using Edvantix.Chassis.EF.Configurations;
using Edvantix.Scheduling.Domain.AggregatesModel.LessonSlotAggregate;

namespace Edvantix.Scheduling.Infrastructure.EntityConfigurations;

/// <summary>
/// EF Core configuration for <see cref="LessonSlot"/>.
/// HasQueryFilter is NOT set here — it is applied in <c>SchedulingDbContext.OnModelCreating</c>
/// because filter expressions require access to the injected <c>ITenantContext</c>.
/// </summary>
internal sealed class LessonSlotConfiguration : IEntityTypeConfiguration<LessonSlot>
{
    public void Configure(EntityTypeBuilder<LessonSlot> builder)
    {
        // Sets HasKey(Id) and UUIDv7 default value
        builder.UseDefaultConfiguration();

        builder.Property(s => s.SchoolId).IsRequired();
        builder.Property(s => s.GroupId).IsRequired();
        builder.Property(s => s.TeacherId).IsRequired();

        // DateTimeOffset must be stored as timestamp with time zone in PostgreSQL (SCH-10)
        builder.Property(s => s.StartTime).IsRequired().HasColumnType("timestamp with time zone");

        builder.Property(s => s.EndTime).IsRequired().HasColumnType("timestamp with time zone");

        // Composite index for efficient teacher conflict queries (HasConflictAsync)
        builder.HasIndex(s => new
        {
            s.TeacherId,
            s.StartTime,
            s.EndTime,
        });
    }
}
