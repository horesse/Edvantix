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
        CancellationToken cancellationToken = default
    ) =>
        await Specification.GetQuery(context.Profiles, spec).FirstOrDefaultAsync(cancellationToken);

    /// <inheritdoc/>
    public async Task<bool> ExistsByAccountIdAsync(
        Guid accountId,
        CancellationToken cancellationToken = default
    ) => await context.Profiles.AnyAsync(p => p.AccountId == accountId, cancellationToken);

    /// <inheritdoc/>
    public async Task<Profile> AddAsync(
        Profile profile,
        CancellationToken cancellationToken = default
    )
    {
        var entry = await context.Profiles.AddAsync(profile, cancellationToken);
        return entry.Entity;
    }

    /// <inheritdoc/>
    public async Task<IReadOnlyList<Profile>> ListAsync(
        ISpecification<Profile> spec,
        CancellationToken cancellationToken = default
    ) => await Specification.GetQuery(context.Profiles, spec).ToListAsync(cancellationToken);

    /// <inheritdoc/>
    public async Task<int> CountAsync(
        ISpecification<Profile> spec,
        CancellationToken cancellationToken = default
    ) => await Specification.GetQuery(context.Profiles, spec).CountAsync(cancellationToken);
}
