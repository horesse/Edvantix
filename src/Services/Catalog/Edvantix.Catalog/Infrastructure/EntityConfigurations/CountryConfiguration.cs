using Edvantix.Chassis.EF.Configurations;

namespace Edvantix.Catalog.Infrastructure.EntityConfigurations;

/// <summary>
/// Конфигурация EF Core для сущности Country.
/// </summary>
public sealed class CountryConfiguration : IEntityTypeConfiguration<Country>
{
    public void Configure(EntityTypeBuilder<Country> builder)
    {
        builder.UseDefaultConfiguration();

        builder.Property(c => c.Alpha2Code).IsRequired().HasMaxLength(DataSchemaLength.Micro);

        builder.Property(c => c.Alpha3Code).IsRequired().HasMaxLength(DataSchemaLength.Micro);

        builder.Property(c => c.Name).IsRequired().HasMaxLength(DataSchemaLength.Large);

        builder.Property(c => c.NumericCode).IsRequired();

        builder.Property(c => c.CurrencyCode).IsRequired().HasMaxLength(DataSchemaLength.Micro);

        builder.Property(c => c.IsActive).IsRequired();

        builder.HasIndex(c => c.Alpha2Code).IsUnique();
        builder.HasIndex(c => c.Alpha3Code).IsUnique();
        builder.HasIndex(c => c.NumericCode).IsUnique();
        builder.HasIndex(c => c.Name);
    }
}
