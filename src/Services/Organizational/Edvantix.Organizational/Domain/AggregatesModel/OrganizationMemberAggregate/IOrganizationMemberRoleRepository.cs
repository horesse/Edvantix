namespace Edvantix.Organizational.Domain.AggregatesModel.OrganizationMemberAggregate;

public interface IOrganizationMemberRoleRepository : IRepository<OrganizationMemberRole>
{
    Task<OrganizationMemberRole?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default
    );

    Task<OrganizationMemberRole?> GetOwnerRoleAsync(
        Guid organizationId,
        CancellationToken cancellationToken = default
    );

    Task AddRangeAsync(
        IReadOnlyList<OrganizationMemberRole> roles,
        CancellationToken cancellationToken = default
    );
}
