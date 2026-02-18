namespace Edvantix.Blog.Domain.AggregatesModel.PostAggregate;

/// <summary>
/// Тип контента поста.
/// </summary>
public enum PostType
{
    /// <summary>Новость — объявления, анонсы, новости платформы.</summary>
    News = 0,

    /// <summary>Changelog — история изменений платформы (релизы, обновления, исправления).</summary>
    Changelog = 1,
}
