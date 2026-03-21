using Edvantix.Chassis.EF.Configurations;
using Edvantix.Scheduling.Domain.AggregatesModel.AttendanceAggregate;

namespace Edvantix.Scheduling.Infrastructure.EntityConfigurations;

/// <summary>
/// EF Core configuration for <see cref="AttendanceRecord"/>.
/// HasQueryFilter is NOT set here — it is applied in <c>SchedulingDbContext.OnModelCreating</c>
/// because filter expressions require access to the injected <c>ITenantContext</c>.
/// Unique index on (LessonSlotId, StudentId) enforces the one-record-per-student-per-slot constraint (D-02).
/// </summary>
internal sealed class AttendanceRecordConfiguration : IEntityTypeConfiguration<AttendanceRecord>
{
    public void Configure(EntityTypeBuilder<AttendanceRecord> builder)
    {
        // Sets HasKey(Id) and UUIDv7 default value
        builder.UseDefaultConfiguration();

        builder.Property(a => a.SchoolId).IsRequired();
        builder.Property(a => a.LessonSlotId).IsRequired();
        builder.Property(a => a.StudentId).IsRequired();
        builder.Property(a => a.Status).IsRequired();
        builder.Property(a => a.CorrelationId).IsRequired();

        // DateTimeOffset must be stored as timestamp with time zone in PostgreSQL (SCH-10)
        builder.Property(a => a.MarkedAt).IsRequired().HasColumnType("timestamp with time zone");

        // Unique composite index: one attendance record per student per slot (D-02).
        // If a student is re-marked, UpdateStatus is called on the existing record.
        builder
            .HasIndex(a => new { a.LessonSlotId, a.StudentId })
            .IsUnique();
    }
}
