using Edvantix.Chassis.Specification.Builders;
using Edvantix.Chassis.Specification.Generic;

namespace Edvantix.Blog.Domain.AggregatesModel.PostAggregate.Specifications;

/// <summary>
/// Спецификация для выборки постов блога с поддержкой фильтрации, поиска и пагинации.
/// Все условия применяются единожды в конструкторе для корректной генерации SQL.
/// </summary>
public sealed class PostSpecification : CommonSpecification<Post>
{
    /// <summary>
    /// Создаёт спецификацию с заданными критериями фильтрации.
    /// </summary>
    /// <param name="status">Фильтр по статусу поста.</param>
    /// <param name="type">Фильтр по типу контента.</param>
    /// <param name="isPremium">Фильтр по признаку премиум-контента.</param>
    /// <param name="authorId">Фильтр по идентификатору автора.</param>
    /// <param name="categoryId">Фильтр по идентификатору категории.</param>
    /// <param name="tagId">Фильтр по идентификатору тега.</param>
    /// <param name="searchText">Текстовый поиск по заголовку и краткому описанию.</param>
    public PostSpecification(
        PostStatus? status = null,
        PostType? type = null,
        bool? isPremium = null,
        long? authorId = null,
        long? categoryId = null,
        long? tagId = null,
        string? searchText = null
    )
    {
        if (status != null)
            Query.Where(p => p.Status == status);

        if (type != null)
            Query.Where(p => p.Type == type);

        if (isPremium != null)
            Query.Where(p => p.IsPremium == isPremium);

        if (authorId != null)
            Query.Where(p => p.AuthorId == authorId);

        if (categoryId != null)
            Query.Where(p => p.Categories.Any(c => c.Id == categoryId));

        if (tagId != null)
            Query.Where(p => p.Tags.Any(t => t.Id == tagId));

        if (searchText != null)
            Query.Where(p =>
                p.Title.Contains(searchText)
                || (p.Summary != null && p.Summary.Contains(searchText))
            );
    }
}
