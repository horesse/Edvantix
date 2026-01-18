using Edvantix.Chassis.Repository.Crud;

namespace Edvantix.ProfileService.Domain.AggregatesModel.EmploymentHistoryAggregate;

public interface IEmploymentHistoryRepository : ICrudRepository<EmploymentHistory, long>;
