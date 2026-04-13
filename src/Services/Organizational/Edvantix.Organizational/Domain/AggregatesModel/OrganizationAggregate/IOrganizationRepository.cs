namespace Edvantix.Organizational.Domain.AggregatesModel.OrganizationAggregate;

/// <summary>Репозиторий агрегата <see cref="Organization"/>.</summary>
public interface IOrganizationRepository : IRepository<Organization>
{
    /// <summary>Возвращает организацию по идентификатору, включая контакты и банковские счета.</summary>
    Task<Organization?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>Возвращает список организаций по спецификации.</summary>
    Task<IReadOnlyCollection<Organization>> ListAsync(
        ISpecification<Organization> specification,
        CancellationToken cancellationToken = default
    );

    /// <summary>Добавляет новую организацию.</summary>
    Task AddAsync(Organization organization, CancellationToken cancellationToken = default);
}
