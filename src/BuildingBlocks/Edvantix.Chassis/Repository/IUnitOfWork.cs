namespace Edvantix.Chassis.Repository;

public interface IUnitOfWork : IDisposable
{
    /// <summary>
    /// Сохраняет все отслеживаемые изменения в базовом хранилище данных.
    /// </summary>
    /// <param name="cancellationToken">Токен для отмены асинхронной операции.</param>
    /// <returns>Количество записей состояния, записанных в хранилище данных.</returns>
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Сохраняет сущности и публикует связанные доменные события в рамках единицы работы.
    /// </summary>
    /// <param name="cancellationToken">Токен для отмены асинхронной операции.</param>
    /// <returns><see langword="true" /> при успешном выполнении операции; иначе <see langword="false" />.</returns>
    Task<bool> SaveEntitiesAsync(CancellationToken cancellationToken = default);
}
