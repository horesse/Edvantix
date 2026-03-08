using Edvantix.Organizational.Domain.AggregatesModel.LegalFormAggregate;

namespace Edvantix.Organizational.Infrastructure.Repositories;

public sealed class LegalFormRepository(OrganizationalDbContext context) : ILegalFormRepository
{
    /// <inheritdoc/>
    public IUnitOfWork UnitOfWork => context;

    /// <inheritdoc/>
    public async Task<IReadOnlyList<LegalForm>> ListAllAsync(CancellationToken ct = default) =>
        await context.Set<LegalForm>().OrderBy(lf => lf.Name).ToListAsync(ct);

    /// <inheritdoc/>
    public async Task<LegalForm?> FindByIdAsync(Guid id, CancellationToken ct = default) =>
        await context.Set<LegalForm>().FirstOrDefaultAsync(lf => lf.Id == id, ct);
}
