using Edvantix.Chassis.Specification.Evaluators;
using Edvantix.Organizations.Domain.AggregatesModel.GroupAggregate;

namespace Edvantix.Organizations.Infrastructure.Repositories;

/// <summary>
/// EF Core implementation of <see cref="IGroupRepository"/>.
/// Tenant and soft-delete query filters are applied globally by <see cref="OrganizationsDbContext"/>.
/// </summary>
public sealed class GroupRepository(OrganizationsDbContext context) : IGroupRepository
{
    private static SpecificationEvaluator Spec => SpecificationEvaluator.Instance;

    /// <inheritdoc/>
    public IUnitOfWork UnitOfWork => context;

    /// <inheritdoc/>
    public async Task<Group?> FindByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default
    ) => await context.Groups.FirstOrDefaultAsync(g => g.Id == id, cancellationToken);

    /// <inheritdoc/>
    public async Task<List<Group>> ListAsync(
        Specification<Group> spec,
        CancellationToken cancellationToken = default
    ) => await Spec.GetQuery(context.Groups, spec).ToListAsync(cancellationToken);

    /// <inheritdoc/>
    public async Task<Group?> FindAsync(
        Specification<Group> spec,
        CancellationToken cancellationToken = default
    ) => await Spec.GetQuery(context.Groups, spec).FirstOrDefaultAsync(cancellationToken);

    /// <inheritdoc/>
    public void Add(Group group) => context.Groups.Add(group);

    /// <inheritdoc/>
    public void Remove(Group group) => context.Groups.Remove(group);
}
