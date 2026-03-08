namespace Edvantix.Catalog.Infrastructure.EntityConfigurations;

/// <summary>
/// Конфигурация EF Core для сущности <see cref="Region"/>.
/// Использует натуральный строковый PK Code (пользовательский идентификатор макрорегиона).
/// </summary>
internal sealed class RegionConfiguration : IEntityTypeConfiguration<Region>
{
    public void Configure(EntityTypeBuilder<Region> builder)
    {
        // Натуральный PK — код макрорегиона (CIS, EU, APAC и т.д.)
        builder.HasKey(r => r.Code);

        builder.Property(r => r.Code).IsRequired().HasMaxLength(DataSchemaLength.Small);

        builder.Property(r => r.NameRu).IsRequired().HasMaxLength(DataSchemaLength.Large);

        builder.Property(r => r.NameEn).IsRequired().HasMaxLength(DataSchemaLength.Large);

        builder.Property(r => r.IsActive).IsRequired();

        builder.HasIndex(r => r.NameEn);
    }
}
