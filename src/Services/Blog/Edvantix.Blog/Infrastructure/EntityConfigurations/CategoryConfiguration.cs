using Edvantix.Chassis.EF.Configurations;

namespace Edvantix.Blog.Infrastructure.EntityConfigurations;

/// <summary>
/// Конфигурация EF Core для сущности Category.
/// </summary>
public sealed class CategoryConfiguration : IEntityTypeConfiguration<Category>
{
    public void Configure(EntityTypeBuilder<Category> builder)
    {
        builder.UseDefaultConfiguration();

        builder.Property(c => c.Name).IsRequired().HasMaxLength(DataSchemaLength.Large);

        builder.Property(c => c.Slug).IsRequired().HasMaxLength(DataSchemaLength.Large);

        builder.Property(c => c.Description).IsRequired(false).HasMaxLength(DataSchemaLength.Max);

        builder.Property(c => c.CreatedAt).IsRequired();

        builder.HasIndex(c => c.Slug).IsUnique();
        builder.HasIndex(c => c.Name);
    }
}
