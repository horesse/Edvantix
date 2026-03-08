using Edvantix.Chassis.Specification.Evaluators;

namespace Edvantix.Organizational.Infrastructure.Repositories;

public sealed class OrganizationRepository(OrganizationalDbContext context)
    : IOrganizationRepository
{
    private static SpecificationEvaluator Spec => SpecificationEvaluator.Instance;

    /// <inheritdoc/>
    public IUnitOfWork UnitOfWork => context;

    /// <inheritdoc/>
    public async Task<Organization?> FindAsync(
        ISpecification<Organization> spec,
        CancellationToken ct = default
    ) => await Spec.GetQuery(context.Set<Organization>(), spec).FirstOrDefaultAsync(ct);

    /// <inheritdoc/>
    public async Task<Organization?> FindByIdAsync(Guid id, CancellationToken ct = default) =>
        await context
            .Set<Organization>()
            .Include(o => o.Members)
            .Include(o => o.Groups)
            .FirstOrDefaultAsync(o => o.Id == id, ct);

    /// <inheritdoc/>
    public async Task<Organization> AddAsync(Organization entity, CancellationToken ct = default)
    {
        var entry = await context.Set<Organization>().AddAsync(entity, ct);
        return entry.Entity;
    }
}
