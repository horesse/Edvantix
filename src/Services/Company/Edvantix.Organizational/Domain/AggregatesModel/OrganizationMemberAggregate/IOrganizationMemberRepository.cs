namespace Edvantix.Organizational.Domain.AggregatesModel.OrganizationMemberAggregate;

public interface IOrganizationMemberRepository : IRepository<OrganizationMember>
{
    Task<OrganizationMember?> FindAsync(
        ISpecification<OrganizationMember> spec,
        CancellationToken ct = default
    );
    Task<IReadOnlyList<OrganizationMember>> ListAsync(
        ISpecification<OrganizationMember> spec,
        CancellationToken ct = default
    );
    Task<int> CountAsync(ISpecification<OrganizationMember> spec, CancellationToken ct = default);
    Task<OrganizationMember?> FindByIdAsync(Guid id, CancellationToken ct = default);
    Task<OrganizationMember> AddAsync(OrganizationMember entity, CancellationToken ct = default);
}
