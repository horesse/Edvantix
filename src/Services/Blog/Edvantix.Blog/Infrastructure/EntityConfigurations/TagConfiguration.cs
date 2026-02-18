using Edvantix.Blog.Domain.AggregatesModel.TagAggregate;
using Edvantix.Constants.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Edvantix.Blog.Infrastructure.EntityConfigurations;

/// <summary>
/// Конфигурация EF Core для сущности Tag.
/// </summary>
public sealed class TagConfiguration : IEntityTypeConfiguration<Tag>
{
    public void Configure(EntityTypeBuilder<Tag> builder)
    {
        builder.ToTable("tags");

        builder.HasKey(t => t.Id);

        builder.Property(t => t.Name).IsRequired().HasMaxLength(DataSchemaLength.Large);

        builder.Property(t => t.Slug).IsRequired().HasMaxLength(DataSchemaLength.Large);

        builder.Property(t => t.CreatedAt).IsRequired();

        builder.HasIndex(t => t.Slug).IsUnique();
        builder.HasIndex(t => t.Name);
    }
}
