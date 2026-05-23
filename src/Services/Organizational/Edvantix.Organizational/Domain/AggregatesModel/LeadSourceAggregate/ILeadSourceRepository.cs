using Edvantix.Chassis.Repository;
using Edvantix.Chassis.Specification;

namespace Edvantix.Organizational.Domain.AggregatesModel.LeadSourceAggregate;

/// <summary>Репозиторий агрегата <see cref="LeadSource"/>.</summary>
public interface ILeadSourceRepository : IRepository<LeadSource>
{
    /// <summary>Добавляет новый источник привлечения.</summary>
    Task AddAsync(LeadSource leadSource, CancellationToken ct = default);

    /// <summary>Возвращает источник привлечения по идентификатору (включая архивные).</summary>
    Task<LeadSource?> GetByIdAsync(Guid id, CancellationToken ct = default);

    /// <summary>Возвращает список источников, удовлетворяющих спецификации.</summary>
    Task<IReadOnlyList<LeadSource>> ListAsync(
        ISpecification<LeadSource> specification,
        CancellationToken ct = default
    );

    /// <summary>Возвращает количество источников, удовлетворяющих спецификации.</summary>
    Task<int> CountAsync(ISpecification<LeadSource> specification, CancellationToken ct = default);

    /// <summary>
    /// Возвращает <see langword="true"/>, если существует хотя бы один источник,
    /// удовлетворяющий спецификации.
    /// </summary>
    Task<bool> AnyAsync(ISpecification<LeadSource> specification, CancellationToken ct = default);

    /// <summary>Дата последнего изменения любой записи организации.</summary>
    Task<DateTime?> GetLastModifiedAtAsync(Guid organizationId, CancellationToken ct = default);
}
