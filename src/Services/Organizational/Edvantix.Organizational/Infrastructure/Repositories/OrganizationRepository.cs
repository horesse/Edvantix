using Edvantix.Chassis.Specification.Evaluators;
using Edvantix.Organizational.Domain.AggregatesModel.OrganizationAggregate;

namespace Edvantix.Organizational.Infrastructure.Repositories;

internal sealed class OrganizationRepository(OrganizationalDbContext context)
    : IOrganizationRepository
{
    public IUnitOfWork UnitOfWork => context;
    private static SpecificationEvaluator Specification => SpecificationEvaluator.Instance;

    public async Task<Organization?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default
    ) =>
        await context
            .Organizations.AsTracking()
            .Include(o => o.Contacts)
            .FirstOrDefaultAsync(o => o.Id == id && !o.IsDeleted, cancellationToken);

    public async Task<IReadOnlyCollection<Organization>> ListAsync(
        ISpecification<Organization> specification,
        CancellationToken cancellationToken = default
    ) =>
        await Specification
            .GetQuery(context.Organizations.AsQueryable(), specification)
            .ToListAsync(cancellationToken);

    public async Task<int> CountAsync(
        ISpecification<Organization> specification,
        CancellationToken cancellationToken = default
    ) =>
        await Specification
            .GetQuery(context.Organizations.AsQueryable(), specification)
            .CountAsync(cancellationToken);

    public async Task AddAsync(
        Organization organization,
        CancellationToken cancellationToken = default
    ) => await context.Organizations.AddAsync(organization, cancellationToken);
}
