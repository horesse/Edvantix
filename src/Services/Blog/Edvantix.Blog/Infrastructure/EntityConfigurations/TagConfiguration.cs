using Edvantix.Chassis.EF.Configurations;

namespace Edvantix.Blog.Infrastructure.EntityConfigurations;

/// <summary>
/// Конфигурация EF Core для сущности Tag.
/// </summary>
internal sealed class TagConfiguration : IEntityTypeConfiguration<Tag>
{
    public void Configure(EntityTypeBuilder<Tag> builder)
    {
        builder.UseDefaultConfiguration();

        builder.Property(t => t.Name).IsRequired().HasMaxLength(DataSchemaLength.Large);

        builder.Property(t => t.Slug).IsRequired().HasMaxLength(DataSchemaLength.Large);

        builder.Property(t => t.CreatedAt).IsRequired();

        builder.HasIndex(t => t.Slug).IsUnique();
        builder.HasIndex(t => t.Name);
    }
}
