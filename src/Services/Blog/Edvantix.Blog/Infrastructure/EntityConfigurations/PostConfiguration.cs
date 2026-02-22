using Edvantix.Chassis.EF.Configurations;

namespace Edvantix.Blog.Infrastructure.EntityConfigurations;

/// <summary>
/// Конфигурация EF Core для сущности Post.
/// Настраивает схему таблицы posts и связи с категориями, тегами и лайками.
/// </summary>
public sealed class PostConfiguration : IEntityTypeConfiguration<Post>
{
    public void Configure(EntityTypeBuilder<Post> builder)
    {
        builder.UseDefaultConfiguration();

        builder.Property(p => p.Title).IsRequired().HasMaxLength(DataSchemaLength.SuperLarge);

        builder.Property(p => p.Slug).IsRequired().HasMaxLength(DataSchemaLength.ExtraLarge);

        // Markdown-контент без ограничения длины
        builder.Property(p => p.Content).IsRequired().HasColumnType("text");

        builder.Property(p => p.Summary).IsRequired(false).HasMaxLength(DataSchemaLength.Max);

        builder.Property(p => p.Type).IsRequired().HasConversion<string>();

        builder.Property(p => p.Status).IsRequired().HasConversion<string>();

        builder.Property(p => p.IsPremium).IsRequired().HasDefaultValue(false);

        builder.Property(p => p.AuthorId).IsRequired();

        builder.Property(p => p.PublishedAt).IsRequired(false);

        builder.Property(p => p.ScheduledAt).IsRequired(false);

        builder.Property(p => p.CoverImageUrl).IsRequired(false).HasMaxLength(DataSchemaLength.Max);

        builder.Property(p => p.LikesCount).IsRequired().HasDefaultValue(0);

        builder.Property(p => p.CreatedAt).IsRequired();

        builder.Property(p => p.UpdatedAt).IsRequired();

        // Уникальный slug для SEO-дружественных URL
        builder.HasIndex(p => p.Slug).IsUnique();

        builder.HasIndex(p => p.Status);
        builder.HasIndex(p => p.Type);
        builder.HasIndex(p => p.AuthorId);
        builder.HasIndex(p => p.PublishedAt);

        // Many-to-many: Post <-> Category через таблицу post_categories
        builder
            .HasMany(p => p.Categories)
            .WithMany()
            .UsingEntity(j =>
            {
                j.ToTable("post_categories");
            });

        // Many-to-many: Post <-> Tag через таблицу post_tags
        builder
            .HasMany(p => p.Tags)
            .WithMany()
            .UsingEntity(j =>
            {
                j.ToTable("post_tags");
            });

        // One-to-many: Post -> PostLike
        builder
            .HasMany(p => p.Likes)
            .WithOne(l => l.Post)
            .HasForeignKey(l => l.PostId)
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .Metadata.FindNavigation(nameof(Post.Likes))!
            .SetPropertyAccessMode(PropertyAccessMode.Field);

        builder
            .Metadata.FindSkipNavigation(nameof(Post.Categories))!
            .SetPropertyAccessMode(PropertyAccessMode.Field);

        builder
            .Metadata.FindSkipNavigation(nameof(Post.Tags))!
            .SetPropertyAccessMode(PropertyAccessMode.Field);
    }
}
