using Edvantix.SharedKernel.SeedWork;

namespace Edvantix.Chassis.Repository;

public interface IRepository<T>
    where T : IAggregateRoot
{
    /// <summary>
    ///     Gets the current unit of work used to coordinate transactional changes for this repository.
    /// </summary>
    IUnitOfWork UnitOfWork { get; }
}
