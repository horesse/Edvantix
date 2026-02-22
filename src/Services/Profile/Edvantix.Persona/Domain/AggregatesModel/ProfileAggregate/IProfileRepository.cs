namespace Edvantix.Persona.Domain.AggregatesModel.ProfileAggregate;

/// <summary>
/// Репозиторий профилей пользователей.
/// <see cref="IUnitOfWork"/> для сохранения изменений доступен через <see cref="IRepository{T}.UnitOfWork"/>.
/// </summary>
public interface IProfileRepository : IRepository<Profile>
{
    /// <summary>
    /// Поиск профиля по спецификации.
    /// Используйте <see cref="ProfileByIdSpec"/> или <see cref="ProfileByAccountIdSpec"/>
    /// для построения запросов с нужными включениями.
    /// </summary>
    Task<Profile?> FindAsync(ISpecification<Profile> spec, CancellationToken ct = default);

    /// <summary>Проверяет, существует ли профиль с указанным AccountId.</summary>
    Task<bool> ExistsByAccountIdAsync(Guid accountId, CancellationToken ct = default);

    /// <summary>
    /// Добавляет новый профиль в контекст. Для сохранения вызовите
    /// <see cref="IUnitOfWork.SaveEntitiesAsync"/>.
    /// </summary>
    Task<Profile> AddAsync(Profile profile, CancellationToken ct = default);
}
