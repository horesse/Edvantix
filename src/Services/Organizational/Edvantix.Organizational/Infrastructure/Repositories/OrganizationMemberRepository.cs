using Edvantix.Chassis.Specification.Evaluators;
using Edvantix.Organizational.Domain.AggregatesModel.OrganizationMemberAggregate;

namespace Edvantix.Organizational.Infrastructure.Repositories;

internal sealed class OrganizationMemberRepository(OrganizationalDbContext context)
    : IOrganizationMemberRepository
{
    public IUnitOfWork UnitOfWork => context;
    private static SpecificationEvaluator Specification => SpecificationEvaluator.Instance;

    public async Task<OrganizationMember?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default
    ) =>
        await context
            .OrganizationMembers.AsTracking()
            .FirstOrDefaultAsync(m => m.Id == id && !m.IsDeleted, cancellationToken);

    public async Task<IReadOnlyCollection<OrganizationMember>> ListByOrganizationAsync(
        Guid organizationId,
        CancellationToken cancellationToken = default
    ) =>
        await context
            .OrganizationMembers.Where(m => m.OrganizationId == organizationId && !m.IsDeleted)
            .ToListAsync(cancellationToken);

    public async Task<IReadOnlyCollection<OrganizationMember>> ListAsync(
        ISpecification<OrganizationMember> specification,
        CancellationToken cancellationToken = default
    ) =>
        await Specification
            .GetQuery(context.OrganizationMembers.AsQueryable(), specification)
            .ToListAsync(cancellationToken);

    public async Task<int> CountAsync(
        ISpecification<OrganizationMember> specification,
        CancellationToken cancellationToken = default
    ) =>
        await Specification
            .GetQuery(context.OrganizationMembers.AsQueryable(), specification)
            .CountAsync(cancellationToken);

    public async Task AddAsync(
        OrganizationMember member,
        CancellationToken cancellationToken = default
    ) => await context.OrganizationMembers.AddAsync(member, cancellationToken);
}
