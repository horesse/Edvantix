namespace Edvantix.Organizational.Domain.AggregatesModel.LegalFormAggregate;

/// <summary>
/// Репозиторий для работы с организационно-правовыми формами.
/// </summary>
public interface ILegalFormRepository : IRepository<LegalForm>
{
    /// <summary>Возвращает все доступные организационно-правовые формы.</summary>
    Task<IReadOnlyList<LegalForm>> ListAllAsync(CancellationToken ct = default);

    /// <summary>Находит организационно-правовую форму по идентификатору.</summary>
    Task<LegalForm?> FindByIdAsync(Guid id, CancellationToken ct = default);
}
