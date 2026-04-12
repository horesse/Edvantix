namespace Edvantix.Organizational.Domain.AggregatesModel.PermissionAggregate;

public interface IPermissionRepository : IRepository<Permission>
{
    Task<IReadOnlyCollection<Permission>> ListAsync(
        ISpecification<Permission> specification,
        CancellationToken token = default
    );
    Task AddRangeAsync(Permission[] permission, CancellationToken token = default);
    void RemoveRange(Permission[] permission);
}
