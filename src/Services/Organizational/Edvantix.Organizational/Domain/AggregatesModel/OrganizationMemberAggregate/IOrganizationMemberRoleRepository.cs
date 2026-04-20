namespace Edvantix.Organizational.Domain.AggregatesModel.OrganizationMemberAggregate;

public interface IOrganizationMemberRoleRepository : IRepository<OrganizationMemberRole>
{
    Task AddAsync(OrganizationMemberRole role, CancellationToken cancellationToken = default);

    Task<OrganizationMemberRole?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default
    );

    /// <summary>Возвращает роль со связанными разрешениями.</summary>
    Task<OrganizationMemberRole?> GetByIdWithPermissionsAsync(
        Guid id,
        CancellationToken cancellationToken = default
    );

    Task<OrganizationMemberRole?> GetOwnerRoleAsync(
        Guid organizationId,
        CancellationToken cancellationToken = default
    );

    Task<IReadOnlyCollection<OrganizationMemberRole>> ListAsync(
        ISpecification<OrganizationMemberRole> specification,
        CancellationToken cancellationToken = default
    );

    Task<int> CountAsync(
        ISpecification<OrganizationMemberRole> specification,
        CancellationToken cancellationToken = default
    );

    Task AddRangeAsync(
        IReadOnlyList<OrganizationMemberRole> roles,
        CancellationToken cancellationToken = default
    );
}
