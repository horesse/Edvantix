using Edvantix.Chassis.Specification.Builders;
using Edvantix.SharedKernel.SeedWork;

namespace Edvantix.Chassis.Specification.Generic;

public class PagedSpecification<TEntity> : CommonSpecification<TEntity>
    where TEntity : class, IAggregateRoot
{
    public int PageSize { get; set; }
    public int CurrentPage { get; set; }

    public PagedSpecification()
    {
        if (PageSize > 0 && CurrentPage > 0)
        {
            Query.Skip((CurrentPage - 1) * PageSize).Take(PageSize);
        }
    }
}
