using Edvantix.Chassis.EF.Configurations;

namespace Edvantix.Catalog.Infrastructure.EntityConfigurations;

/// <summary>
/// Конфигурация EF Core для сущности Timezone.
/// </summary>
public sealed class TimezoneConfiguration : IEntityTypeConfiguration<Timezone>
{
    public void Configure(EntityTypeBuilder<Timezone> builder)
    {
        builder.UseDefaultConfiguration();

        builder.Property(t => t.Code).IsRequired().HasMaxLength(DataSchemaLength.Large);

        builder.Property(t => t.Name).IsRequired().HasMaxLength(DataSchemaLength.Large);

        builder.Property(t => t.UtcOffsetMinutes).IsRequired();

        builder.Property(t => t.IsActive).IsRequired();

        builder.HasIndex(t => t.Code).IsUnique();
        builder.HasIndex(t => t.Name);
    }
}
