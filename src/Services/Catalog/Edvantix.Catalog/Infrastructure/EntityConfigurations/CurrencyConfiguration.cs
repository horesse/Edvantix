namespace Edvantix.Catalog.Infrastructure.EntityConfigurations;

/// <summary>
/// Конфигурация EF Core для сущности <see cref="Currency"/>.
/// Использует натуральный строковый PK Code (ISO 4217).
/// </summary>
internal sealed class CurrencyConfiguration : IEntityTypeConfiguration<Currency>
{
    public void Configure(EntityTypeBuilder<Currency> builder)
    {
        // Натуральный PK — алфавитный код ISO 4217 (USD, EUR и т.д.)
        builder.HasKey(c => c.Code);

        builder.Property(c => c.Code).IsRequired().HasMaxLength(DataSchemaLength.Micro);

        builder.Property(c => c.NameRu).IsRequired().HasMaxLength(DataSchemaLength.Large);

        builder.Property(c => c.NameEn).IsRequired().HasMaxLength(DataSchemaLength.Large);

        builder.Property(c => c.Symbol).IsRequired().HasMaxLength(DataSchemaLength.Micro);

        builder.Property(c => c.NumericCode).IsRequired();

        builder.Property(c => c.DecimalDigits).IsRequired();

        builder.Property(c => c.IsActive).IsRequired();

        builder.HasIndex(c => c.NumericCode).IsUnique();
    }
}
