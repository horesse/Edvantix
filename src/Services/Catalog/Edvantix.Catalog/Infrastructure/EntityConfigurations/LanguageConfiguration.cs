using Edvantix.Chassis.EF.Configurations;

namespace Edvantix.Catalog.Infrastructure.EntityConfigurations;

/// <summary>
/// Конфигурация EF Core для сущности Language.
/// </summary>
public sealed class LanguageConfiguration : IEntityTypeConfiguration<Language>
{
    public void Configure(EntityTypeBuilder<Language> builder)
    {
        builder.UseDefaultConfiguration();

        builder.Property(l => l.Code).IsRequired().HasMaxLength(DataSchemaLength.Small);

        builder.Property(l => l.Name).IsRequired().HasMaxLength(DataSchemaLength.Large);

        builder.Property(l => l.NativeName).IsRequired().HasMaxLength(DataSchemaLength.Large);

        builder.Property(l => l.IsActive).IsRequired();

        builder.HasIndex(l => l.Code).IsUnique();
        builder.HasIndex(l => l.Name);
    }
}
