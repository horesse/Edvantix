using Edvantix.Chassis.Repository.Crud;

namespace Edvantix.Company.Domain.AggregatesModel.UsageAggregate;

public interface IUsageRepository : ICrudRepository<Usage, long>;
