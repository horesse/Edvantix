using Edvantix.Organizational.Domain.AggregatesModel.OrganizationMemberAggregate;

namespace Edvantix.Organizational.Infrastructure.Repositories;

internal sealed class OrganizationMemberRoleRepository(OrganizationalDbContext context)
    : IOrganizationMemberRoleRepository
{
    public IUnitOfWork UnitOfWork => context;

    public async Task<OrganizationMemberRole?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default
    ) =>
        await context.OrganizationMemberRoles.FirstOrDefaultAsync(
            r => r.Id == id && !r.IsDeleted,
            cancellationToken
        );

    public async Task<OrganizationMemberRole?> GetOwnerRoleAsync(
        Guid organizationId,
        CancellationToken cancellationToken = default
    ) =>
        await context.OrganizationMemberRoles.FirstOrDefaultAsync(
            r => r.OrganizationId == organizationId && r.Code == "owner" && !r.IsDeleted,
            cancellationToken
        );

    public async Task AddRangeAsync(
        IReadOnlyList<OrganizationMemberRole> roles,
        CancellationToken cancellationToken = default
    ) => await context.OrganizationMemberRoles.AddRangeAsync(roles, cancellationToken);
}
