using Edvantix.Chassis.Specification.Evaluators;

namespace Edvantix.Organizational.Infrastructure.Repositories;

public sealed class OrganizationMemberRepository(OrganizationalDbContext context)
    : IOrganizationMemberRepository
{
    private static SpecificationEvaluator Spec => SpecificationEvaluator.Instance;

    /// <inheritdoc/>
    public IUnitOfWork UnitOfWork => context;

    /// <inheritdoc/>
    public async Task<OrganizationMember?> FindAsync(
        ISpecification<OrganizationMember> spec,
        CancellationToken ct = default
    ) => await Spec.GetQuery(context.Set<OrganizationMember>(), spec).FirstOrDefaultAsync(ct);

    /// <inheritdoc/>
    public async Task<IReadOnlyList<OrganizationMember>> ListAsync(
        ISpecification<OrganizationMember> spec,
        CancellationToken ct = default
    ) => await Spec.GetQuery(context.Set<OrganizationMember>(), spec).ToListAsync(ct);

    /// <inheritdoc/>
    public async Task<int> CountAsync(
        ISpecification<OrganizationMember> spec,
        CancellationToken ct = default
    ) => await Spec.GetQuery(context.Set<OrganizationMember>(), spec).CountAsync(ct);

    /// <inheritdoc/>
    public async Task<OrganizationMember?> FindByIdAsync(Guid id, CancellationToken ct = default) =>
        await context.Set<OrganizationMember>().FirstOrDefaultAsync(x => x.Id == id, ct);

    /// <inheritdoc/>
    public async Task<OrganizationMember> AddAsync(
        OrganizationMember entity,
        CancellationToken ct = default
    )
    {
        var entry = await context.Set<OrganizationMember>().AddAsync(entity, ct);
        return entry.Entity;
    }
}
