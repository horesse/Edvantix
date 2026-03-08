namespace Edvantix.Catalog.Infrastructure.EntityConfigurations;

/// <summary>
/// Конфигурация EF Core для сущности <see cref="Country"/>.
/// Использует натуральный строковый PK Code (ISO 3166-1 alpha-2).
/// </summary>
internal sealed class CountryConfiguration : IEntityTypeConfiguration<Country>
{
    public void Configure(EntityTypeBuilder<Country> builder)
    {
        // Натуральный PK — двухбуквенный код ISO 3166-1 alpha-2 (US, DE, RU и т.д.)
        builder.HasKey(c => c.Code);

        builder.Property(c => c.Code).IsRequired().HasMaxLength(DataSchemaLength.Micro);

        builder.Property(c => c.Alpha3Code).IsRequired().HasMaxLength(DataSchemaLength.Micro);

        builder.Property(c => c.NameRu).IsRequired().HasMaxLength(DataSchemaLength.Large);

        builder.Property(c => c.NameEn).IsRequired().HasMaxLength(DataSchemaLength.Large);

        builder.Property(c => c.NumericCode).IsRequired();

        builder.Property(c => c.PhonePrefix).IsRequired().HasMaxLength(DataSchemaLength.Micro);

        builder.Property(c => c.CurrencyCode).IsRequired().HasMaxLength(DataSchemaLength.Micro);

        builder.Property(c => c.IsActive).IsRequired();

        builder.HasIndex(c => c.Alpha3Code).IsUnique();
        builder.HasIndex(c => c.NumericCode).IsUnique();
        builder.HasIndex(c => c.NameEn);

        // FK на Currency: страна обязана иметь валюту (Restrict — нельзя удалить валюту при наличии стран)
        builder
            .HasOne<Currency>()
            .WithMany()
            .HasForeignKey(c => c.CurrencyCode)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
