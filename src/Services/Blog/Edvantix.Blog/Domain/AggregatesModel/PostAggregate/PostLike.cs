namespace Edvantix.Blog.Domain.AggregatesModel.PostAggregate;

/// <summary>
/// Лайк пользователя на пост блога.
/// Один пользователь может поставить только один лайк на пост.
/// </summary>
public sealed class PostLike() : Entity
{
    /// <summary>
    /// Создаёт лайк от пользователя на пост.
    /// </summary>
    /// <param name="postId">Идентификатор поста.</param>
    /// <param name="profileId">Идентификатор профиля пользователя.</param>
    public PostLike(Guid postId, Guid profileId)
        : this()
    {
        PostId = postId;
        ProfileId = profileId;
        CreatedAt = DateTime.UtcNow;
    }

    /// <summary>Идентификатор поста.</summary>
    public Guid PostId { get; private set; }

    /// <summary>Идентификатор профиля пользователя, поставившего лайк.</summary>
    public Guid ProfileId { get; private set; }

    /// <summary>Дата и время постановки лайка.</summary>
    public DateTime CreatedAt { get; private set; }

    /// <summary>Навигационное свойство к посту.</summary>
    public Post Post { get; private set; } = null!;
}
