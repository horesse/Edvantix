using Edvantix.SharedKernel.SeedWork;

namespace Edvantix.Chassis.Repository;

public interface IRepository<T>
    where T : IAggregateRoot
{
    IUnitOfWork UnitOfWork { get; }
}
