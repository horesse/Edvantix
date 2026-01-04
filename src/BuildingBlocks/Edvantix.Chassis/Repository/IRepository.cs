namespace Edvantix.Chassis.Repository;

public interface IRepository<T>
    where T : class
{
    IUnitOfWork UnitOfWork { get; }
}
