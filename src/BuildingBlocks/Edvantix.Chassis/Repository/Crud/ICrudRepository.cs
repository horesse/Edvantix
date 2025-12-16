using System.Linq.Expressions;
using Edvantix.Chassis.Specification;
using Edvantix.SharedKernel.SeedWork;
using Microsoft.EntityFrameworkCore.Storage;

namespace Edvantix.Chassis.Repository.Crud;

public interface ICrudRepository<TEntity, TIdentity> : IRepository<TEntity>, IDisposable
    where TEntity : Entity<TIdentity>, IAggregateRoot
    where TIdentity : struct
{
    /// <summary>
    /// Асинхронно извлекает объект типа TEntity, который соответствует заданному условию.
    /// </summary>
    /// <param name="predicate">Лямбда-выражение для фильтрации объектов.</param>
    /// <param name="token">Токен для отмены операции.</param>
    /// <returns></returns>
    Task<TEntity?> GetOrDefaultAsync(
        Expression<Func<TEntity, bool>> predicate,
        CancellationToken token
    );

    /// <summary>
    /// Возвращает объекты типа TEntity в виде IQueryable,
    /// предоставляя возможность построения отложенных запросов.
    /// </summary>
    /// <returns>IQueryable, представляющий объекты типа TEntity.</returns>
    IQueryable<TEntity> GetAsQueryable(bool withDefault = true);

    /// <summary>
    /// Асинхронно возвращает все объекты типа TEntity в виде коллекции.
    /// </summary>
    /// <param name="token">Токен для отмены операции.</param>
    /// <returns>Задача, содержащая коллекцию объектов типа TEntity.</returns>
    Task<List<TEntity>> GetAllAsync(CancellationToken token);

    /// <summary>
    /// Асинхронно возвращает все объекты типа TEntity в виде коллекции по заданному фильтру.
    /// </summary>
    /// <param name="specification"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    Task<List<TEntity>> GetByExpressionAsync(
        ISpecification<TEntity> specification,
        CancellationToken token
    );

    /// <summary>
    /// Асинхронно извлекает объект типа TEntity по указанному идентификатору.
    /// </summary>
    /// <typeparam name="TIdentity">Тип идентификатора. Должен быть значимым типом (struct).</typeparam>
    /// <param name="identity">Идентификатор объекта для извлечения.</param>
    /// <param name="token">Токен для отмены операции.</param>
    /// <returns>Задача, содержащая объект типа TEntity или null, если объект не найден.</returns>
    Task<TEntity?> GetByIdAsync(TIdentity identity, CancellationToken token);

    /// <summary>
    /// Асинхронно возвращает количество объектов типа TEntity.
    /// </summary>
    /// <param name="token">Токен для отмены операции.</param>
    /// <returns>Задача, содержащая количество объектов.</returns>
    Task<int> GetCountAsync(CancellationToken token);

    Task<int> GetCountByExpressionAsync(
        ISpecification<TEntity> specification,
        CancellationToken token
    );

    /// <summary>
    /// Асинхронно проверяет наличие записи по идентификатору
    /// </summary>
    /// <param name="id">Идентификатор сущности.</param>
    /// <param name="token">Токен для отмены операции.</param>
    /// <typeparam name="TIdentity">Тип идентификатора. Должен быть значимым типом (struct).</typeparam>
    /// <returns></returns>
    Task<bool> IsExistAsync(TIdentity id, CancellationToken token);

    /// <summary>
    /// Вставляет новый объект типа TEntity в хранилище данных.
    /// </summary>
    /// <param name="entity">Объект типа TEntity для вставки.</param>
    void Insert(TEntity entity);

    /// <summary>
    /// Асинхронно вставляет объект типа TEntity в хранилище данных.
    /// </summary>
    /// <param name="entity">Объект типа TEntity для вставки.</param>
    /// <param name="token">Токен для отмены операции.</param>
    /// <returns>Задача, представляющая завершение операции вставки.</returns>
    Task<TEntity> InsertAsync(TEntity entity, CancellationToken token);

    /// <summary>
    /// Асинхронно вставляет указанный объект в хранилище данных.
    /// </summary>
    /// <param name="entity">Объект, который необходимо вставить в хранилище данных.</param>
    /// <param name="token">Токен для отмены операции.</param>
    /// <returns>Задача, содержащая результат операции вставки объекта.</returns>
    Task<object> InsertAsync(object entity, CancellationToken token);

    /// <summary>
    /// Асинхронно вставляет коллекцию объектов типа TEntity в хранилище данных.
    /// </summary>
    /// <param name="entities">Коллекция объектов типа TEntity для вставки.</param>
    /// <param name="token">Токен для отмены операции.</param>
    /// <returns>Задача, содержащая коллекцию вставленных объектов.</returns>
    Task<List<TEntity>> InsertRangeAsync(List<TEntity> entities, CancellationToken token);

    /// <summary>
    /// Асинхронно вставляет или обновляет объект типа TEntity в хранилище данных.
    /// </summary>
    /// <param name="entity">Объект типа TEntity для вставки или обновления.</param>
    /// <param name="token">Токен для отмены операции.</param>
    /// <returns>Задача, содержащая вставленный или обновленный объект.</returns>
    Task<TEntity> InsertOrUpdateAsync(TEntity entity, CancellationToken token);

    /// <summary>
    /// Асинхронно вставляет или обновляет коллекцию объектов типа TEntity в хранилище данных.
    /// </summary>
    /// <param name="entities">Коллекция объектов типа TEntity для вставки или обновления.</param>
    /// <param name="token">Токен для отмены операции.</param>
    /// <returns>Задача, содержащая коллекцию вставленных или обновленных объектов.</returns>
    Task<List<TEntity>> InsertOrUpdateAsync(List<TEntity> entities, CancellationToken token);

    /// <summary>
    /// Асинхронно обновляет указанный объект типа TEntity в хранилище данных.
    /// </summary>
    /// <param name="entity">Объект типа TEntity, который необходимо обновить.</param>
    /// <param name="token">Токен для отмены операции.</param>
    /// <returns>Задача, содержащая результат операции обновления объекта.</returns>
    Task<TEntity> UpdateAsync(TEntity entity, CancellationToken token);

    /// <summary>
    /// Асинхронно обновляет коллекцию объектов типа TEntity в хранилище данных.
    /// </summary>
    /// <param name="entities">Коллекция объектов типа TEntity, которые необходимо обновить.</param>
    /// <param name="token">Токен для отмены операции.</param>
    /// <returns>Задача, содержащая результат операции обновления коллекции объектов.</returns>
    Task<List<TEntity>> UpdateAsync(List<TEntity> entities, CancellationToken token);

    /// <summary>
    /// Асинхронно удаляет все объекты типа TEntity из хранилища данных.
    /// </summary>
    /// <param name="token">Токен для отмены операции.</param>
    /// <returns>Задача, представляющая завершение операции удаления.</returns>
    Task DeleteAllAsync(CancellationToken token);

    /// <summary>
    /// Асинхронно удаляет указанный объект типа TEntity из хранилища данных.
    /// </summary>
    /// <param name="entity">Объект типа TEntity, который необходимо удалить.</param>
    /// <param name="token">Токен для отмены операции.</param>
    /// <returns>Задача, представляющая завершение операции удаления.</returns>
    Task DeleteAsync(TEntity entity, CancellationToken token);

    /// <summary>
    /// Асинхронно удаляет коллекцию объектов типа TEntity из хранилища данных.
    /// </summary>
    /// <param name="entities">Коллекция объектов типа TEntity, которые необходимо удалить.</param>\
    /// <param name="token">Токен для отмены операции.</param>
    /// <returns>Задача, представляющая завершение операции удаления.</returns>
    Task DeleteAsync(IEnumerable<TEntity> entities, CancellationToken token);

    /// <summary>
    /// Асинхронно удаляет объект типа TEntity, идентифицируемый указанным идентификатором.
    /// </summary>
    /// <typeparam name="TIdentity">Тип идентификатора. Должен быть значимым типом (struct).</typeparam>
    /// <param name="identity">Идентификатор объекта для удаления.</param>
    /// <param name="token">Токен для отмены операции.</param>
    /// <returns>Задача, представляющая завершение операции удаления.</returns>
    Task DeleteAsync(TIdentity identity, CancellationToken token);

    /// <summary>
    /// Асинхронно удаляет объекты типа TEntity, идентифицируемые коллекцией идентификаторов.
    /// </summary>
    /// <typeparam name="TIdentity">Тип идентификаторов. Должен быть значимым типом (struct).</typeparam>
    /// <param name="identities">Коллекция идентификаторов объектов для удаления.</param>
    /// <param name="token">Токен для отмены операции.</param>
    /// <returns>Задача, представляющая завершение операции удаления.</returns>
    Task DeleteAsync(IEnumerable<TIdentity> identities, CancellationToken token);

    /// <summary>
    /// Асинхронно выполняет необработанный SQL-запрос и возвращает количество затронутых строк.
    /// </summary>
    /// <param name="query">SQL-запрос для выполнения.</param>
    /// <param name="token">Токен для отмены операции.</param>
    /// <returns>Задача, содержащая количество затронутых строк.</returns>
    Task<int> ExecuteRawSqlAsync(string query, CancellationToken token);

    /// <summary>
    /// Асинхронно очищает кэш, связанный с хранилищем данных.
    /// </summary>
    /// <param name="token">Токен для отмены операции.</param>
    /// <returns>Задача, представляющая завершение операции очистки кэша.</returns>
    Task ClearCacheAsync(CancellationToken token);

    /// <summary>
    /// Асинхронно начинает новую транзакцию базы данных.
    /// </summary>
    /// <param name="token">Токен отмены, позволяющий прервать операцию.</param>
    /// <returns>Задача, представляющая транзакцию <see cref="IDbContextTransaction"/>.</returns>
    Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken token);

    Task<bool> SaveEntitiesAsync(CancellationToken token);
}
