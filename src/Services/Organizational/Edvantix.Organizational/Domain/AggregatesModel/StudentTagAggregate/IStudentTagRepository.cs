using Edvantix.Chassis.Repository;
using Edvantix.Chassis.Specification;

namespace Edvantix.Organizational.Domain.AggregatesModel.StudentTagAggregate;

/// <summary>Репозиторий тегов студентов.</summary>
public interface IStudentTagRepository : IRepository<StudentTag>
{
    /// <summary>Добавляет новый тег в хранилище.</summary>
    Task AddAsync(StudentTag studentTag, CancellationToken ct = default);

    /// <summary>Возвращает тег по идентификатору или <c>null</c>, если не найден.</summary>
    Task<StudentTag?> GetByIdAsync(Guid id, CancellationToken ct = default);

    /// <summary>Возвращает список тегов, соответствующих спецификации.</summary>
    Task<IReadOnlyList<StudentTag>> ListAsync(
        ISpecification<StudentTag> specification,
        CancellationToken ct = default
    );

    /// <summary>Возвращает количество тегов, соответствующих спецификации.</summary>
    Task<int> CountAsync(ISpecification<StudentTag> specification, CancellationToken ct = default);

    /// <summary>Проверяет, существует ли хотя бы один тег, соответствующий спецификации.</summary>
    Task<bool> AnyAsync(ISpecification<StudentTag> specification, CancellationToken ct = default);

    /// <summary>Возвращает дату последнего изменения любого тега организации.</summary>
    Task<DateTime?> GetLastModifiedAtAsync(Guid organizationId, CancellationToken ct = default);
}
