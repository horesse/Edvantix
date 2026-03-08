using Edvantix.Chassis.Specification.Evaluators;

namespace Edvantix.Organizational.Infrastructure.Repositories;

public sealed class GroupRepository(OrganizationalDbContext context) : IGroupRepository
{
    private static SpecificationEvaluator Spec => SpecificationEvaluator.Instance;

    /// <inheritdoc/>
    public IUnitOfWork UnitOfWork => context;

    /// <inheritdoc/>
    public async Task<Group?> FindAsync(
        ISpecification<Group> spec,
        CancellationToken ct = default
    ) => await Spec.GetQuery(context.Set<Group>(), spec).FirstOrDefaultAsync(ct);

    /// <inheritdoc/>
    public async Task<IReadOnlyList<Group>> ListAsync(
        ISpecification<Group> spec,
        CancellationToken ct = default
    ) => await Spec.GetQuery(context.Set<Group>(), spec).ToListAsync(ct);

    /// <inheritdoc/>
    public async Task<int> CountAsync(ISpecification<Group> spec, CancellationToken ct = default) =>
        await Spec.GetQuery(context.Set<Group>(), spec).CountAsync(ct);

    /// <inheritdoc/>
    public async Task<Group?> FindByIdAsync(Guid id, CancellationToken ct = default) =>
        await context.Set<Group>().Include(g => g.Members).FirstOrDefaultAsync(g => g.Id == id, ct);

    /// <inheritdoc/>
    public async Task<Group> AddAsync(Group entity, CancellationToken ct = default)
    {
        var entry = await context.Set<Group>().AddAsync(entity, ct);
        return entry.Entity;
    }
}
