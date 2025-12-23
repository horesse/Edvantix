using Edvantix.Chassis.Repository.Crud;

namespace Edvantix.Person.Domain.AggregatesModel.EmploymentHistoryAggregate;

public interface IEmploymentHistoryRepository : ICrudRepository<EmploymentHistory, long>;
