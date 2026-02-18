using Edvantix.Blog.Domain.AggregatesModel.PostAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Edvantix.Blog.Infrastructure.EntityConfigurations;

/// <summary>
/// Конфигурация EF Core для сущности PostLike.
/// Гарантирует уникальность пары (post_id, user_id) — один лайк на пользователя.
/// </summary>
public sealed class PostLikeConfiguration : IEntityTypeConfiguration<PostLike>
{
    public void Configure(EntityTypeBuilder<PostLike> builder)
    {
        builder.ToTable("post_likes");

        builder.HasKey(l => l.Id);

        builder.Property(l => l.PostId).IsRequired();

        builder.Property(l => l.UserId).IsRequired();

        builder.Property(l => l.CreatedAt).IsRequired();

        // Гарантирует, что один пользователь может поставить только один лайк на пост
        builder.HasIndex(l => new { l.PostId, l.UserId }).IsUnique();
    }
}
