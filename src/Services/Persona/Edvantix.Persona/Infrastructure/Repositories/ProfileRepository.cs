using Edvantix.Chassis.Specification.Evaluators;

namespace Edvantix.Persona.Infrastructure.Repositories;

/// <summary>
/// Репозиторий профилей на основе Entity Framework Core.
/// Использует <see cref="SpecificationEvaluator"/> для построения запросов через спецификации.
/// </summary>
public sealed class ProfileRepository(PersonaDbContext context) : IProfileRepository
{
    private static SpecificationEvaluator Specification => SpecificationEvaluator.Instance;

    /// <inheritdoc/>
    public IUnitOfWork UnitOfWork => context;

    /// <inheritdoc/>
    public async Task<Profile?> FindAsync(
        ISpecification<Profile> spec,
        CancellationToken ct = default
    ) => await Specification.GetQuery(context.Set<Profile>(), spec).FirstOrDefaultAsync(ct);

    /// <inheritdoc/>
    public async Task<bool> ExistsByAccountIdAsync(
        Guid accountId,
        CancellationToken ct = default
    ) => await context.Set<Profile>().AnyAsync(p => p.AccountId == accountId, ct);

    /// <inheritdoc/>
    public async Task<Profile> AddAsync(Profile profile, CancellationToken ct = default)
    {
        var entry = await context.Set<Profile>().AddAsync(profile, ct);
        return entry.Entity;
    }
}
