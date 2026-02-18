using Edvantix.Blog.Domain.AggregatesModel.SubscriptionAggregate;

namespace Edvantix.Blog.Features.SubscriptionFeature.Models;

/// <summary>
/// Настройки подписки пользователя на блог платформы.
/// </summary>
public sealed class SubscriptionModel
{
    /// <summary>Идентификатор подписки.</summary>
    public long Id { get; set; }

    /// <summary>Идентификатор профиля подписчика.</summary>
    public long UserId { get; set; }

    /// <summary>Типы контента для подписки (битовая маска).</summary>
    public ContentSubscriptionType ContentTypes { get; set; }

    /// <summary>Признак подписки на новости.</summary>
    public bool SubscribedToNews => (ContentTypes & ContentSubscriptionType.News) != 0;

    /// <summary>Признак подписки на changelogs.</summary>
    public bool SubscribedToChangelog => (ContentTypes & ContentSubscriptionType.Changelog) != 0;

    /// <summary>Дата и время оформления подписки.</summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>Дата и время последнего обновления настроек.</summary>
    public DateTime UpdatedAt { get; set; }
}
