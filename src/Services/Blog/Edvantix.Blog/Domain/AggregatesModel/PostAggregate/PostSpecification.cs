namespace Edvantix.Blog.Domain.AggregatesModel.PostAggregate;

/// <summary>
/// Спецификация для выборки постов блога с поддержкой фильтрации, поиска и пагинации.
/// Все условия применяются единожды в конструкторе для корректной генерации SQL.
/// </summary>
public sealed class PostSpecification : Specification<Post>
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
    /// <param name="includeRelations">
    /// Подгружать категории и теги через Include.
    /// </param>
    public PostSpecification(
        PostStatus? status = null,
        PostType? type = null,
        bool? isPremium = null,
        Guid? authorId = null,
        Guid? categoryId = null,
        Guid? tagId = null,
        string? searchText = null,
        bool includeRelations = false
    )
    {
        if (includeRelations)
        {
            Query.Include(p => p.Categories);
            Query.Include(p => p.Tags);
        }

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

public sealed class PostByIdSpecification : Specification<Post>
{
    public PostByIdSpecification(Guid id, bool includeRelations = true)
    {
        Query.Where(p => p.Id == id);

        if (!includeRelations)
            return;

        Query.Include(p => p.Categories);
        Query.Include(p => p.Tags);
    }
}

public sealed class PostBySlugSpecification : Specification<Post>
{
    public PostBySlugSpecification(string slug, bool includeRelations = true)
    {
        Query.Where(p => p.Slug == slug);

        if (!includeRelations)
            return;

        Query.Include(p => p.Categories);
        Query.Include(p => p.Tags);
    }
}
