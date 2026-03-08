namespace Edvantix.Catalog.Infrastructure.EntityConfigurations;

/// <summary>
/// Конфигурация EF Core для сущности <see cref="Timezone"/>.
/// Использует натуральный строковый PK Code (IANA TZ Database identifier).
/// </summary>
internal sealed class TimezoneConfiguration : IEntityTypeConfiguration<Timezone>
{
    public void Configure(EntityTypeBuilder<Timezone> builder)
    {
        // Натуральный PK — идентификатор IANA TZ Database (Europe/Moscow, America/New_York и т.д.)
        builder.HasKey(t => t.Code);

        builder.Property(t => t.Code).IsRequired().HasMaxLength(DataSchemaLength.Large);

        builder.Property(t => t.NameRu).IsRequired().HasMaxLength(DataSchemaLength.Large);

        builder.Property(t => t.NameEn).IsRequired().HasMaxLength(DataSchemaLength.Large);

        builder.Property(t => t.DisplayName).IsRequired().HasMaxLength(DataSchemaLength.ExtraLarge);

        builder.Property(t => t.UtcOffsetMinutes).IsRequired();

        builder.Property(t => t.IsActive).IsRequired();

        builder.HasIndex(t => t.NameEn);
        builder.HasIndex(t => t.UtcOffsetMinutes);
    }
}
