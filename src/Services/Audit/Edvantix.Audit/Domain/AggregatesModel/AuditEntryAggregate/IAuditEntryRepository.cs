namespace Edvantix.Audit.Domain.AggregatesModel.AuditEntryAggregate;

/// <summary>Репозиторий агрегата <see cref="AuditEntry"/>.</summary>
public interface IAuditEntryRepository : IRepository<AuditEntry>
{
    /// <summary>Возвращает запись аудита по идентификатору.</summary>
    Task<AuditEntry?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>Возвращает страницу записей аудита по спецификации.</summary>
    Task<IReadOnlyCollection<AuditEntry>> ListAsync(
        ISpecification<AuditEntry> specification,
        CancellationToken cancellationToken = default
    );

    /// <summary>Подсчитывает количество записей аудита по спецификации.</summary>
    Task<int> CountAsync(
        ISpecification<AuditEntry> specification,
        CancellationToken cancellationToken = default
    );

    /// <summary>Добавляет новую запись аудита.</summary>
    Task AddAsync(AuditEntry entry, CancellationToken cancellationToken = default);
}
