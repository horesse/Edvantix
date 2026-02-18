namespace Edvantix.Blog.Domain.AggregatesModel.SubscriptionAggregate;

/// <summary>
/// Тип контента, на который подписывается пользователь.
/// Поддерживает битовую маску для комбинирования типов.
/// </summary>
[Flags]
public enum ContentSubscriptionType
{
    /// <summary>Без подписки.</summary>
    None = 0,

    /// <summary>Подписка на новости платформы.</summary>
    News = 1,

    /// <summary>Подписка на changelogs (история изменений).</summary>
    Changelog = 2,

    /// <summary>Подписка на все типы контента.</summary>
    All = News | Changelog,
}
