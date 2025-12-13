using Edvantix.Chassis.Specification.Builders;
using Edvantix.SharedKernel.SeedWork;

namespace Edvantix.Chassis.Specification.Generic;

public class PagedSpecification<TEntity> : CommonSpecification<TEntity>
    where TEntity : class, IAggregateRoot
{
    private readonly int _pageSize;
    private readonly int _currentPage;

    public int PageSize
    {
        get => _pageSize;
        init
        {
            _pageSize = value;
            ApplyFilters();
        }
    }

    public int CurrentPage
    {
        get => _currentPage;
        init
        {
            _currentPage = value;
            ApplyFilters();
        }
    }

    protected override void ApplyFilters()
    {
        if (_pageSize > 0 || _currentPage > 0)
        {
            Query.Skip((_currentPage - 1) * _pageSize).Take(_pageSize);
        }

        base.ApplyFilters();
    }
}
