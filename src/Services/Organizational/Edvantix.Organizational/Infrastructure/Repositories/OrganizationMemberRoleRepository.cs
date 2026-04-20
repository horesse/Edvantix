using Edvantix.Chassis.Specification.Evaluators;
using Edvantix.Organizational.Domain.AggregatesModel.OrganizationMemberAggregate;

namespace Edvantix.Organizational.Infrastructure.Repositories;

internal sealed class OrganizationMemberRoleRepository(OrganizationalDbContext context)
    : IOrganizationMemberRoleRepository
{
    public IUnitOfWork UnitOfWork => context;
    private static SpecificationEvaluator Specification => SpecificationEvaluator.Instance;

    public async Task AddAsync(
        OrganizationMemberRole role,
        CancellationToken cancellationToken = default
    ) => await context.OrganizationMemberRoles.AddAsync(role, cancellationToken);

    public async Task<OrganizationMemberRole?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default
    ) =>
        await context.OrganizationMemberRoles.FirstOrDefaultAsync(
            r => r.Id == id && !r.IsDeleted,
            cancellationToken
        );

    public async Task<OrganizationMemberRole?> GetByIdWithPermissionsAsync(
        Guid id,
        CancellationToken cancellationToken = default
    ) =>
        await context
            .OrganizationMemberRoles.Include(r => r.Permissions)
            .FirstOrDefaultAsync(r => r.Id == id && !r.IsDeleted, cancellationToken);

    public async Task<OrganizationMemberRole?> GetOwnerRoleAsync(
        Guid organizationId,
        CancellationToken cancellationToken = default
    ) =>
        await context.OrganizationMemberRoles.FirstOrDefaultAsync(
            r => r.OrganizationId == organizationId && r.Code == "owner" && !r.IsDeleted,
            cancellationToken
        );

    public async Task<IReadOnlyCollection<OrganizationMemberRole>> ListAsync(
        ISpecification<OrganizationMemberRole> specification,
        CancellationToken cancellationToken = default
    ) =>
        await Specification
            .GetQuery(context.OrganizationMemberRoles.AsQueryable(), specification)
            .ToListAsync(cancellationToken);

    public async Task<int> CountAsync(
        ISpecification<OrganizationMemberRole> specification,
        CancellationToken cancellationToken = default
    ) =>
        await Specification
            .GetQuery(context.OrganizationMemberRoles.AsQueryable(), specification)
            .CountAsync(cancellationToken);

    public async Task AddRangeAsync(
        IReadOnlyList<OrganizationMemberRole> roles,
        CancellationToken cancellationToken = default
    ) => await context.OrganizationMemberRoles.AddRangeAsync(roles, cancellationToken);
}
