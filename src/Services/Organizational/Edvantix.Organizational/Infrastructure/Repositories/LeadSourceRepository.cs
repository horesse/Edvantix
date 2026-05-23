using Edvantix.Chassis.Repository;
using Edvantix.Chassis.Specification;
using Edvantix.Chassis.Specification.Evaluators;
using Edvantix.Organizational.Domain.AggregatesModel.LeadSourceAggregate;

namespace Edvantix.Organizational.Infrastructure.Repositories;

internal sealed class LeadSourceRepository(OrganizationalDbContext context) : ILeadSourceRepository
{
    private static SpecificationEvaluator Evaluator => SpecificationEvaluator.Instance;

    public IUnitOfWork UnitOfWork => context;

    public async Task AddAsync(LeadSource leadSource, CancellationToken ct = default) =>
        await context.LeadSources.AddAsync(leadSource, ct);

    public async Task<LeadSource?> GetByIdAsync(Guid id, CancellationToken ct = default) =>
        await context.LeadSources.AsTracking().FirstOrDefaultAsync(ls => ls.Id == id, ct);

    public async Task<IReadOnlyList<LeadSource>> ListAsync(
        ISpecification<LeadSource> specification,
        CancellationToken ct = default
    ) => await Evaluator.GetQuery(context.LeadSources.AsQueryable(), specification).ToListAsync(ct);

    public async Task<int> CountAsync(
        ISpecification<LeadSource> specification,
        CancellationToken ct = default
    ) => await Evaluator.GetQuery(context.LeadSources.AsQueryable(), specification).CountAsync(ct);

    public async Task<bool> AnyAsync(
        ISpecification<LeadSource> specification,
        CancellationToken ct = default
    ) => await Evaluator.GetQuery(context.LeadSources.AsQueryable(), specification).AnyAsync(ct);

    public async Task<DateTime?> GetLastModifiedAtAsync(
        Guid organizationId,
        CancellationToken ct = default
    ) =>
        await context
            .LeadSources.Where(ls => ls.OrganizationId == organizationId)
            .MaxAsync(ls => (DateTime?)ls.LastModifiedAt, ct);
}
