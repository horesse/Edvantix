namespace Edvantix.Organizational.Domain.AggregatesModel.OrganizationMemberAggregate;

/// <summary>Репозиторий агрегата <see cref="OrganizationMember"/>.</summary>
public interface IOrganizationMemberRepository : IRepository<OrganizationMember>
{
    /// <summary>Возвращает участника по идентификатору.</summary>
    Task<OrganizationMember?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>Возвращает всех участников организации.</summary>
    Task<IReadOnlyCollection<OrganizationMember>> ListByOrganizationAsync(
        Guid organizationId,
        CancellationToken cancellationToken = default
    );

    /// <summary>Возвращает всех участников по спецификации.</summary>
    Task<IReadOnlyCollection<OrganizationMember>> ListAsync(
        ISpecification<OrganizationMember> specification,
        CancellationToken cancellationToken = default
    );

    /// <summary>Подсчитывает количество участников по спецификации.</summary>
    Task<int> CountAsync(
        ISpecification<OrganizationMember> specification,
        CancellationToken cancellationToken = default
    );

    /// <summary>Добавляет участника.</summary>
    Task AddAsync(OrganizationMember member, CancellationToken cancellationToken = default);

    /// <summary>
    /// Возвращает множество кодов разрешений активного участника организации.
    /// Возвращает пустое множество, если участник не найден или не активен.
    /// </summary>
    Task<HashSet<string>> GetActivePermissionsAsync(
        Guid organizationId,
        Guid profileId,
        CancellationToken cancellationToken = default
    );
}
