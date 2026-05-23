using Edvantix.Chassis.Specification.Evaluators;
using Edvantix.Organizational.Domain.AggregatesModel.OrganizationRoleAggregate;

namespace Edvantix.Organizational.Infrastructure.Repositories;

internal sealed class OrganizationRoleRepository(OrganizationalDbContext context)
    : IOrganizationRoleRepository
{
    public IUnitOfWork UnitOfWork => context;
    private static SpecificationEvaluator Specification => SpecificationEvaluator.Instance;

    public async Task AddAsync(
        OrganizationRole role,
        CancellationToken cancellationToken = default
    ) => await context.OrganizationRoles.AddAsync(role, cancellationToken);

    public async Task<OrganizationRole?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default
    ) =>
        await context
            .OrganizationRoles.AsTracking()
            .Include(r => r.Permissions)
            .FirstOrDefaultAsync(r => r.Id == id && !r.IsDeleted, cancellationToken);

    public async Task<OrganizationRole?> GetByIdWithPermissionsAsync(
        Guid id,
        CancellationToken cancellationToken = default
    ) =>
        await context
            .OrganizationRoles.Include(r => r.Permissions)
                .ThenInclude(p => p.Feature)
            .FirstOrDefaultAsync(r => r.Id == id && !r.IsDeleted, cancellationToken);

    public async Task<OrganizationRole?> GetOwnerRoleAsync(
        Guid organizationId,
        CancellationToken cancellationToken = default
    ) =>
        await context.OrganizationRoles.FirstOrDefaultAsync(
            r => r.OrganizationId == organizationId && r.IsSystem && !r.IsDeleted,
            cancellationToken
        );

    public async Task<IReadOnlyCollection<OrganizationRole>> ListAsync(
        ISpecification<OrganizationRole> specification,
        CancellationToken cancellationToken = default
    ) =>
        await Specification
            .GetQuery(context.OrganizationRoles.AsQueryable(), specification)
            .ToListAsync(cancellationToken);

    public async Task<int> CountAsync(
        ISpecification<OrganizationRole> specification,
        CancellationToken cancellationToken = default
    ) =>
        await Specification
            .GetQuery(context.OrganizationRoles.AsQueryable(), specification)
            .CountAsync(cancellationToken);

    public async Task AddRangeAsync(
        IReadOnlyList<OrganizationRole> roles,
        CancellationToken cancellationToken = default
    ) => await context.OrganizationRoles.AddRangeAsync(roles, cancellationToken);
}
