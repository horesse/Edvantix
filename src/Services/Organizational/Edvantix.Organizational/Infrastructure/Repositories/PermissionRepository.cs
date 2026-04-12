using Edvantix.Chassis.Specification.Evaluators;
using Edvantix.Organizational.Domain.AggregatesModel.PermissionAggregate;

namespace Edvantix.Organizational.Infrastructure.Repositories;

internal sealed class PermissionRepository(OrganizationalDbContext context) : IPermissionRepository
{
    public IUnitOfWork UnitOfWork => context;
    private static SpecificationEvaluator Specification => SpecificationEvaluator.Instance;

    public async Task<IReadOnlyCollection<Permission>> ListAsync(
        ISpecification<Permission> specification,
        CancellationToken token = default
    )
    {
        return await Specification
            .GetQuery(context.Permissions.AsQueryable(), specification)
            .ToListAsync(token);
    }

    public async Task AddRangeAsync(Permission[] permissions, CancellationToken token = default) =>
        await context.AddRangeAsync(permissions, token);

    public void RemoveRange(Permission[] permissions) =>
        context.Permissions.RemoveRange(permissions);
}
