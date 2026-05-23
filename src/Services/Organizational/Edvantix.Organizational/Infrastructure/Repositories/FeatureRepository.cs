using Edvantix.Organizational.Domain.AggregatesModel.PermissionAggregate;

namespace Edvantix.Organizational.Infrastructure.Repositories;

internal sealed class FeatureRepository(OrganizationalDbContext context) : IFeatureRepository
{
    public IUnitOfWork UnitOfWork => context;

    public Task<List<Feature>> GetAllAsync(CancellationToken cancellationToken = default) =>
        context.Features.AsNoTracking().ToListAsync(cancellationToken);

    public Task<Feature?> GetByCodeAsync(
        string code,
        CancellationToken cancellationToken = default
    ) => context.Features.FirstOrDefaultAsync(f => f.Code == code, cancellationToken);

    public void Add(Feature feature) => context.Features.Add(feature);
}
