using Edvantix.Chassis.EF.Configurations;

namespace Edvantix.Blog.Infrastructure.EntityConfigurations;

/// <summary>
/// Конфигурация EF Core для сущности PostLike.
/// Гарантирует уникальность пары (post_id, user_id) — один лайк на пользователя.
/// </summary>
internal sealed class PostLikeConfiguration : IEntityTypeConfiguration<PostLike>
{
    public void Configure(EntityTypeBuilder<PostLike> builder)
    {
        builder.UseDefaultConfiguration();

        builder.Property(l => l.PostId).IsRequired();

        builder.Property(l => l.ProfileId).IsRequired();

        builder.Property(l => l.CreatedAt).IsRequired();

        // Гарантирует, что один пользователь может поставить только один лайк на пост
        builder.HasIndex(l => new { l.PostId, UserId = l.ProfileId }).IsUnique();
    }
}
