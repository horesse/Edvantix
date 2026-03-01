using Edvantix.Chassis.EF.Configurations;

namespace Edvantix.Catalog.Infrastructure.EntityConfigurations;

/// <summary>
/// Конфигурация EF Core для сущности Currency.
/// </summary>
public sealed class CurrencyConfiguration : IEntityTypeConfiguration<Currency>
{
    public void Configure(EntityTypeBuilder<Currency> builder)
    {
        builder.UseDefaultConfiguration();

        builder.Property(c => c.Code).IsRequired().HasMaxLength(DataSchemaLength.Micro);

        builder.Property(c => c.Name).IsRequired().HasMaxLength(DataSchemaLength.Large);

        builder.Property(c => c.Symbol).IsRequired().HasMaxLength(DataSchemaLength.Micro);

        builder.Property(c => c.NumericCode).IsRequired();

        builder.Property(c => c.DecimalDigits).IsRequired();

        builder.Property(c => c.IsActive).IsRequired();

        builder.HasIndex(c => c.Code).IsUnique();
        builder.HasIndex(c => c.NumericCode).IsUnique();
    }
}
