using Edvantix.Chassis.Specification.Evaluators;
using Edvantix.Organizational.Domain.AggregatesModel.GroupAggregate;

namespace Edvantix.Organizational.Infrastructure.Repositories;

public sealed class GroupMemberRepository(OrganizationalDbContext context) : IGroupMemberRepository
{
    private static SpecificationEvaluator Spec => SpecificationEvaluator.Instance;

    /// <inheritdoc/>
    public IUnitOfWork UnitOfWork => context;

    /// <inheritdoc/>
    public async Task<GroupMember?> FindAsync(
        ISpecification<GroupMember> spec,
        CancellationToken ct = default
    ) => await Spec.GetQuery(context.Set<GroupMember>(), spec).FirstOrDefaultAsync(ct);

    /// <inheritdoc/>
    public async Task<IReadOnlyList<GroupMember>> ListAsync(
        ISpecification<GroupMember> spec,
        CancellationToken ct = default
    ) => await Spec.GetQuery(context.Set<GroupMember>(), spec).ToListAsync(ct);

    /// <inheritdoc/>
    public async Task<int> CountAsync(
        ISpecification<GroupMember> spec,
        CancellationToken ct = default
    ) => await Spec.GetQuery(context.Set<GroupMember>(), spec).CountAsync(ct);

    /// <inheritdoc/>
    public async Task<GroupMember?> FindByIdAsync(Guid id, CancellationToken ct = default) =>
        await context.Set<GroupMember>().FirstOrDefaultAsync(x => x.Id == id, ct);

    /// <inheritdoc/>
    public async Task<GroupMember> AddAsync(GroupMember entity, CancellationToken ct = default)
    {
        var entry = await context.Set<GroupMember>().AddAsync(entity, ct);
        return entry.Entity;
    }
}
