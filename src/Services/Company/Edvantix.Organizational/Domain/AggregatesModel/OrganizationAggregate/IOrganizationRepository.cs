namespace Edvantix.Organizational.Domain.AggregatesModel.OrganizationAggregate;

public interface IOrganizationRepository : IRepository<Organization>
{
    Task<Organization?> FindAsync(
        ISpecification<Organization> spec,
        CancellationToken ct = default
    );
    Task<Organization?> FindByIdAsync(Guid id, CancellationToken ct = default);
    Task<Organization> AddAsync(Organization entity, CancellationToken ct = default);
}
