using Edvantix.Organizational.Domain.AggregatesModel.PermissionAggregate;

namespace Edvantix.Organizational.Infrastructure.Repositories;

internal sealed class FeatureRepository(OrganizationalDbContext context) : IFeatureRepository
{
    public IUnitOfWork UnitOfWork => context;

    public async Task<Feature?> GetByCodeAsync(
        string code,
        CancellationToken cancellationToken = default
    ) => await context.Features.FirstOrDefaultAsync(f => f.Code == code, cancellationToken);

    public void Add(Feature feature) => context.Features.Add(feature);
}
