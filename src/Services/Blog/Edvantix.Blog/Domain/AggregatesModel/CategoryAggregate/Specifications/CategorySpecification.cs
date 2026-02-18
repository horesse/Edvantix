using Edvantix.Chassis.Specification.Builders;
using Edvantix.Chassis.Specification.Generic;

namespace Edvantix.Blog.Domain.AggregatesModel.CategoryAggregate.Specifications;

/// <summary>
/// Спецификация для поиска категорий блога.
/// </summary>
public sealed class CategorySpecification : CommonSpecification<Category>
{
    private string? _slug;

    /// <summary>Фильтр по slug категории.</summary>
    public string? Slug
    {
        get => _slug;
        init
        {
            _slug = value;
            if (_slug is not null)
                Query.Where(c => c.Slug == _slug);
        }
    }
}
