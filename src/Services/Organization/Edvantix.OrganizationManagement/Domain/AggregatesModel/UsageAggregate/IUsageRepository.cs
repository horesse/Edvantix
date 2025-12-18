using Edvantix.Chassis.Repository.Crud;

namespace Edvantix.OrganizationManagement.Domain.AggregatesModel.UsageAggregate;

public interface IUsageRepository : ICrudRepository<Usage, long>;

