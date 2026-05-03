namespace Edvantix.Organizational.Domain.AggregatesModel.OrganizationRoleAggregate;

public interface IOrganizationRoleRepository : IRepository<OrganizationRole>
{
    Task AddAsync(OrganizationRole role, CancellationToken cancellationToken = default);

    Task<OrganizationRole?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>Возвращает роль со связанными разрешениями.</summary>
    Task<OrganizationRole?> GetByIdWithPermissionsAsync(
        Guid id,
        CancellationToken cancellationToken = default
    );

    Task<OrganizationRole?> GetOwnerRoleAsync(
        Guid organizationId,
        CancellationToken cancellationToken = default
    );

    Task<IReadOnlyCollection<OrganizationRole>> ListAsync(
        ISpecification<OrganizationRole> specification,
        CancellationToken cancellationToken = default
    );

    Task<int> CountAsync(
        ISpecification<OrganizationRole> specification,
        CancellationToken cancellationToken = default
    );

    Task AddRangeAsync(
        IReadOnlyList<OrganizationRole> roles,
        CancellationToken cancellationToken = default
    );
}
