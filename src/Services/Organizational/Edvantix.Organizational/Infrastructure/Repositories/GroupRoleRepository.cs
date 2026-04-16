using Edvantix.Organizational.Domain.AggregatesModel.GroupAggregate;

namespace Edvantix.Organizational.Infrastructure.Repositories;

internal sealed class GroupRoleRepository(OrganizationalDbContext context) : IGroupRoleRepository
{
    public IUnitOfWork UnitOfWork => context;

    public async Task AddRangeAsync(
        IReadOnlyList<GroupRole> roles,
        CancellationToken cancellationToken = default
    ) => await context.GroupRoles.AddRangeAsync(roles, cancellationToken);
}
