using Edvantix.Chassis.Specification.Builders;
using Edvantix.SharedKernel.SeedWork;

namespace Edvantix.Chassis.Specification.Generic;

public class PagedSpecification<TEntity> : Specification<TEntity>
    where TEntity : class, IAggregateRoot
{
    public int PageSize { get; set; }
    public int CurrentPage { get; set; }
    public bool ShowDeleted { get; set; } = false;

    public PagedSpecification()
    {
        if (PageSize > 0 && CurrentPage > 0)
        {
            Query.Skip((CurrentPage - 1) * PageSize).Take(PageSize);
        }
        
        if (typeof(ISoftDelete).IsAssignableFrom(typeof(TEntity)) && !ShowDeleted)
        {
            Query.Where(e => ((ISoftDelete)e).IsDeleted == false);
        }
    }
}
