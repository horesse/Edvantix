namespace Edvantix.Blog.Domain.AggregatesModel.TagAggregate;

/// <summary>
/// Спецификация для поиска тегов блога.
/// </summary>
public sealed class TagSpecification : Specification<Tag>
{
    public TagSpecification(string slug)
    {
        Query.Where(x => x.Slug == slug);
    }
}
