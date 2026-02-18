using Edvantix.Chassis.Specification.Builders;
using Edvantix.Chassis.Specification.Generic;

namespace Edvantix.Blog.Domain.AggregatesModel.TagAggregate.Specifications;

/// <summary>
/// Спецификация для поиска тегов блога.
/// </summary>
public sealed class TagSpecification : CommonSpecification<Tag>
{
    private string? _slug;

    /// <summary>Фильтр по slug тега.</summary>
    public string? Slug
    {
        get => _slug;
        init
        {
            _slug = value;
            if (_slug is not null)
                Query.Where(t => t.Slug == _slug);
        }
    }
}
