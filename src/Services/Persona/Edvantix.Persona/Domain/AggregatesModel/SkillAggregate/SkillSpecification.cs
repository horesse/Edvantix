namespace Edvantix.Persona.Domain.AggregatesModel.SkillAggregate;

/// <summary>Спецификация поиска навыка по ID.</summary>
public sealed class SkillByIdSpec : Specification<Skill>
{
    public SkillByIdSpec(Guid id)
    {
        Query.Where(s => s.Id == id);
    }
}

/// <summary>
/// Спецификация поиска навыков для автодополнения по подстроке имени.
/// Использует LIKE через <see cref="Edvantix.Chassis.Specification.Builders.SpecificationBuilderExtensions.Search{T}"/>.
/// </summary>
public sealed class SkillSearchSpec : Specification<Skill>
{
    /// <param name="query">Подстрока для поиска (без учёта регистра на уровне LIKE).</param>
    /// <param name="limit">Максимальное количество результатов.</param>
    public SkillSearchSpec(string query, int limit)
    {
        Query
            .Search(s => s.Name, $"%{query}%")
            .OrderBy(s => s.Name)
            .Take(limit)
            .AsNoTracking();
    }
}
