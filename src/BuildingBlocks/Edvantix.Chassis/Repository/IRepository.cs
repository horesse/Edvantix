using Edvantix.SharedKernel.SeedWork;

namespace Edvantix.Chassis.Repository;

public interface IRepository<T>
    where T : IAggregateRoot
{
    /// <summary>
    /// Возвращает единицу работы, используемую для координации транзакционных изменений в этом репозитории.
    /// </summary>
    IUnitOfWork UnitOfWork { get; }
}
