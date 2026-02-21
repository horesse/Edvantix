using Edvantix.Chassis.Specification.Evaluators;
using Edvantix.Organizational.Domain.AggregatesModel.InvitationAggregate;

namespace Edvantix.Organizational.Infrastructure.Repositories;

/// <summary>
/// Репозиторий приглашений. Использует CrudRepository (не SoftDelete — статусный lifecycle).
/// </summary>
public sealed class InvitationRepository(OrganizationalDbContext context) : IInvitationRepository
{
    private static SpecificationEvaluator Spec => SpecificationEvaluator.Instance;

    /// <inheritdoc/>
    public IUnitOfWork UnitOfWork => context;

    /// <inheritdoc/>
    public async Task<Invitation?> FindAsync(
        ISpecification<Invitation> spec,
        CancellationToken ct = default
    ) => await Spec.GetQuery(context.Set<Invitation>(), spec).FirstOrDefaultAsync(ct);

    /// <inheritdoc/>
    public async Task<IReadOnlyList<Invitation>> ListAsync(
        ISpecification<Invitation> spec,
        CancellationToken ct = default
    ) => await Spec.GetQuery(context.Set<Invitation>(), spec).ToListAsync(ct);

    /// <inheritdoc/>
    public async Task<Invitation?> FindByIdAsync(Guid id, CancellationToken ct = default) =>
        await context.Set<Invitation>().FirstOrDefaultAsync(x => x.Id == id, ct);

    /// <inheritdoc/>
    public async Task<Invitation> AddAsync(Invitation entity, CancellationToken ct = default)
    {
        var entry = await context.Set<Invitation>().AddAsync(entity, ct);
        return entry.Entity;
    }
}
