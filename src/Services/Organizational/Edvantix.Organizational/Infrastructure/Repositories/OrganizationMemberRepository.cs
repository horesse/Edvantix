using Edvantix.Chassis.Specification.Evaluators;
using Edvantix.Organizational.Domain.AggregatesModel.OrganizationMemberAggregate;
using Edvantix.Organizational.Domain.Enums;

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
            .Include(m => m.Role)
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

    public async Task<Guid?> GetActiveMemberRoleIdAsync(
        Guid organizationId,
        Guid profileId,
        CancellationToken cancellationToken = default
    ) =>
        await context
            .OrganizationMembers.Where(m =>
                m.OrganizationId == organizationId
                && m.ProfileId == profileId
                && m.Status == OrganizationStatus.Active
                && !m.IsDeleted
            )
            .Select(m => (Guid?)m.OrganizationMemberRoleId)
            .FirstOrDefaultAsync(cancellationToken);

    public async Task<HashSet<string>> GetActivePermissionsAsync(
        Guid organizationId,
        Guid profileId,
        CancellationToken cancellationToken = default
    )
    {
        var permissions = await (
            from member in context.OrganizationMembers
            join role in context.OrganizationMemberRoles
                on member.OrganizationMemberRoleId equals role.Id
            join rp in context.Set<OrganizationMemberRolePermission>()
                on role.Id equals rp.OrganizationMemberRoleId
            join perm in context.Permissions on rp.PermissionId equals perm.Id
            where
                member.OrganizationId == organizationId
                && member.ProfileId == profileId
                && member.Status == OrganizationStatus.Active
                && !member.IsDeleted
                && !role.IsDeleted
            select perm.Name
        )
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        return [.. permissions];
    }
}
