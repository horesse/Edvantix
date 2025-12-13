using Edvantix.Chassis.Specification;

namespace Edvantix.Chassis.Endpoints.Requests;

public class PaginationRequest<TSpecification, TEntity>
    where TSpecification : ISpecification<TEntity>
    where TEntity : class
{
    public int Page { get; set; }
    
    public int PageSize { get; set; }
    
    public TSpecification? Specification { get; set; }
}
