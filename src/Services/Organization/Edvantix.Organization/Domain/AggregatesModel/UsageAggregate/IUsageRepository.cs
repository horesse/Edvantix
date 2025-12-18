using Edvantix.Chassis.Repository.Crud;

namespace Edvantix.Organization.Domain.AggregatesModel.UsageAggregate;

public interface IUsageRepository : ICrudRepository<Usage, long>;
