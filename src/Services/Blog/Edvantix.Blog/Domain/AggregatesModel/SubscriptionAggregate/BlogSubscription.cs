namespace Edvantix.Blog.Domain.AggregatesModel.SubscriptionAggregate;

/// <summary>
/// Подписка пользователя на обновления блога платформы.
/// Один пользователь — одна запись подписки с настраиваемыми типами контента.
/// </summary>
public sealed class BlogSubscription() : Entity, IAggregateRoot
{
    /// <summary>
    /// Создаёт подписку пользователя на контент блога.
    /// </summary>
    /// <param name="userId">Идентификатор профиля пользователя.</param>
    /// <param name="contentTypes">Типы контента для подписки (битовая маска).</param>
    public BlogSubscription(long userId, ContentSubscriptionType contentTypes)
        : this()
    {
        if (userId <= 0)
            throw new ArgumentException("Некорректный идентификатор пользователя.", nameof(userId));

        if (contentTypes == ContentSubscriptionType.None)
            throw new ArgumentException(
                "Необходимо выбрать хотя бы один тип контента для подписки.",
                nameof(contentTypes)
            );

        UserId = userId;
        ContentTypes = contentTypes;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>Идентификатор профиля подписавшегося пользователя.</summary>
    public long UserId { get; private set; }

    /// <summary>
    /// Типы контента, на которые подписан пользователь.
    /// Хранится как битовая маска (int) для компактности и гибкости.
    /// </summary>
    public ContentSubscriptionType ContentTypes { get; private set; }

    /// <summary>Дата и время оформления подписки.</summary>
    public DateTime CreatedAt { get; private set; }

    /// <summary>Дата и время последнего обновления настроек подписки.</summary>
    public DateTime UpdatedAt { get; private set; }

    /// <summary>
    /// Обновляет настройки подписки.
    /// </summary>
    /// <param name="contentTypes">Новые типы контента для подписки.</param>
    public void UpdateContentTypes(ContentSubscriptionType contentTypes)
    {
        if (contentTypes == ContentSubscriptionType.None)
            throw new ArgumentException(
                "Необходимо выбрать хотя бы один тип контента.",
                nameof(contentTypes)
            );

        ContentTypes = contentTypes;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Проверяет, подписан ли пользователь на указанный тип контента.
    /// </summary>
    /// <param name="type">Тип контента для проверки.</param>
    public bool IsSubscribedTo(ContentSubscriptionType type) =>
        (ContentTypes & type) != ContentSubscriptionType.None;
}
