namespace Edvantix.Blog.Domain.AggregatesModel.CategoryAggregate;

/// <summary>
/// Спецификация для поиска категорий блога.
/// </summary>
public sealed class CategorySpecification : Specification<Category>
{
    public CategorySpecification(string slug)
    {
        Query.Where(x => x.Slug == slug);
    }
}
