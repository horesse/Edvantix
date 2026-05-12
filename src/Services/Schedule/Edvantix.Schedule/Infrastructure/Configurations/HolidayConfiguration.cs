using Edvantix.Chassis.EF.Configurations;
using Edvantix.Schedule.Domain.AggregatesModel.HolidayAggregate;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Edvantix.Schedule.Infrastructure.Configurations;

internal sealed class HolidayConfiguration : IEntityTypeConfiguration<Holiday>
{
    public void Configure(EntityTypeBuilder<Holiday> builder)
    {
        builder.UseDefaultConfiguration();

        builder.Property(h => h.CountryCode).IsRequired().HasMaxLength(3);
        builder.Property(h => h.Date).IsRequired();
        builder.Property(h => h.Name).IsRequired().HasMaxLength(DataSchemaLength.Medium);
        builder.Property(h => h.IsRecurringAnnually).IsRequired();

        builder.HasIndex(h => new { h.CountryCode, h.Date }).IsUnique();
        builder.HasIndex(h => h.CountryCode);
    }
}
