namespace Edvantix.Persona.Domain.AggregatesModel.SkillAggregate;

/// <summary>
/// Репозиторий глобального каталога навыков.
/// </summary>
public interface ISkillRepository : IRepository<Skill>
{
    /// <summary>Ищет навык по спецификации.</summary>
    Task<Skill?> FindAsync(
        ISpecification<Skill> spec,
        CancellationToken cancellationToken = default
    );

    /// <summary>Возвращает все навыки, соответствующие спецификации.</summary>
    Task<IReadOnlyList<Skill>> FindAllAsync(
        ISpecification<Skill> spec,
        CancellationToken cancellationToken = default
    );

    /// <summary>Проверяет, используется ли навык хотя бы одним профилем.</summary>
    Task<bool> IsUsedByAnyProfileAsync(Guid skillId, CancellationToken cancellationToken = default);

    /// <summary>Добавляет новый навык в каталог. Для сохранения вызовите <see cref="IUnitOfWork.SaveEntitiesAsync"/>.</summary>
    Task<Skill> AddAsync(Skill skill, CancellationToken cancellationToken = default);

    /// <summary>Помечает навык для удаления. Для сохранения вызовите <see cref="IUnitOfWork.SaveEntitiesAsync"/>.</summary>
    void Remove(Skill skill);
}
