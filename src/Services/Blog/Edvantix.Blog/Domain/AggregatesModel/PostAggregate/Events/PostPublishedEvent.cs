using Edvantix.SharedKernel.SeedWork;

namespace Edvantix.Blog.Domain.AggregatesModel.PostAggregate.Events;

/// <summary>
/// Доменное событие, публикуемое при публикации поста.
/// Используется для уведомления подписчиков через сервис Notifications.
/// </summary>
public sealed class PostPublishedEvent(
    ulong postId,
    string title,
    string slug,
    PostType postType,
    bool isPremium,
    ulong authorId
) : DomainEvent
{
    /// <summary>Идентификатор опубликованного поста.</summary>
    public ulong PostId { get; } = postId;

    /// <summary>Заголовок поста.</summary>
    public string Title { get; } = title;

    /// <summary>URL-слаг поста.</summary>
    public string Slug { get; } = slug;

    /// <summary>Тип контента поста.</summary>
    public PostType PostType { get; } = postType;

    /// <summary>Признак премиум-контента.</summary>
    public bool IsPremium { get; } = isPremium;

    /// <summary>Идентификатор профиля автора.</summary>
    public ulong AuthorId { get; } = authorId;
}
