using Edvantix.Chassis.Specification.Evaluators;

namespace Edvantix.Persona.Infrastructure.Repositories;

/// <summary>
/// Репозиторий глобального каталога навыков.
/// Поиск по имени выполняется без учёта регистра.
/// </summary>
public sealed class SkillRepository(PersonaDbContext context) : ISkillRepository
{
    private static SpecificationEvaluator Specification => SpecificationEvaluator.Instance;

    /// <inheritdoc/>
    public IUnitOfWork UnitOfWork => context;

    /// <inheritdoc/>
    public async Task<Skill?> FindAsync(
        ISpecification<Skill> spec,
        CancellationToken cancellationToken = default
    ) => await Specification.GetQuery(context.Skills, spec).FirstOrDefaultAsync(cancellationToken);

    /// <inheritdoc/>
    public async Task<IReadOnlyList<Skill>> FindAllAsync(
        ISpecification<Skill> spec,
        CancellationToken cancellationToken = default
    ) => await Specification.GetQuery(context.Skills, spec).ToListAsync(cancellationToken);

    /// <inheritdoc/>
    public async Task<Skill?> FindByNameAsync(
        string name,
        CancellationToken cancellationToken = default
    )
    {
        var normalized = name.Trim().ToLower();
        return await context.Skills.FirstOrDefaultAsync(
            s => s.Name.ToLower() == normalized,
            cancellationToken
        );
    }

    /// <inheritdoc/>
    public async Task<bool> IsUsedByAnyProfileAsync(
        Guid skillId,
        CancellationToken cancellationToken = default
    ) => await context.Set<ProfileSkill>().AnyAsync(ps => ps.SkillId == skillId, cancellationToken);

    /// <inheritdoc/>
    public async Task<Skill> AddAsync(Skill skill, CancellationToken cancellationToken = default)
    {
        var entry = await context.Skills.AddAsync(skill, cancellationToken);
        return entry.Entity;
    }

    /// <inheritdoc/>
    public void Remove(Skill skill) => context.Skills.Remove(skill);
}
