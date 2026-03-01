using Edvantix.Chassis.EF.Configurations;

namespace Edvantix.Catalog.Infrastructure.EntityConfigurations;

/// <summary>
/// Конфигурация EF Core для сущности Region.
/// </summary>
public sealed class RegionConfiguration : IEntityTypeConfiguration<Region>
{
    public void Configure(EntityTypeBuilder<Region> builder)
    {
        builder.UseDefaultConfiguration();

        builder.Property(r => r.Code).IsRequired().HasMaxLength(DataSchemaLength.Small);

        builder.Property(r => r.Name).IsRequired().HasMaxLength(DataSchemaLength.Large);

        builder.Property(r => r.IsActive).IsRequired();

        builder.HasIndex(r => r.Code).IsUnique();
        builder.HasIndex(r => r.Name);
    }
}
