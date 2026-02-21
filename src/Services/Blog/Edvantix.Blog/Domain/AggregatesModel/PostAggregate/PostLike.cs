using Edvantix.SharedKernel.SeedWork;

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
    /// <param name="userId">Идентификатор профиля пользователя.</param>
    public PostLike(long postId, long userId)
        : this()
    {
        if (postId <= 0)
            throw new ArgumentException("Некорректный идентификатор поста.", nameof(postId));

        if (userId <= 0)
            throw new ArgumentException("Некорректный идентификатор пользователя.", nameof(userId));

        PostId = postId;
        UserId = userId;
        CreatedAt = DateTime.UtcNow;
    }

    /// <summary>Идентификатор поста.</summary>
    public long PostId { get; private set; }

    /// <summary>Идентификатор профиля пользователя, поставившего лайк.</summary>
    public long UserId { get; private set; }

    /// <summary>Дата и время постановки лайка.</summary>
    public DateTime CreatedAt { get; private set; }

    /// <summary>Навигационное свойство к посту.</summary>
    public Post Post { get; private set; } = null!;
}
