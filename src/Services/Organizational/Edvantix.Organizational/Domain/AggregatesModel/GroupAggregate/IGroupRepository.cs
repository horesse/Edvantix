namespace Edvantix.Organizational.Domain.AggregatesModel.GroupAggregate;

/// <summary>Репозиторий агрегата <see cref="Group"/>.</summary>
public interface IGroupRepository : IRepository<Group>
{
    /// <summary>Возвращает группу по идентификатору, включая участников.</summary>
    Task<Group?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>Возвращает все группы организации.</summary>
    Task<IReadOnlyCollection<Group>> ListByOrganizationAsync(
        Guid organizationId,
        CancellationToken cancellationToken = default
    );

    /// <summary>Возвращает группы по спецификации.</summary>
    Task<IReadOnlyCollection<Group>> ListAsync(
        ISpecification<Group> specification,
        CancellationToken cancellationToken = default
    );

    /// <summary>Добавляет новую группу.</summary>
    Task AddAsync(Group group, CancellationToken cancellationToken = default);
}
