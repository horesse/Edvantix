using Edvantix.Chassis.Specification.Evaluators;
using Edvantix.Persona.Domain.AggregatesModel.ProfileAggregate;
using Edvantix.Persona.Domain.AggregatesModel.SkillAggregate;

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
        CancellationToken ct = default
    ) => await Specification.GetQuery(context.Skills, spec).FirstOrDefaultAsync(ct);

    /// <inheritdoc/>
    public async Task<IReadOnlyList<Skill>> FindAllAsync(
        ISpecification<Skill> spec,
        CancellationToken ct = default
    ) => await Specification.GetQuery(context.Skills, spec).ToListAsync(ct);

    /// <inheritdoc/>
    public async Task<Skill?> FindByNameAsync(string name, CancellationToken ct = default)
    {
        var normalized = name.Trim().ToLower();
        return await context.Skills.FirstOrDefaultAsync(s => s.Name.ToLower() == normalized, ct);
    }

    /// <inheritdoc/>
    public async Task<bool> IsUsedByAnyProfileAsync(Guid skillId, CancellationToken ct = default) =>
        await context.Set<ProfileSkill>().AnyAsync(ps => ps.SkillId == skillId, ct);

    /// <inheritdoc/>
    public async Task<Skill> AddAsync(Skill skill, CancellationToken ct = default)
    {
        var entry = await context.Skills.AddAsync(skill, ct);
        return entry.Entity;
    }

    /// <inheritdoc/>
    public void Remove(Skill skill) => context.Skills.Remove(skill);
}
