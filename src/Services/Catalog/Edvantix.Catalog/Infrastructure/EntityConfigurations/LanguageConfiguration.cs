namespace Edvantix.Catalog.Infrastructure.EntityConfigurations;

/// <summary>
/// Конфигурация EF Core для сущности <see cref="Language"/>.
/// Использует натуральный строковый PK Code (ISO 639-1 alpha-2).
/// </summary>
public sealed class LanguageConfiguration : IEntityTypeConfiguration<Language>
{
    public void Configure(EntityTypeBuilder<Language> builder)
    {
        // Натуральный PK — двухбуквенный код ISO 639-1 (en, ru, de и т.д.)
        builder.HasKey(l => l.Code);

        builder.Property(l => l.Code).IsRequired().HasMaxLength(DataSchemaLength.Micro);

        builder.Property(l => l.NameRu).IsRequired().HasMaxLength(DataSchemaLength.Large);

        builder.Property(l => l.NameEn).IsRequired().HasMaxLength(DataSchemaLength.Large);

        builder.Property(l => l.NativeName).IsRequired().HasMaxLength(DataSchemaLength.Large);

        builder.Property(l => l.IsActive).IsRequired();

        builder.HasIndex(l => l.NameEn);
    }
}
